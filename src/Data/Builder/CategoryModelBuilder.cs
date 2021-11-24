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
using LHQ.Utils.Utilities;
using LHQ.Data.Interfaces.Builder;

namespace LHQ.Data.Builder
{
    internal sealed class CategoryModelBuilder : ICategoryModelBuilder
    {
        private readonly ModelContext _modelContext;
        private CategoryModelBuilder _categoryModelBuilder;
        private ResourceModelBuilder _resourceModelBuilder;

        public CategoryModelBuilder(ModelContext modelContext)
        {
            ArgumentValidator.EnsureArgumentNotNull(modelContext, "modelContext");
            _modelContext = modelContext;
        }

        public CategoryElement CurrentCategory { get; private set; }

        internal void SetHolderCategory(CategoryElement category)
        {
            ArgumentValidator.EnsureArgumentNotNull(category, "category");
            CurrentCategory = category;
        }

        public void AddCategory(string name)
        {
            ArgumentValidator.EnsureArgumentNotNull(CurrentCategory, "holderCategory");
            _modelContext.AddCategory(name, CurrentCategory);
        }

        public void AddCategory(string name, Action<ICategoryModelBuilder> categoryBuilder)
        {
            ArgumentValidator.EnsureArgumentNotNull(CurrentCategory, "holderCategory");
            ArgumentValidator.EnsureArgumentNotNull(categoryBuilder, "categoryBuilder");
            CategoryElement newCategory = _modelContext.AddCategory(name, CurrentCategory);

            if (_categoryModelBuilder == null)
            {
                _categoryModelBuilder = new CategoryModelBuilder(_modelContext);
            }

            _categoryModelBuilder.SetHolderCategory(newCategory);
            categoryBuilder(_categoryModelBuilder);
        }

        public void AddResource(string name)
        {
            ArgumentValidator.EnsureArgumentNotNull(CurrentCategory, "holderCategory");
            _modelContext.AddResource(name, CurrentCategory);
        }

        public void AddResource(string name, Action<IResourceModelBuilder> resourceBuilder)
        {
            ArgumentValidator.EnsureArgumentNotNull(CurrentCategory, "holderCategory");
            ArgumentValidator.EnsureArgumentNotNull(resourceBuilder, "resourceBuilder");
            ResourceElement newResource = _modelContext.AddResource(name, CurrentCategory);

            if (_resourceModelBuilder == null)
            {
                _resourceModelBuilder = new ResourceModelBuilder(_modelContext);
            }

            _resourceModelBuilder.SetHolderResource(newResource);
            resourceBuilder(_resourceModelBuilder);
        }
    }
}
