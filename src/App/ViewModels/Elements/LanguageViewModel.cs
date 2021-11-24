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

using System.Globalization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Data;
using LHQ.Data.Interfaces;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.ViewModels.Elements
{
    public class LanguageViewModel : ViewModelElementBase<LanguageElement>, IModelLanguageElement
    {
        private string _formattedName;
        private string _englishName;
        private bool _isPrimary;
        private string _nativeName;
        private bool _pendingInvalidateTranslationCountInfo;
        private int _translatedResourcesCount;
        private int _untranslatedResourcesCount;
        private int _holdResourcesCount;
        private bool _isNeutralCulture;
        private string _name;

        public LanguageViewModel(IShellViewContext shellViewContext, LanguageElement modelLanguage)
            : base(shellViewContext, modelLanguage)
        {
            UnsubscribeFromLocalizationChange();
            BuildModel(modelLanguage);
        }

        public LanguageViewModel(IShellViewContext shellViewContext, UILanguage uiLanguage, bool isPrimary)
            : this(shellViewContext, uiLanguage.Culture, isPrimary)
        { }

        public LanguageViewModel(IShellViewContext shellViewContext, CultureInfo cultureInfo, bool isPrimary)
            : base(shellViewContext, null)
        {
            UnsubscribeFromLocalizationChange();
            BuildModel(null);
            UpdateFrom(cultureInfo);
            IsPrimary = isPrimary;

            TranslatedResourcesCount = 0;
            UntranslatedResourcesCount = 0;
            HoldResourcesCount = 0;
            MarkBuildModelCompleted(true);
        }

        public override string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string FormattedName
        {
            get => _formattedName;
            private set => SetProperty(ref _formattedName, value);
        }

        public string NativeName
        {
            get => _nativeName;
            set => SetProperty(ref _nativeName, value);
        }

        public string EnglishName
        {
            get => _englishName;
            private set => SetProperty(ref _englishName, value);
        }

        public bool IsPrimary
        {
            get => _isPrimary;
            private set => SetProperty(ref _isPrimary, value);
        }

        bool IModelLanguageElement.IsPrimary
        {
            get => IsPrimary;
            set => IsPrimary = value;
        }

        public int TranslatedResourcesCount
        {
            get
            {
                ConditionallyRefreshTranslationCountInfo();
                return _translatedResourcesCount;
            }
            set => SetProperty(ref _translatedResourcesCount, value);
        }

        public int UntranslatedResourcesCount
        {
            get
            {
                ConditionallyRefreshTranslationCountInfo();

                return _untranslatedResourcesCount;
            }
            set => SetProperty(ref _untranslatedResourcesCount, value);
        }

        public int HoldResourcesCount
        {
            get
            {
                ConditionallyRefreshTranslationCountInfo();

                return _holdResourcesCount;
            }
            set
            {
                if (SetProperty(ref _holdResourcesCount, value))
                {
                    RaisePropertyChanged(nameof(HasHoldResources));
                }
            }
        }

        public bool HasHoldResources => HoldResourcesCount > 0;

        public bool IsNeutralCulture
        {
            get => _isNeutralCulture;
            private set => SetProperty(ref _isNeutralCulture, value);
        }

        public override ModelElementType GetModelElementType()
        {
            return ModelElementType.Language;
        }

        protected override void BuildModelInternal(LanguageElement language)
        {
            IsPrimary = language.IsPrimary;

            CultureInfo cultureInfo = CultureCache.Instance.GetCulture(Name);
            FormattedName = cultureInfo == null ? Name : cultureInfo.Format();
            EnglishName = cultureInfo?.EnglishName ?? Name;
            NativeName = cultureInfo?.NativeName ?? Name;
            IsNeutralCulture = cultureInfo != null && cultureInfo.IsNeutralCulture;
        }

        internal void UpdateFrom(CultureInfo cultureInfo)
        {
            Name = cultureInfo.Name;
            FormattedName = cultureInfo.Format();
            EnglishName = cultureInfo.EnglishName;
            NativeName = cultureInfo.NativeName;
            IsNeutralCulture = cultureInfo.IsNeutralCulture;
        }

        internal override void InternalSaveToModelElement(LanguageElement language)
        {
            language.IsPrimary = IsPrimary;
            language.Name = Name;
        }

        internal override bool InternalSetPropertyValue(string propertyName, object value)
        {
            return false;
        }

        internal void InvalidateTranslationCountInfo(bool invalidate)
        {
            _pendingInvalidateTranslationCountInfo = invalidate;
        }

        private void ConditionallyRefreshTranslationCountInfo()
        {
            if (_pendingInvalidateTranslationCountInfo)
            {
                _pendingInvalidateTranslationCountInfo = false;
                GetRootModel().RefreshTranslationCountInfoForAllLanguages();
            }
        }

        internal void UpdateIsPrimary(bool isPrimary)
        {
            IsPrimary = isPrimary;
        }

        internal CultureInfo GetCultureInfo()
        {
            return CultureCache.Instance.GetCulture(Name);
        }
    }
}
