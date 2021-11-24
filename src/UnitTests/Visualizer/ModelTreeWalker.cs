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
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using LHQ.Data;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.UnitTests.Visualizer
{
    internal sealed class ModelTreeWalker
    {
        private readonly Model _model;

        public ModelTreeWalker(Model model)
        {
            _model = model;
        }

        public bool HasNext<T>(T element)
            where T : ModelElementBase
        {
            return Next(element) != null;
        }

        public T Next<T>(T element)
            where T : ModelElementBase
        {
            return (T)FindElement(element, true);
        }

        public CategoryElement Next(CategoryElement element)
        {
            return (CategoryElement)FindElement(element, true);
        }

        public ResourceElement Next(ResourceElement element)
        {
            return (ResourceElement)FindElement(element, true);
        }

        public ResourceParameterElement Next(ResourceParameterElement element)
        {
            return (ResourceParameterElement)FindElement(element, true);
        }

        public bool HasPrevious<T>(T element)
            where T : ModelElementBase
        {
            return Previous(element) != null;
        }

        public T Previous<T>(T element)
            where T : ModelElementBase
        {
            return (T)FindElement(element, false);
        }

        public CategoryElement Previous(CategoryElement element)
        {
            return (CategoryElement)FindElement(element, false);
        }

        public ResourceElement Previous(ResourceElement element)
        {
            return (ResourceElement)FindElement(element, false);
        }

        public ResourceParameterElement Previous(ResourceParameterElement element)
        {
            return (ResourceParameterElement)FindElement(element, false);
        }

        private ModelElementBase FindElement(ModelElementBase element, bool findNext)
        {
            ArgumentValidator.EnsureArgumentNotNull(element, "element");

            ModelElementBase nextElement;

            switch (element.ElementType)
            {
                case ModelElementType.Category:
                {
                    var category = (CategoryElement)element;
                    nextElement = FindCategory(category, findNext);
                    break;
                }
                case ModelElementType.Resource:
                {
                    var resource = (ResourceElement)element;
                    nextElement = FindResource(resource, findNext);
                    break;
                }
                case ModelElementType.ResourceParameter:
                {
                    var resourceParameter = (ResourceParameterElement)element;
                    nextElement = FindResourceParameter(resourceParameter, findNext);
                    break;
                }
                case ModelElementType.ResourceValue:
                {
                    var resourceValue = (ResourceValueElement)element;
                    nextElement = FindResourceValue(resourceValue, findNext);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException("Element type '{0}'".FormatWith(element.ElementType));
                }
            }

            return nextElement;
        }

        private CategoryElement FindCategory(CategoryElement category, bool findNext)
        {
            // parent is model
            List<CategoryElement> orderedParentCategories = category.ParentElement?.Categories.OrderBy(x => x.Name).ToList()
                ?? _model.Categories.OrderBy(x => x.Name).ToList();

            int categoryIndex = orderedParentCategories.FindIndex(
                x => x.Key.Serialize() == category.Key.Serialize() && x.Name == category.Name);

            bool found = findNext
                ? categoryIndex > -1 && categoryIndex < orderedParentCategories.Count - 1
                : categoryIndex > 0;

            if (found)
            {
                return findNext
                    ? orderedParentCategories[categoryIndex + 1]
                    : orderedParentCategories[categoryIndex - 1];
            }

            return null;
        }

        private ResourceElement FindResource(ResourceElement resource, bool findNext)
        {
            List<ResourceElement> orderedParentResources = resource.ParentElement?.Resources.OrderBy(x => x.Name).ToList()
                ?? _model.Resources.OrderBy(x => x.Name).ToList();

            int resourceIndex = orderedParentResources.FindIndex(
                x => x.Key.Serialize() == resource.Key.Serialize() && x.Name == resource.Name);

            bool found = findNext
                ? resourceIndex > -1 && resourceIndex < orderedParentResources.Count - 1
                : resourceIndex > 0;

            if (found)
            {
                return findNext
                    ? orderedParentResources[resourceIndex + 1]
                    : orderedParentResources[resourceIndex - 1];
            }

            return null;
        }

        private ResourceParameterElement FindResourceParameter(ResourceParameterElement resourceParameter, bool findNext)
        {
            if (resourceParameter.Order == 0)
            {
                throw new ModelException(_model, resourceParameter,
                    Errors.Traversal.ElementOrderMustNotBeZero(resourceParameter));
            }

            if (resourceParameter.ParentElement == null)
            {
                throw new ModelException(_model, resourceParameter,
                    Errors.Traversal.ParentElementMustNotBeNull(resourceParameter));
            }

            List<ResourceParameterElement> orderedParentParameters = resourceParameter.ParentElement.Parameters.OrderBy(x => x.Order).ToList();

            int index = orderedParentParameters.FindIndex(
                x => x.Key.Serialize() == resourceParameter.Key.Serialize() && x.Order == resourceParameter.Order);

            bool found = findNext
                ? index > -1 && index < orderedParentParameters.Count - 1
                : index > 0;

            if (found)
            {
                return findNext
                    ? orderedParentParameters[index + 1]
                    : orderedParentParameters[index - 1];
            }

            return null;
        }

        private ResourceValueElement FindResourceValue(ResourceValueElement resourceValue, bool findNext)
        {
            if (resourceValue.ParentElement == null)
            {
                throw new ModelException(_model, resourceValue,
                    Errors.Traversal.ParentElementMustNotBeNull(resourceValue));
            }

            List<ResourceValueElement> orderedParentValues = resourceValue.ParentElement.Values.OrderBy(x => x.LanguageName).ToList();

            int index = orderedParentValues.FindIndex(
                x => x.Key.Serialize() == resourceValue.Key.Serialize() && x.LanguageName == resourceValue.LanguageName);

            bool found = findNext
                ? index > -1 && index < orderedParentValues.Count - 1
                : index > 0;

            if (found)
            {
                return findNext
                    ? orderedParentValues[index + 1]
                    : orderedParentValues[index - 1];
            }

            return null;
        }
    }
}
