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
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Dialogs;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Data;
using LHQ.Data.Comparers;
using LHQ.Data.Interfaces.Key;
using LHQ.Utils;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace LHQ.App.ViewModels.Elements
{
    public sealed class ResourceViewModel : TreeElementViewModel<ResourceElement>
    {
        private string _description;
        private ICollectionView _valuesView;
        private string _translationNote;
        private ResourceParameterElementList _parameters;
        private ICollectionView _parametersView;
        private ResourceElementTranslationState _state;

        public ResourceViewModel(IShellViewContext shellViewContext, ITreeElementViewModel parentElement, ResourceElement modelResource)
            : base(shellViewContext, modelResource, parentElement)
        {
            Values = new ObservableCollectionExt<ResourceValueViewModel>();

            BuildModel(modelResource);
            RaisePropertyChanged(nameof(ParentCategory));

            SuggestTranslationCommand = new DelegateCommand(SuggestTranslationExecute);
            SuggestAllTranslationCommand = new DelegateCommand(SuggestAllTranslationExecute, SuggestAllTranslationCanExecute);
            DefineParametersCommand = new DelegateCommand(DefineParametersExecute, DefineParametersCanExecute);
            LockUnlockResourceValuesCommand = new DelegateCommand(LockUnlockResourceValuesExecute);
            HintPanelViewModel = new HintPanelViewModel(shellViewContext, AppHintType.ResourceValue);
        }

        public string LockUnlockResourceValuesHeader
        {
            get
            {
                int lockedCount = Values.Count(x => x.Locked);
                return lockedCount > 0
                    ? (lockedCount == Values.Count
                        ? Strings.ViewModels.ResourceModel.UnlockAllValues
                        : Strings.ViewModels.ResourceModel.LockAllValues)
                    : Strings.ViewModels.ResourceModel.LockAllValues;
            }
        }

        public string LockUnlockResourceHeader => Locked
            ? Strings.ViewModels.ResourceModel.UnlockResource
            : Strings.ViewModels.ResourceModel.LockResource;

        public string CurrentLockStatusHeader => Locked
            ? Strings.ViewModels.ResourceModel.StateLocked
            : Strings.ViewModels.ResourceModel.StateNotLocked;

        public bool CurrentLockStatusImageVisible => Locked;

        public HintPanelViewModel HintPanelViewModel { get; }

        public ICommand SuggestTranslationCommand { get; }

        public ICommand SuggestAllTranslationCommand { get; }

        public ICommand LockUnlockResourceValuesCommand { get; }

        public ICommand DefineParametersCommand { get; }

        public string Description
        {
            get => _description;
            set => SetPropertyWithUndo(ref _description, value, Strings.Common.Properties.Description);
        }

        public CategoryViewModel ParentCategory
        {
            get => parent as CategoryViewModel;
            private set => SetProperty(ref parent, value);
        }

        public ObservableCollectionExt<ResourceValueViewModel> Values { get; }

        public ICollectionView ValuesView
        {
            get => _valuesView;
            set => SetProperty(ref _valuesView, value);
        }

        public ResourceParameterElementList Parameters
        {
            get => _parameters;
            set => SetPropertyWithUndo(ref _parameters, value, Strings.Common.Properties.Parameters);
        }

        public ICollectionView ParametersView
        {
            get => _parametersView;
            set => SetProperty(ref _parametersView, value);
        }

        public string TranslationNote
        {
            get => _translationNote;
            set => SetPropertyWithUndo(ref _translationNote, value, Strings.Common.Properties.TranslationNote);
        }

        public ResourceElementTranslationState State
        {
            get => _state;
            set
            {
                if (value != _state && value == ResourceElementTranslationState.Final && ModelBuildCompleted)
                {
                    NotifyStateFinalLocksResource();
                }

                SetPropertyWithUndo(ref _state, value, Strings.Common.Properties.TranslationState);
            }
        }

        public bool Locked => State == ResourceElementTranslationState.Final;

        public bool EditAllowed => ShellViewModel.IsEditorMode && !Locked;

        public override bool AllowDrop => false;

        public override bool AllowDrag => true;

        public override ModelElementType GetModelElementType()
        {
            return ModelElementType.Resource;
        }

        private bool DefineParametersCanExecute(object arg)
        {
            return ShellViewModel.IsEditorMode && !Locked;
        }

        private void DefineParametersExecute(object obj)
        {
            if (ResourceParametersDialog.DialogShow(ShellViewContext, Parameters, out ResourceParameterElementList updatedParameters))
            {
                if (!Parameters.HasEqualBaseProps(updatedParameters))
                {
                    IModelElementKeyProvider keyProvider = ShellViewModel.ModelContext.KeyProvider;
                    updatedParameters?.Run(x => { x.UpdateKey(keyProvider.GenerateKey()); });

                    Parameters = updatedParameters;
                }
            }
        }

        protected internal override void RaisePropertyChanged(string propertyName = "")
        {
            base.RaisePropertyChanged(propertyName);

            bool changedState = propertyName == nameof(State);
            if (propertyName == nameof(Name) || changedState)
            {
                RaisePropertyChanged(nameof(LockUnlockResourceHeader));

                if (changedState)
                {
                    RaisePropertyChanged(nameof(Locked));
                    RaisePropertyChanged(nameof(CurrentLockStatusHeader));
                    RaisePropertyChanged(nameof(CurrentLockStatusImageVisible));
                    RaisePropertyChanged(nameof(EditAllowed));
                }
            }
        }

        private bool SuggestAllTranslationCanExecute(object arg)
        {
            return GetRootModel().Languages.Any(x => !x.IsPrimary);
        }

        private void LockUnlockResourceValuesExecute(object obj)
        {
            var lockAll = true;
            int lockedCount = Values.Count(x => x.Locked);
            if (lockedCount > 0)
            {
                lockAll = lockedCount != Values.Count;
            }

            Parent.LockResourceValues(this, lockAll);
        }

        private void NotifyStateFinalLocksResource()
        {
            bool lockValues = false;
            string caption = Strings.ViewModels.ResourceModel.FinalStateConfirmCaption;

            AppStateStorage appStateStorage = AppContext.AppStateStorageService.Current;
            if (!appStateStorage.StateFinalLocksResourceNotified)
            {
                string message = Strings.ViewModels.ResourceModel.FinalStateConfirmMessageNotify;
                DialogService.ShowInfo(caption, message, null);

                AppContext.AppStateStorageService.Update(storage => appStateStorage.StateFinalLocksResourceNotified = true);
            }

            // and if so, ask question if values can be locked together with owner resource
            if (Values.Any(x => !x.Locked))
            {
                var message = Strings.ViewModels.ResourceModel.FinalStateConfirmMessageLock;
                var rememberText = Strings.ViewModels.ResourceModel.FinalStateConfirmRememberText;
                var rememberHint = Strings.ViewModels.ResourceModel.FinalStateConfirmRememberHint;

                bool? lockTranslationsWithResource = ShellViewModel.AppConfig.LockTranslationsWithResource;
                if (lockTranslationsWithResource == null)
                {
                    var rememberChoice = false;
                    DialogResult dialogResult = DialogService.ShowConfirmRemember(caption, message, null,
                        DialogButtons.YesNoCancel, ref rememberChoice, rememberText, rememberHint);

                    if (dialogResult == DialogResult.Cancel)
                    {
                        return;
                    }

                    lockValues = dialogResult == DialogResult.Yes;
                    ShellViewModel.AppConfig.LockTranslationsWithResource = rememberChoice
                        ? lockValues
                        : (bool?)null;

                    AppContext.ApplicationService.SaveAppConfig();
                }
                else
                {
                    lockValues = lockTranslationsWithResource.Value;
                }
            }

            if (lockValues)
            {
                Parent.LockResourceValues(this, true);
            }
        }

        private void SuggestAllTranslationExecute(object args)
        {
            if (args != null)
            {
                SuggestTranslationMode mode = Enum<SuggestTranslationMode>.Parse(args as string);
                InternalAutoTranslate(mode);
            }
        }

        private void SuggestTranslationExecute(object obj)
        {
            var resourceValue = obj as ResourceValueViewModel;
            ResourceValueViewModel referenceValue = resourceValue?.OwnerResource.Values.SingleOrDefault(x => x.IsReferenceLanguage);
            if (referenceValue != null && !referenceValue.Value.IsNullOrEmpty())
            {
                InternalAutoTranslate(SuggestTranslationMode.All, resourceValue.LanguageName);
            }
        }

        private void InternalAutoTranslate(SuggestTranslationMode mode, string onlyLanguage = null)
        {
            ShellService.TranslateResources(new[] { GetElementKey() }, mode, onlyLanguage);
        }

        protected override void BuildModelInternal(ResourceElement resource)
        {
            // values
            Values.Clear();
            Values.AddRange(resource.Values.Select(x => x.ToViewModel(this)));
            var valuesView = (ListCollectionView)CollectionViewSource.GetDefaultView(Values);
            valuesView.CustomSort = new ResourceValueViewModel.IsReferenceLanguageComparer();
            ValuesView = valuesView;

            // parameters
            Parameters = resource.Parameters.Clone();
            var parametersView = (ListCollectionView)CollectionViewSource.GetDefaultView(Parameters);
            parametersView.CustomSort = new ResourceParameterComparer();
            ParametersView = parametersView;

            AddMissingLanguages();

            Description = resource.Description;
            TranslationNote = resource.TranslationNote;
            State = resource.State;

            base.BuildModelInternal(resource);
        }

        protected override void OnUISubscriberRegistrationChanged(bool registered)
        {
            // if (registered)
            // {
            //     UIService.RegisterSubscriber(ValuesHint);
            // }
            // else
            // {
            //     UIService.UnregisterSubscriber(ValuesHint);
            // }
        }

        protected override bool GetIsExpandable()
        {
            return false;
        }

        protected override IEnumerable<ITreeElementViewModel> PopulateChilds(ResourceElement modelElement)
        {
            yield break;
        }

        protected override TreeElementType GetElementType()
        {
            return TreeElementType.Resource;
        }

        internal List<TranslationCountInfo> GetTranslationCountInfo()
        {
            return Values.Select(x => new TranslationCountInfo(x)).ToList();
        }

        internal override void InternalSaveToModelElement(ResourceElement resource)
        {
            resource.Description = Description;
            resource.TranslationNote = TranslationNote;
            resource.State = State;

            resource.Parameters.Clear();
            resource.Parameters.AddRange(Parameters);
            resource.Parameters.Run(x => x.SetParentElement(resource));

            List<ResourceValueElement> modelValues = Values.Select(x => x.SaveToModelElement()).ToList();
            if (modelValues.Count > 0)
            {
                resource.Values.AddRange(modelValues);
            }
        }

        protected override ITreeElementViewModel CreateFromModelElement(ModelElementBase modelElement,
            ITreeElementViewModel parentElement)
        {
            return new ResourceViewModel(ShellViewContext, parentElement, (ResourceElement)modelElement);
        }

        public ResourceValueViewModel FindValueByKey(string modelElementKey)
        {
            return Values.SingleOrDefault(x => x.ElementKey == modelElementKey);
        }

        internal override bool InternalSetPropertyValue(string propertyName, object value)
        {
            bool hasChanges = base.InternalSetPropertyValue(propertyName, value);

            if (propertyName == nameof(State))
            {
                hasChanges = true;
                _state = (ResourceElementTranslationState)value;
                RaisePropertyChanged(nameof(State));
            }
            else if (propertyName == nameof(Description))
            {
                hasChanges = true;
                _description = value as string;
                RaisePropertyChanged(nameof(Description));
            }
            else if (propertyName == nameof(Parameters))
            {
                hasChanges = true;
                _parameters = value as ResourceParameterElementList;
                RaisePropertyChanged(nameof(Parameters));
            }
            else if (propertyName == nameof(TranslationNote))
            {
                hasChanges = true;
                _translationNote = (string)value;
                RaisePropertyChanged(nameof(TranslationNote));
            }

            return hasChanges;
        }

        internal void AddMissingLanguages()
        {
            RootModelViewModel rootModel = GetRootModel();

            var valuesLanguages = new HashSet<string>(Values.Select(x => x.LanguageName));
            foreach (LanguageViewModel language in rootModel.Languages)
            {
                if (!valuesLanguages.Contains(language.Name))
                {
                    var resourceValueViewModel = new ResourceValueViewModel(this, language.Name);
                    Values.Add(resourceValueViewModel);
                }
            }
        }

        internal void RemoveUnusedLanguages()
        {
            RootModelViewModel rootModel = GetRootModel();
            ObservableCollectionExt<LanguageViewModel> currentLanguages = rootModel.Languages;

            Values.RemoveAll(lang => !currentLanguages.Any(x => x.Name.EqualsTo(lang.LanguageName)));
        }

        internal void RefreshIsReferenceLanguage()
        {
            Values.Run(x => x.RefreshIsReferenceLanguage());
            ValuesView.Refresh();
        }

        public ResourceValueViewModel GetReferenceLanguageValue()
        {
            return Values.Single(x => x.IsReferenceLanguage);
        }
    }
}
