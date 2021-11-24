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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Utils.Extensions;
using Syncfusion.Windows.Tools.Controls;

namespace LHQ.App.ViewModels.Dialogs.AppSettings
{
    public class AppSettingsDialogViewModel : DialogViewModelBase, IAppSettingsDialogViewModel
    {
        private readonly List<AppSettingsPageBase> _pages;
        private AppConfig _appConfig;
        private bool _suppressLeavePageSubmit;

        private int _selectedPageIndex;

        public AppSettingsDialogViewModel(IAppContext appContext, AppSettingsDialogPage activePage)
            : base(appContext)
        {
            _suppressLeavePageSubmit = false;
            ResetToDefaultsCommand = new DelegateCommand(ResetToDefaultsExecute);
            TabSelectionChanged = new DelegateCommand<SelectionChangedEventArgs>(TabSelectionChangedExecute);

            PageGeneral = new PageGeneral(appContext, this, AppSettingsDialogPage.General);
            PageTranslator = new PageTranslator(appContext, this, AppSettingsDialogPage.Translator);
            PagePlugins = new PagePlugins(appContext, this, AppSettingsDialogPage.Plugins);

            _pages = new List<AppSettingsPageBase> { PageGeneral, PageTranslator, PagePlugins };
            _appConfig = AppContext.AppConfigFactory.Current;

            BindControls();

            SelectedPageIndex = _pages.FindItemIndex(x => x.Page == activePage);

            ShowHelpCommand = new DelegateCommand(ShowHelpExecute);
        }

        public ICommand ShowHelpCommand { get; }

        public int SelectedPageIndex
        {
            get => _selectedPageIndex;
            set => SetProperty(ref _selectedPageIndex, value);
        }

        public ICommand ResetToDefaultsCommand { get; }

        public PageGeneral PageGeneral { get; }

        public PageTranslator PageTranslator { get; }

        public PagePlugins PagePlugins { get; }

        public DelegateCommand<SelectionChangedEventArgs> TabSelectionChanged { get; set; }

        private void TabSelectionChangedExecute(SelectionChangedEventArgs args)
        {
            if (DialogResult.HasValue)
            {
                return;
            }

            List<TabItemExt> removedItems = args.RemovedItems.Cast<object>().Where(x => x is TabItemExt).Cast<TabItemExt>().ToList();
            TabItemExt unselectedPage = removedItems.FirstOrDefault();

            List<TabItemExt> addedItems = args.AddedItems.Cast<object>().Where(x => x is TabItemExt).Cast<TabItemExt>().ToList();
            TabItemExt selectedPage = addedItems.FirstOrDefault();

            bool leavePage = !_suppressLeavePageSubmit;

            if (unselectedPage != null && leavePage)
            {
                AppSettingsPageBase currentPage = GetPage(unselectedPage);
                if (!currentPage.LeavePage())
                {
                    _suppressLeavePageSubmit = true;
                    SelectedPageIndex = _pages.IndexOf(currentPage);
                    return;
                }
            }

            if (selectedPage != null)
            {
                AppSettingsPageBase newPage = GetPage(selectedPage);
                newPage.EnterPage();
            }

            if (_suppressLeavePageSubmit)
            {
                _suppressLeavePageSubmit = false;
            }
        }

        private AppSettingsPageBase GetPage(TabItemExt tabItem)
        {
            return tabItem.DataContext as AppSettingsPageBase;
        }

        public void BindControls()
        {
            _pages.Run(page => page.BindControls(_appConfig));
        }

        private void ResetToDefaultsExecute(object obj)
        {
            string caption = Strings.ViewModels.AppSettings.ResetSettingsToDefaultsCaption;
            string message = Strings.ViewModels.AppSettings.ResetSettingsToDefaultsMessage;
            string detail = Strings.ViewModels.AppSettings.ResetSettingsToDefaultsDetail;

            if (DialogService.ShowConfirm(caption,message,detail) == Model.DialogResult.Yes)
            {
                _appConfig = AppContext.AppConfigFactory.CreateDefault();
                BindControls();
            }
        }

        protected override bool CanSubmitDialog()
        {
            return true;
        }

        protected override string GetTitle() => Strings.Dialogs.AppSettings.DialogTitle;

        protected override void OnDispose()
        {
            base.OnDispose();
            _pages.Run(page => page.Dispose());
            _pages.Clear();
        }

        private AppSettingsPageBase GetPageByIndex(int index)
        {
            return _pages[index];
        }

        public override bool SubmitDialog()
        {
            AppSettingsPageBase currentPage = _pages[SelectedPageIndex];

            bool submitSuccess = currentPage.LeavePage();
            if (submitSuccess)
            {
                for (var i = 0; i < _pages.Count; i++)
                {
                    AppSettingsPageBase page = _pages[i];
                    submitSuccess = page.SubmitDialog();
                    if (!submitSuccess)
                    {
                        SelectedPageIndex = _pages.IndexOf(page);
                        break;
                    }
                }
            }

            return submitSuccess;
        }

        public AppConfig SaveToConfig()
        {
            _pages.Run(x => x.SaveToConfig(true));
            return _appConfig;
        }

        private void ShowHelpExecute(object obj)
        {
            AppSettingsPageBase page = GetPageByIndex(SelectedPageIndex);
            WebPageDocSection? docSection = null;
            if (page is PageGeneral)
            {
                docSection = WebPageDocSection.PreferencesGeneral;
            }
            else if (page is PageTranslator)
            {
                docSection = WebPageDocSection.PreferencesTranslator;
            }
            else if (page is PagePlugins)
            {
                docSection = WebPageDocSection.PreferencesPlugins;
            }

            if (docSection.HasValue)
            {
                WebPageUtils.ShowDoc(docSection.Value);
            }
        }
    }
}
