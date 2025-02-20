#region License
// Copyright (c) 2021 Peter Šulek / ScaleHQ Solutions s.r.o.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LHQ.Data.Attributes;
using LHQ.Data.Interfaces;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LHQ.Data.ModelStorage
{
    public class ModelSerializerManager
    {
        private IEnumerable<Assembly> _assembliesWithSerializers;
        private bool _initialized; 
        
        private List<IModelSerializer> _modelSerializers;

        private IModelSerializer _currentModelSerializer;

        public void Initialize(IEnumerable<Assembly> assembliesWithSerializers)
        {
            _assembliesWithSerializers = assembliesWithSerializers ?? throw new ArgumentNullException(nameof(assembliesWithSerializers));
        }

        private List<IModelSerializer> GetModelSerializers()
        {
            InternalInit();
            return _modelSerializers;
        }

        private void InternalInit()
        {
            if (!_initialized)
            {
                _modelSerializers = TypeHelper.CreateInstancesFromTypedAttributes<ModelSerializerAttribute, IModelSerializer>(
                    "ModelSerializerManager.Initialize", _assembliesWithSerializers.ToArray());

                if (_modelSerializers.Count == 0)
                {
                    throw new InvalidOperationException("Could not find any model serializer registered!");
                }

                if (_modelSerializers.Count != _modelSerializers.Select(x => x.ModelVersion).Distinct().Count())
                {
                    throw new InvalidOperationException("Each model serializer must target one specific model version!");
                }

                _currentModelSerializer = _modelSerializers.SingleOrDefault(x => x.ModelVersion == ModelConstants.CurrentModelVersion);
                if (_currentModelSerializer == null)
                {
                    throw new InvalidOperationException($"Missing model serializer for current model version '{ModelConstants.CurrentModelVersion}' !");
                }
                _initialized = true;
            }
        }

        // ReSharper disable once UnusedMember.Global
        public static ModelLoadResult DeserializeFrom(string fileName, ModelContext modelContext)
        {
            var loadResult = new ModelLoadResult
            {
                Status = ModelLoadStatus.LoadError
            };

            try
            {
                if (File.Exists(fileName))
                {
                    var manager = new ModelSerializerManager();
                    manager.Initialize(new[] { typeof(ModelSerializerManager).Assembly });
                    string fileContent = FileUtils.ReadAllText(fileName);
                    loadResult = manager.Deserialize(modelContext, fileContent, new ModelLoadOptions(false));
                }
            }
            catch (Exception e)
            {
                DebugUtils.Error("ModelSerializerManager.DeserializeFrom", e);
            }

            return loadResult;
        }

        public ModelLoadResult Deserialize(ModelContext modelContext, string source, 
            ModelLoadOptions loadOptions)
        {
            ModelLoadResult modelLoadResult;

            InternalInit();
            
            try
            {
                JObject jsonRoot = JObject.Parse(source, new JsonLoadSettings
                {
                    CommentHandling = CommentHandling.Ignore
                });

                var upgradeFromModelVersion = loadOptions.UpgradeFromModelVersion;
                var upgrading = upgradeFromModelVersion.HasValue;

                var rawJson = JsonUtils.TokenToString(jsonRoot, Formatting.None);

                List<IModelSerializer> serializersToUse = new List<IModelSerializer>();

                if (upgradeFromModelVersion == null)
                {
                    serializersToUse.Add(_currentModelSerializer);
                }
                else
                {
                    foreach (var serializer in _modelSerializers.OrderBy(x => x.ModelVersion))
                    {
                        if (serializer.ModelVersion >= upgradeFromModelVersion)
                        {
                            serializersToUse.Add(serializer);
                        }
                    }
                }
                     
                var firstSerializer = serializersToUse.First();
                modelLoadResult = firstSerializer.Deserialize(modelContext, jsonRoot, loadOptions);
                if (upgrading && modelLoadResult.Status == ModelLoadStatus.Success)
                {
                    var upgraders = serializersToUse.Skip(1).ToList();
                    if (upgraders.Count == 0)
                    {
                        modelLoadResult.Status = ModelLoadStatus.ModelUpgradeFailed;
                    }
                    else
                    {
                        modelLoadResult.Status = ModelLoadStatus.Success;

                        foreach (var upgrader in upgraders)
                        {
                            if (upgrader.Upgrade(modelContext))
                            {
                                modelContext.Model.Version = upgrader.ModelVersion;
                            }
                            else
                            {
                                modelLoadResult.Status = ModelLoadStatus.ModelUpgradeFailed;
                                break;
                            }
                        }
                    }
                }

                if (modelLoadResult != null)
                {
                    modelLoadResult.RawJson = rawJson;
                }
                else
                {
                    modelLoadResult = new ModelLoadResult
                    {
                        Status = ModelLoadStatus.LoadError
                    };
                }
            }
            catch (Exception e)
            {
                modelLoadResult = new ModelLoadResult
                {
                    Status = ModelLoadStatus.LoadError
                };

                DebugUtils.Error(e.Message);
            }

            return modelLoadResult;
        }

        public string Serialize(ModelContext modelContext, ModelSaveOptions saveOptions)
        {
            InternalInit();
            
            JToken jsonModel = _currentModelSerializer.Serialize(modelContext, saveOptions);
            return JsonUtils.TokenToString(jsonModel, GetFormatting(saveOptions));
        }

        private Formatting GetFormatting(ModelSaveOptions saveOptions)
        {
            return saveOptions.IndentOutput ? Formatting.Indented : Formatting.None;
        }

        public bool IsCompatible(ModelContext modelContext)
        {
            IModelSerializer serializer = GetModelSerializers().SingleOrDefault(x => x.ModelVersion == modelContext.Model.Version);
            return serializer?.IsCompatibleWithCurrent() == true;
        }

        public int[] GetSupportedModelVersions()
        {
            return GetModelSerializers().Select(x => x.ModelVersion).Distinct().ToArray();
        }
    }
}
