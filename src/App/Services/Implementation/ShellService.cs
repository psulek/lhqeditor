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
using System.IO;
using System.Threading.Tasks;
using LHQ.App.Code;
using LHQ.App.Extensions;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Implementation.Messaging;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Dialogs.AppSettings;
using LHQ.Core.DependencyInjection;
using LHQ.Core.Model;
using LHQ.Data;
using LHQ.Data.CodeGenerator;
using LHQ.Data.Extensions;
using LHQ.Data.Interfaces.Key;
using LHQ.Data.ModelStorage;
using LHQ.Data.Templating.Templates;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation
{
    public class ShellService : ShellViewContextServiceBase, IShellService
    {
        private bool _disposed;

        private ModelSaveOptions ModelSaveOptions => ModelSaveOptions.Indent;

        private IUIService UIService => AppContext.UIService;

        private IDialogService DialogService => AppContext.DialogService;

        private IRecentFilesService RecentFilesService => AppContext.RecentFilesService;

        private IModelFileStorage ModelFileStorage => AppContext.ModelFileStorage;

        private ITranslationService TranslationService => AppContext.TranslationService;

        protected IApplicationService ApplicationService => AppContext.ApplicationService;

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        protected override void AppContextUpdated()
        {
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            AppContext.OnAppEvent += HandleAppEvent;
        }

        private void UnregisterEvents()
        {
            AppContext.OnAppEvent -= HandleAppEvent;
        }

        protected void HandleAppEvent(object sender, ApplicationEventArgs eventArgs)
        {
            try
            {
                switch (eventArgs)
                {
                    case ApplicationEventThemeChanged args:
                    {
                        HandleAppEventThemeChanged(args);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{nameof(ApplicationService)}.{nameof(HandleAppEvent)} '{eventArgs.GetType().FullName}' failed", e);
            }
        }

        private void HandleAppEventThemeChanged(ApplicationEventThemeChanged eventArgs)
        {
            VisualManager.Instance.ApplyTheme(ShellViewModel.ShellView);
            ShellViewModel.RaiseObjectPropertiesChanged();

            var selectionInfo = ShellViewContext.TreeViewService.GetSelection();
            if (!selectionInfo.IsEmpty)
            {
                IEnumerable<ITreeElementViewModel> elementsToReselect;
                if (selectionInfo.IsSingleSelection)
                {
                    elementsToReselect = new[] { selectionInfo.Element };
                }
                else
                {
                    elementsToReselect = selectionInfo.GetSelectedElements();
                }

                ShellViewContext.Messenger.SingleDispatch(MessengerDefinitions.ReSelectNodesInTree(elementsToReselect));
            }

            ShellViewContext.ShellViewCache.RemoveItem(ShellViewCacheNames.TreeElementViewConverter, ShellViewCacheKeys.Default);

            ShellViewModel.TreeVisualRefresh();
        }

        protected virtual void HandleShellViewLoadedEvent(ShellViewLoadedEventArgs eventArgs)
        {
            if (!AppContext.AppStartedFirstTime && !AppContext.RunInVsPackage)
            {
                string fileNameToOpenOnStart = GetFileNameToOpenOnStart();
                if (!fileNameToOpenOnStart.IsNullOrEmpty())
                {
                    OpenLastProject(fileNameToOpenOnStart);
                }
            }

            bool appStartedFirstTime = AppContext.AppStartedFirstTime;
            if (appStartedFirstTime)
            {
                UIService.DelayedAction(() =>
                    {
                        ApplicationService.ShowAppSettings();
                    }, TimeSpan.FromSeconds(1));
            }

        }

        public void NotifyShellLoaded()
        {
            FireShellViewEvent(new ShellViewLoadedEventArgs(ShellViewContext));
        }

        public void TranslateResources(IModelElementKey[] resourceKeys, SuggestTranslationMode translationMode, string onlyLanguage)
        {
            if (!ShellViewModel.AppConfig.EnableTranslation)
            {
                var dialogShowInfo = new DialogShowInfo(Strings.Services.Application.TranslationServiceCaption,
                    Strings.Services.Application.TranslationServiceIsNotEnabled,
                    Strings.Services.Application.TranslationServiceEnableConfirm);
                
                if (DialogService.ShowConfirm(dialogShowInfo).DialogResult == DialogResult.Yes)
                {
                    ApplicationService.ShowAppSettings(AppSettingsDialogPage.Translator);
                }
                else
                {
                    return;
                }
            }

            if (TranslationService.ActiveProvider == null)
            {
                return;
            }

            if (!TranslationService.ActiveProvider.IsConfigured)
            {
                var dialogShowInfo = new DialogShowInfo(Strings.Services.Application.TranslationServiceCaption,
                    Strings.Services.Application.TranslationServiceIsNotConfigured,
                    Strings.Services.Application.TranslationServiceConfigureConfirm);
                
                if (DialogService.ShowConfirm(dialogShowInfo).DialogResult == DialogResult.Yes)
                {
                    ApplicationService.ShowAppSettings(AppSettingsDialogPage.Translator);
                }
            }

            if (ShellViewModel.AppConfig.EnableTranslation &&
                TranslationService.ActiveProvider?.IsConfigured == true)
            {
                var translateAsyncOperation = new TranslationAsyncOperation(ShellViewContext, resourceKeys,
                    translationMode, onlyLanguage);
                DialogService.ShowProgressDialog(translateAsyncOperation);
            }
        }

        private string GetFileNameToOpenOnStart()
        {
            string fileName = null;
            var appConfig = ShellViewModel.AppConfig;

            if (appConfig.OpenLastProjectOnStartup && RecentFilesService.Supported)
            {
                fileName = RecentFilesService.LastOpenedFileName;

                if (!fileName.IsNullOrEmpty() && !File.Exists(fileName))
                {
                    RecentFilesService.Remove(fileName);
                    RecentFilesService.ResetLastOpenedFile();
                    fileName = null;
                }
            }

            if (AppContext.ModelFileFromCmdLine.Active && File.Exists(AppContext.ModelFileFromCmdLine.FileName))
            {
                fileName = AppContext.ModelFileFromCmdLine.FileName;
            }

            return fileName;
        }

        public void OpenProject(string fileName)
        {
            OpenProject(fileName, false);
        }

        private void OpenLastProject(string fileName)
        {
            UIService.DelayedAction(() => { OpenProject(fileName, true); }, TimeSpan.FromMilliseconds(100));
        }

        private void OpenProject(string fileName, bool isLastOpened, int? forceUpgradeFromModel = null)
        {
            CallProjectBusyOperation(() =>
                {
                    InternalOpenProject(fileName, isLastOpened, forceUpgradeFromModel);
                }, ProjectBusyOperationType.OpenProject);
        }

        private void InternalOpenProject(string fileName, bool isLastOpened, int? forceUpgradeFromModel = null)
        {
            CloseProject();

            ShellViewModel.TreeIsStartupFocused = false;

            ProjectLoadResult loadResult;

            if (forceUpgradeFromModel > 0)
            {
                loadResult = new ProjectLoadResult(fileName)
                {
                    LoadStatus = ProjectLoadStatus.ModelUpgradeRequired,
                    ModelVersion = forceUpgradeFromModel.Value,
                    MarkIsDirty = false
                };
            }
            else
            {
                try
                {
                    loadResult = LoadModelContextFromFile(fileName, true);
                }
                catch (Exception e)
                {
                    Logger.Error(Strings.Services.Application.OpenProjectFailed(fileName), e);

                    loadResult = null;
                    DialogService.ShowError(new DialogShowInfo(Strings.Services.Application.OpenProjectCaption,
                        Strings.Services.Application.OpenProjectFailed(fileName)));
                }
            }

            try
            {
                string captionFileCompatIssue = Strings.Services.Application.FileCompatibilityIssueCaption;

                void OnLoadingFailed()
                {
                    if (loadResult != null)
                    {
                        if (isLastOpened)
                        {
                            RecentFilesService.Remove(loadResult.FileName);
                            RecentFilesService.ResetLastOpenedFile();
                            RecentFilesService.Save(ShellViewContext);

                            ShellViewModel.ProjectFileName = string.Empty;
                            ShellViewModel.UpdateTitle();
                        }

                        fileName = Path.GetFullPath(loadResult.FileName);

                        if (loadResult.LoadStatus == ProjectLoadStatus.ModelVersionHigherThanApp)
                        {
                            DialogService.ShowWarning(new DialogShowInfo(captionFileCompatIssue,
                                Strings.Services.Application.OpenProjectFailed(fileName),
                                Strings.Services.Application.FileCompatibilityIssueCreatedWithHigherVersion));
                        }
                        else
                        {
                            string caption = isLastOpened
                                ? Strings.Operations.StartupModelLoadErrorCaption
                                : Strings.Operations.Project.OpenProjectCaption;
                            string message = Strings.Services.Application.OpenProjectFailed(fileName);
                            string detail = loadResult.LoadStatus.ToDisplayName(fileName);
                            
                            DialogService.ShowError(new DialogShowInfo(caption, message, detail));
                        }
                    }
                }

                if (loadResult != null)
                {
                    if (loadResult.LoadStatus == ProjectLoadStatus.Success && loadResult.ModelContext != null)
                    {
                        OpenProject(loadResult.ModelContext, fileName);
                    }
                    else
                    {
                        if (loadResult.LoadStatus == ProjectLoadStatus.ModelUpgradeRequired)
                        {
                            bool doUpgrade = forceUpgradeFromModel > 0;

                            if (!doUpgrade)
                            {
                                var dialogShowInfo = new DialogShowInfo(captionFileCompatIssue,
                                    Strings.Services.Application.FileCompatibilityIssueCreatedInPreviousVersion(fileName),
                                    Strings.Services.Application.FileCompatibilityIssueUpgradeToCurrentVersionConfirm);
                                
                                var confirmResult = DialogService.ShowConfirm(dialogShowInfo).DialogResult;
                                doUpgrade = confirmResult == DialogResult.Yes;
                            }

                            if (doUpgrade)
                            {
                                loadResult = LoadModelContextFromFile(fileName, true, loadResult.ModelVersion);

                                if (loadResult?.LoadStatus == ProjectLoadStatus.Success && loadResult.ModelContext != null)
                                {
                                    if (forceUpgradeFromModel > 0)
                                    {
                                        loadResult.MarkIsDirty = true;
                                        try
                                        {
                                            SaveModelContextToFile(fileName, loadResult.ModelContext);
                                            loadResult.MarkIsDirty = false;
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Error($"Saving upgraded project '{fileName}' failed", e);
                                        }
                                    }

                                    OpenProject(loadResult.ModelContext, fileName);
                                }
                                else
                                {
                                    OnLoadingFailed();
                                }
                            }
                        }
                        else
                        {
                            OnLoadingFailed();
                        }
                    }
                }
            }
            finally
            {
                if (loadResult != null && loadResult.MarkIsDirty)
                {
                    ShellViewModel.MarkAsDirty();
                }
                ShellViewModel.TreeIsStartupFocused = true;
            }
        }

        public ModelContext CreateModelContext(string modelName)
        {
            ModelContext modelContext = InternalCreateModelContext();
            modelContext.CreateModel(modelName);
            return modelContext;
        }

        public OperationResult SaveModelContextToFile(string fileName, ModelContext modelContext)
        {
            ArgumentValidator.EnsureArgumentNotNull(modelContext, "modelContext");
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(fileName, "fileName");

            var saveResult = new OperationResult(OperationResultStatus.Success);

            StartProjectOperationIsBusy(ProjectBusyOperationType.SaveProject);
            try
            {
                string fileContent = ModelFileStorage.Save(modelContext, ModelSaveOptions);
                FileUtils.WriteAllText(fileName, fileContent);
            }
            catch (Exception e)
            {
                saveResult.SetError(Strings.Services.Application.ErrorSavingProjectFileToDiskCaption,
                    Strings.Services.Application.ErrorSavingProjectFileToDiskMessage(fileName));
            }
            finally
            {
                StopProjectOperationIsBusy();
            }

            if (saveResult.IsSuccess)
            {
                if (!File.Exists(fileName))
                {
                    saveResult.SetError(Strings.Services.Application.ErrorSavingProjectFileToDiskCaption,
                        Strings.Services.Application.ErrorSavingProjectFileToDiskMessage(fileName));
                }
            }

            FireShellViewEvent(new ShellViewSavedEventArgs(ShellViewContext, saveResult, fileName));
            return saveResult;
        }

        private ModelContext InternalCreateModelContext()
        {
            return new ModelContext(ModelContextOptions.Default);
        }

        private ProjectLoadResult LoadModelContextFromFile(string fileName, bool createEmptyOnError,
            int? upgradeFromModelVersion = null)
        {
            var result = new ProjectLoadResult(fileName);
            if (fileName.IsNullOrEmpty() && !File.Exists(fileName))
            {
                result.LoadStatus = ProjectLoadStatus.FileNotFoundOrAccessDenied;
            }

            bool upgradingModel = upgradeFromModelVersion.HasValue;

            bool createEmpty = fileName.IsNullOrEmpty()
                || result.LoadStatus == ProjectLoadStatus.FileNotFoundOrAccessDenied;

            ModelContext modelContext = null;

            if (!createEmpty)
            {
                string modelContent = FileUtils.ReadAllText(fileName);

                modelContext = new ModelContext(ModelContextOptions.Default);

                ModelLoadResult loadResult = upgradingModel
                    ? ModelFileStorage.Upgrade(modelContext, modelContent, upgradeFromModelVersion.Value)
                    : ModelFileStorage.Load(modelContext, modelContent);

                result.ModelVersion = loadResult.ModelVersion;
                bool loadedFromFile = loadResult.Status == ModelLoadStatus.Success;
                createEmpty = !loadedFromFile;

                if (loadedFromFile)
                {
                    string suffix = DateTime.Now.Ticks.ToString();
                    string dir = Path.GetDirectoryName(fileName);
                    // save original file to backup file
                    string backupFileName = Path.GetFileNameWithoutExtension(fileName);
                    string fileNamExt = Path.GetExtension(fileName);
                    bool backupCreated = false;

                    if (!string.IsNullOrEmpty(dir))
                    {
                        if (upgradingModel)
                        {
                            try
                            {
                                backupFileName = Path.Combine(dir, $"{backupFileName}-backup_v{upgradeFromModelVersion.Value}_{suffix}") + fileNamExt;
                                FileUtils.WriteAllText(backupFileName, modelContent);
                            }
                            catch (Exception e)
                            {
                                Logger.Error($"Open '{fileName}' failed (upgradingModel: true)", e);

                                var file = backupFileName.IsNullOrEmpty() ? string.Empty : Path.GetFullPath(backupFileName);
                                DialogService.ShowError(new DialogShowInfo(Strings.Services.Application.OpenProjectCaption,
                                    Strings.Services.Application.BackupOfOriginalFileFailed("\n", file)));
                            }

                            backupCreated = true;
                        }
                        else
                        {
                            string tmpModelContent = ModelFileStorage.Save(modelContext, new ModelSaveOptions(false));
                            //if (!tmpModelContent.IsNullOrEmpty() && tmpModelContent != loadResult.RawJson)
                            if (!tmpModelContent.IsNullOrEmpty() && string.Compare(tmpModelContent, loadResult.RawJson, StringComparison.Ordinal) != 0)
                            {
                                try
                                {
                                    backupFileName = Path.Combine(dir, $"{backupFileName}-backup_{suffix}") + fileNamExt;
                                    FileUtils.WriteAllText(backupFileName, modelContent);
                                }
                                catch (Exception e)
                                {
                                    Logger.Error($"Open '{fileName}' failed", e);

                                    var file = backupFileName.IsNullOrEmpty() ? string.Empty : Path.GetFullPath(backupFileName);
                                    DialogService.ShowError(new DialogShowInfo(Strings.Services.Application.OpenProjectCaption,
                                        Strings.Services.Application.BackupOfOriginalFileFailed("\n", file)));
                                }

                                backupCreated = true;
                            }
                        }
                    }

                    if (backupCreated)
                    {
                        result.MarkIsDirty = true;
                    }
                }
                else
                {
                    if (upgradingModel)
                    {
                        result.LoadStatus = ProjectLoadStatus.ModelUpgradeFailed;
                    }
                    else
                    {
                        switch (loadResult.Status)
                        {
                            case ModelLoadStatus.ModelSourceInvalid:
                            case ModelLoadStatus.LoadError:
                                {
                                    result.LoadStatus = ProjectLoadStatus.ModelLoadError;
                                    break;
                                }
                            case ModelLoadStatus.ModelUpgraderRequired:
                                {
                                    result.LoadStatus = loadResult.ModelVersion > ModelConstants.CurrentModelVersion
                                        ? ProjectLoadStatus.ModelVersionHigherThanApp
                                        : ProjectLoadStatus.ModelUpgradeRequired;
                                    break;
                                }
                            default:
                            {
                                throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                }
            }

            if (createEmpty)
            {
                if (createEmptyOnError)
                {
                    if (modelContext == null)
                    {
                        modelContext = new ModelContext(ModelContextOptions.Default);
                    }

                    modelContext.RecreateModel(Strings.ViewModels.NewProject.DefaultModelName);
                    modelContext.AddLanguage(CultureCache.English, true);
                }
                else
                {
                    modelContext = null;
                }
            }

            result.ModelContext = modelContext;

            return result;
        }

        public virtual bool SaveProject(string fileName = null, SaveProjectFlags flags = SaveProjectFlags.None)
        {
            if (fileName.IsNullOrEmpty())
            {
                fileName = ShellViewModel.ProjectFileName;
            }

            // in case when project is closed
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            var fileInfo = new FileInfo(fileName);

            string dialogCaption = Strings.Operations.Project.ProjectSaveErrorTitle;
            if (fileInfo.Exists && fileInfo.IsReadOnly)
            {
                string message = Strings.Services.Application.SaveFailedFileIsReadonly(fileName);
                DialogService.ShowError(new DialogShowInfo(dialogCaption, message));
                return false;
            }

            TreeViewService.ForceRefreshPropertyValidator();

            var result = ShellViewModel.SaveModelToProjectFile(fileName);
            if (result.IsSuccess)
            {
                RecentFilesService.Use(fileName);
                RecentFilesService.Save(ShellViewContext);

                bool generateCode = AppConfig.RunTemplateAfterSave || AppContext.RunInVsPackage;
                if (generateCode && !flags.IsFlagSet(SaveProjectFlags.SkipCodeGeneration))
                {
                    StandaloneCodeGenerate();
                }
            }
            else
            {
                DialogService.ShowError(new DialogShowInfo(dialogCaption, result.Message, result.Detail));
            }

            return result.IsSuccess;
        }
        
        public void StandaloneCodeGenerate()
        {
            if (!ShellViewModel.HasCodeGeneratorItemTemplate)
            {
                var dialogResultInfo = DialogService.ShowConfirm(new DialogShowInfo("Run Code Generator", 
                    "Project does not have associated code generator template !",
                    "Do you want to setup code generator template now ?"), dialogIcon: DialogIcon.Error);

                if (dialogResultInfo.DialogResult == DialogResult.Yes)
                {
                    if (ShellViewModel.Commands.ProjectSettingsCommand.CanExecute(null))
                    {
                        ShellViewModel.Commands.ProjectSettingsCommand.Execute(null);
                    }
                }
                
                return;
            }
            
            var standaloneCodeGenerator = AppContext.StandaloneCodeGeneratorService;

            var startTime = DateTime.UtcNow;
            TimeSpan? elapsedTime = null;

            var codeGenResult = new StandaloneCodeGenerateResult();
            StartProjectOperationIsBusy(ProjectBusyOperationType.GenerateCode);
            _ = Task.Run(async () =>
                    {
                        codeGenResult = await standaloneCodeGenerator.GenerateCodeAsync(ShellViewModel.ProjectFileName, ShellViewModel.ModelContext);
                        elapsedTime = DateTime.UtcNow - startTime;
                    })
                .ContinueWith(_ =>
                    {
                        if (elapsedTime == null)
                        {
                            elapsedTime = DateTime.UtcNow - startTime;
                        }
                        
                        UIService.DispatchActionOnUI(() =>
                            {
                                StopProjectOperationIsBusy();
                                if (!codeGenResult.Success)
                                {
                                    DialogService.ShowError(new DialogShowInfo("Code Generator", "Error generating template code !"));
                                }
                            }, TimeSpan.FromMilliseconds(200));
                    });
        }

        public void UpgradeModelToLatest()
        {
            int modelVersion = ShellViewModel.ModelContext.Model.Version;
            OpenProject(ShellViewModel.ProjectFileName, false, modelVersion);
        }
        
        protected virtual void OpenProject(ModelContext modelContext, string fileName)
        {
            // before open to save 'expanded states'
            RecentFilesService.Save(ShellViewContext);
            ShellViewModel.ChangeModelContext(modelContext);
            ShellViewModel.RemoveIsDirty();
            ShellViewModel.ProjectFileName = fileName;
            ShellViewModel.ApplyTreeExpandedStates();
            ShellViewModel.ShellAreaType = ShellAreaType.EditorArea;

            RecentFilesService.Use(fileName);
            RecentFilesService.Save(ShellViewContext);

            ShellViewModel.CheckCodeGeneratorTemplate();

            // if (AppContext.StandaloneCodeGeneratorService.Available)
            // {
            //     string templateId = modelContext.GetCodeGeneratorTemplateId();
            //     ShellViewModel.CodeGeneratorItemTemplate = templateId;
            //     //ShellViewModel.CheckCodeGeneratorTemplate();
            // }
        }

        public void CloseProject()
        {
            RecentFilesService.Save(ShellViewContext);
            NewProject(Strings.ViewModels.NewProject.DefaultModelName,
                CultureCache.English.Name, new ModelOptions(), null);
        }

        public event EventHandler<ShellContextEventArgs> OnShellViewEvent;

        public void NewProject(string modelName, string primaryLanguage, ModelOptions modelOptions, 
            CodeGeneratorTemplate codeGeneratorTemplate)
        {
            ModelContext modelContext = CreateModelContext(modelName);
            modelContext.AddLanguage(primaryLanguage, true);
            modelContext.Model.Options = modelOptions;

            ShellViewModel.ChangeModelContext(modelContext);
            ShellViewModel.RemoveIsDirty();
            ShellViewModel.ProjectFileName = string.Empty;
            ShellViewModel.ShellAreaType = ShellAreaType.EditorArea;
            RecentFilesService.ResetLastOpenedFile();

            if (codeGeneratorTemplate == null)
            {
                ShellViewModel.CodeGeneratorItemTemplate = string.Empty;
            }
            else
            {
                var metadata = modelContext.GetMetadata<CodeGeneratorMetadata>(CodeGeneratorMetadataDescriptor.UID);
                metadata.TemplateId = codeGeneratorTemplate.Id;
                metadata.Template = codeGeneratorTemplate;
                ShellViewModel.CodeGeneratorItemTemplate = codeGeneratorTemplate.Id;
            }
        }

        public bool CheckProjectIsDirty()
        {
            bool result = !ShellViewModel.ProjectIsDirty;

            if (!result)
            {
                string caption = Strings.Services.Application.DetectedUnsavedChangesCaption;
                string message = Strings.Services.Application.DetectedUnsavedChangesMessage;
                string detail = Strings.Services.Application.DetectedUnsavedChangesDetail;

                var confirmResult = DialogService.ShowConfirm(new DialogShowInfo(caption, message, detail), DialogButtons.YesNoCancel).DialogResult;
                result = confirmResult != DialogResult.Cancel;

                if (confirmResult == DialogResult.Yes)
                {
                    ShellViewModel.Commands.SaveProjectCommand.Execute(null);
                    result = !ShellViewModel.ProjectIsDirty; // if mode is still dirty it means save fails, so do cancel!
                }
            }

            return result;
        }

        // ReSharper disable once UnusedMember.Global
        public void ConfirmAndRestartApp(RestartAppReason reason)
        {
            string confirmCaption;
            string confirmMessage;
            string confirmDetail;

            switch (reason)
            {
                case RestartAppReason.ApplyTheme:
                {
                    confirmCaption = Strings.Services.Dialog.RestartApp.ApplyThemeCaption;
                    confirmMessage = Strings.Services.Dialog.RestartApp.ApplyThemeMessage;
                    confirmDetail = Strings.Services.Dialog.RestartApp.ApplyThemeDetail;
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(reason), reason, null);
                }
            }

            if (DialogService.ShowConfirm(new DialogShowInfo(confirmCaption, confirmMessage, confirmDetail)).DialogResult == DialogResult.Yes)
            {
                if (CheckProjectIsDirty())
                {
                    ApplicationService.Exit(true);
                }
            }
        }

        private void CallProjectBusyOperation(Action action, ProjectBusyOperationType busyType)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            UIService.DelayAdvancedAction(
                () => { ShellViewModel.ProjectBusyOperation = busyType; },
                action,
                StopProjectOperationIsBusy);
        }


        public void StartProjectOperationIsBusy(ProjectBusyOperationType type)
        {
            ShellViewModel.ProjectBusyOperation = type;
        }

        public void StopProjectOperationIsBusy()
        {
            ShellViewModel.ProjectBusyOperation = ProjectBusyOperationType.None;
        }

        // ReSharper disable once UnusedMember.Global
        public void SetStatusInProgress(string statusText, bool inProgress)
        {
            if (inProgress)
            {
                StartStatusInProgress(statusText);
            }
            else
            {
                StopStatusInProgress();
            }
        }

        private void StartStatusInProgress(string statusText)
        {
            ShellViewModel.StatusInProgress = true;
            ShellViewModel.StatusInProgressText = statusText;
        }

        private void StopStatusInProgress()
        {
            ShellViewModel.StatusInProgress = false;
            ShellViewModel.StatusInProgressText = string.Empty;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    UnregisterEvents();
                }
                finally
                {
                    _disposed = true;
                }
            }
        }

        public void FireShellViewEvent(ShellContextEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            if (eventArgs is ShellViewLoadedEventArgs shellViewLoadedEventArgs)
            {
                try
                {
                    HandleShellViewLoadedEvent(shellViewLoadedEventArgs);
                }
                catch (Exception e)
                {
                    Logger.Error(
                        $"{nameof(ShellService)}.{nameof(FireShellViewEvent)} '{eventArgs.GetType().FullName}' calling HandleShellViewLoadedEvent failed",
                        e);
                }
            }

            try
            {
                OnShellViewEvent?.Invoke(this, eventArgs);
            }
            catch (Exception e)
            {
                Logger.Error($"{nameof(ShellService)}.{nameof(FireShellViewEvent)} '{eventArgs.GetType().FullName}' failed", e);
            }
        }
    }
}