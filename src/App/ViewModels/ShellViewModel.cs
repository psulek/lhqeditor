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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Components;
using LHQ.App.Components.VirtualTreeListViewControl;
using LHQ.App.Dialogs;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Implementation;
using LHQ.App.Services.Implementation.Messaging;
using LHQ.App.Services.Implementation.Undo;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.App.ViewModels.Elements;
using LHQ.Core.Model;
using LHQ.Data;
using LHQ.Data.CodeGenerator;
using LHQ.Data.Extensions;
using LHQ.Data.Templating.Templates;
using LHQ.Utils;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using UndoAction = LHQ.App.Services.Implementation.Undo.UndoAction;

namespace LHQ.App.ViewModels
{
    public class ShellViewModel : ObservableModelObject, IHasShellViewContext
    {
        private bool _runInVsPackage;

        private bool _showMainMenu;
        private MainMenuViewModel _mainMenu;
        private ObservableCollection<RootModelViewModel> _treeRoot;
        private RootModelViewModel _rootModel;
        private MenuViewModel _menu;
        private bool _isTreeViewFocused;
        private Version _appVersion;
        private string _title;
        private bool _projectIsDirty;
        private string _projectFileName;
        private string _highlightTreeText;

        private bool _isTranslationMode;

        private bool _statusInProgress;

        private bool _lastFreshSave;
        private bool _showOnlyUntranslated;

        private string _lastSearchText;
        private int _lastSearchElementIndex;
        private ShellAreaType _shellAreaType;
        private TreeCollectionView _treeElementsView;
        private string _statusInProgressText;
        private ProjectBusyOperationType _projectBusyOperation;
        private string[] _treeSearchSavedExpandedKeys;
        private bool _treeSearchIsActive;
        private InputBinding _undoInputBinding;
        private InputBinding _redoInputBinding;
        private bool _categoriesSupported;
        private bool _treeIsStartupFocused;
        private ValidationPanelViewModel _validationPanel;
        private string _selectedElementPaths;
        private HintPanelViewModel _hintPanelViewModel;
        private string _codeGeneratorItemTemplate;
        private HintPanelViewModel _newVersionPanelViewModel;

        public ShellViewModel(ShellViewContext shellViewContext, ModelContext modelContext, bool showMainMenu)
            : base(shellViewContext.AppContext, true)
        {
            ShellViewContext = shellViewContext;
            shellViewContext.ShellViewModel = this;
            ModelContext = modelContext;

            DetectTranslationMode();

            AppContext.OnAppEvent += HandleAppEvent;

            TreeSearchIsActive = false;
            SelectedItemChangedCommand = new Syncfusion.Windows.Shared.DelegateCommand<object>(OnSelectedItemChanged);
            ShellAreaType = ShellAreaType.EmptyArea;

            UndoCommands = new ObservableCollectionExt<string>();
            RedoCommands = new ObservableCollectionExt<string>();

            InputBindings = new List<(InputBinding binding, bool applyToView)>();
            ShellInstanceId = Guid.NewGuid();
            AppVersion = AppContext.AppCurrentVersion;

            _projectFileName = string.Empty;
            UpdateTitle();

            ShellViewContext.UndoManager.Changed += UndoManagerOnChanged;

            SelectionChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(OnSelectionChanged);
            LoadedCommand = new DelegateCommand<RoutedEventArgs>(OnShellLoaded);
            TreeSearchCommand = new DelegateCommand<SearchEventArgs>(TreeSearchExecute, TreeSearchCanExecute);
            TreeSearchCancelCommand = new DelegateCommand(TreeSearchCancelExecute);
            TreeSearchExitCommand = new DelegateCommand(TreeSearchExitExecute);
            ErrorNavigateCommand = new DelegateCommand<MouseButtonEventArgs>(ErrorNavigateExecute);

            PreviewKeyDownCommand = new DelegateCommand<KeyEventArgs>(OnPreviewKeyDownCommand);

            TreeLegendExpandedChangeCommand = new DelegateCommand(TreeLegendExpandedChangeExecute);

            CodeGeneratorItemTemplateNavigateCommand = new DelegateCommand(CodeGeneratorItemTemplateNavigateExecute);

            _isTreeViewFocused = true;
            ShowMainMenu = showMainMenu;

            RunInVsPackage = AppContext.RunInVsPackage;

            TreeElementsView = new TreeCollectionView();
            MultiSelectionViewModel = new MultiSelectionViewModel(shellViewContext);
            ValidationPanel = new ValidationPanelViewModel(shellViewContext);

            RootModel = new RootModelViewModel(shellViewContext, ModelContext.Model);
            ApplyTreeExpandedStates();

            Commands = new CommandsViewModel(shellViewContext);

            if (ShowMainMenu)
            {
                MainMenu = new MainMenuViewModel(shellViewContext, Commands);
            }

            Menu = new MenuViewModel(shellViewContext, Commands);

            HintPanelViewModel = new HintPanelViewModel(shellViewContext, AppHintType.CodeGenerator);
            NewVersionPanelViewModel = new HintPanelViewModel(shellViewContext, AppHintType.NewVersion);
        }

        public CodeGeneratorTemplate GetCodeGeneratorTemplate()
        {
            string templateId = HasCodeGeneratorItemTemplate ? CodeGeneratorItemTemplate : string.Empty;
            return ModelContext.GetCodeGeneratorTemplate(templateId);
        }

        public void CheckCodeGeneratorTemplate()
        {
            if (!ModelContext.HasCodeGeneratorTemplate())
            {
                CodeGeneratorTemplate template = GetCodeGeneratorTemplate();
                var metadata = ModelContext.GetMetadata<CodeGeneratorMetadata>(CodeGeneratorMetadataDescriptor.UID);
                metadata.TemplateId = template.Id;
                metadata.Template = template;
                if (!ShellService.SaveProject(hostEnvironmentSave: true))
                {
                    DialogService.ShowError(Strings.Operations.Project.ProjectSaveErrorTitle,
                        Strings.Operations.Project.ProjectSaveFailed, 
                        Strings.Operations.Project.ProjectSaveFailedFileIsReadOnly(ProjectFileName),
                        displayType: AppMessageDisplayType.HostDialog);
                }

                ModelContext.HasCodeGeneratorTemplate();
            }
        }

        private void CodeGeneratorItemTemplateNavigateExecute(object obj)
        {
            string templateId = HasCodeGeneratorItemTemplate ? CodeGeneratorItemTemplate : string.Empty;
            (bool submitted, CodeGeneratorTemplate template) = CodeGeneratorDialog.DialogShow(ShellViewContext, templateId);
            if (submitted)
            {
                var metadata = ModelContext.GetMetadata<CodeGeneratorMetadata>(CodeGeneratorMetadataDescriptor.UID);
                metadata.TemplateId = template.Id;
                metadata.Template = template;
                if (!ShellService.SaveProject(hostEnvironmentSave: true))
                {
                    DialogService.ShowError(Strings.Operations.Project.ProjectSaveErrorTitle,
                        Strings.Operations.Project.ProjectSaveFailed, 
                        Strings.Operations.Project.ProjectSaveFailedFileIsReadOnly(ProjectFileName),
                        displayType: AppMessageDisplayType.HostDialog);
                }
            }
        }

        private void HandleAppEvent(object sender, ApplicationEventArgs e)
        {
        }

        public IShellViewContext ShellViewContext { get; }

        internal ModelContext ModelContext { get; private set; }

        private IRecentFilesService RecentFilesService => AppContext.RecentFilesService;

        public IValidatorContext ValidatorContext => ShellViewContext.ValidatorContext;

        private ITreeViewService TreeViewService => ShellViewContext.TreeViewService;

        public ModelOptions ModelOptions => RootModel.ModelOptions;

        public IMessenger Messenger => ShellViewContext.Messenger;

        public IUndoManager UndoManager => ShellViewContext.UndoManager;

        public IApplicationService ApplicationService => AppContext.ApplicationService;

        public IShellService ShellService => ShellViewContext.ShellService;

        public MultiSelectionViewModel MultiSelectionViewModel { get; }

        public CommandsViewModel Commands { get; }

        public bool CategoriesSupported
        {
            get => _categoriesSupported;
            set => SetProperty(ref _categoriesSupported, value);
        }

        public ValidationPanelViewModel ValidationPanel
        {
            get => _validationPanel;
            set => SetProperty(ref _validationPanel, value);
        }

        public MainMenuViewModel MainMenu
        {
            get => _mainMenu;
            private set => SetProperty(ref _mainMenu, value);
        }

        public bool ShowMainMenu
        {
            get => _showMainMenu;
            private set => SetProperty(ref _showMainMenu, value);
        }

        public RootModelViewModel RootModel
        {
            get => _rootModel;
            set
            {
                _rootModel = value;
                RaisePropertyChanged();

                if (value != null)
                {
                    TreeRoot = new ObservableCollection<RootModelViewModel>
                    {
                        value
                    };
                }
            }
        }

        public ICommand SelectedItemChangedCommand { get; set; }

        public string SelectedElementPaths
        {
            get => _selectedElementPaths;
            set => SetProperty(ref _selectedElementPaths, value);
        }

        public ObservableCollection<RootModelViewModel> TreeRoot
        {
            get => _treeRoot;
            set => SetProperty(ref _treeRoot, value);
        }

        public TreeCollectionView TreeElementsView
        {
            get => _treeElementsView;
            set => SetProperty(ref _treeElementsView, value);
        }

        public bool IsTreeViewFocused
        {
            get
            {
                bool result = _isTreeViewFocused;
                if (_isTreeViewFocused)
                {
                    _isTreeViewFocused = false;
                }

                return result;
            }
            set => SetProperty(ref _isTreeViewFocused, value);
        }

        public MenuViewModel Menu
        {
            get => _menu;
            set => SetProperty(ref _menu, value);
        }

        public Version AppVersion
        {
            get => _appVersion;
            private set => SetProperty(ref _appVersion, value);
        }

        public ObservableCollectionExt<string> UndoCommands { get; }

        public ObservableCollectionExt<string> RedoCommands { get; }

        public string ProjectFileName
        {
            get => _projectFileName;
            set
            {
                if (value != _projectFileName)
                {
                    _projectFileName = value;
                    RaisePropertyChanged();
                    UpdateTitle();
                }
            }
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public bool ProjectIsDirty
        {
            get => _projectIsDirty;
            set
            {
                if (_projectIsDirty != value)
                {
                    _projectIsDirty = value;
                    RaisePropertyChanged();
                    UpdateTitle();
                }
            }
        }

        public bool ShowOnlyUntranslated
        {
            get => _showOnlyUntranslated;
            set => SetProperty(ref _showOnlyUntranslated, value);
        }

        public bool TreeSearchIsActive
        {
            get => _treeSearchIsActive;
            set => SetProperty(ref _treeSearchIsActive, value);
        }

        public string HighlightTreeText
        {
            get => _highlightTreeText;
            set => SetProperty(ref _highlightTreeText, value);
        }

        public bool IsEditorMode => !_isTranslationMode;

        public bool IsTranslationMode
        {
            get => _isTranslationMode;
            set
            {
                if (_isTranslationMode != value)
                {
                    _isTranslationMode = value;
                    RaisePropertyChanged();
                    // ReSharper disable once ExplicitCallerInfoArgument
                    RaisePropertyChanged(nameof(IsEditorMode));
                }
            }
        }

        public HintPanelViewModel NewVersionPanelViewModel
        {
            get => _newVersionPanelViewModel;
            set => SetProperty(ref _newVersionPanelViewModel, value);
        }

        public HintPanelViewModel HintPanelViewModel
        {
            get => _hintPanelViewModel;
            set => SetProperty(ref _hintPanelViewModel, value);
        }

        public ShellAreaType ShellAreaType
        {
            get => _shellAreaType;
            set => SetProperty(ref _shellAreaType, value);
        }

        public string StatusInProgressText
        {
            get => _statusInProgressText;
            set => SetProperty(ref _statusInProgressText, value);
        }

        public bool StatusInProgress
        {
            get => _statusInProgress;
            set => SetProperty(ref _statusInProgress, value);
        }

        public ProjectBusyOperationType ProjectBusyOperation
        {
            get => _projectBusyOperation;
            set => SetProperty(ref _projectBusyOperation, value);
        }

        public bool RunInVsPackage
        {
            get => _runInVsPackage;
            set => SetProperty(ref _runInVsPackage, value);
        }

        public bool TreeIsStartupFocused
        {
            get => _treeIsStartupFocused;
            set => SetProperty(ref _treeIsStartupFocused, value);
        }

        public string CodeGeneratorItemTemplate
        {
            get => _codeGeneratorItemTemplate;
            set
            {
                SetProperty(ref _codeGeneratorItemTemplate, value);
                RaisePropertyChanged(nameof(HasCodeGeneratorItemTemplate));
            }
        }

        public bool HasCodeGeneratorItemTemplate => !string.IsNullOrEmpty(CodeGeneratorItemTemplate);

        public DelegateCommand CodeGeneratorItemTemplateNavigateCommand { get; }

        public DelegateCommand<RoutedEventArgs> LoadedCommand { get; }

        public DelegateCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; }

        public DelegateCommand<KeyEventArgs> PreviewKeyDownCommand { get; }

        public DelegateCommand<SearchEventArgs> TreeSearchCommand { get; }

        public DelegateCommand<MouseButtonEventArgs> ErrorNavigateCommand { get; }

        public DelegateCommand TreeSearchCancelCommand { get; }

        public DelegateCommand TreeSearchExitCommand { get; }

        public DelegateCommand EditElementCommand { get; private set; }

        public List<(InputBinding binding, bool applyToView)> InputBindings { get; }

        public Window MainWindow { get; set; }

        public AppConfig AppConfig => AppContext.AppConfigFactory.Current;

        // Id to identify within one visual studio (when runs as extension) but multiple editor windows to identify correct instance!
        public Guid ShellInstanceId { get; }

        public Style HighlightSearchTreeItemStyle { get; set; }

        public ICommand TreeLegendExpandedChangeCommand { get; }

        internal ShellView ShellView { get; set; }

        public void UpdateTree()
        {
            TreeElementsView.Collection = new TreeElementViewModelCollection(RootModel);
            var sourceCollection = (TreeElementViewModelCollection)TreeElementsView.SourceCollection;
            LoadTree(sourceCollection);
        }

        internal void ResetTree()
        {
            TreeElementsView.Collection.Clear();
        }

        internal void TreeVisualRefresh()
        {
            TreeElementsView.Refresh();
        }

        private void LoadTree(TreeElementViewModelCollection collection)
        {
            collection.FreezeUpdate();
            try
            {
                collection.RootNode.ChildCount = 0;
                collection.LoadTree();
            }
            finally
            {
                collection.ThawUpdate();
            }
        }

        public string GetProjectDirectory()
        {
            return ProjectFileName.IsNullOrEmpty()
                ? string.Empty
                : Path.GetDirectoryName(ProjectFileName);
        }

        private void OnPreviewKeyDownCommand(KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                RootModel.SelectedTreeElement.FocusName = true;
            }
            else if (Keyboard.IsKeyDown(Key.Escape))
            {
                TreeSearchCancelCommand.Execute(null);
            }
        }

        private void OnSelectedItemChanged(object obj)
        { }

        private void ErrorNavigateExecute(MouseButtonEventArgs obj)
        { }

        private void TreeLegendExpandedChangeExecute(object expanded)
        {
            AppContext.AppStateStorageService.Update(storage => { storage.TreeLegendExpanded = (bool)expanded; });
        }

        internal void ApplyTreeExpandedStates()
        {
            if (!ProjectFileName.IsNullOrEmpty())
            {
                List<string> expandedKeys = RecentFilesService.GetExpandedKeys(ProjectFileName);
                if (expandedKeys != null && expandedKeys.Count > 0)
                {
                    TreeViewService.ApplyTreeExpandedKeys(expandedKeys);
                }
            }
        }

        private void TreeSearchExitExecute(object obj)
        {
            ITreeElementViewModel element = TreeViewService.GetSelection().Element;

            Messenger.SingleDispatch(MessengerDefinitions.FocusTree,
                MessengerDefinitions.ReSelectNodeInTree(element));
        }

        private void TreeSearchCancelExecute(object obj)
        {
            if (HighlightTreeText.IsNullOrEmpty())
            {
                return;
            }

            HighlightTreeText = null;

            ITreeElementViewModel selectedTreeElement = RootModel.SelectedTreeElement;
            if (selectedTreeElement != null)
            {
                Messenger.SingleDispatch(MessengerDefinitions.FocusTree,
                    MessengerDefinitions.ReSelectNodeInTree(selectedTreeElement));
            }
            else
            {
                Messenger.SingleDispatch(MessengerDefinitions.FocusTree,
                    MessengerDefinitions.ReSelectNodeInTree(TreeViewService.RootModel));
            }
        }

        private bool TreeSearchCanExecute(SearchEventArgs arg)
        {
            return ProjectBusyOperation == ProjectBusyOperationType.None;
        }

        private void TreeSearchExecute(SearchEventArgs args)
        {
            ShellService.StartProjectOperationIsBusy(ProjectBusyOperationType.TreeSearch);

            bool treeSearchWasActive = TreeSearchIsActive;

            string searchText = args == null ? HighlightTreeText : args.Text;

            if (_lastSearchText != searchText)
            {
                _lastSearchText = searchText;
                _lastSearchElementIndex = -1;
            }

            bool onlyUntranslated = ShowOnlyUntranslated;
            bool isEmptyText = searchText.IsNullOrEmpty();
            Regex regex = isEmptyText ? null : new Regex("(" + searchText + ")", RegexOptions.IgnoreCase);

            HighlightTreeText = searchText;

            TreeSearchIsActive = !HighlightTreeText.IsNullOrEmpty();

            _treeSearchSavedExpandedKeys = TreeViewService.GetTreeExpandedKeys();

            bool restoreSavedExpandedKeys = treeSearchWasActive && !TreeSearchIsActive &&
                _treeSearchSavedExpandedKeys != null &&
                _treeSearchSavedExpandedKeys.Length > 0;

            var treeSelectionInfo = TreeViewService.GetSelection();
            var currentSelectedElement = treeSelectionInfo.IsSingleSelection ? treeSelectionInfo.Element : null;

            List<ITreeElementViewModel> matchedElements;

            using (ShellViewContext.PropertyChangeObserver.BlockAllNotifications())
            {
                matchedElements = TreeViewService.FindAllElements(x =>
                        {
                            bool isMatched = isEmptyText || regex.IsMatch(x.Name);
                            if (isMatched && onlyUntranslated)
                            {
                                if (x.ElementType == TreeElementType.Resource)
                                {
                                    isMatched = x.IsTranslationValid == false;
                                }
                                else if (x.ElementType == TreeElementType.Category)
                                {
                                    isMatched = false;
                                }
                            }

                            if (x.IsVisible != isMatched)
                            {
                                x.IsVisible = isMatched;
                            }
                            return isMatched;
                        })
                    .ToList();
            }

            ShellViewContext.PropertyChangeObserver.RaisePropertiesChanged(matchedElements.Cast<ObservableModelObject>());

            var dispatchActionQueue = UIService.CreateDispatchActionQueue();
            {
                dispatchActionQueue
                    .Add(() => { matchedElements.ForEach(x => { x.ExpandParentsToRoot(AppContext); }); })
                    .Add(() =>
                        {
                            _lastSearchElementIndex++;

                            if (_lastSearchElementIndex >= matchedElements.Count)
                            {
                                _lastSearchElementIndex = 0;
                            }

                            for (var i = 0; i < matchedElements.Count; i++)
                            {
                                ITreeElementViewModel matchedElement = matchedElements[i];

                                if (_lastSearchElementIndex == i && !isEmptyText)
                                {
                                    TreeViewService.Select(matchedElement);
                                }
                            }
                        });

                if (isEmptyText && restoreSavedExpandedKeys)
                {
                    dispatchActionQueue.Add(() =>
                        {
                            TreeViewService.Collapse(RootModel, true);
                            TreeViewService.ApplyTreeExpandedKeys(_treeSearchSavedExpandedKeys);
                        });
                }

                if (currentSelectedElement != null && currentSelectedElement.IsVisible)
                {
                    dispatchActionQueue.Add(() =>
                        {
                            Messenger.SingleDispatch(MessengerDefinitions.TreeNodeScrollIntoView(currentSelectedElement));
                        });
                }

                dispatchActionQueue
                    .Add(ShellService.StopProjectOperationIsBusy)
                    .Run();
            }
        }

        internal void UpdateTitle()
        {
            string newtitle = ProjectFileName.IsNullOrEmpty()
                ? Strings.App.Title
                : "{0} - {1}".FormatWith(Strings.App.Title, ProjectFileName);

            if (ProjectIsDirty)
            {
                newtitle += " *";
            }

#if DEBUG
            newtitle = "{0} ({1})".FormatWith(newtitle, AppVersion.ToString());
#endif

            Title = newtitle;
        }

        public IViewModelElement GetViewModel(ModelElementType modelElementType, string elementKey, string elementParentKey,
            out ITreeElementViewModel treeElement)
        {
            IViewModelElement result = null;
            treeElement = null;

            if (modelElementType == ModelElementType.Model)
            {
                if (RootModel.ElementKey.EqualsTo(elementKey))
                {
                    treeElement = RootModel;
                    result = RootModel;
                }
            }
            else
            {
                switch (modelElementType)
                {
                    case ModelElementType.Language:
                    {
                        result = TreeViewService.RootModel.FindLanguageByKey(elementKey);
                        break;
                    }
                    case ModelElementType.Category:
                    case ModelElementType.Resource:
                    {
                        result = TreeViewService.FindElementByKey(elementKey);
                        treeElement = (ITreeElementViewModel)result;
                        break;
                    }
                    case ModelElementType.ResourceParameter:
                    {
                        throw new NotSupportedException("ResourceParameter");
                    }
                    case ModelElementType.ResourceValue:
                    {
                        treeElement = TreeViewService.FindElementByKey(elementParentKey);
                        var resource = treeElement as ResourceViewModel;
                        result = resource?.FindValueByKey(elementKey);
                        break;
                    }
                    default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(modelElementType));
                    }
                }
            }

            return result;
        }

        private void UndoManagerOnChanged(object sender, UndoActionArgs args)
        {
            var undoManager = (IUndoManager)sender;
            UndoCommands.Clear();
            UndoCommands.AddRange(undoManager.GetUndoCommands().Select(x => x.GetName()));
            RedoCommands.Clear();
            RedoCommands.AddRange(undoManager.GetRedoCommands().Select(x => x.GetName()));

            ProjectIsDirty = undoManager.CanUndo || undoManager.CanRedo && _lastFreshSave;

            if (!ProjectIsDirty)
            {
                ProjectIsDirty = HasModelAnyChanges();
            }

            if (args.Action.In(UndoAction.Redo, UndoAction.Redo) && _lastFreshSave)
            {
                _lastFreshSave = false;
            }

            Menu.RefreshUndoLocalizations();
        }

        private bool HasModelAnyChanges()
        {
            bool hasChanges = RootModel.Languages.Any(x => x.HasChanges);
            if (!hasChanges)
            {
                foreach (ITreeElementViewModel treeElement in TreeViewService.GetChildsAsFlatList(null))
                {
                    hasChanges = treeElement.HasChanges;

                    if (!hasChanges && treeElement.ElementType == TreeElementType.Resource)
                    {
                        var resourceElement = (ResourceViewModel)treeElement;
                        hasChanges = resourceElement.Values.Any(x => x.HasChanges);
                    }

                    if (hasChanges)
                    {
                        break;
                    }
                }
            }

            return hasChanges;
        }

        private void OnShellLoaded(RoutedEventArgs args)
        {
            var userControl = args.Source as UserControl;

            var inputBindings = InputBindings.Where(x => x.applyToView).Select(x => x.binding).ToList();

            userControl?.InputBindings.AddRange(inputBindings);

            ShellService.NotifyShellLoaded();
        }

        internal void AddInputBinding(ICommand command, KeyGesture keyGesture, bool applyToView)
        {
            // when running as standalone app, always apply to view!
            if (!RunInVsPackage)
            {
                applyToView = true;
            }

            var inputBinding = new InputBinding(command, keyGesture);
            InputBindings.Add((inputBinding, applyToView));

            if (keyGesture.Key == Key.Z && keyGesture.Modifiers == ModifierKeys.Control)
            {
                _undoInputBinding = inputBinding;
            }
            else if (keyGesture.Key == Key.Y && keyGesture.Modifiers == ModifierKeys.Control)
            {
                _redoInputBinding = inputBinding;
            }
        }

        public void AttachUndoRedoInputBindings(FrameworkElement control)
        {
            if (control.InputBindings.IndexOf(_undoInputBinding) == -1)
            {
                control.InputBindings.Add(_undoInputBinding);
            }

            if (control.InputBindings.IndexOf(_redoInputBinding) == -1)
            {
                control.InputBindings.Add(_redoInputBinding);
            }
        }

        internal void ChangeModelContext(ModelContext modelContext)
        {
            ArgumentValidator.EnsureArgumentNotNull(modelContext, "modelContext");
            CloseCurrent();

            ModelContext = modelContext;

            HighlightTreeText = null;
            RootModel.RebuildModel(ModelContext.Model);

            UndoManager.Reset();
            Messenger.SingleDispatch(MessengerDefinitions.FocusTree);
            _lastFreshSave = false;

            DetectTranslationMode();
        }

        private void DetectTranslationMode()
        {
            //IsTranslationMode = ModelContext.HasMetadata<ModelBranchMetadata>(ModelContext.Model);
            // TODO: Detect translation mode!
            IsTranslationMode = false;
        }

        internal void NotifySaveToElement(ViewModelBase viewModel, ModelElementBase element)
        { }

        internal OperationResult SaveModelToProjectFile(string fileName)
        {
            var saveResult = new OperationResult(OperationResultStatus.Success);

            try
            {
                FlushPendingChanges();

                saveResult = ValidateBeforeSave(fileName);
                if (saveResult.IsSuccess)
                {
                    RootModel.UpdateVersion(true); // increase model version on save

                    Data.Model model = RootModel.SaveToModelElement();
                    ModelContext.AssignModelFrom(model);
                    saveResult = ShellService.SaveModelContextToFile(fileName, ModelContext);
                    _lastFreshSave = saveResult.IsSuccess;
                }
            }
            catch (Exception e)
            {
                saveResult.SetError(
                    Strings.Services.Application.ErrorSavingProjectFileToDiskCaption,
                    Strings.Services.Application.ErrorSavingProjectFileToDiskMessage(fileName));

                DebugUtils.Log($"ApplicationService.SaveModelContextToFile '{fileName}', error: {e.GetFullExceptionDetails()}");
            }
            finally
            {
                if (saveResult.IsFailure)
                {
                    RootModel.UpdateVersion(false); // decrease model version on save failure
                }
            }

            if (saveResult.IsSuccess)
            {
                RemoveIsDirty();
                ProjectFileName = fileName;
            }

            return saveResult;
        }

        public void FlushPendingChanges()
        {
            ITreeElementViewModel selectedTreeElement = RootModel.SelectedTreeElement;
            if (selectedTreeElement != null)
            {
                selectedTreeElement.FlushCurrentChange = true;
                selectedTreeElement.FlushCurrentChange = false;
            }
        }

        private OperationResult ValidateBeforeSave(string fileName)
        {
            var result = new OperationResult(OperationResultStatus.Success);
            var errorCaption = Strings.Services.Application.ProjectChangesCantSaveCaption;

            if (!ModelOptions.Categories)
            {
                bool hasAnyCategory = TreeViewService.FindAllElements(x => x.ElementType == TreeElementType.Category).Any();
                if (hasAnyCategory)
                {
                    result.SetError(errorCaption, Strings.Services.Application.SaveFailedProjectLayout1Error);
                }
            }

            if (ModelOptions.Categories && ModelOptions.Resources == ModelOptionsResources.Categories)
            {
                if (RootModel.Children.Any(x => x.ElementType == TreeElementType.Resource))
                {
                    result.SetError(errorCaption,
                        Strings.Services.Application.SaveFailedProjectLayout2Error);
                }
            }

            if (ValidatorContext.HasErrors)
            {
                result.SetError(errorCaption, 
                    Strings.Services.Application.SaveFailedProjectContainsValidationErrors);
            }

            return result;
        }

        private void CloseCurrent()
        {
            TreeViewService.ResetTree();
            TreeViewService.Unselect(RootModel);
        }

        private void OnSelectionChanged(SelectionChangedEventArgs args)
        {
            var tree = (VirtualTreeListView)args.Source;
            IList selectedItems = tree.SelectedItems;

            List<ITreeElementViewModel> selectedElements = selectedItems
                .Cast<VirtualTreeListViewNode>()
                .Select(x => x.Tag)
                .Cast<ITreeElementViewModel>()
                .ToList();

            RootModel.SelectedTreeElements = selectedElements;

            var selection = TreeViewService.GetSelection();

            string elementPaths = string.Empty;

            if (selection.IsSingleSelection)
            {
                var parentNames = GetSelectionParentNames(selection);

                elementPaths = Ellipsis.Compact(string.Join(@"/", parentNames), 40, EllipsisFormat.Start);
                elementPaths = elementPaths.StartsWith("/") ? elementPaths : "/" + elementPaths;
                if (selection.IsSingleSelection && selection.ElementIsResource)
                {
                    elementPaths += "/" + selection.Element.Name;
                }
            }

            SelectedElementPaths = elementPaths;
        }

        public void OnClosed()
        {
            ShellViewContext.ClipboardManager.Clear();
            RecentFilesService.Save(ShellViewContext);
        }

        public void OnClosing(CancelEventArgs args)
        {
            if (ProjectIsDirty)
            {
                DialogResult confirmResult = DialogService.ShowConfirm(
                    Strings.Operations.ExitApp.ConfirmCaption, 
                    Strings.Operations.ExitApp.ConfirmMessage, 
                    Strings.Operations.ExitApp.ConfirmDetail, DialogButtons.YesNoCancel);

                args.Cancel = confirmResult == DialogResult.Cancel;
                if (confirmResult == DialogResult.Yes)
                {
                    Commands.SaveProjectCommand.Execute(null);
                }
            }
        }

        internal void RemoveIsDirty()
        {
            ProjectIsDirty = false;
        }

        internal void MarkAsDirty()
        {
            ProjectIsDirty = true;
            Menu.RefreshUndoLocalizations();
        }

        public List<string> GetSelectionParentNames(TreeSelectionInfo selection)
        {
            if (selection == null)
            {
                selection = TreeViewService.GetSelection();
            }

            List<string> parentRecursiveNames = null;
            ITreeElementViewModel parentCategory = selection.ParentElement;
            if (selection.ElementIsCategory)
            {
                parentRecursiveNames = selection.ElementAsCategory.GetParentNames(true, true);
            }
            else if (selection.ElementIsResource)
            {
                parentRecursiveNames = parentCategory.GetParentNames(true, true);
            }

            if (parentRecursiveNames == null)
            {
                parentRecursiveNames = new List<string>();
            }

            if (parentRecursiveNames.Count == 0)
            {
                parentRecursiveNames.Insert(0, RootModel.Name);
            }

            return parentRecursiveNames;
        }
    }
}
