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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Elements;
using LHQ.Utils;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.ViewModels
{
    public sealed class LanguageSelectorViewModel : ViewModelBase
    {
        private readonly List<string> _languagesToExclude;
        private readonly bool _showAllCultures;
        private bool _countrySpecific;
        private CultureInfoItem _selectedLanguage;
        private ObservableCollectionExt<CultureInfoItem> _cultures;
        private bool _searchByCultureCode;

        private string _textSearchPath;

        public LanguageSelectorViewModel(IAppContext appContext, bool countrySpecific, bool isStartupFocused)
            : this(appContext, null, countrySpecific, isStartupFocused)
        { }

        public LanguageSelectorViewModel(IAppContext appContext, List<string> languagesToExclude,
            bool countrySpecific, bool isStartupFocused)
            : base(appContext, true)
        {
            TextSearchPath = "CultureDisplayName";
            SearchCommand = new DelegateCommand(SearchCommandExecute);
            PreviewTextInputCommand = new DelegateCommand<TextCompositionEventArgs>(PreviewTextInputCommandExecute);

            Cultures = new ObservableCollectionExt<CultureInfoItem>();
            _languagesToExclude = languagesToExclude ?? new List<string>();
            _showAllCultures = false;
            _countrySpecific = countrySpecific;

            SearchByCultureCode = false;
            IsStartupFocused = isStartupFocused;
            RefreshCultures(false);
        }

        public ICommand PreviewTextInputCommand { get; }
        public ICommand SearchCommand { get; }

        public bool SearchByCultureCode
        {
            get => _searchByCultureCode;
            set
            {
                if (SetProperty(ref _searchByCultureCode, value))
                {
                    TextSearchPath = _searchByCultureCode ? "CultureName" : "CultureDisplayName";

                    RaisePropertyChanged();
                    RefreshCultures(true);
                }
            }
        }

        public ObservableCollectionExt<CultureInfoItem> Cultures
        {
            get => _cultures;
            set => SetProperty(ref _cultures, value);
        }

        public bool CountrySpecific
        {
            get => _countrySpecific;
            set
            {
                if (SetProperty(ref _countrySpecific, value))
                {
                    RaisePropertyChanged();
                    RefreshCultures(true);
                }
            }
        }

        public string TextSearchPath
        {
            get => _textSearchPath;
            set => SetProperty(ref _textSearchPath, value);
        }

        public CultureInfoItem SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (SetProperty(ref _selectedLanguage, value))
                {
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsStartupFocused { get; }

        private static List<CultureInfoItem> LoadCultures(bool showAllCultures,
            List<string> languagesToExclude, bool countrySpecific, bool searchByCode)
        {
            IEnumerable<CultureInfo> allCultures = CultureCache.Instance.GetAll();

            if (!showAllCultures)
            {
                bool onlyNeutral = !countrySpecific;
                allCultures = allCultures
                    .Where(x =>
                        {
                            return !x.IsGenericInvariantCulture() && languagesToExclude.All(y => y != x.Name) &&
                                (onlyNeutral && x.IsNeutralCulture || !onlyNeutral && !x.IsNeutralCulture);
                        });
            }

            var result = allCultures
                .Select(x => x.ToCultureInfoItem(searchByCode));

            result = searchByCode ? result.OrderBy(x => x.CultureName) : result.OrderBy(x => x.CultureDisplayName);
            return result.ToList();
        }

        private void RefreshCultures(bool backupAndRestore)
        {
            string bkpSelectedCultureName = null;
            string bkpSelectedParentCultureName = null;

            if (backupAndRestore)
            {
                bkpSelectedCultureName = SelectedLanguage?.CultureName;
                var parentLCID = SelectedLanguage?.Culture.Parent.LCID ?? -1;
                bkpSelectedParentCultureName = SelectedLanguage?.CultureParentName;
                if (parentLCID == 127) // 127 -> invariant culture 
                {
                    bkpSelectedParentCultureName = SelectedLanguage?.Culture.TextInfo.CultureName;
                }
            }

            Cultures = new ObservableCollectionExt<CultureInfoItem>(LoadCultures(_showAllCultures,
                _languagesToExclude, _countrySpecific, SearchByCultureCode));

            if (backupAndRestore)
            {
                CultureInfoItem foundCulture = Cultures.SingleOrDefault(x => x.Culture.Name == bkpSelectedCultureName);
                if (foundCulture == null)
                {
                    foundCulture = Cultures.SingleOrDefault(x => x.Culture.Name == bkpSelectedParentCultureName);
                }

                if (foundCulture == null)
                {
                    foundCulture = _countrySpecific ? CultureInfoItem.EnglishUS : CultureInfoItem.English;
                }

                Select(foundCulture);
            }
        }

        public void Select(CultureInfoItem cultureInfoItem)
        {
            if (cultureInfoItem != null)
            {
                CultureInfoItem item = Cultures.SingleOrDefault(x => x.CultureName == cultureInfoItem.CultureName);
                if (item == null)
                {
                    CultureInfo cultureInfo = CultureCache.Instance.GetCulture(cultureInfoItem.CultureName);
                    if (cultureInfo?.IsNeutralCulture == false)
                    {
                        _countrySpecific = true;

                        Cultures = new ObservableCollectionExt<CultureInfoItem>(LoadCultures(_showAllCultures,
                            _languagesToExclude, _countrySpecific, SearchByCultureCode));

                        item = Cultures.SingleOrDefault(x => x.CultureName == cultureInfoItem.CultureName);

                        RaisePropertyChanged(nameof(CountrySpecific));
                    }
                }

                if (item != null)
                {
                    SelectedLanguage = item;
                }
            }
        }

        private void SearchCommandExecute(object obj)
        {
            RaisePropertyChanged();
            RefreshCultures(true);
        }

        private void PreviewTextInputCommandExecute(TextCompositionEventArgs e)
        {
            if (e.Source is ComboBox comboBox)
            {
                var viewModel = (LanguageSelectorViewModel)comboBox.DataContext;
                var searchEdit = (TextBox)comboBox.Template.FindName("PART_EditableTextBox", comboBox);
                string searchText = string.Empty;
                int selectionStart = searchEdit.SelectionStart;
                if (selectionStart > 0)
                {
                    searchText = searchEdit.Text.Substring(0, selectionStart) + e.Text;
                }

                bool notContainsText = viewModel.SearchByCultureCode
                    ? !viewModel.Cultures.Any(
                        x => x.CultureName.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) > -1)
                    : !viewModel.Cultures.Any(
                        x => x.CultureDisplayName.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) > -1);

                if (notContainsText)
                {
                    e.Handled = true;
                }
            }
        }
    }

    public class ComboBoxItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NameTemplate { get; set; }
        public DataTemplate CodeTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement fe && fe.TemplatedParent is ComboBoxItem comboBoxItem)
            {
                var parent = ItemsControl.ItemsControlFromItemContainer(comboBoxItem);
                if (parent is ComboBox comboBox && comboBox.DataContext is LanguageSelectorViewModel dataContext)
                {
                    return dataContext.SearchByCultureCode ? CodeTemplate : NameTemplate;
                }
            }

            return NameTemplate;
        }
    }
}