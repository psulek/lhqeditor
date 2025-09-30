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
using LHQ.App.Services.Implementation;
using LHQ.App.Services.Implementation.Undo;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.App.ViewModels.Elements;
using LHQ.Data;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.UndoUnits
{
    internal class MoveChildsUndoUnit<TModelElement> : UndoCommandUnit, IModelElementUndoCommand
        where TModelElement : ModelElementBase, new()
    {
        private readonly string _targetElementKey;
        private readonly string _sourceElementKey;
        private readonly Dictionary<string, string> _childsElements;

        public MoveChildsUndoUnit(TreeElementViewModel<TModelElement> targetElement,
            List<ITreeElementViewModel> childs) : base(Guid.NewGuid().ToString())
        {
            ArgumentValidator.EnsureArgumentNotNull(targetElement, "targetElement");
            ArgumentValidator.EnsureArgumentNotNull(childs, nameof(childs));

            _targetElementKey = targetElement.ElementKey;
            _childsElements = childs.ToDictionary(x => x.ElementKey, x => x.Name);

            ITreeElementViewModel firstChild = childs.First();
            _sourceElementKey = firstChild.Parent == null
                ? TreeViewService.RootModel.ElementKey
                : firstChild.Parent.ElementKey;

            int countCategories = childs.Count(x => x.ElementType == TreeElementType.Category);
            int countResources = childs.Count(x => x.ElementType == TreeElementType.Resource);

            string commaText = countCategories > 0 && countResources > 0 ? ", " : string.Empty;

            TreeElementType treeElementType = targetElement.ElementType;
            string targetElementName = targetElement.Name;

            InternalUpdateGetName(
                () =>
                    {
                        string categoryCountText = TextHelper.GetCategoryCountText(countCategories, true);
                        string resourceCountText = TextHelper.GetResourceCountText(countResources, true);

                        return Strings.Operations.Move.MoveChildsUndoUnitCommandName(categoryCountText,
                            commaText, resourceCountText, treeElementType.ToDisplayName(), targetElementName);
                    });
        }

        private ITreeViewService TreeViewService => UndoManager.ShellViewContext.TreeViewService;

        private ITransactionManager TransactionManager => UndoManager.ShellViewContext.TransactionManager;

        private TransactionUndoManager UndoManager => (TransactionUndoManager)Manager;

        protected override void InternalDo()
        {
            if (ValidateSource(out ITreeElementViewModel sourceElement) && ValidateTarget(out ITreeElementViewModel targetElement))
            {
                List<ITreeElementViewModel> childs = FindChilds();
                if (childs.Count > 0)
                {
                    MakeChildNamesUnique(childs, targetElement);

                    targetElement.InternalMove(childs);

                    sourceElement.UpdateVersion(true);
                    targetElement.UpdateVersion(true);

                    childs.Run(x => x.UpdateVersion(true));
                }
            }
        }

        private void MakeChildNamesUnique(List<ITreeElementViewModel> childs, ITreeElementViewModel targetElement)
        {
            var target = targetElement as ICategoryLikeViewModel;

            using (TransactionManager.TemporaryBlockUndoManager())
            {
                foreach (ITreeElementViewModel child in childs)
                {
                    string uniqueName;
                    if (child.ElementType == TreeElementType.Category)
                    {
                        uniqueName = NameHelper.GenerateUniqueCategoryName(child.Name, target);
                    }
                    else if (child.ElementType == TreeElementType.Resource)
                    {
                        uniqueName = NameHelper.GenerateUniqueResourceName(child.Name, target);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }

                    if (child.Name != uniqueName)
                    {
                        child.Name = uniqueName;
                    }
                }
            }
        }

        private void RestoreChildOriginalNames(List<ITreeElementViewModel> childs)
        {
            using (TransactionManager.TemporaryBlockUndoManager())
            {
                foreach (ITreeElementViewModel child in childs)
                {
                    string originalName = _childsElements[child.ElementKey];

                    if (child.Name != originalName)
                    {
                        child.Name = originalName;
                    }
                }
            }
        }

        protected override void InternalUndo()
        {
            if (ValidateSource(out ITreeElementViewModel sourceElement) && ValidateTarget(out ITreeElementViewModel targetElement))
            {
                List<ITreeElementViewModel> childs = FindChilds();
                if (childs.Count > 0)
                {
                    sourceElement.InternalMove(childs);

                    RestoreChildOriginalNames(childs);

                    sourceElement.UpdateVersion(false);
                    targetElement.UpdateVersion(false);
                    childs.Run(x => x.UpdateVersion(false));
                }
            }
        }

        protected override UndoCommandCompleted InternalCompleted(UndoCommandKind undoKind)
        {
            return UndoCommandCompleted.Default;
        }

        private bool ValidateSource(out ITreeElementViewModel source)
        {
            return Validate(true, out source);
        }

        private bool ValidateTarget(out ITreeElementViewModel target)
        {
            return Validate(false, out target);
        }

        private bool Validate(bool validateSource, out ITreeElementViewModel element)
        {
            // if current tree element is disposed, it means it was removed so remove current undo cmd!
            element = validateSource ? FindSource() : FindTarget();
            bool isValid = element != null && !element.IsDisposed;

            if (element == null || element.IsDisposed)
            {
                string key = validateSource ? _sourceElementKey : _targetElementKey;
                UndoManager.RemoveAllForModelElement(key);
            }

            return isValid;
        }

        private List<ITreeElementViewModel> FindChilds()
        {
            var result = new List<ITreeElementViewModel>();
            foreach (string childElementKey in _childsElements.Keys)
            {
                ITreeElementViewModel child = TreeViewService.FindElementByKey(childElementKey);
                if (child != null)
                {
                    result.Add(child);
                }
            }

            return result;
        }

        private ITreeElementViewModel FindSource()
        {
            return TreeViewService.FindElementByKey(_sourceElementKey);
        }

        private ITreeElementViewModel FindTarget()
        {
            return TreeViewService.FindElementByKey(_targetElementKey);
        }

        public bool CanRemove(string keyToRemove)
        {
            return _sourceElementKey == keyToRemove || _targetElementKey == keyToRemove
                || _childsElements.ContainsKey(keyToRemove);
        }
    }
}
