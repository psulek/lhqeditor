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
using System.Globalization;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.Data;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.App.ViewModels.Elements
{
    public class ResourceValueViewModel : ViewModelElementBase<ResourceValueElement>
    {
        private string _languageName;
        private bool _isReferenceLanguage;
        private bool _locked;
        private string _resourceValue;
        private bool _isTranslationValid;
        private bool _visible;
        private string _displayName;
        private bool _isAutoFocus;
        private CultureInfo _languageCulture;
        private bool _autoTranslated;

        public ResourceValueViewModel(ResourceViewModel ownerResourceModel, ResourceValueElement modelResourceValue)
            : base(ownerResourceModel.ShellViewContext, modelResourceValue, false)
        {
            OwnerResource = ownerResourceModel;
            InitCommands();

            BuildModel(modelResourceValue);
        }

        public ResourceValueViewModel(ResourceViewModel ownerResourceModel, string languageName, string value = null)
            : base(ownerResourceModel.ShellViewContext, null)
        {
            OwnerResource = ownerResourceModel;
            InitCommands();

            BuildModel(new ResourceValueElement(GenerateElementKey())
            {
                LanguageName = languageName,
                Value = value ?? string.Empty,
                Locked = false
            });
        }

        private ITreeViewService TreeViewService => ShellViewContext.TreeViewService;

        public ICommand LockResourceValueCommand { get; private set; }

        public ResourceViewModel OwnerResource { get; }

        public string TranslateMenuHeader => Strings.ViewModels.ResourceValueModel.TranslateMenuHeader(LanguageEnglishName);

        public bool TranslationIsAllowed => !IsReferenceLanguage && EditAllowed;

        public double TranslationItemOpacity => TranslationIsAllowed ? 1 : 0.5;

        public bool EditAllowed => ShellViewModel.IsEditorMode && !Locked;

        public string LockMenuHeader => Locked
            ? Strings.ViewModels.ResourceValueModel.UnlockMenuHeader(LanguageEnglishName)
            : Strings.ViewModels.ResourceValueModel.LockMenuHeader(LanguageEnglishName);

        public string LockMenuToolTip => Locked
            ? Strings.ViewModels.ResourceValueModel.UnlockMenuTooltip(LanguageEnglishName)
            : Strings.ViewModels.ResourceValueModel.LockMenuTooltip(LanguageEnglishName);

        public string LockIconTooltip => Locked
            ? Strings.ViewModels.ResourceValueModel.LockIconTooltipIsLocked(LanguageEnglishName)
            : Strings.ViewModels.ResourceValueModel.LockIconTooltipIsNotLocked(LanguageEnglishName);

        public string AutoTranslatedTooltip => AutoTranslated
            ? Strings.ViewModels.ResourceValueModel.AutoTranslatedTooltip(LanguageEnglishName)
            : null;

        public string BurgerMenuHeader => Strings.ViewModels.ResourceValueModel.BurgerMenuHeader(LanguageEnglishName);

        public override string Name
        {
            get => DisplayName;
            set => throw new NotSupportedException();
        }

        public string DisplayName
        {
            get => _displayName;
            private set => SetProperty(ref _displayName, value);
        }

        private string LanguageEnglishName { get; set; }

        public string LanguageName
        {
            get => _languageName;
            set
            {
                if (_languageName != value)
                {
                    _languageName = value;
                    RaisePropertyChanged();

                    _languageCulture = CultureCache.Instance.GetCulture(value);
                    DisplayName = _languageCulture.EnglishName;

                    LanguageEnglishName = _languageCulture.EnglishName;
                }
            }
        }

        public bool IsReferenceLanguage
        {
            get => _isReferenceLanguage;
            private set => SetProperty(ref _isReferenceLanguage, value);
        }

        public bool Visible
        {
            get => _visible;
            set => SetProperty(ref _visible, value);
        }

        public bool Locked
        {
            get => _locked;
            set
            {
                if (_locked != value)
                {
                    bool oldValue = _locked;
                    string ownerResourceName = OwnerResource.Name;

                    string GetDescription()
                    {
                        return value
                            ? Strings.ViewModels.ResourceValueModel.PropertyLockedIsTrue(ownerResourceName, DisplayName)
                            : Strings.ViewModels.ResourceValueModel.PropertyLockedIsFalse(ownerResourceName, DisplayName);
                    }

                    IUndoCommand command = CreatePropertyChangeUndoCommand(this, OwnerResource, nameof(Locked),
                        value, oldValue, GetDescription);

                    TransactionManager.Execute(command);
                }
            }
        }

        public bool AutoTranslated
        {
            get => _autoTranslated;
            set
            {
                if (value != _autoTranslated)
                {
                    _autoTranslated = value;

                    bool oldValue = _autoTranslated;
                    string ownerResourceName = OwnerResource.Name;

                    string GetDescription()
                    {
                        return value
                            ? Strings.ViewModels.ResourceValueModel.PropertyAutoTranslatedIsTrue(ownerResourceName, DisplayName)
                            : Strings.ViewModels.ResourceValueModel.PropertyAutoTranslatedIsFalse(ownerResourceName, DisplayName);
                    }

                    IUndoCommand command = CreatePropertyChangeUndoCommand(this, OwnerResource, nameof(AutoTranslated),
                        value, oldValue, GetDescription);

                    TransactionManager.Execute(command);
                }
            }
        }

        public string Value
        {
            get => _resourceValue;
            set
            {
                if (value != _resourceValue)
                {
                    string oldValue = _resourceValue;

                    string GetInfo()
                    {
                        return "{0} {1}".FormatWith(DisplayName, Strings.Common.Properties.TranslationValue);
                    }

                    string GetDescription()
                    {
                        return Strings.Operations.PropertyChangeTransactionInfo1(GetInfo(), value);
                    }

                    IUndoCommand undoCommand = CreatePropertyChangeUndoCommand(this, OwnerResource,
                        nameof(Value), value, oldValue, GetDescription);

                    TransactionManager.Execute(undoCommand);
                }
            }
        }

        public bool IsTranslationValid
        {
            get => _isTranslationValid;
            private set => SetProperty(ref _isTranslationValid, value);
        }

        public bool IsAutoFocus
        {
            get
            {
                bool result = _isAutoFocus;
                if (result)
                {
                    _isAutoFocus = false;
                }

                return result;
            }
            set => SetProperty(ref _isAutoFocus, value);
        }

        private void InitCommands()
        {
            LockResourceValueCommand = new DelegateCommand(LockResourceValueExecute);
        }

        public override ModelElementType GetModelElementType()
        {
            return ModelElementType.ResourceValue;
        }

        private void LockResourceValueExecute(object args)
        {
            var isChecked = (bool)args;
            Locked = !isChecked;
        }

        protected override void BuildModelInternal(ResourceValueElement modelResourceValue)
        {
            LanguageName = modelResourceValue.LanguageName;
            Value = modelResourceValue.Value;
            Locked = modelResourceValue.Locked;
            AutoTranslated = modelResourceValue.Auto;

            RootModelViewModel rootModelViewModel = GetRootModel();
            IsReferenceLanguage = rootModelViewModel.GetPrimaryLanguageName().EqualsTo(LanguageName);
            Visible = true;
        }

        protected override void OnBuildModelComplete()
        {
            RefreshIsTranslationValid();
        }

        protected internal override void RaisePropertyChanged(string propertyName = "")
        {
            base.RaisePropertyChanged(propertyName);

            if (propertyName == nameof(Locked))
            {
                RaisePropertyChanged(nameof(LockMenuToolTip));
                RaisePropertyChanged(nameof(LockIconTooltip));
                RaisePropertyChanged(nameof(LockMenuHeader));
                RaisePropertyChanged(nameof(EditAllowed));
            }
            else if (propertyName == nameof(AutoTranslated))
            {
                RaisePropertyChanged(nameof(AutoTranslatedTooltip));
            }
            else if (propertyName == nameof(IsReferenceLanguage))
            {
                RaisePropertyChanged(nameof(TranslationIsAllowed));
                RaisePropertyChanged(nameof(TranslationItemOpacity));
            }
        }

        internal void RefreshIsReferenceLanguage()
        {
            RootModelViewModel rootModelViewModel = GetRootModel();
            IsReferenceLanguage = rootModelViewModel.GetPrimaryLanguageName().EqualsTo(LanguageName);
        }

        private void RefreshIsTranslationValid()
        {
            if (!IsBuildModelInProgress)
            {
                bool isTranslationValid = !Value.IsNullOrEmpty();
                if (isTranslationValid != IsTranslationValid)
                {
                    IsTranslationValid = isTranslationValid;
                    TreeViewService.NotifyTranslationValidChanged();
                }
            }
        }


        internal override void InternalSaveToModelElement(ResourceValueElement resourceValue)
        {
            resourceValue.LanguageName = LanguageName;
            resourceValue.Value = Value;
            resourceValue.Locked = Locked;
            resourceValue.Auto = AutoTranslated;
        }

        internal override bool InternalSetPropertyValue(string propertyName, object value)
        {
            var hasChanges = false;

            if (propertyName == nameof(Value))
            {
                hasChanges = true;
                _resourceValue = (string)value;
                RaisePropertyChanged(nameof(Value));
                RefreshIsTranslationValid();

                if (ModelBuildCompleted)
                {
                    // manuall set of '_autoTranslated' to false
                    _autoTranslated = false;
                    RaisePropertyChanged(nameof(AutoTranslated));

                    if (OwnerResource.State == ResourceElementTranslationState.New)
                    {
                        OwnerResource.State = ResourceElementTranslationState.Edited;
                    }
                }
            }
            else if (propertyName == nameof(Locked))
            {
                hasChanges = true;
                _locked = (bool)value;
                RaisePropertyChanged(nameof(Locked));
            }
            else if (propertyName == nameof(AutoTranslated))
            {
                hasChanges = true;
                _autoTranslated = (bool)value;
                RaisePropertyChanged(nameof(AutoTranslated));
            }

            return hasChanges;
        }

        internal void ChangeLanguage(string languageName)
        {
            CultureInfo cultureInfo = CultureCache.Instance.GetCulture(languageName);
            ArgumentValidator.EnsureArgumentNotNull(cultureInfo, "cultureInfo");
            LanguageName = languageName;
        }

        internal bool SetAutoTranslated(string translation, bool autoTranslated)
        {
            bool wasChangedValue = InternalSetPropertyValue(nameof(Value), translation);
            bool wasChangedAutoFlag = InternalSetPropertyValue(nameof(AutoTranslated), autoTranslated);

            return wasChangedValue || wasChangedAutoFlag;
        }

        public sealed class IsReferenceLanguageComparer : Comparer<ResourceValueViewModel>
        {
            public override int Compare(ResourceValueViewModel x, ResourceValueViewModel y)
            {
                int result = y.IsReferenceLanguage.CompareTo(x.IsReferenceLanguage);
                if (result == 0)
                {
                    result = string.CompareOrdinal(x.LanguageEnglishName, y.LanguageEnglishName);
                }

                return result;
            }
        }
    }
}
