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

using System;
using System.Globalization;
using LHQ.Utils.Utilities;
using LHQ.Data.Interfaces.Builder;

namespace LHQ.Data.Builder
{
    internal sealed class ModelBuilder : IModelBuilder
    {
        private readonly ModelContext _context;
        private CategoryModelBuilder _categoryModelBuilder;
        private ResourceModelBuilder _resourceModelBuilder;

        internal ModelBuilder(ModelContext context)
        {
            ArgumentValidator.EnsureArgumentNotNull(context, "context");
            _context = context;
        }

        public Model Model => _context.Model;

        public IModelBuilder AddLanguage(string name, bool isPrimary)
        {
            _context.AddLanguage(name, isPrimary);
            return this;
        }

        public IModelBuilder AddLanguage(CultureInfo cultureInfo, bool isPrimary)
        {
            _context.AddLanguage(cultureInfo, isPrimary);
            return this;
        }

        public IModelBuilder AddCategory(string name)
        {
            _context.AddCategory(name);
            return this;
        }

        public IModelBuilder AddCategory(string name, Action<ICategoryModelBuilder> categoryBuilder)
        {
            ArgumentValidator.EnsureArgumentNotNull(categoryBuilder, "categoryBuilder");
            CategoryElement newCategory = _context.AddCategory(name);

            if (_categoryModelBuilder == null)
            {
                _categoryModelBuilder = new CategoryModelBuilder(_context);
            }

            _categoryModelBuilder.SetHolderCategory(newCategory);
            categoryBuilder(_categoryModelBuilder);

            return this;
        }

        public IModelBuilder AddResource(string name)
        {
            _context.AddResource(name, null);
            return this;
        }

        public IModelBuilder AddResource(string name, Action<IResourceModelBuilder> resourceBuilder)
        {
            ArgumentValidator.EnsureArgumentNotNull(resourceBuilder, "resourceBuilder");
            ResourceElement newResource = _context.AddResource(name, null);

            if (_resourceModelBuilder == null)
            {
                _resourceModelBuilder = new ResourceModelBuilder(_context);
            }

            _resourceModelBuilder.SetHolderResource(newResource);
            resourceBuilder(_resourceModelBuilder);
            return this;
        }
    }
}
