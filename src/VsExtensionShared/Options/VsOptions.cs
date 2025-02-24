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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using EnvDTE;
using LHQ.App.Code;
using LHQ.App.Dialogs.AppSettings;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Dialogs.AppSettings;
using Microsoft.VisualStudio.Shell;
using AppContext = LHQ.App.Services.Implementation.AppContext;

namespace LHQ.VsExtension.Options
{
    public class VsOptions : UIElementDialogPage
    {
        private AppSettingsView _page;
        private AppVisualTheme _backupTheme;
        private AppVisualTheme? _newTheme;
        private bool _pageFirstCreate;

        private AppConfig _appConfig;
        private readonly TimeSpan _dialogDelayTimeout = TimeSpan.FromMilliseconds(100);

        public AppSettingsDialogViewModel Model { get; private set; }

        public AppConfig AppConfig => _appConfig ?? (_appConfig = AppContext.AppConfigFactory.Current);

        public AppContext AppContext => VsPackageService.AppContext;

        protected override UIElement Child
        {
            get
            {
                if (_page == null)
                {
                    BackupTheme();
                    _pageFirstCreate = true;
                    _page = new AppSettingsView();
                    VisualManager.Instance.ApplyTheme(_page);
                }

                return _page;
            }
        }

        private void BackupTheme()
        {
            if (!_pageFirstCreate)
            {
                _backupTheme = VisualManager.Instance.Theme;

                VisualManager.Instance.BlockChangeNotifications(true);
                VisualManager.Instance.SetTheme(AppVisualTheme.Light);
            }
        }

        internal static AppSettingsDialogPage InitActivePage = AppSettingsDialogPage.General;
        private List<string> _closedEditorFileNames;

        protected override void OnActivate(CancelEventArgs e)
        {
            _newTheme = null;
            BackupTheme();
            _closedEditorFileNames = null;

            if (Model == null)
            {
                VsOptions vsOptions = VsPackage.Options;
                AppContext _ = vsOptions.AppContext;
                Model = new AppSettingsDialogViewModel(AppContext, InitActivePage);
                _page.DataContext = Model;
            }

            base.OnActivate(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            Model = null;
            InitActivePage = AppSettingsDialogPage.General;

            VisualManager.Instance.BlockChangeNotifications(false);

            if (_newTheme != null)
            {
                AppContext.ApplicationService.SetTheme(_newTheme.Value, _backupTheme);
            }
            else
            {
                AppContext.ApplicationService.SetTheme(_backupTheme, _backupTheme);
            }

            if (_closedEditorFileNames != null)
            {
                AppContext.UIService.DispatchActionOnUI(() =>
                    {
                        VsPackageService.ReopenEditors(_closedEditorFileNames);
                    },
                _dialogDelayTimeout);
            }

            _pageFirstCreate = false;
            base.OnClosed(e);
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            _page.Focus();
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                bool canApply = Model.SubmitDialog();
                if (canApply && VsPackageService.HasEditorWithUnsavedChanges())
                {
                    var caption = App.Localization.Strings.Dialogs.VsOptions.ApplyChangesCaption;
                    var message = App.Localization.Strings.Dialogs.VsOptions.ApplyChangesMessage;
                    var detail = App.Localization.Strings.Dialogs.VsOptions.ApplyChangesDetail;

                    var dialogShowInfo = new DialogShowInfo(caption, message, detail);
                    canApply = AppContext.DialogService.ShowConfirm(dialogShowInfo).DialogResult == DialogResult.Yes;
                }

                if (canApply)
                {
                    AppConfig appConfig = Model.SaveToConfig();
                    IAppConfigFactory appConfigFactory = AppContext.AppConfigFactory;
                    appConfigFactory.Set(appConfig);
                    _appConfig = appConfig;

                    if (_appConfig.Theme != _backupTheme)
                    {
                        _newTheme = _appConfig.Theme;
                    }

                    VsPackageService.CloseOpenedEditors(out var projectItems, vsSaveChanges.vsSaveChangesNo);
                    if (projectItems != null)
                    {
                        _closedEditorFileNames = projectItems.Select(x => x.FileNames[0]).ToList();
                    }
                }
                else
                {
                    e.ApplyBehavior = ApplyKind.Cancel;
                }
            }
        }
    }
}
