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
using System.Threading.Tasks;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Implementation.PluginSystem;
using LHQ.App.Services.Interfaces;
using LHQ.Core.Interfaces;
using LHQ.Utils.Extensions;

namespace LHQ.App.ViewModels.Dialogs.AppSettings
{
    public class PageTranslator : AppSettingsPageBase
    {
        private bool _enableTranslation;
        private string _selectedTranslator;
        private List<ITranslationProvider> _translators;
        private bool _showConfigureHint;
        private bool _showTest;
        private bool _showConfigure;

        public PageTranslator(IAppContext appContext, IAppSettingsDialogViewModel ownerViewModel, AppSettingsDialogPage page)
            : base(appContext, ownerViewModel, page)
        {
            ConfigureCommand = new DelegateCommand(ConfigureExecute, ConfigureCanExecute);
            TestCommand = new DelegateCommand(TestExecute, TestCanExecute);
        }

        private PluginServiceContainer PluginServiceContainer => AppContext.PluginServiceContainer as PluginServiceContainer;

        public override string Name => Strings.ViewModels.AppSettings.PageTranslator.PageTitle;

        public DelegateCommand ConfigureCommand { get;  }

        public DelegateCommand TestCommand { get; }

        public List<ITranslationProvider> Translators
        {
            get => _translators;
            set => SetProperty(ref _translators, value);
        }

        public string SelectedTranslator
        {
            get => _selectedTranslator;
            set
            {
                if (SetProperty(ref _selectedTranslator, value))
                {
                    UpdateShowConfigureHint();
                }
            }
        }

        public bool EnableTranslation
        {
            get => _enableTranslation;
            set
            {
                if (SetProperty(ref _enableTranslation, value))
                {
                    UpdateShowConfigureHint();
                }
            }
        }

        public bool ShowConfigureHint
        {
            get => _showConfigureHint;
            set => SetProperty(ref _showConfigureHint, value);
        }

        public bool ShowConfigure
        {
            get => _showConfigure;
            set => SetProperty(ref _showConfigure, value);
        }

        public bool ShowTest
        {
            get => _showTest;
            set => SetProperty(ref _showTest, value);
        }

        protected internal override void BindControls(AppConfig appConfig)
        {
            base.BindControls(appConfig);
            EnableTranslation = appConfig.EnableTranslation;
            Translators = AppContext.TranslationService.GetProviders();
            ITranslationProvider selectedProvider = Translators.SingleOrDefault(x => x.Key == appConfig.TranslatorProviderKey);
            SelectedTranslator = selectedProvider?.Key;
            UpdateShowConfigureHint();
        }

        private bool TestCanExecute(object arg)
        {
            return ConfigureCanExecute(arg);
        }

        private void TestExecute(object obj)
        {
            ITranslationProvider selectedProvider = GetSelectedProvider();
            if (selectedProvider != null)
            {
                DialogService.ShowProgressDialog(new TestTranslationAsyncOperation(AppContext, selectedProvider));
            }
        }

        private bool ConfigureCanExecute(object arg)
        {
            return !SelectedTranslator.IsNullOrEmpty();
        }

        private void ConfigureExecute(object obj)
        {
            ITranslationProvider selectedProvider = GetSelectedProvider();
            string ownerPluginModuleKey = selectedProvider == null ? null : PluginServiceContainer.GetOwnerPluginModuleKey(selectedProvider);
            if (!ownerPluginModuleKey.IsNullOrEmpty())
            {
                if (OwnerDialog.PagePlugins.ConfigurePlugin(ownerPluginModuleKey))
                {
                    OwnerDialog.PagePlugins.SaveToConfig(true);
                }
                UpdateShowConfigureHint();
            }
        }

        private ITranslationProvider GetSelectedProvider()
        {
            return SelectedTranslator.IsNullOrEmpty() ? null : Translators.SingleOrDefault(x => x.Key == SelectedTranslator);
        }

        private void UpdateShowConfigureHint()
        {
            bool showConfigureHint = EnableTranslation && !SelectedTranslator.IsNullOrEmpty();
            bool showTest = showConfigureHint;
            ShowConfigure = showConfigureHint;
            if (showConfigureHint)
            {
                ITranslationProvider selectedProvider = GetSelectedProvider();
                if (selectedProvider != null)
                {
                    showConfigureHint = !selectedProvider.IsConfigured;
                    showTest = !showConfigureHint;
                }
                else
                {
                    showTest = false;
                }
            }

            ShowConfigureHint = showConfigureHint;
            ShowTest = showTest;
        }

        protected internal override void SaveToConfig(bool finalSave)
        {
            AppConfig.EnableTranslation = EnableTranslation;
            AppConfig.TranslatorProviderKey = SelectedTranslator;
        }

        protected internal override bool SubmitDialog()
        {
            if (EnableTranslation && SelectedTranslator == null)
            {
                DialogService.ShowError(new DialogShowInfo(Strings.Common.ValidationError,
                    Strings.ViewModels.AppSettings.PageTranslator.TranslatorMustBeSelected));

                return false;
            }

            return true;
        }

        private class TestTranslationAsyncOperation : AsyncOperation
        {
            private readonly ITranslationProvider _translationProvider;

            public TestTranslationAsyncOperation(IAppContext appContext, ITranslationProvider translationProvider)
                : base(appContext)
            {
                _translationProvider = translationProvider;
                ProgressMessage = Strings.Dialogs.AppSettings.PageTranslator.Test.TestingText(translationProvider.DisplayName);
                ShowCancelButton = true;
            }

            public override string ProgressMessage { get; }

            public override bool ShowCancelButton { get; }

            protected override async Task InternalStart()
            {
                string error = await TestTranslationServiceAsync();
                if (error.IsNullOrEmpty())
                {
                    Result.SetSuccess();
                }
                else
                {
                    Result.SetError(error);
                }
            }

            private async Task<string> TestTranslationServiceAsync()
            {
                string error = Strings.ViewModels.AppSettings.PageTranslator.TestingProviderFailed(_translationProvider.DisplayName);

                try
                {
                    var testResult = await TaskAsyncExtensions.WaitUntil(CancellationToken, _translationProvider.TestServiceAsync);
                    if (testResult != null)
                    {
                        error = testResult.IsSuccess ? null : testResult.Message;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(error, e);
                }

                return error;
            }

            protected override Task InternalCompleted(bool userCancelled)
            {
                IDialogService dialogService = AppContext.DialogService;

                if (userCancelled)
                {
                    var testingText = Strings.Dialogs.AppSettings.PageTranslator.Test.TestingText(_translationProvider.DisplayName);
                    dialogService.ShowError(new DialogShowInfo(testingText, Strings.Services.Translation.TestingWasCancelledByUser));
                }
                else
                {
                    if (Result.IsSuccess)
                    {
                        var title = Strings.Dialogs.AppSettings.PageTranslator.Test.TestSucceedTitle;
                        var text = Strings.Dialogs.AppSettings.PageTranslator.Test.TestSucceed(_translationProvider.DisplayName);

                        dialogService.ShowInfo(new DialogShowInfo(title, text));
                    }
                    else
                    {
                        dialogService.ShowError(new DialogShowInfo(Strings.Common.Error, Result.Message, Result.Detail));
                    }
                }

                return Task.CompletedTask;
            }
        }
    }
}
