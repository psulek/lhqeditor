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
using System.Windows;
using LHQ.App.Code;
using LHQ.App.Components.Converters;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.UndoUnits;
using LHQ.App.ViewModels.Elements;
using LHQ.Data;
using LHQ.Utils;
using LHQ.Utils.Extensions;

namespace LHQ.App.ViewModels
{
    public class MultiSelectionViewModel : ShellViewModelBase
    {
        private readonly ITreeViewService _treeViewService;
        private readonly TranslationStateConverter _translationStateConverter;
        private RootModelViewModel _model;
        private bool _hasModel;
        private bool _modelHasDescription;
        private ObservableCollectionExt<CategoryViewModel> _categories;
        private bool _hasCategories;
        private ObservableCollectionExt<ResourceViewModel> _resources;
        private bool _hasResources;
        private Thickness _resourcesMargin;
        private string _categoriesHeader;
        private string _resourcesHeader;
        private List<ITreeElementViewModel> _selectedElements;

        public MultiSelectionViewModel(IShellViewContext shellViewContext)
            : base(shellViewContext, false)
        {
            _treeViewService = shellViewContext.TreeViewService;
            _translationStateConverter = new TranslationStateConverter(TranslationStateConverterType.Text);
            Categories = new ObservableCollectionExt<CategoryViewModel>();
            Resources = new ObservableCollectionExt<ResourceViewModel>();

            ActionsMenuItems = new ObservableCollectionExt<MenuItemViewModel>();
            BuildActionsMenuItems();
        }

        public List<ITreeElementViewModel> SelectedElements
        {
            get => _selectedElements;
            set
            {
                SetProperty(ref _selectedElements, value);
                Build();
            }
        }

        public RootModelViewModel Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        public ObservableCollectionExt<MenuItemViewModel> ActionsMenuItems { get; }

        public bool HasModel
        {
            get => _hasModel;
            set => SetProperty(ref _hasModel, value);
        }

        public bool ModelHasDescription
        {
            get => _modelHasDescription;
            set => SetProperty(ref _modelHasDescription, value);
        }

        public ObservableCollectionExt<CategoryViewModel> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public bool HasCategories
        {
            get => _hasCategories;
            set => SetProperty(ref _hasCategories, value);
        }

        public ObservableCollectionExt<ResourceViewModel> Resources
        {
            get => _resources;
            set => SetProperty(ref _resources, value);
        }

        public bool HasResources
        {
            get => _hasResources;
            set => SetProperty(ref _hasResources, value);
        }

        public Thickness ResourcesMargin
        {
            get => _resourcesMargin;
            set => SetProperty(ref _resourcesMargin, value);
        }

        public string CategoriesHeader
        {
            get => _categoriesHeader;
            set => SetProperty(ref _categoriesHeader, value);
        }

        public string ResourcesHeader
        {
            get => _resourcesHeader;
            set => SetProperty(ref _resourcesHeader, value);
        }

        private void BuildActionsMenuItems()
        {
            ActionsMenuItems.Clear();
            ActionsMenuItems.Add(new MenuItemSeparatorViewModel());

            ActionsMenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.ViewModels.MultiSelection.ChangeStateToNew,
                Tag = ResourceElementTranslationState.New,
                Command = new DelegateCommand(o => GenericActionExecute(o, ModelActionKind.ChangeStateToNew))
            });
            ActionsMenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.ViewModels.MultiSelection.ChangeStateToEdited,
                Tag = ResourceElementTranslationState.Edited,
                Command = new DelegateCommand(o => GenericActionExecute(o, ModelActionKind.ChangeStateToEdited))
            });
            ActionsMenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.ViewModels.MultiSelection.ChangeStateToNeedsReview,
                Tag = ResourceElementTranslationState.NeedsReview,
                Command = new DelegateCommand(o => GenericActionExecute(o, ModelActionKind.ChangeStateToNeedsReview))
            });
            ActionsMenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.ViewModels.MultiSelection.ChangeStateToFinal,
                Tag = ResourceElementTranslationState.Final,
                Command = new DelegateCommand(o => GenericActionExecute(o, ModelActionKind.ChangeStateToFinal))
            });

            ActionsMenuItems.Add(new MenuItemSeparatorViewModel());

            ActionsMenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.Views.ResourceView.TranslateEmptyValuesMenuHeader,
                Command = new DelegateCommand(o => GenericActionExecute(o, ModelActionKind.TranslateEmptyValues))
            });

            ActionsMenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.Views.ResourceView.TranslateAllValuesMenuHeader,
                Command = new DelegateCommand(o => GenericActionExecute(o, ModelActionKind.TranslateAllValues))
            });

            ActionsMenuItems.Add(new MenuItemSeparatorViewModel());

            ActionsMenuItems.Add(new MenuItemViewModel
            {
                Header = Strings.ViewModels.MultiSelection.ExportResourcesToFile,
                ImageVisible = false,
                Command = new DelegateCommand(o => GenericActionExecute(o, ModelActionKind.ExportResources))
            });
        }

        private void Build()
        {
            Categories.Clear();
            Resources.Clear();
            Model = null;

            foreach (ITreeElementViewModel element in _selectedElements)
            {
                switch (element.ElementType)
                {
                    case TreeElementType.Model:
                    {
                        Model = (RootModelViewModel)element;
                        break;
                    }
                    case TreeElementType.Category:
                    {
                        Categories.Add((CategoryViewModel)element);
                        break;
                    }
                    case TreeElementType.Resource:
                    {
                        Resources.Add((ResourceViewModel)element);
                        break;
                    }
                    default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }

            HasModel = Model != null;
            ModelHasDescription = Model != null && !Model.Description.IsNullOrEmpty();

            CategoriesHeader = Strings.ViewModels.MultiSelection.CategoriesHeader(Categories.Count);
            HasCategories = Categories.Count > 0;

            ResourcesHeader = Strings.ViewModels.MultiSelection.ResourcesHeader(Resources.Count);
            HasResources = Resources.Count > 0;
            ResourcesMargin = new Thickness(0, HasModel || HasCategories ? 10 : 0, 0, 0);
        }

        private void GenericActionExecute(object obj, ModelActionKind actionKind)
        {
            var treeElementType = (TreeElementType)obj;

            var showRecursiveConfirm = true;
            Func<bool, IEnumerable<ResourceViewModel>> getResources;
            switch (treeElementType)
            {
                case TreeElementType.Model:
                {
                    getResources = GetModelResources;
                    break;
                }
                case TreeElementType.Category:
                {
                    getResources = GetCategoryResources;
                    break;
                }
                case TreeElementType.Resource:
                {
                    getResources = b => Resources;
                    showRecursiveConfirm = false;
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            ExecuteAction(treeElementType, actionKind, getResources, showRecursiveConfirm);
        }

        private IEnumerable<ResourceViewModel> GetModelResources(bool recursive)
        {
            return recursive
                ? _treeViewService.FindAllElements(x => x.ElementType == TreeElementType.Resource)
                    .Cast<ResourceViewModel>()
                : _treeViewService.RootModel.Children.Where(x => x.ElementType == TreeElementType.Resource)
                    .Cast<ResourceViewModel>();
        }

        private IEnumerable<ResourceViewModel> GetCategoryResources(bool recursive)
        {
            var resources = new List<ResourceViewModel>();
            var processedCategories = new List<CategoryViewModel>();

            foreach (CategoryViewModel category in Categories.OrderBy(x => x.GetParentsDepth()))
            {
                if (processedCategories.Contains(category))
                {
                    continue;
                }

                IEnumerable<ITreeElementViewModel> childElements = recursive
                    ? _treeViewService.GetChildsAsFlatList(category)
                    : category.Children;

                foreach (ITreeElementViewModel childElement in childElements)
                {
                    if (childElement.ElementType == TreeElementType.Resource)
                    {
                        resources.Add((ResourceViewModel)childElement);
                    }
                    else
                    {
                        var childCategory = (CategoryViewModel)childElement;
                        if (!processedCategories.Contains(childCategory))
                        {
                            processedCategories.Add(childCategory);
                        }
                    }
                }
            }

            return resources;
        }

        private void ExecuteAction(TreeElementType selectedElementType, ModelActionKind action, 
            Func<bool, IEnumerable<ResourceViewModel>> getResources, bool showRecursiveConfirm)
        {
            ResourceElementTranslationState? state;

            string dialogCaption;
            string dialogMessage;
            string dialogDetail;
            
            string confirmMessage = Strings.ViewModels.MultiSelection.ScanCategoriesRecursivelyConfirmMessage;
            DialogButtons confirmButtons = DialogButtons.YesNoCancel;
            string confirmYesButtonHeader = Strings.ViewModels.MultiSelection.ButtonRecursive;
            string confirmNoButtonHeader = Strings.ViewModels.MultiSelection.ButtonFirstLevel;

            var recursiveScan = false;

            var rootModelNoResourcesUnder =
                selectedElementType == TreeElementType.Model && ShellViewModel.ModelOptions.Resources != ModelOptionsResources.All;

            List<ResourceViewModel> resourcesToProcess = null;
            Action actionToDo;

            if (action.In(ModelActionKind.ChangeStateToNew, ModelActionKind.ChangeStateToEdited,
                ModelActionKind.ChangeStateToNeedsReview, ModelActionKind.ChangeStateToFinal))
            {
                state = GetTranslationState(action);
                string actionText = _translationStateConverter.GetText(state.Value);
                dialogCaption = Strings.ViewModels.MultiSelection.ChangeResourcesStateFailedCaption;
                dialogMessage = Strings.ViewModels.MultiSelection.ChangeResourcesStateFailedMessage(actionText);
                dialogDetail = Strings.ViewModels.MultiSelection.ChangeResourcesStateFailedDetail;

                if (rootModelNoResourcesUnder)
                {
                    confirmMessage = Strings.ViewModels.MultiSelection.ChangeStateAllResourcesRecursive(actionText);
                    confirmButtons = DialogButtons.YesNo;
                    confirmYesButtonHeader = null;
                    confirmNoButtonHeader = null;
                }

                actionToDo = () =>
                    {
                        TransactionManager.Execute(new MultiSelectionActionUndoUnit(ShellViewContext, resourcesToProcess, state.Value));
                    };
            }
            else if (action == ModelActionKind.ExportResources)
            {
                dialogCaption = Strings.ViewModels.MultiSelection.ExportResourcesToFile;
                dialogMessage = Strings.ViewModels.MultiSelection.ExportResourcesFailedMessage;
                dialogDetail = Strings.ViewModels.MultiSelection.ExportResourcesFailedDetail;

                if (rootModelNoResourcesUnder)
                {
                    confirmMessage = Strings.ViewModels.MultiSelection.ExportAllResourcesRecursive;
                    confirmButtons = DialogButtons.YesNo;
                    confirmYesButtonHeader = null;
                    confirmNoButtonHeader = null;
                }

                actionToDo = () =>
                    {
                        AppContext.DialogService.ShowExportResourcesDialog(ShellViewContext, recursiveScan,
                            resourcesToProcess.Select(x => x.ElementKey).ToList());
                    };
            }
            else if (action.In(ModelActionKind.TranslateAllValues, ModelActionKind.TranslateEmptyValues))
            {
                dialogCaption = Strings.ViewModels.MultiSelection.TranslateResourcesCaption;
                dialogMessage = Strings.ViewModels.MultiSelection.TranslateResourcesFailedMessage;
                dialogDetail = Strings.ViewModels.MultiSelection.TranslateResourcesFailedDetail;

                if (rootModelNoResourcesUnder)
                {
                    confirmMessage = Strings.ViewModels.MultiSelection.TranslateAllResourcesRecursive;
                    confirmButtons = DialogButtons.YesNo;
                    confirmYesButtonHeader = null;
                    confirmNoButtonHeader = null;
                }

                actionToDo = () => { RecursiveAutoTranslateAll(action, resourcesToProcess); };
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Value {action} is out of range of enum 'ModelActionKind'.");
            }

            if (showRecursiveConfirm)
            {
                DialogResult dialogResult = DialogService.ShowConfirm(dialogCaption,
                    confirmMessage, null, confirmButtons, yesButtonHeader: confirmYesButtonHeader,
                    noButtonHeader: confirmNoButtonHeader);

                if (dialogResult == DialogResult.Cancel)
                {
                    return;
                }

                recursiveScan = dialogResult == DialogResult.Yes;
            }

            resourcesToProcess = getResources(recursiveScan).ToList();
            if (resourcesToProcess.Count == 0)
            {
                DialogService.ShowWarning(dialogCaption, dialogMessage, dialogDetail);
            }
            else
            {
                actionToDo();
            }
        }

        private void RecursiveAutoTranslateAll(ModelActionKind action, List<ResourceViewModel> resourcesToProcess)
        {
            var resourceKeys = resourcesToProcess.Select(x => x.GetElementKey()).ToArray();
            var translationMode = action == ModelActionKind.TranslateEmptyValues ? SuggestTranslationMode.Empty : SuggestTranslationMode.All;
            ShellService.TranslateResources(resourceKeys, translationMode, null);
        }

        private ResourceElementTranslationState GetTranslationState(ModelActionKind actionKind)
        {
            switch (actionKind)
            {
                case ModelActionKind.ChangeStateToNew:
                {
                    return ResourceElementTranslationState.New;
                }
                case ModelActionKind.ChangeStateToEdited:
                {
                    return ResourceElementTranslationState.Edited;
                }
                case ModelActionKind.ChangeStateToNeedsReview:
                {
                    return ResourceElementTranslationState.NeedsReview;
                }
                case ModelActionKind.ChangeStateToFinal:
                {
                    return ResourceElementTranslationState.Final;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(actionKind), actionKind, null);
                }
            }
        }
    }

    internal enum ModelActionKind
    {
        ChangeStateToNew = 0,
        ChangeStateToEdited = 1,
        ChangeStateToNeedsReview = 2,
        ChangeStateToFinal = 3,
        ExportResources = 4,
        TranslateEmptyValues = 5,
        TranslateAllValues = 6
    }
}
