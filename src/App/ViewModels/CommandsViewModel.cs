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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Dialogs;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Implementation.Messaging;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Dialogs;
using LHQ.App.ViewModels.Dialogs.AppSettings;
using LHQ.App.ViewModels.Elements;
using LHQ.Data;
using LHQ.Utils.Extensions;
using Microsoft.Win32;

namespace LHQ.App.ViewModels
{
    public class CommandsViewModel : ShellViewModelBase, IAppCommands
    {
        public CommandsViewModel(IShellViewContext shellViewContext)
            : base(shellViewContext, true)
        {
            FocusTreePanelCommand = CreateMenuDelegateCommand(FocusTreePanelExecute,
                new KeyGesture(Key.D1, ModifierKeys.Alt, "Alt+1"));

            SaveProjectCommand = CreateMenuDelegateCommand(SaveProjectExecute, SaveProjectCanExecute,
                new KeyGesture(Key.S, ModifierKeys.Control, "Ctrl+S"));

            SaveProjectAsCommand = CreateDelegateCommand(SaveProjectAsExecute, SaveProjectAsCanExecute);

            NewCategoryCommand = CreateMenuDelegateCommand(NewCategoryExecute, NewCategoryCanExecute,
                new KeyGesture(Key.Insert, ModifierKeys.Control, "Ctrl+Ins"), false);

            NewResourceCommand = CreateMenuDelegateCommand(NewResourceExecute, NewResourceCanExecute,
                new KeyGesture(Key.Insert, ModifierKeys.None, "Insert"), false);

            RenameCommand = CreateMenuDelegateCommand(RenameExecute, RenameCanExecute,
                new KeyGesture(Key.F2, ModifierKeys.None, "F2"));

            DeleteElementCommand = CreateMenuDelegateCommand(DeleteElementExecute, DeleteElementCanExecute,
                new KeyGesture(Key.Delete, ModifierKeys.None, "Delete"));

            CutElementCommand = CreateMenuDelegateCommand(CutElementExecute, CutElementCanExecute,
                new KeyGesture(Key.X, ModifierKeys.Control, "Ctrl+X"));

            CopyElementCommand = CreateMenuDelegateCommand(CopyElementExecute, CopyElementCanExecute,
                new KeyGesture(Key.C, ModifierKeys.Control, "Ctrl+C"));

            CopyElementAsSpecial1Command = CreateMenuDelegateCommand(CopyElementAsSpecial1Execute, CopyElementAsSpecial1CanExecute,
                new KeyGesture(Key.C, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift+C"), false);

            CopyElementAsSpecial2Command = CreateMenuDelegateCommand(CopyElementAsSpecial2Execute, CopyElementAsSpecial2CanExecute,
                new KeyGesture(Key.C, ModifierKeys.Control | ModifierKeys.Alt, "Ctrl+Alt+C"), false);

            MarkForExportCommand = CreateMenuDelegateCommand(MarkForExportCommandExecute, MarkForExportCommandCanExecute,
                new KeyGesture(Key.M, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift+M"), false);

            UnMarkForExportCommand = CreateMenuDelegateCommand(UnMarkForExportCommandExecute, UnMarkForExportCommandCanExecute,
                new KeyGesture(Key.U, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift+U"), false);

            UnMarkAllForExportCommand = CreateMenuDelegateCommand(UnMarkAllForExportCommandExecute, UnMarkAllForExportCommandCanExecute,
                new KeyGesture(Key.U, ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt, "Ctrl+Shift+Alt+U"), false);

            ExportMarkedCommand = CreateDelegateCommand(ExportMarkedCommandExecute, ExportMarkedCommandCanExecute);

            PasteElementCommand = CreateMenuDelegateCommand(PasteElementExecute, PasteElementCanExecute,
                new KeyGesture(Key.V, ModifierKeys.Control, "Ctrl+V"));

            DuplicateElementCommand = CreateMenuDelegateCommand(DuplicateElementExecute, DuplicateElementCanExecute,
                 new KeyGesture(Key.D, ModifierKeys.Control, "Ctrl+D"), false);

            LanguageSettingsCommand = CreateDelegateCommand(LanguageSettingsExecute, LanguageSettingsCanExecute);
            StandaloneCodeGenerateCommand = CreateDelegateCommand(StandaloneCodeGenerateExecute, StandaloneCodeGenerateCanExecute);
            
            ProjectSettingsCommand = CreateDelegateCommand(ProjectSettingsExecute, ProjectSettingsCanExecute);

            PluginsCommand = CreateDelegateCommand(PluginsCommandExecute, PluginsCommandCanExecute);
            
            TranslatorCommand = CreateDelegateCommand(TranslatorCommandExecute, TranslatorCommandCanExecute);

            ExpandAllElementCommand = CreateMenuDelegateCommand(ExpandAllElementExecute, ExpandAllElementCanExecute,
                new KeyGesture(Key.OemPlus, ModifierKeys.Control, "Ctrl+Shift+"));

            ExpandElementCommand = CreateDelegateCommand(ExpandElementExecute, ExpandElementCanExecute);

            CollapseAllElementCommand = CreateMenuDelegateCommand(CollapseAllElementExecute, CollapseAllElementCanExecute,
                new KeyGesture(Key.OemMinus, ModifierKeys.Control, "Ctrl+Shift-"));

            CollapseElementCommand = CreateDelegateCommand(CollapseElementExecute, CollapseElementCanExecute);

            UndoCommand = CreateMenuDelegateCommand(UndoExecute, UndoCanExecute,
                new KeyGesture(Key.Z, ModifierKeys.Control, "Ctrl+Z"));

            RedoCommand = CreateMenuDelegateCommand(RedoExecute, RedoCanExecute,
                new KeyGesture(Key.Y, ModifierKeys.Control, "Ctrl+Y"));

            ExportCommand = CreateMenuDelegateCommand(ExportProjectExecute, ExportProjectCanExecute,
                new KeyGesture(Key.E, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift+E"));

            ImportCommand = CreateMenuDelegateCommand(ImportResourcesExecute, ImportResourcesCanExecute,
                new KeyGesture(Key.I, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift+I"));

            bool runInVsPackage = AppContext.RunInVsPackage;

            NewProjectCommand = runInVsPackage
                ? (ICommand)CreateDelegateCommand(NewProjectExecute, NewProjectCanExecute)
                : CreateMenuDelegateCommand(NewProjectExecute, NewProjectCanExecute,
                    new KeyGesture(Key.N, ModifierKeys.Control, "Ctrl+N"));

            OpenProjectCommand = runInVsPackage
                ? (ICommand)CreateDelegateCommand(OpenProjectExecute, OpenProjectCanExecute)
                : CreateMenuDelegateCommand(OpenProjectExecute, OpenProjectCanExecute,
                    new KeyGesture(Key.O, ModifierKeys.Control, "Ctrl+O"));

            CloseProjectCommand = runInVsPackage
                ? (ICommand)CreateDelegateCommand(CloseProjectExecute, CloseProjectCanExecute)
                : CreateMenuDelegateCommand(CloseProjectExecute, CloseProjectCanExecute,
                new KeyGesture(Key.W, ModifierKeys.Control, "Ctrl+W"));

            RecentProjectCommand = new DelegateCommand<RecentFileInfo>(RecentProjectExecute, RecentProjectCanExecute);

            FindElementCommand = CreateMenuDelegateCommand(FindElementExecute, FindElementCanExecute,
                new KeyGesture(Key.F, ModifierKeys.Control, "Ctrl+F"), false);

            PreferencesCommand = CreateDelegateCommand(PreferencesExecute, PreferencesCanExecute);
            AboutCommand = CreateDelegateCommand(AboutExecute);
            OnlineManualCommand = CreateDelegateCommand(OnlineManualExecute);
            ProductWebPageCommand = CreateDelegateCommand(ProductWebPageExecute);
            ExitApplication = CreateDelegateCommand(ExitApplicationExecute);
            WhatsNewCommand = CreateDelegateCommand(WhatsNewExecute);
        }


        private void UnMarkAllForExportCommandExecute(object obj)
        {
            TreeViewService.GetChildsAsFlatList(null).Run(x => x.IsMarkedForExport = false);
        }

        private bool UnMarkAllForExportCommandCanExecute(object obj)
        {
            TreeSelectionInfo selection = TreeViewService.GetSelection();
            bool canExecute = selection.IsSingleSelection && selection.ElementIsModel;
            if (canExecute)
            {
                // if at least 1 element is marked for export
                canExecute = TreeViewService.GetChildsAsFlatList(null).Count(x => x.IsMarkedForExport) > 0;
            }

            return canExecute;
        }
        
        private void MarkForExportCommandExecute(object obj)
        {
            MarkElementsForExport(true);
        }
        
        private void UnMarkForExportCommandExecute(object obj)
        {
            MarkElementsForExport(false);
        }

        private void MarkElementsForExport(bool mark)
        {
            var selectionInfo = TreeViewService.GetSelection();
            if (!selectionInfo.IsEmpty)
            {
                selectionInfo.GetSelectedElements().ForEach(x =>
                    {
                        if (x.ElementIsResource)
                        {
                            x.IsMarkedForExport = mark;
                        }
                    });
            }
        }

        private bool MarkForExportCommandCanExecute(object obj)
        {
            return MarkElementsForExportCanExecute(obj, true);
        }

        private bool UnMarkForExportCommandCanExecute(object obj)
        {
            return MarkElementsForExportCanExecute(obj, false);
        }

        private bool MarkElementsForExportCanExecute(object obj, bool mark)
        {
            bool canExecute = ShellIsNotBusy && ShellViewModel.IsEditorMode;
            if (canExecute)
            {
                TreeSelectionInfo selection = TreeViewService.GetSelection();
                canExecute = !selection.IsEmpty && !selection.ElementIsModel;
                if (canExecute)
                {
                    // all selected elements must be resources...
                    var selectedElements = selection.GetSelectedElements();
                    canExecute = selectedElements.Count(x => !x.ElementIsResource) == 0;
                    if (canExecute)
                    {
                        canExecute = selectedElements.Any(x => x.IsMarkedForExport == !mark);
                    }
                }
            }

            // var canExecute = CopyElementCanExecute(obj);
            // if (canExecute)
            // {
            //     TreeSelectionInfo selection = TreeViewService.GetSelection();
            //     if (selection.IsSingleSelection)
            //     {
            //         canExecute = selection.Element.IsMarkedForExport != mark;
            //     } 
            // }
            
            return canExecute;
        }
        
        private void ExportMarkedCommandExecute(object obj)
        {
            var selection = TreeViewService.GetSelection();

            List<string> elementKeys = new List<string>();
            
            foreach (ITreeElementViewModel element in TreeViewService.GetChildsAsFlatList(null))
            {
                if (element.ElementIsResource && element.IsMarkedForExport)
                {
                    elementKeys.Add(element.ElementKey);
                }
            }

            AppContext.DialogService.ShowExportResourcesDialog(ShellViewContext, selection.ElementIsModel, elementKeys);
        }

        private bool ExportMarkedCommandCanExecute(object arg)
        {
            return TreeViewService.GetChildsAsFlatList(null).Count(x => x.IsMarkedForExport) > 0;
        }


        private bool ShellIsBusy =>
            !ShellViewModel.ProjectBusyOperation.In(ProjectBusyOperationType.None, ProjectBusyOperationType.TreeSearch);

        private bool ShellIsNotBusy =>
            ShellViewModel.ProjectBusyOperation.In(ProjectBusyOperationType.None, ProjectBusyOperationType.TreeSearch);

        private ITreeViewService TreeViewService => ShellViewContext.TreeViewService;

        private IApplicationService ApplicationService => AppContext.ApplicationService;

        private ITreeElementClipboardManager ClipboardManager => ShellViewContext.ClipboardManager;

        private RootModelViewModel RootModelViewModel => ShellViewModel.RootModel;

        private IMessenger Messenger => ShellViewContext.Messenger;

        public MenuDelegateCommand FocusTreePanelCommand { get; }

        public MenuDelegateCommand SaveProjectCommand { get; }

        public ICommand SaveProjectAsCommand { get; }

        public MenuDelegateCommand NewCategoryCommand { get; }

        public MenuDelegateCommand NewResourceCommand { get; }

        public MenuDelegateCommand RenameCommand { get; }

        public MenuDelegateCommand DeleteElementCommand { get; }

        public MenuDelegateCommand CutElementCommand { get; }

        public MenuDelegateCommand CopyElementCommand { get; }

        public MenuDelegateCommand CopyElementAsSpecial1Command { get; }

        public MenuDelegateCommand CopyElementAsSpecial2Command { get; }
        
        public MenuDelegateCommand MarkForExportCommand { get; }
        
        public MenuDelegateCommand UnMarkForExportCommand { get; }
        
        public MenuDelegateCommand UnMarkAllForExportCommand { get; }
        
        public DelegateCommand ExportMarkedCommand { get; }

        public MenuDelegateCommand PasteElementCommand { get; }

        public MenuDelegateCommand DuplicateElementCommand { get; }

        public ICommand LanguageSettingsCommand { get; }
        
        public ICommand StandaloneCodeGenerateCommand { get; }

        public MenuDelegateCommand ExportCommand { get; }

        public MenuDelegateCommand ImportCommand { get; }

        public ICommand ProjectSettingsCommand { get; }

        public ICommand PluginsCommand { get; }
        
        public ICommand TranslatorCommand { get; }

        public MenuDelegateCommand ExpandAllElementCommand { get; }

        public ICommand ExpandElementCommand { get; }

        public MenuDelegateCommand CollapseAllElementCommand { get; }

        public ICommand CollapseElementCommand { get; }

        public MenuDelegateCommand UndoCommand { get; }

        public MenuDelegateCommand RedoCommand { get; }

        public ICommand NewProjectCommand { get; }

        public ICommand OpenProjectCommand { get; }

        public ICommand CloseProjectCommand { get; }

        public ICommand ExitApplication { get; }

        public DelegateCommand<RecentFileInfo> RecentProjectCommand { get; }

        public ICommand PreferencesCommand { get; }

        public MenuDelegateCommand FindElementCommand { get; }

        public ICommand AboutCommand { get; }

        public ICommand OnlineManualCommand { get; }

        public ICommand ProductWebPageCommand { get; }

        public ICommand WhatsNewCommand { get; }

        private IRecentFilesService RecentFilesService => AppContext.RecentFilesService;

        public MenuItemViewModel[] CreateCopySpecialMenuItems()
        {
            List<MenuItemViewModel> menuItems = new List<MenuItemViewModel>();
            var copySpecialGroup = new MenuItemViewModel
            {
                Header = "Copy As...",
            };
            menuItems.Add(copySpecialGroup);
            
            copySpecialGroup.MenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.Menu.CopySpecialMenu.CopyAsXamlMarkup,
                Command = CopyElementAsSpecial1Command,
                GestureText = CopyElementAsSpecial1Command.KeyGesture.DisplayString,
                Tag = CopyElementSpecialTag.XamlMarkup
            });

            copySpecialGroup.MenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.Menu.CopySpecialMenu.CopyAsCSharp,
                Command = CopyElementAsSpecial2Command,
                GestureText = CopyElementAsSpecial2Command.KeyGesture.DisplayString,
                Tag = CopyElementSpecialTag.CSharpCode
            });
            
            menuItems.Add(new MenuItemViewModel
            {
                Header = "Mark for Export",
                Command = MarkForExportCommand,
                GestureText = MarkForExportCommand.KeyGesture.DisplayString
            });

            menuItems.Add(new MenuItemViewModel
            {
                Header = "Unmark for Export",
                Command = UnMarkForExportCommand,
                GestureText = UnMarkForExportCommand.KeyGesture.DisplayString
            });
            
            menuItems.Add(new MenuItemViewModel
            {
                Header = "Unmark all for Export",
                Command = UnMarkAllForExportCommand,
                GestureText = UnMarkAllForExportCommand.KeyGesture.DisplayString
            });
            
            menuItems.Add(new MenuItemViewModel
            {
                Header = "Export Marked...",
                Command = ExportMarkedCommand
            });

            return menuItems.ToArray();
        }

        private MenuDelegateCommand CreateMenuDelegateCommand(Action<object> execute, KeyGesture keyGesture,
            bool applyToView = true)
        {
            return CreateMenuDelegateCommand(execute, null, keyGesture, applyToView);
        }

        private MenuDelegateCommand CreateMenuDelegateCommand(Action<object> execute, Func<object, bool> canExecute, KeyGesture keyGesture,
            bool applyToView = true)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return SetupCommandInputBinding(new MenuDelegateCommand(execute, canExecute, keyGesture), applyToView);
        }

        private DelegateCommand CreateDelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            return new DelegateCommand(execute, canExecute);
        }

        private void FocusTreePanelExecute(object obj)
        {
            Messenger.SingleDispatch(MessengerDefinitions.FocusTree);
        }

        private bool SaveProjectAsCanExecute(object arg)
        {
            return GlobalCanEdit && ShellIsNotBusy;
        }

        private string GetFileDialogFilter()
        {
            return Strings.App.ModelFileFilter;
        }

        private void SaveProjectAsExecute(object obj)
        {
            string initDirectory = ShellViewModel.GetProjectDirectory();

            string fileName = ShellViewModel.ProjectFileName.IsNullOrEmpty()
                ? string.Empty
                : Path.GetFileName(ShellViewModel.ProjectFileName);

            var saveFileDialog = new SaveFileDialog
            {
                DefaultExt = AppConstants.GetModelFileExtension(true),
                Filter = GetFileDialogFilter(),
                InitialDirectory = initDirectory,
                FileName = fileName ?? string.Empty
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                ShellService.SaveProject(saveFileDialog.FileName);
            }
        }

        private bool GlobalCanEdit => true;

        private bool SaveProjectCanExecute(object arg)
        {
            return GlobalCanEdit && ShellViewModel.ProjectIsDirty && !ShellIsBusy;
        }

        private void SaveProjectExecute(object obj)
        {
            if (ShellViewModel.ProjectFileName.IsNullOrEmpty() || !File.Exists(ShellViewModel.ProjectFileName))
            {
                SaveProjectAsCommand.Execute(null);
            }
            else
            {
                ShellService.SaveProject(ShellViewModel.ProjectFileName);
            }
        }

        private bool ExportProjectCanExecute(object arg)
        {
            bool canExecute = GlobalCanEdit && ShellIsNotBusy && ShellViewModel.IsEditorMode;
            if (canExecute)
            {
                bool hasResources = ShellViewModel.RootModel.AllResourcesCount > 0;
                canExecute = hasResources;
            }

            return canExecute;
        }

        private void ExportProjectExecute(object obj)
        {
            var selection = TreeViewService.GetSelection();
            AppContext.DialogService.ShowExportResourcesDialog(ShellViewContext, selection.ElementIsModel);
        }

        private bool ImportResourcesCanExecute(object arg)
        {
            return GlobalCanEdit && ShellIsNotBusy && ShellViewModel.IsEditorMode;
        }

        private void ImportResourcesExecute(object obj)
        {
            DialogService.ShowImportResourcesDialog(ShellViewContext);
        }

        private bool RenameCanExecute(object arg)
        {
            return GlobalCanEdit && ShellIsNotBusy && ShellViewModel.IsEditorMode && TreeViewService.GetSelection().IsSingleSelection;
        }

        private void RenameExecute(object obj)
        {
            TreeSelectionInfo selection = TreeViewService.GetSelection();
            Messenger.SingleDispatch(MessengerDefinitions.FocusAutoSelectName(selection.Element));
        }

        private bool RedoCanExecute(object arg)
        {
            return GlobalCanEdit && ShellIsNotBusy && ShellViewModel.IsEditorMode && ShellViewContext.UndoManager.CanRedo;
        }

        private void RedoExecute(object obj)
        {
            UndoManager.Redo();
        }

        private bool UndoCanExecute(object arg)
        {
            return GlobalCanEdit && ShellIsNotBusy && ShellViewModel.IsEditorMode && UndoManager.CanUndo;
        }

        private void UndoExecute(object obj)
        {
            UndoManager.Undo();
        }

        private MenuDelegateCommand SetupCommandInputBinding(MenuDelegateCommand command, bool applyToView)
        {
            if (command.KeyGesture != null)
            {
                ShellViewModel.AddInputBinding(command, command.KeyGesture, applyToView);
            }

            return command;
        }

        private bool NewCategoryCanExecute(object obj)
        {
            return GlobalCanEdit && 
                ShellIsNotBusy && 
                ShellViewModel.IsEditorMode
                && RootModelViewModel.ModelOptions.Categories
                && TreeViewService.GetSelection().IsSingleSelection;
        }

        private void NewCategoryExecute(object obj)
        {
            TreeSelectionInfo selection = TreeViewService.GetSelection();

            List<string> parentNames = ShellViewModel.GetSelectionParentNames(selection);
            var parentElement =
                (ICategoryLikeViewModel)(selection.ElementIsResource ? selection.ParentElement : selection.Element);

            string categoryName = NewElementDialog.DialogShow(ShellViewContext, parentElement, parentNames, false);
            if (!categoryName.IsNullOrEmpty())
            {
                if (parentElement != null && parentElement.IsExpandable && !parentElement.GetIsExpanded())
                {
                    TreeViewService.Expand(selection.Element, false);
                }

                ICategoryLikeViewModel categoryLikeViewModel = null;

                if (selection.ElementIsModel || selection.ElementIsCategory)
                {
                    categoryLikeViewModel = (ICategoryLikeViewModel)selection.Element;
                }
                else if (selection.ElementIsResource)
                {
                    categoryLikeViewModel = (ICategoryLikeViewModel)selection.ParentElement;
                }

                if (categoryLikeViewModel != null)
                {
                    CategoryViewModel categoryViewModel = categoryLikeViewModel.AddChildCategory(categoryName);
                    TreeViewService.Select(categoryViewModel);
                }
            }
        }

        private bool NewResourceCanExecute(object obj)
        {
            bool canExecute = GlobalCanEdit && ShellIsNotBusy && ShellViewModel.IsEditorMode;
            if (canExecute)
            {
                TreeSelectionInfo selection = TreeViewService.GetSelection();
                canExecute = selection.IsSingleSelection;
            }

            return canExecute;
        }

        private void NewResourceExecute(object obj)
        {
            TreeSelectionInfo selection = TreeViewService.GetSelection();

            if (selection.IsSingleSelection && selection.ElementIsModel && ShellViewModel.ModelOptions.Resources != ModelOptionsResources.All)
            {
                var caption = Strings.ViewModels.NewElement.NewResourceUnderRootNotAllowedCaption;
                var message = Strings.ViewModels.NewElement.NewResourceUnderRootNotAllowedMessage;
                var detail = Strings.ViewModels.NewElement.NewResourceUnderRootNotAllowedDetail;

                bool exit = true;

                if (DialogService.ShowConfirm(caption, message, detail) == DialogResult.Yes)
                {
                    ShellViewModel.RootModel.ModelOptions.Resources = ModelOptionsResources.All;
                    exit = ShellService.SaveProject() == false;
                }

                if (exit)
                {
                    return;
                }
            }


            var parentNames = ShellViewModel.GetSelectionParentNames(selection);
            var parentElement = (ICategoryLikeViewModel)(selection.ElementIsResource ? selection.ParentElement : selection.Element);

            string resourceName = NewElementDialog.DialogShow(ShellViewContext, parentElement, parentNames, true);
            if (!resourceName.IsNullOrEmpty())
            {
                if (parentElement != null && parentElement.IsExpandable && !parentElement.GetIsExpanded())
                {
                    TreeViewService.Expand(selection.Element, false);
                }

                ICategoryLikeViewModel categoryLikeViewModel = selection.ElementIsResource
                    ? (ICategoryLikeViewModel)selection.ParentElement
                    : (ICategoryLikeViewModel)selection.Element;

                categoryLikeViewModel.AddChildResource(resourceName);
            }
        }

        private bool DeleteElementCanExecute(object obj)
        {
            bool canExecute = GlobalCanEdit && ShellIsNotBusy && ShellViewModel.IsEditorMode;
            if (canExecute)
            {
                TreeSelectionInfo selection = TreeViewService.GetSelection();
                bool canDelete = !selection.IsEmpty && (selection.IsMultiSelection || !selection.ElementIsModel);
                if (canDelete && selection.IsMultiSelection)
                {
                    List<ITreeElementViewModel> allSelected = selection.GetSelectedElements();
                    int parentDistinctCount = allSelected.Select(x => x.Parent).Distinct().Count();
                    int elementTypeDistinctCount = allSelected.Select(x => x.ElementType).Distinct().Count();

                    // can delete only when all selected elements has same parent and same element type!
                    canDelete = parentDistinctCount == 1 && elementTypeDistinctCount == 1;
                }

                canExecute = canDelete;
            }

            return canExecute;
        }

        private void DeleteElementExecute(object obj)
        {
            TreeSelectionInfo selection = TreeViewService.GetSelection();
            List<ITreeElementViewModel> selectedElements = selection.GetSelectedElements();

            if (DeleteElementsDialog.DialogShow(AppContext, selectedElements))
            {
                ITreeElementViewModel parentElement = selection.ParentElement;
                // TODO: if multiple elements, remove them as 1 undo command!
                selectedElements.ForEach(x => parentElement.RemoveChild(x));

                TreeViewService.Select(parentElement);
            }
        }

        private bool CutElementCanExecute(object obj)
        {
            bool canExecute = GlobalCanEdit && ShellIsNotBusy && ShellViewModel.IsEditorMode;
            if (canExecute)
            {
                TreeSelectionInfo selection = TreeViewService.GetSelection();
                bool selectionIsValid = !selection.IsEmpty && !selection.ElementIsModel;

                if (selectionIsValid)
                {
                    List<ITreeElementViewModel> selectedElements = selection.GetSelectedElements();
                    int uniqueParentCount = selectedElements.Select(x => x.ParentElementKey).Distinct().Count();
                    selectionIsValid = uniqueParentCount == 1;
                }

                canExecute = selectionIsValid;
            }

            return canExecute;
        }

        private void CutElementExecute(object obj)
        {
            ClipboardManager.SetSelectionToClipboard(true);
        }

        private bool CopyElementCanExecute(object obj)
        {
            bool canExecute = ShellIsNotBusy && ShellViewModel.IsEditorMode;
            if (canExecute)
            {
                TreeSelectionInfo selection = TreeViewService.GetSelection();
                bool selectionIsValid = !selection.IsEmpty && !selection.ElementIsModel;

                if (selectionIsValid)
                {
                    List<ITreeElementViewModel> selectedElements = selection.GetSelectedElements();
                    int uniqueParentCount = selectedElements.Select(x => x.ParentElementKey).Distinct().Count();
                    selectionIsValid = uniqueParentCount == 1;
                }

                canExecute = selectionIsValid;
            }

            return canExecute;
        }

        private void CopyElementExecute(object obj)
        {
            ClipboardManager.SetSelectionToClipboard(false);
        }

        private bool CopyElementAsSpecial1CanExecute(object obj)
        {
            bool canExecute = CopyElementCanExecute(obj);
            if (canExecute)
            {
                var selection = TreeViewService.GetSelection();
                canExecute = selection.IsSingleSelection && selection.ElementIsResource;
            }

            return canExecute;
        }

        private void CopyElementAsSpecial1Execute(object obj)
        {
            ClipboardManager.SetSelectionToClipboard(false, CopyElementSpecialTag.XamlMarkup);
        }

        private bool CopyElementAsSpecial2CanExecute(object obj)
        {
            bool canExecute = CopyElementCanExecute(obj);
            if (canExecute)
            {
                var selection = TreeViewService.GetSelection();
                canExecute = selection.IsSingleSelection && selection.ElementIsResource;
            }

            return canExecute;
        }

        private void CopyElementAsSpecial2Execute(object obj)
        {
            ClipboardManager.SetSelectionToClipboard(false, CopyElementSpecialTag.CSharpCode);
        }

        private bool PasteElementCanExecute(object obj)
        {
            bool canExecute = GlobalCanEdit && ShellIsNotBusy && ShellViewModel.IsEditorMode;
            if (canExecute)
            {
                TreeSelectionInfo selection = TreeViewService.GetSelection();
                bool validClipboardData = ClipboardManager.HasValidClipboardData();

                bool canPaste = selection.IsSingleSelection && validClipboardData;

                if (canPaste)
                {
                    string targetElementKey = selection.ElementIsResource
                        ? selection.Element.ParentElementKey
                        : selection.Element.ElementKey;
                    TreeElementType targetElementType = selection.ElementIsResource
                        ? selection.Element.Parent.ElementType
                        : selection.Element.ElementType;

                    TreeElementClipboardData clipboardData = ClipboardManager.GetClipboardData();
                    canPaste = clipboardData.Items.Count > 0; // has any items to paste?

                    if (canPaste)
                    {
                        // if selection is 'Model' and resources under root are NOT allowed! look if there is any resource in clipboard and if so -> disallow paste
                        ModelOptions modelOptions = ShellViewModel.ModelOptions;
                        bool resourcesUnderModel = modelOptions.Resources == ModelOptionsResources.All;
                        if (targetElementType == TreeElementType.Model && !resourcesUnderModel)
                        {
                            if (clipboardData.Items.Any(x => x.ModelElementType == ModelElementType.Resource))
                            {
                                canPaste = false;
                            }
                        }

                        if (canPaste)
                        {
                            // if there are any categories in clipboard we need to check max folder depth validity
                            if (clipboardData.Items.Any(x => x.ModelElementType == ModelElementType.Category))
                            {
                                canPaste = modelOptions.Categories;
                            }

                            if (canPaste)
                            {
                                string cbFirstElementParentKey = clipboardData.Items.First().ElementParentKey;
                                // target element key and parent key of item from clipboard(cb) cant be equal for cut, but allowed for copy
                                canPaste = !targetElementKey.EqualsTo(cbFirstElementParentKey) ||
                                    clipboardData.IsMarkedForCut == false;

                                if (canPaste)
                                {
                                    if (ClipboardManager.IsSameModel(clipboardData) && !clipboardData.IsMarkedForCut)
                                    {
                                        clipboardData.IterateTreeItems(args =>
                                            {
                                                if (args.Data.ElementKey == targetElementKey ||
                                                    args.Data.ElementParentKey == targetElementKey)
                                                {
                                                    canPaste = false;
                                                    args.Cancel = true;
                                                }
                                            });
                                    }
                                }
                            }
                        }
                    }
                }

                canExecute = canPaste;
            }

            return canExecute;
        }

        private void PasteElementExecute(object obj)
        {
            TreeSelectionInfo selection = TreeViewService.GetSelection();
            TreeElementClipboardData clipboardData = ClipboardManager.GetClipboardData();

            if (!selection.IsEmpty)
            {
                ITreeElementViewModel targetElement = selection.ElementIsResource ? selection.Element.Parent : selection.Element;
                PasteElementInternal(clipboardData, targetElement);
            }
        }

        private void PasteElementInternal(TreeElementClipboardData clipboardData, ITreeElementViewModel targetElement)
        {
            if (clipboardData?.Items != null && clipboardData.Items.Count > 0)
            {
                if (targetElement.IsExpandable && !targetElement.GetIsExpanded())
                {
                    TreeViewService.Expand(targetElement, false);
                }

                // if IsShellInstanceEqual returns true this means that cut/copy and paste is done within same app shell instance
                // and there is no need to create new view model(s) for elements, just look for than by elementKey...
                bool cutAllowed = ClipboardManager.IsSameShellInstance(clipboardData);

                List<string> allUniqueLanguages = clipboardData.GetAllUniqueLanguages();
                List<string> missingLanguages = RootModelViewModel.GetMissingLanguages(allUniqueLanguages);

                if (missingLanguages.Count > 0 && PasteConfirmDialog.DialogShow(AppContext, missingLanguages))
                {
                    RootModelViewModel.AddMissingLanguages(missingLanguages);
                }

                if (clipboardData.IsMarkedForCut && cutAllowed)
                {
                    // find top-level items tree elements
                    List<string> topLevelElementKeys = clipboardData.Items.Select(x => x.ElementKey).Distinct().ToList();

                    List<ITreeElementViewModel> elementsToPaste = TreeViewService
                        .FindAllElements(x => topLevelElementKeys.Contains(x.ElementKey))
                        .ToList();

                    targetElement.Move(elementsToPaste);

                    // remove 'is marked for cut' flag after move
                    elementsToPaste.ForEach(x => x.IsMarkedForCut = false);

                    ClipboardManager.Clear();
                }
                else
                {
                    targetElement.PasteFromClipboard(clipboardData);
                }
            }
        }

        private bool DuplicateElementCanExecute(object arg)
        {
            bool canExecute = ShellIsNotBusy && ShellViewModel.IsEditorMode;
            if (canExecute)
            {
                TreeSelectionInfo selection = TreeViewService.GetSelection();
                bool selectionIsValid = !selection.IsEmpty && !selection.IsMultiSelection && !selection.ElementIsModel;

                if (selectionIsValid)
                {
                    List<ITreeElementViewModel> selectedElements = selection.GetSelectedElements();
                    int uniqueParentCount = selectedElements.Select(x => x.ParentElementKey).Distinct().Count();
                    selectionIsValid = uniqueParentCount == 1;
                }

                canExecute = selectionIsValid;
            }

            return canExecute;
        }

        private void DuplicateElementExecute(object obj)
        {
            bool canExecute = ShellIsNotBusy && ShellViewModel.IsEditorMode;
            if (canExecute)
            {
                TreeSelectionInfo selection = TreeViewService.GetSelection();

                // copy current selection to clipboard (ctrl+c)
                ClipboardManager.SetSelectionToClipboard(false);

                // paste clipboard data into target element
                TreeElementClipboardData clipboardData = ClipboardManager.GetClipboardData();
                if (clipboardData?.Items.Count > 0)
                {
                    PasteElementInternal(clipboardData, selection.Element.Parent);
                }
            }
        }


        private bool LanguageSettingsCanExecute(object obj)
        {
            return GlobalCanEdit && ShellIsNotBusy && ShellViewModel.IsEditorMode;
        }

        private void LanguageSettingsExecute(object obj)
        {
            if (LanguageSettingsDialog.DialogShow(ShellViewContext, out List<string> allLanguages, out Dictionary<string, string> replacedLanguages,
                out string primaryLanguageName))
            {
                RootModelViewModel.UpdateLanguages(allLanguages, replacedLanguages, primaryLanguageName);
            }
        }

        private bool StandaloneCodeGenerateCanExecute(object arg)
        {
            return ShellIsNotBusy && AppContext.StandaloneCodeGeneratorService.Available;
        }

        private void StandaloneCodeGenerateExecute(object obj)
        {
            ShellService.StandaloneCodeGenerate().ContinueWith(_ =>
                {
                    RaiseObjectPropertiesChanged();                    
                });
        }

        private bool PluginsCommandCanExecute(object arg)
        {
            return ShellIsNotBusy;
        }

        private void PluginsCommandExecute(object obj)
        {
            DialogService.ShowAppSettings(AppSettingsDialogPage.Plugins);
        }

        private bool TranslatorCommandCanExecute(object arg)
        {
            return ShellIsNotBusy;
        }

        private void TranslatorCommandExecute(object obj)
        {
            DialogService.ShowAppSettings(AppSettingsDialogPage.Translator);
        }

        private bool ProjectSettingsCanExecute(object arg)
        {
            return GlobalCanEdit && ShellIsNotBusy && ShellViewModel.IsEditorMode;
        }

        private void ProjectSettingsExecute(object obj)
        {
            ModelOptions clonedOptions = ShellViewModel.ModelOptions.Clone();
            if (ProjectSettingsDialog.DialogShow(ShellViewContext, clonedOptions, out var newModelVersion))
            {
                ShellViewModel.RootModel.ModelOptions = clonedOptions;
                if (ShellViewModel.ModelContext.Model.Version != newModelVersion)
                {
                    ShellViewModel.RootModel.ModelVersion = newModelVersion.ToString();
                    ShellViewModel.ModelContext.Model.Version = newModelVersion;
                }

                ShellService.SaveProject();
            }
        }

        private bool ExpandAllElementCanExecute(object arg)
        {
            bool canExecute = GlobalCanEdit && ShellIsNotBusy;
            if (canExecute)
            {
                TreeSelectionInfo selection = TreeViewService.GetSelection();
                canExecute = TreeViewService.RootModel.GetHasChildren() && selection.IsSingleSelection &&
                    !selection.ElementIsResource;
            }

            return canExecute;
        }

        private bool ExpandElementCanExecute(object arg)
        {
            bool canExecute = GlobalCanEdit && ShellIsNotBusy;
            if (canExecute)
            {
                TreeSelectionInfo selection = TreeViewService.GetSelection();
                canExecute = selection.IsSingleSelection && !selection.ElementIsResource;
            }

            return canExecute;
        }

        private void ExpandElementExecute(object obj)
        {
            ITreeElementViewModel selectedElement = TreeViewService.GetSelectedElements().SingleOrDefault();
            if (selectedElement != null)
            {
                TreeViewService.Expand(selectedElement, false);
            }
        }

        private void ExpandAllElementExecute(object obj)
        {
            List<ITreeElementViewModel> selectedElements = TreeViewService.GetSelectedElements();
            ITreeElementViewModel element = selectedElements.Count == 1 ? selectedElements.First() : RootModelViewModel;
            TreeViewService.Expand(element, true);
        }

        private bool CollapseAllElementCanExecute(object arg)
        {
            bool canExecute = GlobalCanEdit && ShellIsNotBusy;
            if (canExecute)
            {
                TreeSelectionInfo selection = TreeViewService.GetSelection();
                canExecute = TreeViewService.RootModel.GetHasChildren() && selection.IsSingleSelection &&
                    !selection.ElementIsResource;
            }

            return canExecute;
        }

        private void CollapseAllElementExecute(object obj)
        {
            List<ITreeElementViewModel> selectedElements = TreeViewService.GetSelectedElements();
            ITreeElementViewModel element = selectedElements.Count == 1 ? selectedElements.First() : RootModelViewModel;
            TreeViewService.Collapse(element, true);
        }

        private bool CollapseElementCanExecute(object arg)
        {
            bool canExecute = GlobalCanEdit && ShellIsNotBusy;
            if (canExecute)
            {
                TreeSelectionInfo selection = TreeViewService.GetSelection();
                canExecute = selection.IsSingleSelection && !selection.ElementIsResource;
            }

            return canExecute;
        }

        private void CollapseElementExecute(object obj)
        {
            ITreeElementViewModel selectedElement = TreeViewService.GetSelectedElements().SingleOrDefault();
            if (selectedElement != null)
            {
                TreeViewService.Collapse(selectedElement, false);
            }
        }

        private void AboutExecute(object obj)
        {
            AboutDialog.DialogShow(AppContext);
        }

        private void OnlineManualExecute(object obj)
        {
            WebPageUtils.ShowDoc();
        }

        private void ProductWebPageExecute(object obj)
        {
            WebPageUtils.ShowProductWebPage();
        }

        private bool PreferencesCanExecute(object arg)
        {
            return GlobalCanEdit && ShellIsNotBusy;
        }

        private void PreferencesExecute(object obj)
        {
            AppContext.ApplicationService.ShowAppSettings();
        }

        private bool FindElementCanExecute(object arg)
        {
            return GlobalCanEdit && ShellIsNotBusy;
        }

        private void FindElementExecute(object obj)
        {
            Messenger.SingleDispatch(MessengerDefinitions.FocusSearchPanel);
        }

        private bool RecentProjectCanExecute(object arg)
        {
            return GlobalCanEdit && ShellIsNotBusy;
        }

        private void RecentProjectExecute(RecentFileInfo recentFileInfo)
        {
            if (ShellService.CheckProjectIsDirty())
            {
                if (!File.Exists(recentFileInfo.FileName))
                {
                    if (DialogService.ShowConfirm(
                            Strings.Operations.Project.RecentProjectDoestNotExistCaption,
                            Strings.Operations.Project.RecentProjectDoestNotExistMessage(recentFileInfo.FileName),
                            Strings.Operations.Project.RecentProjectDoestNotExistDetail)
                        == DialogResult.Yes)
                    {
                        RecentFilesService.Remove(recentFileInfo.FileName);
                        RecentFilesService.Save(ShellViewContext);
                    }
                }
                else
                {
                    ShellService.OpenProject(recentFileInfo.FileName);
                }
            }
        }

        private void ExitApplicationExecute(object obj)
        {
            ApplicationService.Exit(false);
        }

        private void WhatsNewExecute(object obj)
        {
            var updateCheckInfo = AppContext.UpdateInfo;
            if (updateCheckInfo?.CurrentVersionDescriptions != null)
            {
                var helpItems = updateCheckInfo.CurrentVersionDescriptions
                    .Select(x => x.IsNullOrEmpty() ? BulletListItem.Separator() : BulletListItem.CheckItem(x)).ToList();
                var currentVersion = AppContext.AppCurrentVersion.ToString();
                using (var viewModel = new HelpItemsDialogViewModel(AppContext, helpItems,
                    Strings.Services.Updates.WhatsNewInCurrentVersion,
                    Strings.Services.Updates.WhatsNewInVersion(currentVersion)))
                {
                    DialogWindow.DialogShow<HelpItemsDialogViewModel, HelpItemsDialog>(viewModel);
                }
            }
        }

        private bool CloseProjectCanExecute(object arg)
        {
            return !ShellViewModel.ProjectFileName.IsNullOrEmpty() && !ShellIsBusy;
        }

        private void CloseProjectExecute(object obj)
        {
            if (ShellService.CheckProjectIsDirty())
            {
                ShellService.CloseProject();
            }
        }

        private bool OpenProjectCanExecute(object arg)
        {
            return GlobalCanEdit && ShellIsNotBusy;
        }

        private void OpenProjectExecute(object arg)
        {
            if (ShellService.CheckProjectIsDirty())
            {
                string initDirectory = ShellViewModel.GetProjectDirectory();

                var openFileDialog = new OpenFileDialog
                {
                    DefaultExt = AppConstants.GetModelFileExtension(true),
                    Filter = Strings.App.ModelFileFilter,
                    InitialDirectory = initDirectory
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    ShellService.OpenProject(openFileDialog.FileName);
                }
            }
        }

        private bool NewProjectCanExecute(object arg)
        {
            return GlobalCanEdit && !ShellIsBusy;
        }

        private void NewProjectExecute(object obj)
        {
            if (ShellService.CheckProjectIsDirty())
            {
                string modelName = Strings.ViewModels.NewProject.DefaultModelName;
                var dialogResult = NewProjectDialog.DialogShow(ShellViewContext, modelName);

                if (dialogResult.Submitted)
                {
                    ShellService.NewProject(dialogResult.ModelName, dialogResult.PrimaryLanguage,
                        dialogResult.ModelOptions, dialogResult.Template);
                    
                    // var codeGeneratorMetadata = modelContext.GetMetadata<CodeGeneratorMetadata>(CodeGeneratorMetadataDescriptor.UID);
                    // codeGeneratorMetadata.Template = _dialogResult.Template;
                    // codeGeneratorMetadata.TemplateId = templateId;
                    //
                    // var modelFileStorage = new ModelFileStorage();
                    // modelFileStorage.Initialize();
                    // string fileContent = modelFileStorage.Save(modelContext, ModelSaveOptions.Indent);

                    if (dialogResult.OpenLanguageSettings)
                    {
                        if (LanguageSettingsCommand.CanExecute(null))
                        {
                            LanguageSettingsCommand.Execute(null);
                        }
                    }
                }
            }
        }
    }
}
