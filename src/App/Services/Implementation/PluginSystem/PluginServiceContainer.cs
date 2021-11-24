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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LHQ.Core.DependencyInjection;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Utils.Extensions;

namespace LHQ.App.Services.Implementation.PluginSystem
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class PluginServiceContainer : AppContextServiceBase, IPluginServiceContainer
    {
        private readonly ConcurrentDictionary<string, List<IPluginAwareService>> _instances;

        public PluginServiceContainer()
        {
            _instances = new ConcurrentDictionary<string, List<IPluginAwareService>>();
        }

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        public void Register<TService>(IPluginModule pluginModule)
            where TService : IPluginAwareService, new()
        {
            if (pluginModule == null)
            {
                Logger.Error("Error registering plugin service into system, plugin module instance could not be empty!");
                return;
            }

            string pluginModuleKey = pluginModule.Key;
            if (pluginModuleKey.IsNullOrEmpty())
            {
                Logger.Error("Error registering plugin service into system, plugin module instance must have non null/empty module key!");
                return;
            }

            List<IPluginAwareService> pluginServices = _instances.GetOrAdd(pluginModuleKey, _ => new List<IPluginAwareService>());

            TService service = default;
            Type serviceType = typeof(TService);
            try
            {
                Logger.Info($"Registering plugin service '{serviceType.FullName}' from plugin module '{pluginModuleKey}'");

                service = new TService
                {
                    PluginSystemContext = new PluginSystemContext(AppContext, pluginModule)
                };
            }
            catch (Exception e)
            {
                Logger.Error($"Error registering plugin service '{serviceType.FullName}' from plugin module '{pluginModuleKey}', " +
                    $"failed to create instance of plugin module type : {serviceType.AssemblyQualifiedName}", e);
            }

            if (service != null)
            {
                try
                {
                    service.Initialize();
                    pluginServices.Add(service);
                }
                catch (Exception e)
                {
                    Logger.Error($"Error initializing plugin service '{serviceType.FullName}' from plugin module '{pluginModuleKey}'", e);
                }
            }
        }

        public List<T> GetAll<T>()
            where T : IPluginAwareService
        {
            Type interfaceType = typeof(T);
            return _instances.Values.SelectMany(x => x).Where(x => interfaceType.IsInstanceOfType(x)).Cast<T>().ToList();
        }

        public string GetOwnerPluginModuleKey<T>(T pluginAwareService)
            where T : IPluginAwareService
        {
            foreach (KeyValuePair<string, List<IPluginAwareService>> pair in _instances)
            {
                string pluginModuleKey = pair.Key;
                List<IPluginAwareService> pluginAwareServices = pair.Value;
                if (pluginAwareServices.Any(x => x == (IPluginAwareService)pluginAwareService))
                {
                    return pluginModuleKey;
                }
            }

            return null;
        }

        public void Unload(List<IPluginModule> pluginModules)
        {
            string[] pluginModuleKeys = pluginModules.Select(x => x.Key).ToArray();

            foreach (string pluginModuleKey in pluginModuleKeys)
            {
                _instances.TryRemove(pluginModuleKey, out _);
            }
        }
    }
}
