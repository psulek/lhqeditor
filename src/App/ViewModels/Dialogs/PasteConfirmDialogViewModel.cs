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

using System.Collections.Generic;
using System.Linq;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Elements;
using LHQ.Utils.Utilities;

namespace LHQ.App.ViewModels.Dialogs
{
    public class PasteConfirmDialogViewModel : DialogViewModelBase
    {
        public PasteConfirmDialogViewModel(IAppContext appContext, List<string> missingLanguageNames) : base(appContext)
        {
            ArgumentValidator.EnsureArgumentNotNull(missingLanguageNames, "missingLanguageNames");

            LanguagesToAdd = missingLanguageNames.Distinct().Select(x => new LanguageItem(appContext, x)).ToList();

            ConfirmCaption = Strings.Dialogs.PasteConfirm.Caption;
            MissingLanguagesText = Strings.Dialogs.PasteConfirm.AddMissingLanguages;
        }

        public string ConfirmCaption { get; }

        public string MissingLanguagesText { get; }

        public List<LanguageItem> LanguagesToAdd { get; }

        protected override string GetTitle()
        {
            return Strings.Dialogs.PasteConfirm.DialogTitle;
        }

        protected override bool CanSubmitDialog()
        {
            return LanguagesToAdd.Any(x => x.Selected);
        }

        internal List<string> GetSelectedLanguages()
        {
            return LanguagesToAdd.Where(x => x.Selected).Select(x => x.LanguageName).ToList();
        }

        public sealed class LanguageItem : ObservableModelObject
        {
            private string _displayName;
            private string _languageName;
            private bool _selected;

            public LanguageItem(IAppContext appContext, string languageName) : base(appContext)
            {
                LanguageName = languageName;
                CultureInfoItem cultureInfoItem = CultureCache.Instance.GetCulture(languageName).ToCultureInfoItem();
                DisplayName = cultureInfoItem.CultureDisplayName;
                Selected = true;
            }

            public string DisplayName
            {
                get => _displayName;
                set => SetProperty(ref _displayName, value);
            }

            public string LanguageName
            {
                get => _languageName;
                set => SetProperty(ref _languageName, value);
            }

            public bool Selected
            {
                get => _selected;
                set => SetProperty(ref _selected, value);
            }
        }
    }
}
