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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Extensions;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Elements;
using LHQ.Core.Interfaces;
using LHQ.Data;
using LHQ.Data.Comparers;
using LHQ.Utils;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using Microsoft.Win32;

namespace LHQ.App.ViewModels.Dialogs.Export
{
    public class ExportDialogViewModel : DialogViewModelBase
    {
        private readonly IShellViewContext _shellViewContext;
        private readonly List<string> _resourceKeysToExport;
        private readonly IAppStateStorageService _appStorageService;
        private bool _isSelectAll;
        private string _selectedExporter;
        private string _selectedFile;
        private readonly string _initialDirectory;
        private IAsyncOperation _exporterAsyncOperation;

        public ExportDialogViewModel(IShellViewContext shellViewContext, bool fromRootModel,
            bool recursive, List<string> resourceKeysToExport)
            : base(shellViewContext.AppContext)
        {
            _initialDirectory = shellViewContext.ShellViewModel.GetProjectDirectory();
            _shellViewContext = shellViewContext;
            _resourceKeysToExport = resourceKeysToExport;
            _appStorageService = AppContext.AppStateStorageService;

            Exporters = shellViewContext.ResourceImportExportService.GetExporters();
            HasExporters = Exporters.Count > 0;
            IResourceExporter firstExporter = Exporters.FirstOrDefault();
            SelectedExporter = firstExporter?.Key;
            SelectedFile = string.Empty;

            // make copy (because dialog can be cancelled)
            var rootModel = shellViewContext.ShellViewModel.RootModel;
            List<LanguageSelectionViewModel> languages = rootModel.Languages
                .Select(x => new LanguageSelectionViewModel(this, x))
                .ToList();
            
            if (_resourceKeysToExport?.Count > 0)
            {
                ElementsToExport = shellViewContext.TreeViewService
                    .FindAllElements(x => x.ElementType == TreeElementType.Resource && _resourceKeysToExport.Contains(x.ElementKey))
                    .Cast<ResourceViewModel>()
                    .Select(x => new ExportResourceItemViewModel(x.GetParentNames(true, true, false).ToDelimitedString("/"), x.ElementType))
                    .ToList();
            }

            ElementsToExportVisible = ElementsToExport?.Count > 0;
            
            languages.Sort(new LanguagePrimaryComparer());

            CheckAndUnCheck = new DelegateCommand<bool>(CheckAndUnCheckExecute);
            Languages = languages.ToObservableCollectionExt();
            SelectFileCommand = new DelegateCommand(SelectFileExecute, SelectFileCanExecute);
            ShowHelpCommand = new DelegateCommand(ShowHelpExecute);
            CopyElementsToClipboard = new DelegateCommand(CopyElementsToClipboardExecute);

            if (fromRootModel)
            {
                ExportSourceText = recursive ? Strings.Dialogs.Export.ExportingAllResourcesRecursive : Strings.Dialogs.Export.ExportingAllResources;
            }
            else
            {
                ExportSourceText = Strings.Dialogs.Export.ExportingSelectedResources(resourceKeysToExport.Count);
            }
        }

        private void CopyElementsToClipboardExecute(object obj)
        {
            string items = ElementsToExport.Select(x => x.Name).ToDelimitedString(Environment.NewLine);
            Clipboard.Clear();
            Clipboard.SetText(items);
        }

        public ICommand ShowHelpCommand { get; }
        
        public ICommand CopyElementsToClipboard { get; }

        public bool HasExporters { get; set; }

        public string ExportSourceText { get; }

        public List<ExportResourceItemViewModel> ElementsToExport { get; set; }

        public bool ElementsToExportVisible { get; set; }

        public string SelectedExporter
        {
            get => _selectedExporter;
            set => SetProperty(ref _selectedExporter, value);
        }

        public string SelectedFile
        {
            get => _selectedFile;
            set => SetProperty(ref _selectedFile, value);
        }

        public List<IResourceExporter> Exporters { get; }

        public ICommand SelectFileCommand { get; set; }

        public DelegateCommand<bool> CheckAndUnCheck { get; set; }

        public CultureInfo[] SelectedCultures { get; private set; }

        public ObservableCollectionExt<LanguageSelectionViewModel> Languages { get; }

        public bool IsSelectAll
        {
            get => _isSelectAll;
            set => SetProperty(ref _isSelectAll, value);
        }

        internal IResourceExporter GetSelectedExporter()
        {
            return SelectedExporter.IsNullOrEmpty() ? null : Exporters.Single(x => x.Key == SelectedExporter);
        }

        protected override string GetTitle()
        {
            return Strings.Dialogs.Export.PageTitle;
        }

        private bool SelectFileCanExecute(object obj)
        {
            return !SelectedExporter.IsNullOrEmpty();
        }

        private void SelectFileExecute(object obj)
        {
            IResourceExporter exporter = GetSelectedExporter();
            if (exporter == null)
            {
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = exporter.FilesFilter,
                InitialDirectory = _initialDirectory,
                FileName = SelectedFile
            };

            if (!_appStorageService.Current.LastExportedResourcesPath.IsNullOrEmpty())
            {
                saveFileDialog.InitialDirectory = _appStorageService.Current.LastExportedResourcesPath;
            }

            if (saveFileDialog.ShowDialog() == true && !saveFileDialog.FileName.IsNullOrEmpty())
            {
                SelectedFile = saveFileDialog.FileName;
            }
        }

        public override bool SubmitDialog()
        {
            if (Languages.Count(x => x.IsSelected) <= 1)
            {
                DialogService.ShowError(new DialogShowInfo(Strings.Dialogs.Export.PageTitle,
                    Strings.Dialogs.Export.Validations.MinimumLanguagesMessage));

                return false;
            }

            if (SelectedFile.IsNullOrEmpty())
            {
                DialogService.ShowError(new DialogShowInfo(Strings.Dialogs.Export.PageTitle,
                    Strings.Dialogs.Export.Validations.MissingFileNameMessage));

                return false;
            }

            SelectedCultures = Languages
                .Where(x => !x.IsPrimary && x.IsSelected)
                .Select(x => CultureCache.Instance.GetCulture(x.Name))
                .ToArray();


            AppContext.AppStateStorageService.Update(storage =>
                {
                    storage.LastExportedResourcesPath = Path.GetDirectoryName(SelectedFile);
                });

            DoExport();

            return true;
        }

        private void DoExport()
        {
            if (_exporterAsyncOperation == null)
            {
                var exporter = GetSelectedExporter();
                ModelContext modelContext = _shellViewContext.ShellViewModel.ModelContext;
                _exporterAsyncOperation = _shellViewContext.ResourceImportExportService.CreateExporterAsyncOperation(exporter, 
                    SelectedFile, SelectedCultures, modelContext, _resourceKeysToExport);

                DialogService.ShowProgressDialog(_exporterAsyncOperation);

                if (_exporterAsyncOperation.Result.IsSuccess)
                {
                    CloseDialog();
                }
                else
                {
                    // to be able to re-run import!
                    _exporterAsyncOperation = null;
                }
            }
        }

        protected override void OnClosingWindow(CancelEventArgs e)
        {
            if (_exporterAsyncOperation?.CanBeCancelled == true)
            {
                _exporterAsyncOperation.Cancel(true);
            }
        }

        private void CheckAndUnCheckExecute(bool isCheckedAll)
        {
            Languages.Run(lang => lang.IsSelected = isCheckedAll);
        }

        internal void NotifySelectedChanged()
        {
            IsSelectAll = Languages.All(x => x.IsSelected);
        }

        private void ShowHelpExecute(object obj)
        {
            WebPageUtils.ShowDoc(WebPageDocSection.ExportResources);
        }
    }

    public class ExportResourceItemViewModel : ObservableObject
    {
        public ExportResourceItemViewModel(string name, TreeElementType elementType)
        {
            Name = name;
            ElementType = elementType;
        }

        public string Name { get; set; }
        public TreeElementType ElementType { get; set; }
    }
}
