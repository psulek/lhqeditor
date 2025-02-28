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
using LHQ.Data;
using LHQ.Data.Interfaces;
using LHQ.Data.Interfaces.Key;
using LHQ.Utils.Extensions;

namespace LHQ.App.ViewModels.Elements
{
    public abstract class CategoryLikeViewModel<TModelElement> : TreeElementViewModel<TModelElement>, ICategoryLikeViewModel
        where TModelElement : ModelElementBase, new()
    {
        private string _description;

        protected CategoryLikeViewModel(IShellViewContext shellViewContext, TModelElement modelElement,
            ITreeElementViewModel parentViewModel)
            : base(shellViewContext, modelElement, parentViewModel)
        { }

        public string Description
        {
            get => _description;
            set => SetPropertyWithUndo(ref _description, value, Strings.Common.Properties.Description);
        }

        public virtual bool EditAllowed => ShellViewModel.IsEditorMode;

        protected override bool GetIsExpandable()
        {
            return true;
        }

        public CategoryViewModel AddChildCategory(string categoryName)
        {
            return AddChildCategory(categoryName, false);
        }

        private CategoryViewModel AddChildCategory(string categoryName, bool disconnected)
        {
            IModelElementKey parentKey = ParseModelElementKey(ParentElementKey);
            IModelElementKey categoryKey = GenerateElementKey();

            var newCategory = new CategoryElement(categoryKey)
            {
                Name = categoryName,
                ParentKey = parentKey
            };

            var newCategoryViewModel = new CategoryViewModel(ShellViewContext, this, newCategory);

            if (!disconnected)
            {
                newCategoryViewModel =
                    (CategoryViewModel)(this as ITreeElementViewModel).AddChildSorted(newCategoryViewModel);
            }

            return newCategoryViewModel;
        }

        public ResourceViewModel AddChildResource(string resourceName)
        {
            IModelElementKey parentKey = ParseModelElementKey(ParentElementKey);
            IModelElementKey resourceKey = GenerateElementKey();

            var newResource = new ResourceElement(resourceKey)
            {
                Name = resourceName,
                ParentKey = parentKey
            };

            LanguageViewModel primaryLanguage = GetRootModel().PrimaryLanguage;

            newResource.Values.Add(new ResourceValueElement(GenerateElementKey(), newResource)
            {
                Value = resourceName.FromPascalCasingToWords(),
                LanguageName = primaryLanguage.Name,
                Locked = false,
                Auto = false
            });

            var newResourceViewModel = (this as ITreeElementViewModel).AddChildSorted(new ResourceViewModel(ShellViewContext, this, newResource)) as ResourceViewModel;
            if (newResourceViewModel != null)
            {
                newResourceViewModel.Values.First().IsAutoFocus = true;
            }

            return newResourceViewModel;
        }

        public IEnumerable<CategoryViewModel> GetCategories()
        {
            return Children.Where(x => x.ElementType == TreeElementType.Category).Cast<CategoryViewModel>();
        }

        public IEnumerable<ResourceViewModel> GetResources()
        {
            return Children.Where(x => x.ElementType == TreeElementType.Resource).Cast<ResourceViewModel>();
        }

        protected abstract IEnumerable<ResourceViewModel> PopulateChildResources(TModelElement modelElement);

        protected abstract IEnumerable<CategoryViewModel> PopulateChildCategories(TModelElement modelElement);

        protected override IEnumerable<ITreeElementViewModel> PopulateChilds(TModelElement modelElement)
        {
            foreach (CategoryViewModel category in PopulateChildCategories(modelElement))
            {
                yield return category;
            }

            foreach (ResourceViewModel resource in PopulateChildResources(modelElement))
            {
                yield return resource;
            }
        }

        protected override void BuildModelInternal(TModelElement modelElement)
        {
            if (modelElement is IModelDescriptionElement modelDescriptionElement)
            {
                Description = modelDescriptionElement.Description;
            }

            base.BuildModelInternal(modelElement);
        }

        internal override void InternalSaveToModelElement(TModelElement modelElement)
        {
            if (modelElement is IModelDescriptionElement modelDescriptionElement)
            {
                modelDescriptionElement.Description = Description;
            }
        }

        internal override bool InternalSetPropertyValue(string propertyName, object value)
        {
            bool hasChanges = base.InternalSetPropertyValue(propertyName, value);

            if (propertyName == nameof(Description))
            {
                hasChanges = true;
                _description = value as string;
                RaisePropertyChanged(nameof(Description));
            }

            return hasChanges;
        }
    }
}
