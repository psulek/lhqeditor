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

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Extensions;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Core.Interfaces;
using LHQ.Core.Model;
using LHQ.Utils.Extensions;
using Microsoft.Win32;

namespace LHQ.App.ViewModels.Dialogs
{
    public class ImportDialogViewModel : DialogViewModelBase
    {
        private readonly IShellViewContext _shellViewContext;
        private readonly IAppStateStorageService _appStorageService;
        private string _selectedImporter;
        private string _selectedFile;
        private bool _modeImportAsNew;
        private bool _modeAutoMerge;
        private readonly string _initialDirectory;

        private IAsyncOperation _importerAsyncOperation;
        private bool _allowImportNewLanguages;

        public ImportDialogViewModel(IShellViewContext shellViewContext)
            : base(shellViewContext.AppContext)
        {
            _shellViewContext = shellViewContext;
            _appStorageService = AppContext.AppStateStorageService;
            _initialDirectory = shellViewContext.ShellViewModel.GetProjectDirectory();
            Importers = _shellViewContext.ResourceImportExportService.GetImporters();
            IResourceImporter firstImporter = Importers.FirstOrDefault();
            SelectedImporter = firstImporter?.Key;
            SelectedFile = string.Empty;
            SelectedFiles = null;
            HasImporters = Importers.Count > 0;
            SelectFileCommand = new DelegateCommand(SelectFileExecute, SelectFileCanExecute);
            AllowImportNewLanguages = true;

            string lastSelectedImporterKey = _appStorageService.Current.LastSelectedImporterKey;
            if (!lastSelectedImporterKey.IsNullOrEmpty())
            {
                if (Importers.Any(x => x.Key == lastSelectedImporterKey))
                {
                    SelectedImporter = lastSelectedImporterKey;
                }
            }

            var importMergeMode = _appStorageService.Current.GetLastSelectedResourceImportMergeMode();
            ModeAutoMerge = importMergeMode == ResourceImportMergeMode.AutoMerge;

            ShowHelpCommand = new DelegateCommand(ShowHelpExecute);
        }

        public ICommand ShowHelpCommand { get; }

        public List<IResourceImporter> Importers { get; }

        public ICommand SelectFileCommand { get; set; }

        public string[] SelectedFiles { get; private set; }

        public bool HasImporters { get; set; }

        public string SelectedImporter
        {
            get => _selectedImporter;
            set => SetProperty(ref _selectedImporter, value);
        }

        public string SelectedFile
        {
            get => _selectedFile;
            set => SetProperty(ref _selectedFile, value);
        }

        public ResourceImportMergeMode SelectedImportMode =>
            ModeImportAsNew ? ResourceImportMergeMode.ImportAsNew : ResourceImportMergeMode.AutoMerge;

        public bool AllowImportNewLanguages
        {
            get => _allowImportNewLanguages;
            set => SetProperty(ref _allowImportNewLanguages, value);
        }

        public bool ModeImportAsNew
        {
            get => _modeImportAsNew;
            set
            {
                if (SetProperty(ref _modeImportAsNew, value))
                {
                    if (value && ModeAutoMerge)
                    {
                        ModeAutoMerge = false;
                    }
                }
            }
        }

        public bool ModeAutoMerge
        {
            get => _modeAutoMerge;
            set
            {
                if (SetProperty(ref _modeAutoMerge, value))
                {
                    if (value && ModeImportAsNew)
                    {
                        ModeImportAsNew = false;
                    }
                }
            }
        }

        protected override string GetTitle() => Strings.ViewModels.Import.PageTitle;

        private bool SelectFileCanExecute(object obj)
        {
            return !SelectedImporter.IsNullOrEmpty();
        }

        internal IResourceImporter GetSelectedImporter()
        {
            return SelectedImporter.IsNullOrEmpty() ? null : Importers.Single(x => x.Key == SelectedImporter);
        }

        private void SelectFileExecute(object obj)
        {
            IResourceImporter importer = GetSelectedImporter();
            if (importer == null)
            {
                DialogService.ShowInfo(new DialogShowInfo(Strings.ViewModels.Import.PageTitle,
                    Strings.ViewModels.Import.PleaseSelectImporter));

                return;
            }

            var openFileDialog = new OpenFileDialog
            {
                Multiselect = importer.AllowMultipleFilesAtOnce,
                Filter = importer.FilesFilter,
                InitialDirectory = _initialDirectory
            };

            if (!_appStorageService.Current.LastImportedResourcesPath.IsNullOrEmpty())
            {
                openFileDialog.InitialDirectory = _appStorageService.Current.LastImportedResourcesPath;
            }

            if (openFileDialog.ShowDialog() == true && openFileDialog.FileNames.Length > 0)
            {
                var bkpSelectedFiles = SelectedFiles;
                var bkpSelectedFile = SelectedFile;

                SelectedFiles = openFileDialog.FileNames;
                SelectedFile = SelectedFiles.Length > 1
                    ? SelectedFiles.Select(Path.GetFileName).ToDelimitedString()
                    : SelectedFiles[0];

                if (!ValidateSelectedFiles())
                {
                    SelectedFiles = bkpSelectedFiles;
                    SelectedFile = bkpSelectedFile;
                }
            }
        }

        private bool ValidateSelectedFiles()
        {
            IResourceImporter importer = GetSelectedImporter();
            bool valid = importer != null;

            if (valid)
            {
                var unknownFileExtensions = new List<string>();
                foreach (string file in SelectedFiles)
                {
                    string ext = Path.GetExtension(file)?.ToLowerInvariant();

                    if (!ext.IsNullOrEmpty() && importer.FileExtensions.All(x => x.ToLowerInvariant() != ext))
                    {
                        unknownFileExtensions.Add(file);
                    }
                }

                valid = unknownFileExtensions.Count == 0;

                if (!valid)
                {
                    string unsupportedFileExts = unknownFileExtensions.Select(Path.GetExtension).ToDelimitedString();
                    DialogService.ShowError(new DialogShowInfo(Strings.ViewModels.Import.PageTitle,
                            Strings.ViewModels.Import.UnsupportedFileExtensionsMessage,
                            Strings.ViewModels.Import.UnsupportedFileExtensionsDetail(importer.DisplayName, unsupportedFileExts)));
                }
            }

            return valid;
        }

        protected override bool AutoCloseOnSubmit { get; } = false;

        public override bool SubmitDialog()
        {
            if (SelectedImporter.IsNullOrEmpty())
            {
                DialogService.ShowError(new DialogShowInfo(GetTitle(), Strings.ViewModels.Import.PleaseSelectImporter));
                return false;
            }

            if (SelectedFile.IsNullOrEmpty())
            {
                DialogService.ShowError(new DialogShowInfo(GetTitle(), Strings.ViewModels.Import.PleaseSelectFilesToImport));
                return false;
            }

            if (!ModeAutoMerge && !ModeImportAsNew)
            {
                DialogService.ShowError(new DialogShowInfo(GetTitle(), Strings.ViewModels.Import.PleaseSelectImportMode));
                return false;
            }

            bool submitValid = SelectedFiles != null && SelectedFiles.Length > 0 && ValidateSelectedFiles();

            if (submitValid)
            {
                var appStateStorageService = AppContext.AppStateStorageService;
                appStateStorageService.Update(storage =>
                    {
                        if (SelectedFiles?.Length > 0)
                        {
                            string selectedPath = Path.GetDirectoryName(SelectedFiles.First());
                            storage.LastImportedResourcesPath = selectedPath;
                        }

                        storage.LastSelectedImporterKey = SelectedImporter;
                        storage.SetLastSelectedResourceImportMergeMode(SelectedImportMode);
                    });

                DoImport();
            }

            return submitValid;
        }

        private void DoImport()
        {
            if (_importerAsyncOperation == null)
            {
                var importer = GetSelectedImporter();
                _importerAsyncOperation = _shellViewContext.ResourceImportExportService.CreateImporterAsyncOperation(importer, 
                    SelectedImportMode, AllowImportNewLanguages, SelectedFiles);
                DialogService.ShowProgressDialog(_importerAsyncOperation);

                if (_importerAsyncOperation.Result.IsSuccess)
                {
                    CloseDialog();
                }
                else
                {
                    // to be able to re-run import!
                    _importerAsyncOperation = null;
                }
            }
        }

        protected override void OnClosingWindow(CancelEventArgs e)
        {
            if (_importerAsyncOperation?.CanBeCancelled == true)
            {
                _importerAsyncOperation.Cancel(true);
            }
        }

        private void ShowHelpExecute(object obj)
        {
            WebPageUtils.ShowDoc(WebPageDocSection.ImportResources);
        }
    }
}
