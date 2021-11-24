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
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Dialogs;
using LHQ.App.ViewModels.Elements;

namespace LHQ.App.Dialogs
{
    /// <summary>
    ///     Interaction logic for AddLanguageDialog.xaml
    /// </summary>
    public partial class EditLanguageDialog
    {
        public EditLanguageDialog()
        {
            InitializeComponent();
        }

        public static bool DialogShow(IAppContext appContext, LanguageViewModel language, List<string> languagesToExclude,
            bool countrySpecific, out CultureInfoItem selectedLanguage, out bool? changedIsPrimary)
        {
            selectedLanguage = null;
            changedIsPrimary = null;
            var isPrimary = false;
            CultureInfoItem culture = null;

            if (language != null)
            {
                culture = language.GetCultureInfo().ToCultureInfoItem();
                isPrimary = language.IsPrimary;
            }

            using (var viewModel = new EditLanguageDialogViewModel(appContext, culture, isPrimary, languagesToExclude, countrySpecific))
            {
                bool? dialogResult = DialogShow<EditLanguageDialogViewModel, EditLanguageDialog>(viewModel);
                bool result = dialogResult == true;
                if (result)
                {
                    selectedLanguage = viewModel.LanguageSelector.SelectedLanguage;
                    if (!isPrimary)
                    {
                        changedIsPrimary = viewModel.IsPrimary;
                    }
                }

                return result;
            }
        }
    }
}
