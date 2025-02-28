#region License
// Copyright (c) 2025 Peter Šulek / ScaleHQ Solutions s.r.o.
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
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Dialogs.AppSettings;
using LHQ.App.Extensions;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Implementation.PluginSystem;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.PluginSystem;
using LHQ.Core.Interfaces;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.ViewModels.Dialogs.AppSettings
{
    public class PagePlugins : AppSettingsPageBase
    {
        private readonly Dictionary<string, IPluginConfig> _pluginConfigs;
        private List<PluginItem> _plugins;
        private PluginItem _selectedPlugin;

        public PagePlugins(IAppContext appContext, IAppSettingsDialogViewModel ownerViewModel, AppSettingsDialogPage page)
            : base(appContext, ownerViewModel, page)
        {
            _pluginConfigs = new Dictionary<string, IPluginConfig>();

            PluginsKeyDownCommand = new DelegateCommand<KeyEventArgs>(PluginsKeyDownExecute);
            ConfigurePluginCommand = new DelegateCommand<PluginItem>(ConfigurePluginExecute);
        }

        public IPluginManager PluginManager => AppContext.PluginManager;

        public DelegateCommand<KeyEventArgs> PluginsKeyDownCommand { get; set; }

        public DelegateCommand<PluginItem> ConfigurePluginCommand { get; }

        public List<PluginItem> Plugins
        {
            get => _plugins;
            set => SetProperty(ref _plugins, value);
        }

        public PluginItem SelectedPlugin
        {
            get => _selectedPlugin;
            set
            {
                SetProperty(ref _selectedPlugin, value);
                RaisePropertyChanged(nameof(HasSelectedPlugin));
                RaisePropertyChanged(nameof(SelectedPluginHelpItems));
            }
        }

        public bool HasSelectedPlugin => SelectedPlugin != null;

        public List<BulletListItem> SelectedPluginHelpItems => GetSelectedPluginModule()?.GetHelpItems() ?? new List<BulletListItem>();

        private IPluginModule GetSelectedPluginModule()
        {
            var pluginKey = SelectedPlugin?.Key;
            return !pluginKey.IsNullOrEmpty() ? PluginManager.GetPluginModule(pluginKey) : null;
        }


        public override string Name => Strings.ViewModels.AppSettings.PagePlugins.PageTitle;

        protected internal override void BindControls(AppConfig appConfig)
        {
            base.BindControls(appConfig);
            LoadPlugins();
        }

        private void LoadPlugins()
        {
            var plugins = new List<PluginItem>();
            foreach (PluginModuleRegistration registration in PluginManager.GetPluginModules())
            {
                string pluginModuleKey = registration.Key;
                GetOrLoadPluginConfig(pluginModuleKey);
                plugins.Add(new PluginItem(AppContext, registration, this));
            }

            Plugins = plugins;
        }

        private IPluginConfig GetOrLoadPluginConfig(string pluginModuleKey)
        {
            IPluginConfig pluginConfig = _pluginConfigs.ContainsKey(pluginModuleKey) ? _pluginConfigs[pluginModuleKey] : null;

            if (pluginConfig == null)
            {
                IPluginModule pluginModule = PluginManager.GetPluginModule(pluginModuleKey);
                if (pluginModule != null)
                {
                    pluginConfig = PluginManager.GetPluginConfig(pluginModuleKey) ?? pluginModule.CreateDefaultConfig();
                    pluginConfig = JsonUtils.CloneObject(pluginConfig);
                }
            }

            if (!_pluginConfigs.ContainsKey(pluginModuleKey))
            {
                _pluginConfigs.Add(pluginModuleKey, pluginConfig);
            }
            else
            {
                if (_pluginConfigs[pluginModuleKey] == null)
                {
                    _pluginConfigs[pluginModuleKey] = pluginConfig;
                }
            }

            return pluginConfig;
        }

        internal bool ConfigurePlugin(string pluginKey)
        {
            PluginItem pluginItem = Plugins.SingleOrDefault(x => x.Key == pluginKey);
            return pluginItem != null && InternalConfigurePlugin(pluginItem);
        }

        private void ConfigurePluginExecute(PluginItem pluginItem)
        {
            InternalConfigurePlugin(pluginItem);
        }

        private bool InternalConfigurePlugin(PluginItem pluginItem)
        {
            bool result = pluginItem.IsEnabled;
            if (result)
            {
                IPluginModule pluginModule = PluginManager.GetPluginModule(pluginItem.Key);
                if (pluginModule == null)
                {
                    result = SubmitDialog();
                    if (result)
                    {
                        try
                        {
                            SaveToConfig(false);

                            pluginModule = PluginManager.GetPluginModule(pluginItem.Key);
                            if (pluginModule != null)
                            {
                                OwnerDialog.BindControls();
                                pluginItem = Plugins.SingleOrDefault(x => x.Key == pluginItem.Key);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Failed to save changes of plugins page", e);
                        }
                    }
                }

                if (result && pluginItem != null && pluginModule != null)
                {
                    IPluginConfig pluginConfig = GetOrLoadPluginConfig(pluginModule.Key);

                    PluginConfigureDialog.Result dialogResult = PluginConfigureDialog.DialogShow(AppContext, pluginModule,
                        JsonUtils.CloneObject(pluginConfig));

                    result = dialogResult.Submitted;
                    if (result)
                    {
                        _pluginConfigs[pluginModule.Key] = dialogResult.PluginConfig;
                    }
                }
            }

            return result;
        }

        private void PluginsKeyDownExecute(KeyEventArgs args)
        {
            if (args.Key == Key.Space)
            {
                if (SelectedPlugin != null)
                {
                    SelectedPlugin.IsEnabled = !SelectedPlugin.IsEnabled;
                }
            }
        }

        protected internal override void SaveToConfig(bool finalSave)
        {
            bool pluginsEnabledChanged = Plugins.Any(x => x.EnabledChanged);
            if (pluginsEnabledChanged || finalSave)
            {
                EnableDisablePlugins();
            }

            // if current selected translator is not set, set it to 1st registered provider (if any found from plugins)
            if (AppConfig.TranslatorProviderKey.IsNullOrEmpty())
            {
                List<ITranslationProvider> translationProviders = AppContext.TranslationService.GetProviders();
                ITranslationProvider firstProvider = translationProviders.Count > 0 ? translationProviders.First() : null;
                if (firstProvider != null)
                {
                    AppConfig.TranslatorProviderKey = firstProvider.Key;
                }
            }
        }

        private void EnableDisablePlugins()
        {
            IPluginConfig GetPluginConfig(PluginItem pluginItem)
            {
                return GetOrLoadPluginConfig(pluginItem.Key);
            }

            var pluginsChanges = Plugins.Select(x => (x.Key, x.IsEnabled, GetPluginConfig(x))).ToList();
            PluginManager.UpdatePlugins(pluginsChanges);

            bool showPluginsDisabledDialog = false;

            Plugins.ForEach(plugin =>
                {
                    bool isEnabled = PluginManager.GetIsEnabledPluginModule(plugin.Key);
                    // when call to 'PluginManager.UpdatePlugins' disable plugin (due to insufficient license or other?)
                    // revert back to disabled also on UI pluginItem..
                    if (plugin.IsEnabled && !isEnabled)
                    {
                        plugin.IsEnabled = false;
                        showPluginsDisabledDialog = true;
                    }
                });

            Plugins.Run(plugin => plugin.EnabledChanged = false);
            _pluginConfigs.Clear();

            if (showPluginsDisabledDialog)
            {
                // TODO: Show some message that plugin is disabled?
                //DialogService.ShowProductFeatureDisabledDialog(Strings.Dialogs.AppSettings.PagePlugins.Title, ProductFeature.Plugins);
            }
        }

        protected internal override bool SubmitDialog()
        {
            return true;
        }

        public class PluginItem : ObservableModelObject
        {
            private readonly PagePlugins _ownerPage;
            private bool _isEnabled;
            private bool _enabledChanged;

            public PluginItem(IAppContext appContext, PluginModuleRegistration registration, PagePlugins ownerPage)
                : base(appContext)
            {
                _ownerPage = ownerPage;
                _isEnabled = registration.Enabled;
                DisplayName = registration.DisplayName;
                Key = registration.Key;
                Version = registration.Version;
                ConfigureTooltip = Strings.ViewModels.AppSettings.PagePlugins.ConfigurePluginTitle(registration.DisplayName);
                EnabledChanged = false;
                IsConfigurable = registration.IsConfigurable;
            }

            public bool EnabledChanged
            {
                get => _enabledChanged;
                set => SetProperty(ref _enabledChanged, value);
            }

            public bool IsEnabled
            {
                get => _isEnabled;
                set
                {
                    if (SetProperty(ref _isEnabled, value))
                    {
                        _ownerPage.EnableDisablePlugins();
                        RaisePropertyChanged(nameof(IsConfigurable));
                    }
                }
            }

            public string DisplayName { get; }

            public bool IsConfigurable { get; }

            public string Key { get; }

            public string Version { get; }

            public string ConfigureTooltip { get; }
        }
    }
}
