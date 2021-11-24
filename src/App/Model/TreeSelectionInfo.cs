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
using LHQ.App.Localization;
using LHQ.App.ViewModels.Elements;
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.App.Model
{
    public sealed class TreeSelectionInfo
    {
        private readonly List<ITreeElementViewModel> _selectedElements;

        public TreeSelectionInfo(List<ITreeElementViewModel> selectedElements)
        {
            _selectedElements = selectedElements;

            IsEmpty = _selectedElements == null || _selectedElements.Count == 0;
            IsMultiSelection = _selectedElements != null && _selectedElements.Count > 1;
            IsSingleSelection = _selectedElements != null && _selectedElements.Count == 1;
            Element = selectedElements?.FirstOrDefault();
            ParentElement = Element?.Parent;

            DepthLevel = 1;
            if (Element != null)
            {
                ITreeElementViewModel parent = Element.Parent;
                while (parent != null)
                {
                    DepthLevel++;
                    parent = parent.Parent;
                }
            }
        }

        public bool IsEmpty { get; }

        public bool IsMultiSelection { get; }

        public bool IsSingleSelection { get; }

        public int DepthLevel { get; }

        public ITreeElementViewModel Element { get; }

        public ITreeElementViewModel ParentElement { get; }

        public bool ElementIsModel => IsSingleSelection && ElementType == TreeElementType.Model;

        public bool ElementIsCategory => IsSingleSelection && ElementType == TreeElementType.Category;

        public bool ElementIsResource => IsSingleSelection && ElementType == TreeElementType.Resource;

        public CategoryViewModel ElementAsCategory => (CategoryViewModel)Element;

        public ResourceViewModel ElementAsResource => (ResourceViewModel)Element;

        public TreeElementType ElementType
        {
            get
            {
                if (!IsSingleSelection)
                {
                    throw new InvalidOperationException("Single selection mode is required to call this!");
                }

                return Element.ElementType;
            }
        }

        public List<ITreeElementViewModel> GetSelectedElements()
        {
            return _selectedElements;
        }

        public string GetDisplayText()
        {
            if (_selectedElements.Count == 0)
            {
                return Strings.Model.Tree.SelectionNone;
            }

            if (_selectedElements.Count == 1)
            {
                TreeElementType elementType = _selectedElements.First().ElementType;
                switch (elementType)
                {
                    case TreeElementType.Model:
                    {
                        return Strings.Model.Tree.ProjectRoot;
                    }
                    case TreeElementType.Category:
                    {
                        return Strings.Views.CategoryView.Category;
                    }
                    case TreeElementType.Resource:
                    {
                        return Strings.Views.ResourceView.Resource;
                    }
                    default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return Strings.Model.Tree.SelectionMultiple(_selectedElements.Count);
        }
    }
}
