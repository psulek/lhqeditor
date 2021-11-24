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
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Implementation.Undo;
using LHQ.App.Services.Interfaces;
using LHQ.App.UndoUnits;
using LHQ.Core.Model.Translation;
using LHQ.Data;
using LHQ.Data.Comparers;
using LHQ.Data.Extensions;
using LHQ.Data.Metadata;
using LHQ.Utils;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using Syncfusion.Windows.Tools.Controls;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace LHQ.App.ViewModels.Elements
{
    public class RootModelViewModel : CategoryLikeViewModel<Data.Model>
    {
        private LanguageViewModel _primaryLanguage;
        private bool _hasLanguages;
        private List<ITreeElementViewModel> _selectedTreeElements;
        private TreeSelectionInfo _selectionInfo;
        private string _modelVersion;
        private ICollectionView _languagesView;
        private int _allResourcesCount;
        private bool _disableConfirmPrimaryLanguageChange;
        private string _selectionDisplayText;
        private ModelOptions _modelOptions;
        private bool _modelIsEmpty;

        public RootModelViewModel(IShellViewContext shellViewContext, Data.Model model)
            : base(shellViewContext, model, null)
        {
            EditNameCommand = new DelegateCommand(EditNameExecute);

            Languages = new ObservableCollectionExt<LanguageViewModel>();
            BuildModel(model);
        }

        private void EditNameExecute(object obj)
        {
            string newName = Name;
            string caption = Strings.ViewModels.RootModel.EditModelNameCaption;

            if (DialogService.ShowPrompt(caption,
                Strings.ViewModels.RootModel.EditModelNameMessage, ref newName) && newName != Name)
            {
                if (DialogService.ShowConfirm(caption,
                    Strings.ViewModels.RootModel.EditModelNameWarningMessage, 
                    Strings.ViewModels.RootModel.EditModelNameWarningDetail) == DialogResult.Yes)
                {
                    Name = newName;
                }
            }
        }

        public ICommand EditNameCommand { get; set; }

        private ModelMetadataList Metadata { get; set; }

        public ITreeElementViewModel SelectedTreeElement { get; private set; }

        public List<TreeElementType> ElementTemplates { get; private set; }

        public List<ITreeElementViewModel> SelectedTreeElements
        {
            get => _selectedTreeElements;
            set
            {
                _selectedTreeElements = value;
                RaisePropertyChanged(nameof(SelectedTreeElements));

                ITreeElementViewModel oldSelectedElement = SelectedTreeElement;

                if (oldSelectedElement != null)
                {
                    oldSelectedElement.FlushCurrentChange = true;
                    oldSelectedElement.FlushCurrentChange = false;
                }

                SelectedTreeElement = value?.Count == 1 ? value.First() : null;
                RaisePropertyChanged(nameof(SelectedTreeElement));

                _selectionInfo = new TreeSelectionInfo(value);
                SelectionDisplayText = _selectionInfo.GetDisplayText();
            }
        }

        public ObservableCollectionExt<LanguageViewModel> Languages { get; }

        public ICollectionView LanguagesView
        {
            get => _languagesView;
            set => SetProperty(ref _languagesView, value);
        }

        public string SelectionDisplayText
        {
            get => _selectionDisplayText;
            set => SetProperty(ref _selectionDisplayText, value);
        }

        // ReSharper disable once UnusedMember.Global
        public bool HasLanguages
        {
            get => _hasLanguages;
            set => SetProperty(ref _hasLanguages, value);
        }

        public string ModelVersion
        {
            get => _modelVersion;
            set => SetProperty(ref _modelVersion, value);
        }

        public LanguageViewModel PrimaryLanguage
        {
            get => _primaryLanguage;
            set
            {
                if (_primaryLanguage != value)
                {
                    InternalSetPrimaryLanguage(value);
                    SetPrimaryLanguage(value.Name, false);
                }
            }
        }

        public string PrimaryLanguageDisplayName
        {
            get
            {
                CultureInfo primaryLanguageCulture = GetPrimaryLanguageCulture();
                return primaryLanguageCulture?.Format();
            }
        }

        public int AllResourcesCount
        {
            get => _allResourcesCount;
            set => SetProperty(ref _allResourcesCount, value);
        }

        public bool ModelIsEmpty
        {
            get => _modelIsEmpty;
            set => SetProperty(ref _modelIsEmpty, value);
        }

        public ModelOptions ModelOptions
        {
            get => _modelOptions;
            set => SetProperty(ref _modelOptions, value);
        }

        public override bool AllowDrop => true;

        public override bool AllowDrag => false;

        public DelegateCommand<SelectionChangedEventArgs> PrimaryLanguageChangedCommand { get; private set; }

        public override ModelElementType GetModelElementType()
        {
            return ModelElementType.Model;
        }

        private void InternalSetPrimaryLanguage(LanguageViewModel value)
        {
            _primaryLanguage = value;
            RaisePropertyChanged(nameof(PrimaryLanguage));
        }

        protected internal override void RaisePropertyChanged(string propertyName = "")
        {
            base.RaisePropertyChanged(propertyName);

            if (propertyName == nameof(PrimaryLanguage))
            {
                RaisePropertyChanged(nameof(PrimaryLanguageDisplayName));
            }

            if (propertyName == nameof(SelectedTreeElements))
            {
                ShellViewModel.MultiSelectionViewModel.SelectedElements = SelectedTreeElements;
            }
        }

        private bool CanChangePrimaryLanguage(LanguageViewModel language)
        {
            if (_disableConfirmPrimaryLanguageChange)
            {
                return true;
            }

            string message =
                Strings.ViewModels.RootModel.ChangePrimaryLanguageConfirmMessage(language.FormattedName);
            string caption = Strings.ViewModels.RootModel.ChangePrimaryLanguageConfirmCaption;

            return DialogService.ShowConfirm(caption, message, null) == DialogResult.Yes;
        }

        internal void RebuildModel(Data.Model model)
        {
            ResetBuildStage();

            using (TreeViewService.TemporaryBlock(TemporaryBlockType.RefreshTranslationValidity))
            {
                BuildModel(model);
            }
        }

        private void OnPrimaryLanguageChanged(SelectionChangedEventArgs args)
        {
            var comboBox = (ComboBoxAdv)args.Source;
            if (comboBox != null && (comboBox.Tag == null || !(bool)comboBox.Tag))
            {
                BindingExpression bindingExpression = comboBox.GetBindingExpression(Selector.SelectedItemProperty);
                if (bindingExpression != null && comboBox.SelectedItem is LanguageViewModel newSelection)
                {
                    if (CanChangePrimaryLanguage(newSelection))
                    {
                        bindingExpression.UpdateSource();
                    }
                    else
                    {
                        comboBox.Tag = true;
                        bindingExpression.UpdateTarget();
                        comboBox.Tag = false;
                    }
                }
            }
        }

        internal CultureInfo GetPrimaryLanguageCulture()
        {
            string languageName = GetPrimaryLanguageName();
            return languageName.IsNullOrEmpty() ? null : CultureCache.Instance.GetCulture(languageName);
        }

        protected override void BuildModelInternal(Data.Model modelElement)
        {
            ShellViewModel.RootModel = this;

            PrimaryLanguageChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(OnPrimaryLanguageChanged);

            if (modelElement != null)
            {
                List<LanguageElement> distinctLanguages = modelElement.Languages.Distinct(new LanguageEqualityByNameComparer()).ToList();
                IEnumerable<LanguageViewModel> languageViewModels;

                if (distinctLanguages.Count == 0)
                {
                    CultureInfo defaultCulture = UIService.DefaultCulture;

                    languageViewModels = UIService.UiLanguages.Select(
                        x => new LanguageViewModel(ShellViewContext, x, x.Culture.Name == defaultCulture.Name));
                }
                else
                {
                    languageViewModels = distinctLanguages.Select(x => x.ToViewModel(ShellViewContext));
                }

                Languages.Clear();
                Languages.AddRange(languageViewModels);

                var view = (ListCollectionView)CollectionViewSource.GetDefaultView(Languages);
                view.CustomSort = new LanguagePrimaryComparer();
                LanguagesView = view;

                _disableConfirmPrimaryLanguageChange = true;
                InternalSetPrimaryLanguage(Languages.Single(x => x.IsPrimary));
                _disableConfirmPrimaryLanguageChange = false;

                Metadata = modelElement.Metadata;
                ModelVersion = modelElement.Version.ToString();
                ModelOptions = modelElement.Options;

                ValidatorContext.Clear();

                UpdateVersion(modelElement.Version);
            }

            base.BuildModelInternal(modelElement);

            SelectedTreeElements = new List<ITreeElementViewModel>();

            ElementTemplates = new List<TreeElementType> { TreeElementType.Model, TreeElementType.Category, TreeElementType.Resource };

            ShellViewModel.UpdateTree();

            TreeViewService.NotifyPropertyValidator();

            RefreshAllResourcesCount();

            TreeViewService.Expand(this, false);
            TreeViewService.Select(this);
        }

        internal string GetPrimaryLanguageName()
        {
            return PrimaryLanguage.Name;
        }

        protected override TreeElementType GetElementType()
        {
            return TreeElementType.Model;
        }

        public bool NameEditAllowed => base.EditAllowed && !AppContext.RunInVsPackage;

        protected override IEnumerable<ResourceViewModel> PopulateChildResources(Data.Model modelElement)
        {
            return modelElement.Resources.OrderByName().Select(x => x.ToViewModel(this, ShellViewContext));
        }

        protected override IEnumerable<CategoryViewModel> PopulateChildCategories(Data.Model modelElement)
        {
            return modelElement.Categories.OrderByName().Select(x => x.ToViewModel(this, ShellViewContext));
        }

        public TreeSelectionInfo GetSelection()
        {
            return _selectionInfo;
        }

        internal override void InternalSaveToModelElement(Data.Model model)
        {
            base.InternalSaveToModelElement(model);

            List<LanguageElement> modelLanguages = Languages.Select(x => x.SaveToModelElement()).ToList();
            if (modelLanguages.Count > 0)
            {
                model.Languages.AddRange(modelLanguages);
            }

            model.Metadata = Metadata;

            List<CategoryElement> childCategories = GetCategories().Select(x => x.SaveToModelElement()).ToList();
            if (childCategories.Count > 0)
            {
                model.Categories.AddRange(childCategories);
            }

            List<ResourceElement> childResources = GetResources().Select(x => x.SaveToModelElement()).ToList();
            if (childResources.Count > 0)
            {
                model.Resources.AddRange(childResources);
            }

            model.Options = ModelOptions;
        }

        internal void SetPrimaryLanguage(string languageName, bool updateCurrentModel = true)
        {
            CultureInfo cultureInfo = CultureCache.Instance.GetCulture(languageName);
            ArgumentValidator.EnsureArgumentNotNull(cultureInfo, "cultureInfo");

            LanguageViewModel languageViewModel = Languages.Single(x => x.Name.EqualsTo(languageName));

            // mark all languages as not primary
            Languages.Run(x => x.UpdateIsPrimary(false));
            languageViewModel.UpdateIsPrimary(true);

            // update required language as primary
            if (updateCurrentModel)
            {
                PrimaryLanguage = languageViewModel;
            }

            RaisePropertyChanged(nameof(PrimaryLanguageDisplayName));

            TreeViewService.FindAllElements(x => x.ElementType == TreeElementType.Resource)
                .Cast<ResourceViewModel>()
                .Run(x => x.RefreshIsReferenceLanguage());
        }

        internal void InvalidateTranslationCountInfo()
        {
            foreach (LanguageViewModel languageViewModel in Languages)
            {
                languageViewModel.InvalidateTranslationCountInfo(true);
            }
        }

        internal void RefreshTranslationCountInfoForAllLanguages()
        {
            List<ResourceViewModel> allResources = TreeViewService.GetChildsAsFlatList(null)
                .Where(x => x.ElementType == TreeElementType.Resource)
                .Cast<ResourceViewModel>()
                .ToList();

            using (ShellViewContext.PropertyChangeObserver.BlockNotifications(Languages, true))
            {
                // reset values to zero!
                foreach (LanguageViewModel languageViewModel in Languages)
                {
                    languageViewModel.TranslatedResourcesCount = 0;
                    languageViewModel.UntranslatedResourcesCount = 0;
                    languageViewModel.HoldResourcesCount = 0;
                    languageViewModel.InvalidateTranslationCountInfo(false);
                }

                foreach (ResourceViewModel resource in allResources)
                {
                    List<TranslationCountInfo> translationCountInfos = resource.GetTranslationCountInfo();

                    foreach (TranslationCountInfo translationCountInfo in translationCountInfos)
                    {
                        LanguageViewModel languageViewModel = Languages.SingleOrDefault(x => x.Name == translationCountInfo.LanguageName);

                        if (languageViewModel != null)
                        {
                            switch (translationCountInfo.CountType)
                            {
                                case TranslationCountType.Translated:
                                {
                                    languageViewModel.TranslatedResourcesCount++;
                                    break;
                                }
                                case TranslationCountType.Untranslated:
                                {
                                    languageViewModel.UntranslatedResourcesCount++;
                                    break;
                                }
                                case TranslationCountType.Holded:
                                {
                                    languageViewModel.HoldResourcesCount++;
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
            }
        }

        internal void RefreshAllResourcesCount()
        {
            var allResourcesCount = 0;
            var modelIsEmpty = true;
            foreach (ITreeElementViewModel element in TreeViewService.GetChildsAsFlatList(null))
            {
                if (element.ElementType == TreeElementType.Resource)
                {
                    allResourcesCount++;
                }

                if (element.ElementType != TreeElementType.Model && modelIsEmpty)
                {
                    modelIsEmpty = false;
                }
            }

            AllResourcesCount = allResourcesCount;
            ModelIsEmpty = modelIsEmpty;
        }

        protected override ITreeElementViewModel CreateFromModelElement(ModelElementBase modelElement,
            ITreeElementViewModel parentElement)
        {
            return new RootModelViewModel(ShellViewContext, (Data.Model)modelElement);
        }

        public LanguageViewModel FindLanguageByKey(string languageElementKey)
        {
            return Languages.SingleOrDefault(x => x.ElementKey == languageElementKey);
        }

        public List<string> GetMissingLanguages(List<string> otherLanguageList)
        {
            List<string> currentLanguages = Languages.Select(x => x.Name).ToList();
            return otherLanguageList.Where(otherLang => !currentLanguages.Any(x => x.EqualsTo(otherLang))).ToList();
        }

        public void UpdateLanguages(List<string> languages, Dictionary<string, string> replacedLanguages, string primaryLanguageName)
        {
            TransactionManager.Execute(new UpdateLanguagesUndoUnit(languages, replacedLanguages, primaryLanguageName));
        }

        public void AddMissingLanguages(List<string> missingLanguages)
        {
            TransactionManager.Execute(new UndoCommand(Guid.NewGuid().ToString())
            {
                Name = () => Strings.Views.RootView.AddMissingLanguages(missingLanguages.ToDelimitedString()),
                Do = () => AddOrRemoveLanguages(missingLanguages, true),
                Undo = () => AddOrRemoveLanguages(missingLanguages, false)
            });
        }

        internal void AddOrRemoveLanguages(List<string> languages, bool add)
        {
            if (add)
            {
                languages = GetMissingLanguages(languages);
                if (languages.Count > 0)
                {
                    foreach (string language in languages)
                    {
                        CultureInfo cultureInfo = CultureCache.Instance.GetCulture(language);
                        var languageViewModel = new LanguageViewModel(ShellViewContext, cultureInfo, false);
                        Languages.Add(languageViewModel);
                    }

                    TreeViewService.FindAllElements(x => x.ElementType == TreeElementType.Resource)
                        .Cast<ResourceViewModel>()
                        .Run(x => x.AddMissingLanguages());
                }
            }
            else
            {
                int oldLanguageCount = Languages.Count;
                Languages.RemoveAll(x => languages.Any(y => y.EqualsTo(x.Name)));
                if (Languages.Count < oldLanguageCount)
                {
                    TreeViewService.FindAllElements(x => x.ElementType == TreeElementType.Resource)
                        .Cast<ResourceViewModel>()
                        .Run(x => x.RemoveUnusedLanguages());
                }
            }
        }

        public void AutoTranslateResources(TranslationResult translationResult, List<string> resourceKeys, bool onlyEmptyValues)
        {
            TransactionManager.Execute(new AutoTranslateMultiResourcesUndoUnit(this, translationResult, resourceKeys, onlyEmptyValues));
        }
    }
}
