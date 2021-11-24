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
using LHQ.App.Model;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Utils.Utilities;
using Newtonsoft.Json;

namespace LHQ.App.Services.Implementation.PluginSystem
{
    public sealed class PluginModuleRegistration
    {
        public PluginModuleRegistration(IPluginModule pluginModule, bool enabled) : this()
        {
            Update(pluginModule);
            Enabled = enabled;
            Status = PluginModuleInstanceStatus.Unknown;
        }

        public PluginModuleRegistration()
        {
            Errors = new List<string>();
        }

        [JsonProperty("TypeName")]
        public string TypeName { get; private set; }

        [JsonProperty("AssemblyFile")]
        public string AssemblyFile { get; private set; }

        [JsonProperty("Key")]
        public string Key { get; private set; }

        [JsonProperty("DisplayName")]
        public string DisplayName { get; private set; }

        [JsonProperty("Version")]
        public string Version { get; private set; }

        [JsonProperty("Help")]
        public string Help { get; private set; }

        [JsonProperty("Enabled")]
        public bool Enabled { get; set; }

        [JsonIgnore]
        public PluginModuleInstanceStatus Status { get; private set; }

        [JsonIgnore]
        public List<string> Errors { get; }

        [JsonIgnore]
        public bool IsConfigurable { get; private set; }

        public void Update(IPluginModule pluginModule)
        {
            Type type = pluginModule.GetType();
            IsConfigurable = pluginModule.IsConfigurable;
            TypeName = type.GetSimpleAssemblyQualifiedName();
            AssemblyFile = Path.GetFileName(type.Assembly.Location);
            Key = pluginModule.Key;
            DisplayName = pluginModule.DisplayName;
            Version = pluginModule.Version;
            Help = pluginModule.Description;
        }

        public void UpdateStatus(PluginModuleInstanceStatus status)
        {
            Status = status;
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }
}
