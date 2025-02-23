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
using LHQ.App.Model;
using LHQ.App.Services.Interfaces.PluginSystem;
using LHQ.Core.Attributes;
using LHQ.Core.DependencyInjection;
using LHQ.Core.Interfaces;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Utils;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation.PluginSystem
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class PluginManager : AppContextServiceBase, IPluginManager
    {
        //private const string PublicKeyToken = "d8cff9c478cc93d2";
        private const string ReserverdPluginKeyPrefix = "LHQ.";
        private const string ReserverdPluginFilePrefix = "LHQ.Plugin.";
        private static readonly Type _typeIPluginModule = typeof(IPluginModule);

        private PluginManagerConfigStorage _pluginManagerConfigStorage;
        private PluginManagerConfig _pluginManagerConfig;

        private Dictionary<string, PluginModuleItem> _pluginModules;

        private Dictionary<string, IPluginConfigStorage> _pluginConfigStorages;


        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        {}


        public void Initialize()
        {
            InternalInitialize(true);
        }

        private void InternalInitialize(bool setupTranslator)
        {
            _pluginConfigStorages = new Dictionary<string, IPluginConfigStorage>();
            _pluginModules = new Dictionary<string, PluginModuleItem>();

            _pluginManagerConfigStorage = new PluginManagerConfigStorage { AppContext = AppContext };
            _pluginManagerConfigStorage.ConfigureDependencies(AppContext.ServiceContainer);

            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.GetName().Name.StartsWith(ReserverdPluginFilePrefix, true)).ToList();
                //.Where(x => x.GetName().GetPublicKeyTokenAsHex() == PublicKeyToken).ToList();

            string appFolder = AppContext.AppFolder;
            var searchPattern = "LHQ.Plugin.*.dll";
            Logger.Info($"Searching for plugins in: {appFolder} ({searchPattern}) ...");
            string[] pluginAssemblyFiles = Directory.GetFiles(appFolder, searchPattern, SearchOption.TopDirectoryOnly);
            Logger.Info($"Searching for plugins in: {appFolder} found {pluginAssemblyFiles.Length} plugins");
            if (pluginAssemblyFiles.Length > 0)
            {
                foreach (string pluginAssemblyFile in pluginAssemblyFiles)
                {
                    try
                    {
                        Logger.Info($"Loading plugin assembly: {pluginAssemblyFile} ...");
                        Assembly pluginAssembly = Assembly.LoadFile(pluginAssemblyFile);
                        if (!assemblies.Contains(pluginAssembly))
                        {
                            assemblies.Add(pluginAssembly);
                            
                            Logger.Info($"Loading plugin assembly: {pluginAssemblyFile} succeed.");
                        }
                        else
                        {
                            Logger.Info($"Plugin file '{pluginAssemblyFile}' is already loaded in AppDomain.");
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"Error loading plugin file '{pluginAssemblyFile}'.", e);
                    }
                }
            }

            List<Type> pluginModuleTypes = null;
            try
            {
                Logger.Info($"Searching for 'plugin module registration' in {assemblies.Count} assemblies...");
                
                pluginModuleTypes = TypeHelper.LoadTypesFromTypedAttributes<PluginModuleRegistrationAttribute, IPluginModule>(
                    "GetPluginModules", assemblies.ToArray());
                
                Logger.Info($"Searching for 'plugin module registration' in {assemblies.Count}, found " 
                    + (pluginModuleTypes == null ? "-" : $"{pluginModuleTypes.Count} types"));
            }
            catch (Exception e)
            {
                Logger.Error("Error loading plugin types from assemblies!", e);
            }

            _pluginManagerConfig = _pluginManagerConfigStorage.Load() ?? new PluginManagerConfig();

            if (pluginModuleTypes != null && pluginModuleTypes.Count > 0)
            {
                foreach (Type pluginModuleType in pluginModuleTypes)
                {
                    PluginModuleRegistration registration = _pluginManagerConfig.Get(pluginModuleType);

                    bool enabled = registration == null || registration.Enabled;
                    if (enabled)
                    {
                        IPluginModule pluginModule;
                        try
                        {
                            pluginModule = Activator.CreateInstance(pluginModuleType) as IPluginModule;
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"Failed to load plugin instance from type '{pluginModuleType.AssemblyQualifiedName}'", e);
                            pluginModule = null;
                        }

                        if (pluginModule == null)
                        {
                            registration?.AddError("Failed to load plugin instance.");
                            registration?.UpdateStatus(PluginModuleInstanceStatus.LoadFailed);

                            Logger.Error(
                                $"Type '{pluginModuleType.AssemblyQualifiedName}' does not implement {_typeIPluginModule.AssemblyQualifiedName}!");
                        }
                        else
                        {
                            // if plugin module key starts with reserved word, 
                            // check if its assembly public key token points to app vendor's (if its our plugin or not)
                            if (pluginModule.Key.ToUpper().StartsWith(ReserverdPluginKeyPrefix))
                            {
                                // string pluginModulePublicKeyToken = pluginModuleType.Assembly.GetName().GetPublicKeyTokenAsHex();
                                // if (pluginModulePublicKeyToken != PublicKeyToken)
                                // {
                                //     Logger.Error(
                                //         $"Type '{pluginModuleType.AssemblyQualifiedName}' has invalid prefix '{ReserverdPluginKeyPrefix}' for its Key name.");
                                //
                                //     continue;
                                // }
                            }

                            if (registration == null)
                            {
                                registration = _pluginManagerConfig.Add(pluginModule);
                            }
                            else
                            {
                                registration.Update(pluginModule);
                            }

                            registration.UpdateStatus(PluginModuleInstanceStatus.LoadSuccess);

                            _pluginModules.Add(pluginModule.Key, new PluginModuleItem(registration, pluginModule));
                        }
                    }
                    else
                    {
                        _pluginModules.Add(registration.Key, new PluginModuleItem(registration, null));
                        registration.UpdateStatus(PluginModuleInstanceStatus.Disabled);
                    }
                }
            }

            var pluginsToLoad = _pluginModules
                .Where(x => x.Value.Registration.Status == PluginModuleInstanceStatus.LoadSuccess)
                .Select(x => x.Value)
                .ToList();

            LoadPlugins(pluginsToLoad);

            if (setupTranslator)
            {
                SetupTranslatorProvider();
            }

            SavePluginsConfig();
        }

        private void LoadPlugins(List<PluginModuleItem> plugins)
        {
            var pluginServiceContainer = AppContext.ServiceContainer.Get<IPluginServiceContainer>();
            var modulesToUnload = new List<IPluginModule>();

            void MarkForUnload(PluginModuleItem pluginModuleItem)
            {
                if (pluginModuleItem.PluginModule != null)
                {
                    modulesToUnload.Add(pluginModuleItem.PluginModule);
                }
            }

            var pluginModulesToUnload = plugins.Select(x => x.PluginModule).ToList();
            if (pluginModulesToUnload.Count > 0)
            {
                UnloadPluginModules(pluginModulesToUnload);
            }

            foreach (PluginModuleItem pluginModuleItem in plugins)
            {
                PluginModuleRegistration registration = pluginModuleItem.Registration;
                IPluginModule pluginModule = pluginModuleItem.PluginModule;

                IPluginConfigStorage pluginConfigStorage = new PluginConfigStorage(pluginModule);
                pluginConfigStorage.AppContext = AppContext;
                pluginConfigStorage.ConfigureDependencies(AppContext.ServiceContainer);
                _pluginConfigStorages.Add(pluginModule.Key, pluginConfigStorage);

                IPluginConfig pluginConfig = null;
                try
                {
                    pluginConfig = pluginConfigStorage.Load();
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed to load config for plugin '{pluginModule.Key}', using default plugin config.", e);
                }

                if (pluginConfig == null)
                {
                    try
                    {
                        pluginConfig = pluginModule.CreateDefaultConfig();
                    }
                    catch (Exception e)
                    {
                        pluginConfig = null;
                        Logger.Error($"Failed to create default config for plugin '{pluginModule.Key}', plugin will not be loaded.", e);
                    }

                    if (pluginConfig == null)
                    {
                        registration.UpdateStatus(PluginModuleInstanceStatus.LoadFailed);
                        registration.AddError("Creating plugin default config failed.");
                    }
                    else
                    {
                        try
                        {
                            pluginConfigStorage.Save(pluginConfig);
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"Failed to save new config for plugin '{pluginModule.Key}'.", e);
                        }
                    }
                }

                if (registration.Status == PluginModuleInstanceStatus.LoadSuccess)
                {
                    try
                    {
                        pluginModule.PluginConfig = pluginConfig;
                        pluginModule.Initialize(pluginServiceContainer);
                        Logger.Info($"Plugin module '{pluginModule.Key}' was initialized.");
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"Plugin module '{pluginModule.Key}' failed to initialize, plugin will not be loaded.", e);

                        registration.UpdateStatus(PluginModuleInstanceStatus.LoadFailed);
                        registration.AddError("Failed to initialize plugin.");

                        pluginModuleItem.PluginModule = null;
                        MarkForUnload(pluginModuleItem);
                    }
                }
                else
                {
                    // if load does not go well (load config failed, etc) just remove plugin module instance
                    pluginModuleItem.PluginModule = null;
                    MarkForUnload(pluginModuleItem);
                }
            }

            UnloadPluginModules(modulesToUnload);
        }

        private void SetupTranslatorProvider()
        {
            AppConfig appConfig = AppContext.AppConfigFactory.Current;
            List<ITranslationProvider> translationProviders = AppContext.TranslationService.GetProviders();
            // if current selected translator is not set, set it to 1st registered provider
            if (appConfig.TranslatorProviderKey.IsNullOrEmpty())
            {
                ITranslationProvider firstProvider = translationProviders.Count > 0 ? translationProviders.First() : null;
                if (firstProvider != null)
                {
                    appConfig.TranslatorProviderKey = firstProvider.Key;
                    ApplicationService.SaveAppConfig();
                }
            }
        }

        private void SavePluginsConfig()
        {
            try
            {
                // save plugins config to file...
                _pluginManagerConfigStorage.Save(_pluginManagerConfig);
            }
            catch (Exception e)
            {
                Logger.Error("Failed to save plugins config.", e);
            }
        }

        public IPluginModule GetPluginModule(string pluginModuleKey)
        {
            if (_pluginModules.ContainsKey(pluginModuleKey))
            {
                PluginModuleItem pluginModuleItem = _pluginModules[pluginModuleKey];
                return pluginModuleItem.PluginModule;
            }

            return null;
        }

        public bool GetIsEnabledPluginModule(string pluginModuleKey)
        {
            if (_pluginModules.ContainsKey(pluginModuleKey))
            {
                PluginModuleItem pluginModuleItem = _pluginModules[pluginModuleKey];
                return pluginModuleItem.Registration.Enabled;
            }

            return false;
        }

        public IPluginConfig GetPluginConfig(string pluginModuleKey)
        {
            return GetPluginModule(pluginModuleKey)?.PluginConfig;
        }

        public void UpdatePlugins(List<(string pluginKey, bool enabled, IPluginConfig config)> pluginsChanges)
        {
            var modulesToLoad = new List<PluginModuleItem>();
            var modulesToUnload = new List<IPluginModule>();

            foreach ((string pluginKey, bool enabled, IPluginConfig config) pair in pluginsChanges)
            {
                string pluginModuleKey = pair.pluginKey;
                bool newEnabled = pair.enabled;
                IPluginConfig pluginConfig = pair.config;

                if (GetPluginModule(pluginModuleKey) != null && pluginConfig != null)
                {
                    SavePluginConfig(pluginModuleKey, pluginConfig);
                }

                PluginModuleRegistration registration = _pluginManagerConfig.GetByKey(pluginModuleKey);
                bool currentEnabled = registration.Enabled;
                if (currentEnabled != newEnabled)
                {
                    registration.Enabled = newEnabled;
                    PluginModuleItem pluginModuleItem = _pluginModules[pluginModuleKey];

                    if (newEnabled)
                    {
                        modulesToLoad.Add(pluginModuleItem);
                    }
                    else
                    {
                        if (pluginModuleItem.PluginModule != null)
                        {
                            modulesToUnload.Add(pluginModuleItem.PluginModule);
                        }

                        // unload plugin module when enabled was changed to false
                        pluginModuleItem.PluginModule = null;
                    }
                }
            }

            SavePluginsConfig();

            if (modulesToLoad.Count > 0)
            {
                InternalInitialize(false);
            }

            UnloadPluginModules(modulesToUnload);
        }

        private void UnloadPluginModules(List<IPluginModule> pluginModules)
        {
            if (pluginModules.Count > 0)
            {
                var pluginServiceContainer = AppContext.PluginServiceContainer as PluginServiceContainer;
                try
                {
                    pluginServiceContainer?.Unload(pluginModules);
                }
                catch (Exception e)
                {
                    string keys = pluginModules.Select(x => x.Key).ToDelimitedString(decorateItemWith: "'");
                    Logger.Error($"Failed to unload plugin modules ({keys}) from plugin service container", e);
                }
            }
        }

        private void SavePluginConfig(string pluginModuleKey, IPluginConfig pluginConfig = null)
        {
            if (_pluginModules.ContainsKey(pluginModuleKey))
            {
                PluginModuleItem pluginModuleItem = _pluginModules[pluginModuleKey];
                IPluginModule pluginModule = pluginModuleItem.PluginModule;
                if (pluginConfig != null)
                {
                    pluginModule.PluginConfig = pluginConfig;
                }
                else
                {
                    pluginConfig = pluginModule.PluginConfig;
                    if (pluginConfig == null)
                    {
                        try
                        {
                            pluginConfig = pluginModule.CreateDefaultConfig();
                            pluginModule.PluginConfig = pluginConfig;
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"Failed to create default config for plugin '{pluginModule.Key}'.", e);
                            if (pluginModule.PluginConfig == null)
                            {
                                pluginModuleItem.Registration.UpdateStatus(PluginModuleInstanceStatus.LoadFailed);
                                pluginModuleItem.Registration.AddError("Creating plugin default config failed.");
                            }
                        }
                    }
                }

                try
                {
                    IPluginConfigStorage pluginConfigStorage = _pluginConfigStorages[pluginModuleKey];
                    pluginConfigStorage.Save(pluginConfig);
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to save plugin config to file.", e);
                }
            }
        }

        public List<PluginModuleRegistration> GetPluginModules()
        {
            return _pluginModules.Select(x => x.Value.Registration).ToList();
        }

        private class PluginModuleItem
        {
            public PluginModuleItem(PluginModuleRegistration registration, IPluginModule pluginModule)
            {
                Registration = registration;
                PluginModule = pluginModule;
            }

            public PluginModuleRegistration Registration { get; }

            public IPluginModule PluginModule { get; set; }
        }
    }
}
