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
using System.Linq;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Dialogs;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Dialogs;
using LHQ.Data;
using LHQ.Data.Templating;
using LHQ.Data.Templating.Templates;
using LHQ.Utils;
using LHQ.Utils.Extensions;

namespace LHQ.App.ViewModels
{
    public class ProjectSettingsViewModel : ShellViewModelBase
    {
        private bool _resourcesUnderRoot;
        private bool _categories;
        private bool _resources;
        private string _layoutImage;
        private int _selectedModelVersion;
        private bool _modelVersionVisible;
        private bool _generatorTemplatesVisible;
        private int _generatorTemplateIndex;
        private string _generatorTemplateName;
        private bool _hasSelectedCodeGenerator;

        private bool _isUpgradeRequested;

        private bool _upgradeLabelVisible;
        private bool _canChangeGeneratorTemplate;
        private string _generatorTemplateId;
        private CodeGeneratorTemplate _template;

        public ProjectSettingsViewModel(IShellViewContext shellViewContext)
            : base(shellViewContext, false)
        {
            ModelOptions modelOptions = shellViewContext.ShellViewModel.ModelOptions;
            Categories = modelOptions.Categories;
            Resources = !modelOptions.Categories;
            ResourcesUnderRoot = modelOptions.Resources == ModelOptionsResources.All;

            var selectedModelVersion = shellViewContext.ShellViewModel.ModelContext.Model.Version;
            ModelVersion = $"v{selectedModelVersion}";
            if (selectedModelVersion == ModelConstants.CurrentModelVersion)
            {
                ModelVersion += " (Latest)";
            }

            ModelVersionVisible = true;

            ChangeCodeGeneratorSettingsCommand = new DelegateCommand(ChangeCodeGeneratorSettingsExecute);

            var allTemplates = CodeGeneratorTemplateManager.Instance.GetAllTemplates();
            GeneratorTemplates = new ObservableCollectionExt<KeyValue<string, string>>(allTemplates.Select(x => KeyValue.Create(x.Key, x.Value)));
            GeneratorTemplateIndex = -1;
            CanChangeGeneratorTemplate = false;
            GeneratorTemplateId = string.Empty;
            GeneratorTemplateName = string.Empty;
        }

        private ITreeViewService TreeViewService => ShellViewContext.TreeViewService;

        public string LayoutImage
        {
            get => _layoutImage;
            set => SetProperty(ref _layoutImage, value);
        }

        public string ModelVersion { get; }

        public ObservableCollectionExt<KeyValue<string, string>> GeneratorTemplates { get; private set; }

        public string GeneratorTemplateId
        {
            get => _generatorTemplateId;
            set => SetProperty(ref _generatorTemplateId, value);
        }

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

        public bool CanChangeGeneratorTemplate
        {
            get => _canChangeGeneratorTemplate;
            set => SetProperty(ref _canChangeGeneratorTemplate, value);
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
                    //GeneratorTemplateName = template.Value;
                }
                else
                {
                    Template = null;
                    //GeneratorTemplateName = string.Empty;
                }
            }
        }

        public ICommand ChangeCodeGeneratorSettingsCommand { get; }

        public CodeGeneratorTemplate Template
        {
            get => _template;
            private set
            {
                SetProperty(ref _template, value);
                GeneratorTemplateName = _template == null ? string.Empty : _template.Name;
                GeneratorTemplateId = _template == null ? string.Empty : _template.Id;
            }
        }

        public bool ModelVersionVisible
        {
            get => _modelVersionVisible;
            set => SetProperty(ref _modelVersionVisible, value);
        }

        public bool Resources
        {
            get => _resources;
            set
            {
                _resources = value;
                if (value != _resources)
                {
                    _resources = value;
                    RaisePropertyChanged();

                    if (value && Categories)
                    {
                        Categories = false;
                    }
                }
            }
        }

        public bool Categories
        {
            get => _categories;
            set
            {
                if (value != _categories)
                {
                    _categories = value;
                    RaisePropertyChanged();

                    if (value && Resources)
                    {
                        Resources = false;
                    }
                }
            }
        }

        public bool ResourcesUnderRoot
        {
            get => _resourcesUnderRoot;
            set => SetProperty(ref _resourcesUnderRoot, value);
        }
        
        public bool Validate()
        {
            var valid = true;
            var dialogCaption = Strings.Operations.ProjectSettings.ValidationErrorCaption;

            if (Categories)
            {
                if (!ResourcesUnderRoot)
                {
                    bool hasAnyResource = TreeViewService.RootModel.Children.Any(x => x.ElementType == TreeElementType.Resource);
                    if (hasAnyResource)
                    {
                        DialogService.ShowError(new DialogShowInfo(dialogCaption, Strings.Operations.ProjectSettings.ValidationError1));
                        valid = false;
                    }
                }
            }
            else
            {
                bool hasAnyCategory = TreeViewService.FindAllElements(x => x.ElementType == TreeElementType.Category).Any();
                if (hasAnyCategory)
                {
                    DialogService.ShowError(new DialogShowInfo(dialogCaption, Strings.Operations.ProjectSettings.ValidationError2));
                    valid = false;
                }
            }

            return valid;
        }

        public void Save(ModelOptions modelOptions)
        {
            if (modelOptions == null)
            {
                throw new ArgumentNullException(nameof(modelOptions));
            }

            modelOptions.Categories = Categories;
            modelOptions.Resources = ResourcesUnderRoot
                ? ModelOptionsResources.All
                : ModelOptionsResources.Categories;

            if (!Categories)
            {
                modelOptions.Resources = ModelOptionsResources.All;
            }
        }

        private void ChangeCodeGeneratorSettingsExecute(object obj)
        {
            string templateId = Template?.Id ?? string.Empty;
            (bool submitted, CodeGeneratorTemplate template) = CodeGeneratorDialog.DialogShow(ShellViewContext, templateId, Template);
            if (submitted)
            {
                Template = template;
            }
        }

        protected internal override void RaisePropertyChanged(string propertyName = "")
        {
            base.RaisePropertyChanged(propertyName);

            if (propertyName == nameof(Categories) || propertyName == nameof(Resources) || propertyName == nameof(ResourcesUnderRoot) ||
                propertyName == string.Empty)
            {
                var theme = (VisualManager.Instance.Theme == AppVisualTheme.Automatic
                    ? AppVisualTheme.Light
                    : VisualManager.Instance.Theme).ToLowerInvariant();

                int imageNo = 3;

                if (Categories)
                {
                    imageNo = ResourcesUnderRoot ? 1 : 2;
                }

                var image = $"/images/layout0{imageNo}-{theme}.png";

                LayoutImage = ResourceHelper.GetImageComponentUri(image);
            }
        }

        public void SelectTemplateById(string templateId)
        {
            GeneratorTemplateIndex = GeneratorTemplates.FindItemIndex(x => x.Key == templateId);
        }

        public void SetTemplate(CodeGeneratorTemplate template)
        {
            GeneratorTemplateIndex = template == null ? -1 : GeneratorTemplates.FindItemIndex(x => x.Key == template.Id);
            if (template != null)
            {
                Template = template;
            }
        }
    }
}
