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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using FontAwesome.WPF;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Utils;

namespace LHQ.App.ViewModels
{
    public class MainMenuViewModel : ShellViewModelBase
    {
        private ICollectionView _recentProjectsView;
        private bool _hasRecentProjects;

        public MainMenuViewModel(IShellViewContext shellViewContext, CommandsViewModel commands)
            : base(shellViewContext, true)
        {
            MainMenuItems = new ObservableCollectionExt<MenuItemViewModel>();
            bool runInVsPackage = ShellViewModel.RunInVsPackage;

            // File
            MenuItemViewModel menuFile = AddMainMenuItem(null, Strings.MainMenu.File.MenuItem, 
                null, null, null);

            // File -> New project
            AddMainMenuItem(menuFile, Strings.MainMenu.File.NewProject, 
                commands.NewProjectCommand, @"/LHQ.App;component/Images/project-new.png",
                runInVsPackage 
                    ? null
                    : (commands.NewProjectCommand as MenuDelegateCommand).KeyGesture);

            // File -> Open project
            AddMainMenuItem(menuFile, Strings.MainMenu.File.OpenProject,
                commands.OpenProjectCommand, @"/LHQ.App;component/Images/project-open.png",
                runInVsPackage
                    ? null
                    : (commands.OpenProjectCommand as MenuDelegateCommand).KeyGesture);

            // Separator
            AddMainMenuItemSeparator(menuFile);

            // File -> Close project
            AddMainMenuItem(menuFile, Strings.MainMenu.File.CloseProject,
                commands.CloseProjectCommand, null,
                runInVsPackage
                    ? null
                    : (commands.CloseProjectCommand as MenuDelegateCommand).KeyGesture);

            // File -> Save project
            AddMainMenuItem(menuFile, Strings.MainMenu.File.SaveProject, 
                commands.SaveProjectCommand, @"/LHQ.App;component/Images/project-save.png",
                commands.SaveProjectCommand.KeyGesture);

            // File -> Save project as
            AddMainMenuItem(menuFile, Strings.MainMenu.File.SaveProjectAs,
                commands.SaveProjectAsCommand, null,
                null);
                //commands.SaveProjectAsCommand.KeyGesture);

            AddMainMenuItemSeparator(menuFile);

            // File -> Project settings
            AddMainMenuItem(menuFile, Strings.MainMenu.File.ProjectSettings,
                commands.ProjectSettingsCommand, @"/LHQ.App;component/Images/settings.png", null);

            // Separator
            AddMainMenuItemSeparator(menuFile);

            // File -> Import
            AddMainMenuItem(menuFile, runInVsPackage ? Strings.MainMenu.File.Import : Strings.MainMenu.File.ImportResources,
                commands.ImportCommand, @"/LHQ.App;component/Images/import.png", 
                 runInVsPackage ? null : commands.ImportCommand.KeyGesture);

            // File -> Export
            AddMainMenuItem(menuFile, runInVsPackage ? Strings.MainMenu.File.Export : Strings.MainMenu.File.ExportResources,
                commands.ExportCommand, @"/LHQ.App;component/Images/export.png", 
                runInVsPackage ? null : commands.ExportCommand.KeyGesture);

            // Separator
            AddMainMenuItemSeparator(menuFile);

            // File -> Recent projects
            MenuRecentProjects = AddMainMenuItem(menuFile, Strings.MainMenu.File.RecentProjects, null, null, null);
            RecentProjectsView = CollectionViewSource.GetDefaultView(RecentFilesService.RecentItems);
            RecentProjectsView.CollectionChanged += RecentProjectsCollectionChanged;
            RefreshRecentProjectItems();

            HasRecentProjects = RecentFilesService.RecentItems.Count > 0;

            // Separator
            AddMainMenuItemSeparator(menuFile);

            // File -> Exit
            AddMainMenuItem(menuFile, Strings.MainMenu.File.ExitApp, commands.ExitApplication, null, null);

            // Edit
            MenuItemViewModel menuEdit = AddMainMenuItem(null, Strings.MainMenu.Edit.MenuItem, null, null, null);

            // Edit -> Undo
            AddMainMenuItem(menuEdit, Strings.MainMenu.Edit.Undo, 
                commands.UndoCommand, @"/LHQ.App;component/Images/undo.png",
                commands.UndoCommand.KeyGesture);

            // Edit -> Redo
            AddMainMenuItem(menuEdit, Strings.MainMenu.Edit.Redo,
                commands.RedoCommand, @"/LHQ.App;component/Images/redo.png",
                commands.RedoCommand.KeyGesture);

            // Separator
            AddMainMenuItemSeparator(menuEdit);

            // Edit -> Cut
            AddMainMenuItem(menuEdit, Strings.MainMenu.Edit.Cut,
                commands.CutElementCommand, @"/LHQ.App;component/Images/cut.png",
                commands.CutElementCommand.KeyGesture);

            // Edit -> Copy
            AddMainMenuItem(menuEdit, Strings.MainMenu.Edit.Copy, 
                commands.CopyElementCommand, @"/LHQ.App;component/Images/copy.png",
                commands.CopyElementCommand.KeyGesture);

            // Edit -> Paste
            AddMainMenuItem(menuEdit, Strings.MainMenu.Edit.Paste, 
                commands.PasteElementCommand, @"/LHQ.App;component/Images/paste.png",
                commands.PasteElementCommand.KeyGesture);

            // Edit -> Duplicate
            AddMainMenuItem(menuEdit, Strings.MainMenu.Edit.Duplicate,
                commands.DuplicateElementCommand, @"",
                null);
                //commands.DuplicateElementCommand.KeyGesture);

            // Separator
            AddMainMenuItemSeparator(menuEdit);

            // Edit -> New category
            var newCategoryMenuItem = AddMainMenuItem(menuEdit, Strings.MainMenu.Edit.NewCategory, 
                commands.NewCategoryCommand, @"/LHQ.App;component/Images/element_category.png", 
                commands.NewCategoryCommand.KeyGesture);
            newCategoryMenuItem.SetAwesomeImage(FontAwesomeIcon.Folder, VisualManager.Instance.CategoryElementIconBrush);


            // Edit -> New resource
            var newResourceMenuItem = AddMainMenuItem(menuEdit, Strings.MainMenu.Edit.NewResource, 
                commands.NewResourceCommand, @"/LHQ.App;component/Images/element_resource.png", 
                commands.NewResourceCommand.KeyGesture);
            newResourceMenuItem.SetAwesomeImage(FontAwesomeIcon.File, VisualManager.Instance.ResourceElementIconBrush);


            // Edit -> Rename
            AddMainMenuItem(menuEdit, Strings.MainMenu.Edit.Rename, 
                commands.RenameCommand, null, commands.RenameCommand.KeyGesture);

            // Edit -> Delete
            AddMainMenuItem(menuEdit, Strings.MainMenu.Edit.Delete, 
                commands.DeleteElementCommand, @"/LHQ.App;component/Images/delete.png", 
                commands.DeleteElementCommand.KeyGesture);

            // Separator
            AddMainMenuItemSeparator(menuEdit);

            // Edit -> Find...
            AddMainMenuItem(menuEdit, Strings.MainMenu.Edit.FindElement, 
                commands.FindElementCommand, null, 
                null);
                //commands.FindElementCommand.KeyGesture);

            // Edit -> Focus Navigation Tree...
            AddMainMenuItem(menuEdit, Strings.MainMenu.Edit.FocusNavigation, 
                commands.FocusTreePanelCommand, null, commands.FocusTreePanelCommand.KeyGesture);

            // Separator
            AddMainMenuItemSeparator(menuEdit);

            // Edit -> Preferences
            AddMainMenuItem(menuEdit, Strings.MainMenu.Edit.Preferences, 
                commands.PreferencesCommand, null, null);

            // Languages
            MenuItemViewModel menuLanguages = AddMainMenuItem(null, Strings.MainMenu.Languages.MenuItem, null, null, null);

            // Languages -> Settings
            AddMainMenuItem(menuLanguages, Strings.MainMenu.Languages.Settings, 
                commands.LanguageSettingsCommand, null, null);

            // Help
            MenuItemViewModel menuHelp = AddMainMenuItem(null, Strings.MainMenu.Help.MenuItem, null, null, null);

            // Help -> Whats New?
            AddMainMenuItem(menuHelp, Strings.Menu.Help.WhatsNew, commands.WhatsNewCommand, null, null);

            // Help -> Online Manual
            AddMainMenuItem(menuHelp, Strings.MainMenu.Help.OnlineManual, commands.OnlineManualCommand, null, null);

            // Help -> Product Web Page
            AddMainMenuItem(menuHelp, Strings.MainMenu.Help.ProductWebPage, commands.ProductWebPageCommand, null, null);

            // Help -> About
            AddMainMenuItem(menuHelp, Strings.MainMenu.Help.About, commands.AboutCommand, null, null);

            ShellViewModel.PropertyChanged += ShellViewOnPropertyChanged;
        }

        public MenuItemViewModel MenuRecentProjects { get; }

        public ObservableCollectionExt<MenuItemViewModel> MainMenuItems { get; }

        private IRecentFilesService RecentFilesService => AppContext.RecentFilesService;

        public ICollectionView RecentProjectsView
        {
            get => _recentProjectsView;
            set => SetProperty(ref _recentProjectsView, value);
        }

        public bool HasRecentProjects
        {
            get => _hasRecentProjects;
            set => SetProperty(ref _hasRecentProjects, value);
        }

        private void RefreshRecentProjectItems()
        {
            MenuRecentProjects.MenuItems.Clear();
            foreach (RecentFileInfo recentFileInfo in RecentProjectsView)
            {
                MenuRecentProjects.MenuItems.Add(new MenuItemViewModel
                {
                    Header = recentFileInfo.Name,
                    Command = ShellViewModel.Commands.RecentProjectCommand,
                    CommandParameter = recentFileInfo
                });
            }
        }

        private void RecentProjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            RefreshRecentProjectItems();
        }

        private MenuItemViewModel AddMainMenuItem(MenuItemViewModel parent, string header, ICommand command, string imageSource,
            KeyGesture keyGesture)
        {
            var newItem = new MenuItemViewModel
            {
                Header = header,
                Command = command,
                GestureText = keyGesture?.DisplayString,
                ImageUri = imageSource
            };

            if (parent != null)
            {
                parent.MenuItems.Add(newItem);
            }
            else
            {
                MainMenuItems.Add(newItem);
            }

            return newItem;
        }

        private void AddMainMenuItemSeparator(MenuItemViewModel parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            parent.MenuItems.Add(new MenuItemSeparatorViewModel());
        }

        private void ShellViewOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ShellViewModel.ProjectBusyOperation))
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}
