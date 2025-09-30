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

using System.ComponentModel;
using LHQ.App.Extensions;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Elements;
using LHQ.Utils;

namespace LHQ.App.ViewModels.Dialogs.AppSettings
{
    public class PageGeneral : AppSettingsPageBase
    {
        private readonly IUIService _uiService;

        private KeyValue<string, AppVisualTheme> _themeAutomatic;
        private KeyValue<string, AppVisualTheme> _themeLight;
        private KeyValue<string, AppVisualTheme> _themeDark;

        private bool _openLastProjectOnStartup;
        private bool? _showHints;
        private LanguageSelectorViewModel _projectLanguageSelector;
        private UILanguage _selectedLocalization;
        private bool _checkUpdatesOnAppStart;
        private bool _hasSelectedDefaultLanguage;
        private bool? _lockTranslationsWithResource;
        private int _selectedThemeIndex;
        private bool _showHintsIsThreeState;
        private bool _runTemplateAfterSave;

        public PageGeneral(IAppContext appContext, IAppSettingsDialogViewModel ownerViewModel, AppSettingsDialogPage page)
            : base(appContext, ownerViewModel, page)
        {
            RunInVsPackage = AppContext.RunInVsPackage;

            ProjectLanguageSelector = new LanguageSelectorViewModel(appContext, false, true);
            ProjectLanguageSelector.PropertyChanged += HandleProjectLanguageSelectorChange;

            _uiService = appContext.UIService;
            Localizations = _uiService.UiLanguages.ToObservableCollectionExt();

            ColorThemes = new ObservableCollectionExt<KeyValue<string, AppVisualTheme>>();
            if (RunInVsPackage)
            {
                ColorThemes.Add(ThemeAutomatic);
            }
            ColorThemes.Add(ThemeLight);
            ColorThemes.Add(ThemeDark);
        }

        public override string Name => Strings.ViewModels.AppSettings.PageGeneral.PageTitle;

        private KeyValue<string, AppVisualTheme> ThemeAutomatic => 
            _themeAutomatic ?? (_themeAutomatic = KeyValue.Create(Strings.Common.Themes.Automatic, AppVisualTheme.Automatic));

        private KeyValue<string, AppVisualTheme> ThemeLight => 
            _themeLight ?? (_themeLight = KeyValue.Create(Strings.Common.Themes.Light, AppVisualTheme.Light));

        private KeyValue<string, AppVisualTheme> ThemeDark => 
            _themeDark ?? (_themeDark = KeyValue.Create(Strings.Common.Themes.Dark, AppVisualTheme.Dark));

        public int SelectedThemeIndex
        {
            get => _selectedThemeIndex;
            set => SetProperty(ref _selectedThemeIndex, value);
        }

        public ObservableCollectionExt<KeyValue<string, AppVisualTheme>> ColorThemes { get; }

        public bool OpenLastProjectOnStartup
        {
            get => _openLastProjectOnStartup;
            set => SetProperty(ref _openLastProjectOnStartup, value);
        }

        public bool HasSelectedDefaultLanguage
        {
            get => _hasSelectedDefaultLanguage;
            set => SetProperty(ref _hasSelectedDefaultLanguage, value);
        }

        public bool? ShowHints
        {
            get => _showHints;
            set => SetProperty(ref _showHints, value);
        }

        public bool ShowHintsIsThreeState
        {
            get => _showHintsIsThreeState;
            set => SetProperty(ref _showHintsIsThreeState, value);
        }

        public bool RunInVsPackage { get; }

        public bool? LockTranslationsWithResource
        {
            get => _lockTranslationsWithResource;
            set => SetProperty(ref _lockTranslationsWithResource, value);
        }

        public bool CheckUpdatesOnAppStart
        {
            get => _checkUpdatesOnAppStart;
            set => SetProperty(ref _checkUpdatesOnAppStart, value);
        }

        public bool RunTemplateAfterSave
        {
            get => _runTemplateAfterSave;
            set => SetProperty(ref _runTemplateAfterSave, value);
        }

        public LanguageSelectorViewModel ProjectLanguageSelector
        {
            get => _projectLanguageSelector;
            set => SetProperty(ref _projectLanguageSelector, value);
        }

        public ObservableCollectionExt<UILanguage> Localizations { get; }

        public UILanguage SelectedLocalization
        {
            get => _selectedLocalization;
            set => SetProperty(ref _selectedLocalization, value);
        }

        protected internal override void BindControls(AppConfig appConfig)
        {
            base.BindControls(appConfig);

            ShowHintsIsThreeState = false;
            switch (appConfig.AppHints)
            {
                case AppHintType.None:
                {
                    ShowHints = false;
                    break;
                }
                case AppHintType.All:
                {
                    ShowHints = true;
                    break;
                }
                default:
                {
                    ShowHints = null;
                    ShowHintsIsThreeState = true;
                    break;
                }
            }

            SelectedLocalization = _uiService.GetUiLanguage(appConfig.UILanguage);
            OpenLastProjectOnStartup = appConfig.OpenLastProjectOnStartup;
            CheckUpdatesOnAppStart = appConfig.CheckUpdatesOnAppStart;
            RunTemplateAfterSave = appConfig.RunTemplateAfterSave;
            LockTranslationsWithResource = appConfig.LockTranslationsWithResource;
            ProjectLanguageSelector.Select(appConfig.GetDefaultProjectCulture() ?? CultureInfoItem.English);

            KeyValue<string, AppVisualTheme> themeItem = ThemeLight;
            if (appConfig.Theme == AppVisualTheme.Light)
            {
                themeItem = ThemeLight;
            }
            else if (appConfig.Theme == AppVisualTheme.Dark)
            {
                themeItem = ThemeDark;
            }

            if (RunInVsPackage && appConfig.DetectTheme.GetValueOrDefault(true))
            {
                themeItem = ThemeAutomatic;
            }

            SelectedThemeIndex = ColorThemes.IndexOf(themeItem);
        }

        protected internal override void RaisePropertyChanged(string propertyName = "")
        {
            base.RaisePropertyChanged(propertyName);
            if (propertyName == nameof(ShowHints))
            {
                if (ShowHintsIsThreeState)
                {
                    ShowHintsIsThreeState = false;
                }
            }
        }

        private void HandleProjectLanguageSelectorChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LanguageSelectorViewModel.SelectedLanguage))
            {
                HasSelectedDefaultLanguage = ProjectLanguageSelector.SelectedLanguage != null;
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            ProjectLanguageSelector.Dispose();
        }

        protected internal override bool SubmitDialog()
        {
            if (!HasSelectedDefaultLanguage)
            {
                DialogService.ShowError(new DialogShowInfo(Strings.ViewModels.AppSettings.PageGeneral.DefaultLanguageValidationCaption,
                    Strings.ViewModels.AppSettings.PageGeneral.DefaultLanguageValidationMessage));

                return false;
            }

            return true;
        }

        protected internal override void SaveToConfig(bool finalSave)
        {
            CultureInfoItem defaultProjectLanguage = ProjectLanguageSelector.SelectedLanguage;
            AppConfig.DefaultProjectLanguage = defaultProjectLanguage?.Culture.Name ?? CultureInfoItem.English.CultureName;

            bool? uiShowHints = ShowHints;
            if (uiShowHints.HasValue)
            {
                AppConfig.AppHints = uiShowHints.Value ? AppHintType.All : AppHintType.None;
            }

            AppConfig.OpenLastProjectOnStartup = OpenLastProjectOnStartup;
            AppConfig.UILanguage = SelectedLocalization.Culture.Name;
            AppConfig.CheckUpdatesOnAppStart = CheckUpdatesOnAppStart;
            AppConfig.LockTranslationsWithResource = LockTranslationsWithResource;
            AppConfig.RunTemplateAfterSave = RunTemplateAfterSave;

            var appVisualTheme = AppVisualTheme.Light;
            var detectTheme = false;

            KeyValue<string, AppVisualTheme> selectedTheme = GetSelectedTheme();
            if (selectedTheme == ThemeAutomatic && RunInVsPackage)
            {
                appVisualTheme = AppVisualTheme.Light;
                detectTheme = true;
            }
            else if (selectedTheme == ThemeDark)
            {
                appVisualTheme = AppVisualTheme.Dark;
            }
            else if (selectedTheme == ThemeLight)
            {
                appVisualTheme = AppVisualTheme.Light;
            }

            AppConfig.Theme = appVisualTheme;
            AppConfig.DetectTheme = detectTheme;
        }

        private KeyValue<string, AppVisualTheme> GetSelectedTheme()
        {
            return ColorThemes[SelectedThemeIndex];
        }
    }
}
