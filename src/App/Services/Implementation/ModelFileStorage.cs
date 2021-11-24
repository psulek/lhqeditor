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

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using LHQ.App.Services.Interfaces;
using LHQ.Core.DependencyInjection;
using LHQ.Data;
using LHQ.Data.ModelStorage;

namespace LHQ.App.Services.Implementation
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class ModelFileStorage : AppContextServiceBase, IModelFileStorage, IHasInitialize
    {
        private readonly ModelSerializerManager _serializerManager;

        public ModelFileStorage()
        {
            _serializerManager = new ModelSerializerManager();
        }

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        public ModelContext Clone(ModelContext source, bool preserveKeys = false)
        {
            var saveOptions = new ModelSaveOptions
            {
                SaveResourceKeys = true
            };
            string serializedContent = Save(source, saveOptions);

            var clonedModelContext = new ModelContext(source.Options);
            var loadOptions = new ModelLoadOptions(preserveKeys);
            ModelLoadResult loadResult = _serializerManager.Deserialize(clonedModelContext, serializedContent, loadOptions);
            return loadResult.Status == ModelLoadStatus.Success ? clonedModelContext : null;
        }

        public ModelLoadResult Load(ModelContext modelContext, string content)
        {
            var loadOptions = new ModelLoadOptions(false);
            return _serializerManager.Deserialize(modelContext, content, loadOptions);
        }

        public ModelLoadResult Upgrade(ModelContext modelContext, string content, int fromModelVersion)
        {
            var loadOptions = new ModelLoadOptions(false)
            {
                UpgradeFromModelVersion = fromModelVersion
            };
            return _serializerManager.Deserialize(modelContext, content, loadOptions);
        }

        public string Save(ModelContext modelContext, ModelSaveOptions modelSaveOptions)
        {
            return _serializerManager.Serialize(modelContext, modelSaveOptions);
        }

        public void Initialize()
        {
            Assembly assembly = typeof(ModelSerializerManager).Assembly;
            _serializerManager.Initialize(new[] { assembly });
        }
    }
}
