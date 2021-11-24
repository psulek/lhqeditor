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
using System.IO;
using LHQ.App.Services.Implementation;
using LHQ.App.Services.Interfaces;
using LHQ.Core.DependencyInjection;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LHQ.App.Code
{
    public abstract class ConfigFileStorageBase<T> : AppContextServiceBase, IConfigStorage<T>
    {
        private readonly JsonLoadSettings _jsonLoadSettings = new JsonLoadSettings
        {
            CommentHandling = CommentHandling.Ignore
        };

        private IFileSystemService _fileSystemService;
        private string _fullFileName;

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        {
            _fileSystemService = serviceContainer.Get<IFileSystemService>();
            _fullFileName = _fileSystemService.GetDataFileName(GetFileName());
        }

        public abstract string GetFileName();

        protected bool TryGetJsonValue<TResult>(JObject json, string attributeName, JTokenType type, out TResult result)
        {
            result = default;
            bool valid = json.TryGetValue(attributeName, out JToken token);
            if (valid)
            {
                valid = token.Type == type;
                if (valid)
                {
                    result = token.Value<TResult>();
                }
            }

            return valid;
        }

        public virtual T Load()
        {
            T result = default;
            if (File.Exists(_fullFileName))
            {
                try
                {
                    string fileContent = FileUtils.ReadAllText(_fullFileName);

                    if (!fileContent.IsNullOrEmpty())
                    {
                        JObject jsonRoot = JObject.Parse(fileContent, _jsonLoadSettings);
                        result = LoadFromJson(jsonRoot);
                    }
                }
                catch (Exception e)
                {
                    DebugUtils.Error($"{GetType().Namespace}.Load() failed", e);
                }
            }

            return result;
        }

        protected abstract T LoadFromJson(JObject jsonRoot);

        public virtual void Save(T config)
        {
            SaveToFile(config, _fullFileName);
        }

        internal void SaveToFile(T config, string fileName)
        {
            ArgumentValidator.EnsureArgumentNotNull(config, "config");

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(fileName));
            }

            JObject jsonRoot = SaveToJson(config);
            ArgumentValidator.EnsureArgumentNotNull(jsonRoot, "jsonRoot");

            string json = jsonRoot.ToString(Formatting.Indented);
            FileUtils.WriteAllText(fileName, json);
        }

        protected abstract JObject SaveToJson(T config);
    }
}
