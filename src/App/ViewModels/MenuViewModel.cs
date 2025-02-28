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

using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using FontAwesome.WPF;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.App.ViewModels.Elements;
using LHQ.Utils;

namespace LHQ.App.ViewModels
{
    public class MenuViewModel : ShellViewModelBase
    {
        private readonly MenuItemViewModel _primaryLanguageMenuItem;

        public MenuViewModel(IShellViewContext shellViewContext, CommandsViewModel commands)
            : base(shellViewContext, true)
        {
            bool runInVsPackage = ShellViewModel.RunInVsPackage;
            Commands = commands;
            MenuItems = new ObservableCollectionExt<MenuItemViewModel>();
            ContextMenuItems = new ObservableCollectionExt<MenuItemViewModel>();

            if (!runInVsPackage)
            {
                AddMenuItem(Strings.Menu.OpenProject, 
                    commands.OpenProjectCommand, @"/LHQ.App;component/Images/project-open.png",
                    (commands.OpenProjectCommand as MenuDelegateCommand).KeyGesture);

                AddMenuItem(Strings.Menu.SaveProject, 
                    commands.SaveProjectCommand, @"/LHQ.App;component/Images/project-save.png",
                    commands.SaveProjectCommand.KeyGesture);

                AddMenuItemSeparator();
            }

            NewCategoryMenuItem = AddMenuItem(Strings.Menu.NewCategory, 
                commands.NewCategoryCommand, @"/LHQ.App;component/Images/element_category.png",
                commands.NewCategoryCommand.KeyGesture);
            NewCategoryMenuItem.GetVisible = NewCategoryGetVisible;
            NewCategoryMenuItem.SetAwesomeImage(FontAwesomeIcon.Folder, VisualManager.Instance.CategoryElementIconBrush);

            NewResourceMenuItem = AddMenuItem(Strings.Menu.NewResource, 
                commands.NewResourceCommand, @"/LHQ.App;component/Images/element_resource.png",
                commands.NewResourceCommand.KeyGesture);
            NewResourceMenuItem.SetAwesomeImage(FontAwesomeIcon.File, VisualManager.Instance.ResourceElementIconBrush);


            DeleteElementMenuItem = AddMenuItem(Strings.Menu.Delete, 
                commands.DeleteElementCommand, @"/LHQ.App;component/Images/delete.png",
                commands.DeleteElementCommand.KeyGesture);

            if (!runInVsPackage)
            {
                AddMenuItemSeparator();

                AddMenuItem(Strings.Menu.Undo, 
                    commands.UndoCommand, @"/LHQ.App;component/Images/undo.png",
                    commands.UndoCommand.KeyGesture);

                AddMenuItem(Strings.Menu.Redo, 
                    commands.RedoCommand, @"/LHQ.App;component/Images/redo.png",
                    commands.RedoCommand.KeyGesture);
            }

            AddMenuItemSeparator();

            _primaryLanguageMenuItem = AddMenuItem(Strings.Menu.LanguagesRoot(RootModelViewModel.Languages.Count, RootModelViewModel.PrimaryLanguage.EnglishName),
                commands.LanguageSettingsCommand, null, null);
            _primaryLanguageMenuItem.ToolTip = Strings.Menu.LanguagesTooltip;

            if (runInVsPackage)
            {
                AddMenuItem(Strings.Menu.Import, 
                    commands.ImportCommand, @"/LHQ.App;component/Images/import.png",
                    null);

                var menuExport = AddMenuItem(Strings.Menu.Export,
                    commands.ExportCommand, @"/LHQ.App;component/Images/export.png",
                    null);
                menuExport.ToolTip = Strings.Menu.ExportTooltip;

                var menuProperties = AddMenuItem(Strings.Menu.Properties, 
                    commands.ProjectSettingsCommand, @"/LHQ.App;component/Images/settings.png",
                    null);
                menuProperties.ToolTip = Strings.Menu.PropertiesTooltip;

                AddMenuItemSeparator();

                // Plugins
                var menuPlugins = AddMenuItem(Strings.Menu.Plugins, commands.PluginsCommand, string.Empty, null);
                menuPlugins.ToolTip = Strings.Menu.PluginsTooltip;

                // Translator
                var menuTranslator = AddMenuItem(Strings.Menu.Translator, commands.TranslatorCommand, string.Empty, null);
                menuTranslator.ToolTip = Strings.Menu.TranslatorTooltip;

                // Help
                MenuItemViewModel menuHelp = AddMenuItem(Strings.Menu.Help.MenuItem, null, null, null);

                // Help -> Whats New?
                AddMenuItem(Strings.Menu.Help.WhatsNew, commands.WhatsNewCommand, null, null, menuHelp);

                // Help -> Preferences
                AddMenuItem(Strings.Menu.Help.Preferences, commands.PreferencesCommand, null, null, menuHelp);

                // Help -> Online Manual
                AddMenuItem(Strings.Menu.Help.OnlineManual, commands.OnlineManualCommand, null, null, menuHelp);

                // Help -> Product Web Page
                AddMenuItem(Strings.Menu.Help.ProductWebPage, commands.ProductWebPageCommand, null, null, menuHelp);

                // Help -> About
                AddMenuItem(Strings.Menu.Help.About, commands.AboutCommand, null, null, menuHelp);
            }
            else
            {
                AddMenuItem("Run Code Generator", commands.StandaloneCodeGenerateCommand, 
                    @"/LHQ.App;component/Images/play.png", null);
            }

            // build tree view context menu
            ContextMenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.Menu.ExpandAll,
                Command = commands.ExpandAllElementCommand,
                GestureText = commands.ExpandAllElementCommand.KeyGesture.DisplayString
            });
            ContextMenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.Menu.CollapseAll,
                Command = commands.CollapseAllElementCommand,
                GestureText = commands.CollapseAllElementCommand.KeyGesture.DisplayString
            });
            ContextMenuItems.Add(MenuItemSeparatorViewModel.Create());
            ContextMenuItems.Add(NewCategoryMenuItem.Clone());
            ContextMenuItems.Add(NewResourceMenuItem.Clone());
            ContextMenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.Menu.Rename,
                Command = commands.RenameCommand,
                GestureText = commands.RenameCommand.KeyGesture.DisplayString
            });
            ContextMenuItems.Add(DeleteElementMenuItem.Clone());
            ContextMenuItems.Add(MenuItemSeparatorViewModel.Create());
            ContextMenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.Menu.Cut,
                Command = commands.CutElementCommand,
                GestureText = commands.CutElementCommand.KeyGesture.DisplayString
            });
            ContextMenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.Menu.Copy,
                Command = commands.CopyElementCommand,
                GestureText = commands.CopyElementCommand.KeyGesture.DisplayString
            });

            var copySpecialMenuItems = commands.CreateCopySpecialMenuItems();
            if (copySpecialMenuItems != null)
            {
                foreach (var specialMenuItem in copySpecialMenuItems)
                {
                    ContextMenuItems.Add(specialMenuItem);
                }
            }

            ContextMenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.Menu.Paste,
                Command = commands.PasteElementCommand,
                GestureText = commands.PasteElementCommand.KeyGesture.DisplayString
            });

            ContextMenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.Menu.Duplicate,
                Command = commands.DuplicateElementCommand,
                //GestureText = commands.DuplicateElementCommand.KeyGesture.DisplayString
            });

            RootModelViewModel.PropertyChanged += RootModelViewModelOnPropertyChanged;
            RootModelViewModel.LanguagesView.CollectionChanged += RootModelLanguagesChanged;
            ShellViewModel.PropertyChanged += ShellViewOnPropertyChanged;
        }

        public MenuItemViewModel DeleteElementMenuItem { get; }

        public MenuItemViewModel NewResourceMenuItem { get; }

        public ObservableCollectionExt<MenuItemViewModel> ContextMenuItems { get; }

        public MenuItemViewModel NewCategoryMenuItem { get; }

        public CommandsViewModel Commands { get; }

        public ObservableCollectionExt<MenuItemViewModel> MenuItems { get; }

        public string PrimaryLanguageDisplayName => RootModelViewModel.PrimaryLanguageDisplayName;

        public RootModelViewModel RootModelViewModel => ShellViewModel.RootModel;

        public string UndoTooltip
        {
            get
            {
                IUndoCommand nextUndo = ShellViewContext.UndoManager.PeekUndo();
                return nextUndo == null
                    ? Strings.Menu.UndoTooltip
                    : Strings.Menu.UndoTooltipNamed(nextUndo.GetName());
            }
        }

        public string RedoTooltip
        {
            get
            {
                IUndoCommand nextRedo = ShellViewContext.UndoManager.PeekRedo();
                return nextRedo != null
                    ? Strings.Menu.RedoTooltipNamed(nextRedo.GetName())
                    : Strings.Menu.RedoTooltip;
            }
        }

        private bool NewCategoryGetVisible()
        {
            return ShellViewModel.IsEditorMode && RootModelViewModel.ModelOptions.Categories;
        }

        private MenuItemViewModel AddMenuItem(string header, ICommand command, string imageSource,
            KeyGesture keyGesture, MenuItemViewModel parent = null)
        {
            var menuItem = new MenuItemViewModel
            {
                Header = header,
                Command = command,
                GestureText = keyGesture?.DisplayString,
                ImageUri = imageSource
            };

            if (parent == null)
            {
                MenuItems.Add(menuItem);
            }
            else
            {
                parent.MenuItems.Add(menuItem);
            }

            return menuItem;
        }

        private void AddMenuItemSeparator()
        {
            MenuItems.Add(new MenuItemSeparatorViewModel());
        }

        private void ShellViewOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ShellViewModel.ProjectBusyOperation))
            {
                CommandManager.InvalidateRequerySuggested();
            }
            else if (e.PropertyName == nameof(ShellViewModel.IsEditorMode))
            {
                NewCategoryMenuItem.RaiseObjectPropertiesChanged();
            }
        }

        private void RootModelLanguagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _primaryLanguageMenuItem.Header = Strings.Menu.LanguagesRoot(RootModelViewModel.Languages.Count, RootModelViewModel.PrimaryLanguage.EnglishName);
        }

        private void RootModelViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RootModelViewModel.PrimaryLanguage))
            {
                RaisePropertyChanged(nameof(PrimaryLanguageDisplayName));
                _primaryLanguageMenuItem.Header = Strings.Menu.LanguagesRoot(RootModelViewModel.Languages.Count, RootModelViewModel.PrimaryLanguage.EnglishName);
            }
            else if (e.PropertyName == nameof(RootModelViewModel.ModelOptions))
            {
                NewCategoryMenuItem.RaiseObjectPropertiesChanged();
            }
        }

        internal void RefreshUndoLocalizations()
        {
            RaisePropertyChanged(nameof(RedoTooltip));
            RaisePropertyChanged(nameof(UndoTooltip));
        }

        protected override void DoLocalizeStrings()
        {
            RefreshUndoLocalizations();
        }
    }
}
