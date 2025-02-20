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
using System.Linq;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Data;
using LHQ.Data.Templating;
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
        // private bool _generatorTemplatesVisible;
        // private int _generatorTemplateIndex;

        public ProjectSettingsViewModel(IShellViewContext shellViewContext)
            : base(shellViewContext, false)
        {
            ModelOptions modelOptions = shellViewContext.ShellViewModel.ModelOptions;
            Categories = modelOptions.Categories;
            Resources = !modelOptions.Categories;
            ResourcesUnderRoot = modelOptions.Resources == ModelOptionsResources.All;

            var modelFileStorage = AppContext.ServiceContainer.Get<IModelFileStorage>();
            var modelVersions = modelFileStorage.GetSupportedModelVersions();
            ModelVersions = new ObservableCollectionExt<int>(modelVersions);

            SelectedModelVersion = shellViewContext.ShellViewModel.ModelContext.Model.Version;
            ModelVersionVisible = true;
            // GeneratorTemplatesVisible = false;
            
            // var allTemplates = CodeGeneratorTemplateManager.Instance.GetAllTemplates();
            // GeneratorTemplates = new ObservableCollectionExt<KeyValue<string, string>>(allTemplates.Select(x => KeyValue.Create(x.Key, x.Value)));
            // GeneratorTemplateIndex = -1;
        }

        private ITreeViewService TreeViewService => ShellViewContext.TreeViewService;

        public string LayoutImage
        {
            get => _layoutImage;
            set => SetProperty(ref _layoutImage, value);
        }

        public int SelectedModelVersion
        {
            get => _selectedModelVersion;
            set => SetProperty(ref _selectedModelVersion, value);
        }

        public ObservableCollectionExt<int> ModelVersions { get; }

        // public ObservableCollectionExt<KeyValue<string, string>> GeneratorTemplates { get; }
        //
        // public int GeneratorTemplateIndex
        // {
        //     get => _generatorTemplateIndex;
        //     set => SetProperty(ref _generatorTemplateIndex, value);
        // }
        //
        // public bool GeneratorTemplatesVisible
        // {
        //     get => _generatorTemplatesVisible;
        //     set => SetProperty(ref _generatorTemplatesVisible, value);
        // }

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
                        DialogService.ShowError(dialogCaption, Strings.Operations.ProjectSettings.ValidationError1, null);
                        valid = false;
                    }
                }
            }
            else
            {
                bool hasAnyCategory = TreeViewService.FindAllElements(x => x.ElementType == TreeElementType.Category).Any();
                if (hasAnyCategory)
                {
                    DialogService.ShowError(dialogCaption, Strings.Operations.ProjectSettings.ValidationError2, null);
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
    }
}
