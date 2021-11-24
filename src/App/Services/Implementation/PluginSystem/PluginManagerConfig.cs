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
using System.Linq;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation.PluginSystem
{
    public class PluginManagerConfig
    {
        public PluginManagerConfig()
        {
            Plugins = new PluginsDictionary();
        }

        private PluginsDictionary Plugins { get; }

        public PluginModuleRegistration Get(Type pluginModuleType)
        {
            return Plugins.Find(pluginModuleType);
        }

        public PluginModuleRegistration GetByKey(string pluginModuleKey)
        {
            return Plugins.GetByKey(pluginModuleKey);
        }

        public PluginModuleRegistration Add(IPluginModule pluginModule)
        {
            var registration = new PluginModuleRegistration(pluginModule, true);
            try
            {
                Plugins.Add(pluginModule.Key, registration);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return registration;
        }
    }

    public class PluginsDictionary
    {
        private readonly Dictionary<string, PluginModuleRegistration> _dictionary;

        public PluginsDictionary()
        {
            _dictionary = new Dictionary<string, PluginModuleRegistration>();
        }

        public void Add(string key, PluginModuleRegistration registration)
        {
            _dictionary.Add(key, registration);
        }

        public PluginModuleRegistration Find(Type pluginModuleType)
        {
            return _dictionary.FirstOrDefault(pair => pair.Value.TypeName == pluginModuleType.GetSimpleAssemblyQualifiedName()).Value;
        }

        public PluginModuleRegistration GetByKey(string key)
        {
            return _dictionary[key];
        }
    }
}
