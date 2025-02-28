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
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Dialogs;
using LHQ.App.Extensions;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Elements;
using LHQ.Utils;
using LHQ.Utils.Extensions;

namespace LHQ.App.ViewModels.Dialogs
{
    public class LanguageSettingsDialogViewModel : DialogViewModelBase
    {
        private readonly Dictionary<string, string> _replacedLanguages;
        private bool _hasPrimaryLanguage;
        private ICollectionView _languagesView;
        private LanguageViewModel _selectedLanguage;
        private bool _selectedLanguageIsPrimary;
        private string _removeLanguageTooltip;
        private readonly IShellViewContext _shellViewContext;

        public LanguageSettingsDialogViewModel(IShellViewContext shellViewContext) : base(shellViewContext.AppContext)
        {
            _shellViewContext = shellViewContext;
            RootModelViewModel rootModel = shellViewContext.ShellViewModel.RootModel;

            // make copy (because dialog can be cancelled)
            List<LanguageViewModel> languages =
                rootModel.Languages.Select(x =>
                        {
                            var lang = new LanguageViewModel(shellViewContext, x.GetCultureInfo(), x.IsPrimary)
                            {
                                TranslatedResourcesCount = x.TranslatedResourcesCount
                            };
                            return lang;
                        })
                    .ToList();

            Languages = languages.ToObservableCollectionExt();

            _replacedLanguages = languages.ToDictionary(x => x.Name, x => string.Empty);
            UpdateHasPrimaryLanguage();

            var languagesView = (ListCollectionView)CollectionViewSource.GetDefaultView(Languages);
            languagesView.SortDescriptions.Add(new SortDescription(nameof(LanguageViewModel.IsPrimary), ListSortDirection.Descending));
            languagesView.SortDescriptions.Add(new SortDescription(nameof(LanguageViewModel.Name), ListSortDirection.Ascending));
            LanguagesView = languagesView;

            AddLanguageCommand = new DelegateCommand(AddLanguageExecute);
            EditLanguageCommand = new DelegateCommand(EditLanguageExecute, EditLanguageCanExecute);
            RemoveLanguageCommand = new DelegateCommand(RemoveLanguageExecute, RemoveLanguageCanExecute);
            PreviewMouseDoubleClickCommand = new DelegateCommand<MouseButtonEventArgs>(OnPreviewMouseDoubleClick);
            ShowHelpCommand = new DelegateCommand(ShowHelpExecute);
        }

        public ICommand ShowHelpCommand { get; }

        public ObservableCollectionExt<LanguageViewModel> Languages { get; }

        public ICollectionView LanguagesView
        {
            get => _languagesView;
            set => SetProperty(ref _languagesView, value);
        }

        public LanguageViewModel SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    RaisePropertyChanged();
                    SelectedLanguageIsPrimary = value != null && value.IsPrimary;
                }
            }
        }

        public bool SelectedLanguageIsPrimary
        {
            get => _selectedLanguageIsPrimary;
            set
            {
                SetProperty(ref _selectedLanguageIsPrimary, value);
                RemoveLanguageTooltip = value
                    ? Strings.ViewModels.LanguageSettings.PrimaryLanguageCantBeRemoved
                    : Strings.ViewModels.LanguageSettings.ButtonRemoveTooltip;
            }
        }

        public ICommand AddLanguageCommand { get; }

        public ICommand EditLanguageCommand { get; }

        public ICommand RemoveLanguageCommand { get; }

        public DelegateCommand<MouseButtonEventArgs> PreviewMouseDoubleClickCommand { get; }

        public string RemoveLanguageTooltip
        {
            get => _removeLanguageTooltip;
            set => SetProperty(ref _removeLanguageTooltip, value);
        }

        private void UpdateHasPrimaryLanguage()
        {
            _hasPrimaryLanguage = Languages.Any(x => x.IsPrimary);
        }

        private void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (EditLanguageCommand.CanExecute(null))
            {
                EditLanguageCommand.Execute(null);
            }
        }

        private void AddLanguageExecute(object obj)
        {
            LanguageViewModel primaryLanguage = Languages.Single(x => x.IsPrimary);
            bool isNeutralCulture = primaryLanguage.IsNeutralCulture;
            bool countrySpecific = !isNeutralCulture;
            List<string> languagesToExclude = GetCurrentLanguages();

            if (EditLanguageDialog.DialogShow(AppContext, null, languagesToExclude, countrySpecific, out CultureInfoItem language,
                out _))
            {
                Languages.Add(new LanguageViewModel(_shellViewContext, language.Culture, false));

                UpdateHasPrimaryLanguage();
            }
        }

        private bool EditLanguageCanExecute(object arg)
        {
            return SelectedLanguage != null;
        }

        private List<string> GetCurrentLanguages()
        {
            return Languages.Select(x => x.Name).ToList();
        }

        private void EditLanguageExecute(object obj)
        {
            string selectedLanguageName = _selectedLanguage.Name;

            bool countrySpecific = !_selectedLanguage.IsNeutralCulture;
            if (EditLanguageDialog.DialogShow(AppContext, _selectedLanguage, null,
                countrySpecific, out CultureInfoItem language, out bool? changedIsPrimary))
            {
                void FinalizeEditLanguage()
                {
                    _selectedLanguage.UpdateFrom(language.Culture);

                    // if is primary flag was changed
                    if (changedIsPrimary.HasValue && _selectedLanguage.IsPrimary != changedIsPrimary.Value &&
                        changedIsPrimary.Value)
                    {
                        LanguageViewModel currentPrimary = Languages.Single(x => x.IsPrimary);

                        var dialogShowInfo = new DialogShowInfo(Strings.ViewModels.LanguageSettings.ChangePrimaryLanguageConfirmCaption, 
                            Strings.ViewModels.LanguageSettings.ChangePrimaryLanguageConfirmMessage(currentPrimary.EnglishName,
                                _selectedLanguage.EnglishName), 
                            Strings.ViewModels.LanguageSettings.ChangePrimaryLanguageConfirmDetail(_selectedLanguage.EnglishName));
                        
                        if (DialogService.ShowConfirm(dialogShowInfo).DialogResult == Model.DialogResult.Yes)
                        {
                            Languages.Run(x => x.UpdateIsPrimary(false));
                            _selectedLanguage.UpdateIsPrimary(true);
                        }
                    }

                    UpdateHasPrimaryLanguage();
                    LanguagesView.Refresh();
                }

                if (!language.CultureName.EqualsTo(selectedLanguageName) &&
                    _replacedLanguages.ContainsKey(selectedLanguageName))
                {
                    string fromLang = _selectedLanguage.EnglishName;
                    string toLang = language.CultureDisplayName;

                    var dialogShowInfo = new DialogShowInfo(Strings.ViewModels.LanguageSettings.ReplaceLanguageCaption,
                        Strings.ViewModels.LanguageSettings.ReplaceLanguageMessage(fromLang, toLang),
                        Strings.ViewModels.LanguageSettings.ReplaceLanguageDetail(fromLang, toLang));
                    
                    if (DialogService.ShowConfirm(dialogShowInfo).DialogResult == Model.DialogResult.Yes)
                    {
                        _replacedLanguages[selectedLanguageName] = language.CultureName;
                        FinalizeEditLanguage();
                    }
                }
                else
                {
                    FinalizeEditLanguage();
                }
            }
        }

        private bool RemoveLanguageCanExecute(object arg)
        {
            return SelectedLanguage != null && !SelectedLanguage.IsPrimary;
        }

        private void RemoveLanguageExecute(object obj)
        {
            LanguageViewModel language = SelectedLanguage;
            string languageName = "{0} ({1})".FormatWith(language.EnglishName, language.Name);

            var dialogShowInfo = new DialogShowInfo(Strings.ViewModels.LanguageSettings.RemoveLanguageConfirmCaption(languageName),
                Strings.ViewModels.LanguageSettings.RemoveLanguageConfirmMessage(languageName),
                Strings.ViewModels.LanguageSettings.RemoveLanguageConfirmDetail);
            
            if (DialogService.ShowConfirm(dialogShowInfo).DialogResult == Model.DialogResult.Yes)
            {
                Languages.Remove(language);
                UpdateHasPrimaryLanguage();
            }
        }

        protected override string GetTitle()
        {
            return Strings.ViewModels.LanguageSettings.PageTitle;
        }

        protected override bool CanSubmitDialog()
        {
            return Languages.Count > 0 && _hasPrimaryLanguage;
        }

        internal Dictionary<string, string> GetReplacedLanguages()
        {
            return _replacedLanguages.Where(x => !x.Value.IsNullOrEmpty()).ToDictionary(x => x.Key, x => x.Value);
        }

        private void ShowHelpExecute(object obj)
        {
            WebPageUtils.ShowDoc(WebPageDocSection.LanguageSettings);
        }
    }
}
