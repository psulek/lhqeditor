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
using System.Collections.Generic;
using System.Linq;
using LHQ.App.Code;
using LHQ.App.Extensions;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.App.ViewModels.Elements;
using LHQ.Data;
using LHQ.Data.Interfaces.Key;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.UndoUnits
{
    internal class AddChildsUndoUnit<TModelElement> : TreeElementUndoUnit<TModelElement>
        where TModelElement : ModelElementBase, new()
    {
        private readonly List<ModelElementBase> _modelElements;
        private bool _isInitialized;

        public AddChildsUndoUnit(TreeElementViewModel<TModelElement> parent, IEnumerable<ModelElementBase> elements)
            : base(parent)
        {
            ArgumentValidator.EnsureArgumentNotNull(elements, "elements");

            ShellModelContext = parent.ShellViewModel.ModelContext;
            _modelElements = elements.ToList();
            _isInitialized = false;
        }

        private ModelContext ShellModelContext { get; }

        private void Initialize()
        {
            int countCategories = _modelElements.Count(x => x.ElementType == ModelElementType.Category);
            int countResources = _modelElements.Count(x => x.ElementType == ModelElementType.Resource);

            string categoryCountText = TextHelper.GetCategoryCountText(countCategories, true);
            string resourceCountText = TextHelper.GetResourceCountText(countResources, true);
            string commaText = countCategories > 0 && countResources > 0 ? ", " : string.Empty;

            TreeElementViewModel<TModelElement> parent = FindParent();

            TreeElementType parentElementType = parent.ElementType;
            string parentName = parent.Name;

            InternalUpdateGetName(() => Strings.Services.UndoUnits.AddChilds.ActionName($"{categoryCountText}{commaText}{resourceCountText}", parentElementType.ToDisplayName(), parentName));

            _isInitialized = true;
        }

        protected override void InternalDo()
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (Validate(out TreeElementViewModel<TModelElement> parent))
            {
                PregenerateKeys(_modelElements);

                List<ITreeElementViewModel> newChilds = CreateViewModels(parent);

                if (newChilds.Count > 0)
                {
                    RecursiveAddNewChildNodes(newChilds, parent);

                    newChilds.ForEach(x => parent.InternalAddChildSorted(x, false));

                    parent.UpdateVersion(true);

                    TreeViewService.NotifyTranslationValidChanged();
                }
            }
        }

        private void RecursiveAddNewChildNodes(List<ITreeElementViewModel> viewModels, ITreeElementViewModel parentViewModel)
        {
            foreach (ITreeElementViewModel viewModel in viewModels)
            {
                parentViewModel.ListViewNode.AddChild(viewModel);
                if (viewModel.Children != null)
                {
                    RecursiveAddNewChildNodes(viewModel.Children.ToList(), viewModel);
                }
            }
        }

        protected override void InternalUndo()
        {
            if (Validate(out TreeElementViewModel<TModelElement> parent))
            {
                List<ITreeElementViewModel> viewModels = FindViewModels();

                if (viewModels.Count > 0)
                {
                    foreach (ITreeElementViewModel child in viewModels.Where(x => !x.IsDisposed))
                    {
                        parent.InternalRemoveChild(child);
                    }

                    parent.UpdateVersion(false);

                    TreeViewService.NotifyTranslationValidChanged();
                }
            }
        }

        protected override UndoCommandCompleted InternalCompleted(UndoCommandKind undoKind)
        {
            var result = base.InternalCompleted(undoKind);

            if (Validate(out TreeElementViewModel<TModelElement> parent))
            {
                List<ITreeElementViewModel> viewModels = FindViewModels();

                // when child was removed
                if (viewModels.Count == 1)
                {
                    ITreeElementViewModel firstChild = viewModels.First();

                    if (firstChild.IsDisposed)
                    {
                        throw new ObjectDisposedException(
                            "PasteFromClipboardUndoUnit.InternalCompleted child (key: {0} is disposed!".FormatWith(firstChild.ElementKey));
                    }

                    parent.RestoreSelectedElement(firstChild);
                }
                else
                {
                    parent.RestoreSelectedElement(parent);
                }
            }

            return result;
        }

        private IModelElementKey GenerateElementKey()
        {
            return ShellModelContext.KeyProvider.GenerateKey();
        }

        private List<ITreeElementViewModel> CreateViewModels(TreeElementViewModel<TModelElement> parent)
        {
            var result = new List<ITreeElementViewModel>();
            if (_modelElements != null)
            {
                foreach (ModelElementBase modelElement in _modelElements)
                {
                    ITreeElementViewModel elementViewModel;

                    switch (modelElement.ElementType)
                    {
                        case ModelElementType.Model:
                        case ModelElementType.Language:
                        case ModelElementType.ResourceParameter:
                        case ModelElementType.ResourceValue:
                        {
                            throw new NotSupportedException();
                        }
                        case ModelElementType.Category:
                        {
                            var category = (CategoryElement)modelElement;
                            string backupName = category.Name;
                            category.Name = NameHelper.GenerateUniqueCategoryName(category.Name,
                                (ICategoryLikeViewModel)parent);

                            elementViewModel = new CategoryViewModel(ShellViewContext, parent, (CategoryElement)modelElement);
                            category.Name = backupName;
                            break;
                        }
                        case ModelElementType.Resource:
                        {
                            var resource = (ResourceElement)modelElement;
                            string backupName = resource.Name;
                            resource.Name = NameHelper.GenerateUniqueResourceName(resource.Name,
                                (ICategoryLikeViewModel)parent);

                            elementViewModel = new ResourceViewModel(ShellViewContext, parent, (ResourceElement)modelElement);
                            resource.Name = backupName;
                            break;
                        }
                        default:
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }

                    result.Add(elementViewModel);
                }
            }

            return result;
        }

        private void PregenerateKeys(List<ModelElementBase> elements)
        {
            if (elements != null)
            {
                foreach (ModelElementBase element in elements)
                {
                    IModelElementKey modelElementKey = GenerateElementKey();
                    ShellModelContext.UpdateKey(element, modelElementKey);

                    if (element is CategoryElement category)
                    {
                        if (category.Categories != null && category.Categories.Count > 0)
                        {
                            PregenerateKeys(category.Categories.Cast<ModelElementBase>().ToList());
                        }

                        if (category.Resources != null && category.Resources.Count > 0)
                        {
                            PregenerateKeys(category.Resources.Cast<ModelElementBase>().ToList());
                        }
                    }

                    if (element is ResourceElement resource)
                    {
                        if (resource.Values != null && resource.Values.Count > 0)
                        {
                            PregenerateKeys(resource.Values.Cast<ModelElementBase>().ToList());
                        }

                        if (resource.Parameters != null && resource.Parameters.Count > 0)
                        {
                            PregenerateKeys(resource.Parameters.Cast<ModelElementBase>().ToList());
                        }
                    }
                }
            }
        }

        private List<ITreeElementViewModel> FindViewModels()
        {
            List<ITreeElementViewModel> result;

            if (_modelElements != null)
            {
                List<string> modelElementKeys =
                    _modelElements.Select(x => x.Key.Serialize()).Distinct().ToList();

                result = TreeViewService.FindAllElements(x => modelElementKeys.Contains(x.ElementKey)).ToList();
            }
            else
            {
                result = new List<ITreeElementViewModel>();
            }

            return result;
        }
    }
}
