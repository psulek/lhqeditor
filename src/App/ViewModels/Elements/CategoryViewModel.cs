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
using System.Linq;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Data;
using LHQ.Data.Extensions;

namespace LHQ.App.ViewModels.Elements
{
    public sealed class CategoryViewModel : CategoryLikeViewModel<CategoryElement>
    {
        public CategoryViewModel(IShellViewContext shellViewContext, ITreeElementViewModel parentElement, CategoryElement modelCategory)
            : base(shellViewContext, modelCategory, parentElement)
        {
            BuildModel(modelCategory);

            RaisePropertyChanged(nameof(ParentCategory));
        }

        public CategoryViewModel ParentCategory => parent as CategoryViewModel;

        public override bool AllowDrop => true;

        public override bool AllowDrag => true;

        public override ModelElementType GetModelElementType()
        {
            return ModelElementType.Category;
        }

        protected override IEnumerable<ResourceViewModel> PopulateChildResources(CategoryElement category)
        {
            return category.Resources.OrderByName().Select(x => x.ToViewModel(this, ShellViewContext));
        }

        protected override IEnumerable<CategoryViewModel> PopulateChildCategories(CategoryElement category)
        {
            return category.Categories.OrderByName().Select(x => x.ToViewModel(this, ShellViewContext));
        }

        protected override TreeElementType GetElementType()
        {
            return TreeElementType.Category;
        }

        internal override void InternalSaveToModelElement(CategoryElement category)
        {
            base.InternalSaveToModelElement(category);

            List<CategoryElement> childCategories = GetCategories().Select(x => x.SaveToModelElement()).ToList();
            if (childCategories.Count > 0)
            {
                category.Categories.AddRange(childCategories);
            }

            List<ResourceElement> childResources = GetResources().Select(x => x.SaveToModelElement()).ToList();
            if (childResources.Count > 0)
            {
                category.Resources.AddRange(childResources);
            }
        }

        protected override ITreeElementViewModel CreateFromModelElement(ModelElementBase modelElement,
            ITreeElementViewModel parentElement)
        {
            return new CategoryViewModel(ShellViewContext, parentElement, (CategoryElement)modelElement);
        }
    }
}
