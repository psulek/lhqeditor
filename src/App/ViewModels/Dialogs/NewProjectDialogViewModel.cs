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

using System.Linq;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Dialogs;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Elements;
using LHQ.Data.Templating;
using LHQ.Data.Templating.Templates;
using LHQ.Utils;
using LHQ.Utils.Extensions;

namespace LHQ.App.ViewModels.Dialogs
{
    public class NewProjectDialogViewModel : DialogViewModelBase
    {
        private readonly IShellViewContext _shellViewContext;
        private bool _generatorTemplatesVisible;
        private int _generatorTemplateIndex;

        /// <summary>
        /// Extra info provided from Visual Studio extension.
        /// </summary>
        public sealed class VsExtraInfo
        {
            public string ProjectName { get; }
            public string TemplateName { get; }
            public string TemplateDescription { get; }
            public string TemplateId { get; }

            public VsExtraInfo(string projectName, string templateName, string templateDescription, string templateId)
            {
                ProjectName = projectName;
                TemplateName = templateName;
                TemplateDescription = templateDescription;
                TemplateId = templateId;
            }
        }

        private string _modelName;
        private LanguageSelectorViewModel _languageSelector;
        private string _error;
        private bool _hasError;
        private bool _openLanguageSettings;
        private string _generatorTemplateName;
        private bool _hasSelectedCodeGenerator;

        public NewProjectDialogViewModel(IShellViewContext shellViewContext) : base(shellViewContext.AppContext)
        {
            _shellViewContext = shellViewContext;
            CultureInfoItem defaultProjectCulture = AppContext.AppConfigFactory.Current.GetDefaultProjectCulture() ?? CultureInfoItem.English;

            LanguageSelector = new LanguageSelectorViewModel(shellViewContext.AppContext, false, false);
            LanguageSelector.Select(defaultProjectCulture);

            ProjectSettings = new ProjectSettingsViewModel(shellViewContext);
            ProjectSettings.ModelVersionVisible = false;
            ShowHelpCommand = new DelegateCommand(ShowHelpExecute);
            ChangeCodeGeneratorSettingsCommand = new DelegateCommand(ChangeCodeGeneratorSettingsExecute);

            // var allTemplates = CodeGeneratorTemplateManager.Instance.GetAllTemplates();
            // GeneratorTemplates = new ObservableCollectionExt<KeyValue<string, string>>(allTemplates.Select(x => KeyValue.Create(x.Key, x.Value)));
            // GeneratorTemplateIndex = -1;
            //
            // if (extraInfo != null)
            // {
            //     GeneratorTemplateIndex = GeneratorTemplates.FindItemIndex(x => x.Key == extraInfo.TemplateId);
            // }
            //
            if (AppContext.RunInVsPackage)
            {
                OpenLanguageSettings = false;
            }
        }

        public bool RunInVsPackage => AppContext.RunInVsPackage;
       
        //public string TemplateId { get; set; }

        public ProjectSettingsViewModel ProjectSettings { get; set; }

        public ICommand ShowHelpCommand { get; }
        
        public ICommand ChangeCodeGeneratorSettingsCommand { get; }

        //public VsExtraInfo ExtraInfo { get; set; }

        public string ModelName
        {
            get => _modelName;
            set => SetProperty(ref _modelName, value);
        }

        public ObservableCollectionExt<KeyValue<string, string>> GeneratorTemplates { get; private set; }

        public string GeneratorTemplateName
        {
            get => _generatorTemplateName;
            set => SetProperty(ref _generatorTemplateName, value);
        }

        public bool HasSelectedCodeGenerator
        {
            get => _hasSelectedCodeGenerator;
            set => SetProperty(ref _hasSelectedCodeGenerator, value);
        }

        public int GeneratorTemplateIndex
        {
            get => _generatorTemplateIndex;
            set
            {
                SetProperty(ref _generatorTemplateIndex, value);
                
                var template = value > -1 && value < GeneratorTemplates.Count ? GeneratorTemplates[value] : null;
                HasSelectedCodeGenerator = template != null;
                if (template != null)
                {
                    Template = CodeGeneratorTemplateManager.Instance.CreateTemplate(template.Key);
                    GeneratorTemplateName = template.Value;
                }
                else
                {
                    Template = null;
                    GeneratorTemplateName = string.Empty;
                }
            }
        }

        public string Error
        {
            get => _error;
            set
            {
                if (SetProperty(ref _error, value))
                {
                    HasError = !_error.IsNullOrEmpty();
                }
            }
        }

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public LanguageSelectorViewModel LanguageSelector
        {
            get => _languageSelector;
            set => SetProperty(ref _languageSelector, value);
        }

        public bool OpenLanguageSettings
        {
            get => _openLanguageSettings;
            set => SetProperty(ref _openLanguageSettings, value);
        }

        public string ProjectNameText => RunInVsPackage ? Strings.ViewModels.NewProject.ProjectNameVS : Strings.ViewModels.NewProject.ProjectName;

        public CodeGeneratorTemplate Template { get; set; }

        private void ChangeCodeGeneratorSettingsExecute(object obj)
        {
            // if (!RunInVsPackage)
            // {
            //     return;
            // }

            string templateId = Template?.Id ?? string.Empty;
            (bool submitted, CodeGeneratorTemplate template) = CodeGeneratorDialog.DialogShow(_shellViewContext, templateId, Template);
            if (submitted)
            {
                Template = template;
            }
        }

        private void ShowHelpExecute(object obj)
        {
            WebPageUtils.ShowDoc(WebPageDocSection.CreateNewProject);
        }

        protected override string GetTitle()
        {
            return RunInVsPackage ? Strings.ViewModels.NewProject.DialogTitleVS : Strings.ViewModels.NewProject.DialogTitle;
        }

        protected override bool CanSubmitDialog()
        {
            return !HasError;
        }

        public override bool SubmitDialog()
        {
            if (LanguageSelector.SelectedLanguage == null)
            {
                DialogService.ShowError(new DialogShowInfo(GetTitle(), 
                    Strings.ViewModels.NewProject.PrimaryLanguageValidationMessage));

                return false;
            }

            return true;
        }

        protected override string ValidateProperty(string propertyName)
        {
            string error = null;
            if (propertyName == nameof(ModelName) && ModelName.IsNullOrEmpty())
            {
                error = Strings.ViewModels.NewProject.ProjectNameIsRequired;
            }

            Error = error;
            return Error;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            LanguageSelector.Dispose();
        }

        public void SetExtraInfo(VsExtraInfo extraInfo)
        {
            var allTemplates = CodeGeneratorTemplateManager.Instance.GetAllTemplates();
            GeneratorTemplates = new ObservableCollectionExt<KeyValue<string, string>>(allTemplates.Select(x => KeyValue.Create(x.Key, x.Value)));
            GeneratorTemplateIndex = -1;

            if (extraInfo != null)
            {
                GeneratorTemplateIndex = GeneratorTemplates.FindItemIndex(x => x.Key == extraInfo.TemplateId);
            }
        }
    }
}
