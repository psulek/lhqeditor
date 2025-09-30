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

using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Utils.Extensions;

namespace LHQ.App.ViewModels.Dialogs.AppSettings
{
    public class PluginConfigureDialogViewModel : DialogViewModelBase
    {
        private IPluginModule _plugin;
        private IPluginConfig _config;

        public PluginConfigureDialogViewModel(IAppContext appContext, IPluginModule plugin, IPluginConfig pluginConfig)
            : base(appContext)
        {
            Plugin = plugin;
            Config = pluginConfig;
            ResetToDefaultsCommand = new DelegateCommand(ResetToDefaultsExecute);
            PluginHelpCommand = new DelegateCommand(PluginHelpExecute);
        }

        public ICommand ResetToDefaultsCommand { get; }

        public ICommand PluginHelpCommand { get; }

        public IPluginModule Plugin
        {
            get => _plugin;
            set => SetProperty(ref _plugin, value);
        }

        public IPluginConfig Config
        {
            get => _config;
            set => SetProperty(ref _config, value);
        }

        public bool HasHelp => Plugin.HelpItems != null && Plugin.HelpItems.Count > 0;

        public bool HasDescription => !Plugin.Description.IsNullOrEmpty();

        protected override string GetTitle()
        {
            return Strings.ViewModels.AppSettings.PagePlugins.ConfigurePluginTitle(Plugin?.DisplayName ?? string.Empty);
        }

        private void ResetToDefaultsExecute(object obj)
        {
            var dialogShowInfo = new DialogShowInfo(Strings.ViewModels.AppSettings.PagePlugins.ResetPluginToDefaultsCaption, 
                Strings.ViewModels.AppSettings.PagePlugins.ResetPluginToDefaultsMessage(Plugin?.DisplayName ?? string.Empty));
            
            if (DialogService.ShowConfirm(dialogShowInfo).DialogResult == Model.DialogResult.Yes)
            {
                if (Plugin != null)
                {
                    Config = Plugin.CreateDefaultConfig();
                }
            }
        }

        private void PluginHelpExecute(object obj)
        {
            DialogService.ShowPluginHelp(Plugin);
        }
    }
}
