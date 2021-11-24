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
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;

namespace LHQ.App.ViewModels.Dialogs
{
    public class EditLanguageDialogViewModel : DialogViewModelBase
    {
        private readonly bool _isAddMode;
        private LanguageSelectorViewModel _languageSelector;
        private bool _isPrimary;

        public EditLanguageDialogViewModel(IAppContext appContext, CultureInfoItem selectedCulture,
            bool isPrimary, List<string> languagesToExclude, bool countrySpecific): base(appContext)
        {
            _isAddMode = selectedCulture == null;
            IsPrimary = isPrimary;
            CanChangePrimaryFlag = !isPrimary;

            LanguageSelector = new LanguageSelectorViewModel(appContext, languagesToExclude, countrySpecific, true);
            LanguageSelector.Select(selectedCulture);
        }

        public string DialogCaption => GetDialogCaption();

        public LanguageSelectorViewModel LanguageSelector
        {
            get => _languageSelector;
            set => SetProperty(ref _languageSelector, value);
        }

        public bool IsPrimary
        {
            get => _isPrimary;
            set => SetProperty(ref _isPrimary, value);
        }

        public bool CanChangePrimaryFlag { get; }

        protected override string GetTitle()
        {
            return _isAddMode
                ? Strings.Dialogs.EditLanguage.DialogTitleAdd
                : Strings.Dialogs.EditLanguage.DialogTitleEdit;
        }

        private string GetDialogCaption()
        {
            return _isAddMode
                ? Strings.Dialogs.EditLanguage.DialogCaptionAdd
                : Strings.Dialogs.EditLanguage.DialogCaptionEdit;
        }

        protected override bool CanSubmitDialog()
        {
            return LanguageSelector.SelectedLanguage != null;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            LanguageSelector.Dispose();
        }
    }
}
