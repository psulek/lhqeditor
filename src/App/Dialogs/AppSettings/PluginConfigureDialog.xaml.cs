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

using System.Windows;
using LHQ.App.Code;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Dialogs.AppSettings;
using LHQ.Core.Interfaces.PluginSystem;

namespace LHQ.App.Dialogs.AppSettings
{
    /// <summary>
    ///     Interaction logic for PluginConfigureDialog.xaml
    /// </summary>
    public partial class PluginConfigureDialog
    {
        public PluginConfigureDialog()
        {
            InitializeComponent();

            VisualManager.Instance.ApplyTheme(this);

            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Unloaded -= OnUnloaded;
        }

        protected override void InternalOnLoaded()
        {}

        public static Result DialogShow(IAppContext appContext,
            IPluginModule plugin, IPluginConfig pluginConfig)
        {
            using (var viewModel = new PluginConfigureDialogViewModel(appContext, plugin, pluginConfig))
            {
                bool submitted = DialogShow<PluginConfigureDialogViewModel, PluginConfigureDialog>(viewModel) == true;
                IPluginConfig newPluginConfig = null;
                if (submitted)
                {
                    newPluginConfig = viewModel.Config;
                }

                return new Result(submitted, newPluginConfig);
            }
        }

        public class Result
        {
            public Result(bool submitted, IPluginConfig pluginConfig)
            {
                Submitted = submitted;
                PluginConfig = pluginConfig;
            }

            public bool Submitted { get; }

            public IPluginConfig PluginConfig { get; }
        }
    }
}
