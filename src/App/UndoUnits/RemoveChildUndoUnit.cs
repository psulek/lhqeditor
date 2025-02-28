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
using LHQ.App.Components.VirtualTreeListViewControl;
using LHQ.App.Extensions;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.App.ViewModels.Elements;
using LHQ.Data;
using LHQ.Utils.Extensions;

namespace LHQ.App.UndoUnits
{
    internal class RemoveChildUndoUnit<TModelElement> : TreeElementChildUndoUnit<TModelElement>
        where TModelElement : ModelElementBase, new()
    {
        private readonly Func<ITreeElementViewModel> _recreateNewChild;
        private readonly TreeNodeDescriptor _savedChildNodeDescriptor;

        public RemoveChildUndoUnit(TreeElementViewModel<TModelElement> parent, ITreeElementViewModel child)
            : base(parent, child)
        {
            TreeElementType treeElementType = child.ElementType;
            string childName = child.Name;
            TreeElementType parentElementType = parent.ElementType;
            string parentElementName = parent.Name;

            InternalUpdateGetName(() => Strings.Operations.RemoveChildUndoUnitCommandName(treeElementType.ToDisplayName(),
                childName, parentElementType.ToDisplayName(), parentElementName));

            // save child viewmodel to model element to be able re-add it as viewmodel on Undo action..
            SavedChildModelElement = child.SaveToModelElement();
            if (SavedChildModelElement == null)
            {
                throw new InvalidOperationException("Save to model element failed!");
            }

            _savedChildNodeDescriptor = child.ListViewNode.CreateDescriptor();

            _recreateNewChild = () =>
                {
                    ITreeElementViewModel newElement = child.CreateFromModelElement(SavedChildModelElement, parent);
                    Dictionary<string, ITreeElementViewModel> flatElements = TreeViewService.GetChildsAsFlatList(newElement)
                        .Append(newElement)
                        .ToDictionary(x => x.ElementKey, x => x);

                    List<TreeNodeDescriptor> descriptors =
                        _savedChildNodeDescriptor.GetChildsAsFlatList()
                            .Append(_savedChildNodeDescriptor)
                            .ToList();

                    foreach (TreeNodeDescriptor descriptor in descriptors)
                    {
                        string elementKey = (descriptor.Tag as ITreeElementViewModel)?.ElementKey;

                        if (!string.IsNullOrEmpty(elementKey) && flatElements.TryGetValue(elementKey, out ITreeElementViewModel actualTreeElement))
                        {
                            descriptor.UpdateTag(actualTreeElement);
                        }
                    }

                    return newElement;
                };
        }

        protected override void InternalDo()
        {
            if (Validate(out TreeElementViewModel<TModelElement> parent))
            {
                ITreeElementViewModel child = FindChild();

                if (child != null)
                {
                    if (child.IsDisposed)
                    {
                        throw new ObjectDisposedException(
                            "RemoveChildUndoUnit.InternalDo child (key: {0} is disposed!".FormatWith(ChildElementKey));
                    }

                    // save child viewmodel to model element to be able re-add it as viewmodel on Undo action..
                    SavedChildModelElement = child.SaveToModelElement();
                    if (SavedChildModelElement == null)
                    {
                        throw new InvalidOperationException("Save to model element failed!");
                    }

                    parent.InternalRemoveChild(child);

                    parent.UpdateVersion(true);

                    TreeViewService.NotifyTranslationValidChanged();
                }
                else
                {
                    UndoManager.RemoveAllForModelElement(ChildElementKey);
                }
            }
        }

        protected override void InternalUndo()
        {
            if (Validate(out TreeElementViewModel<TModelElement> parent))
            {
                ITreeElementViewModel child = FindChild();
                bool canRecreate = child == null || child.IsDisposed;

                if (canRecreate)
                {
                    if (SavedChildModelElement == null)
                    {
                        throw new InvalidOperationException("RemoveChildUndoUnit.InternalUndo savedChildModelElement is null!");
                    }

                    // must update 'child' reference to new instance!
                    child = _recreateNewChild();

                    parent.ListViewNode.AddFromDescriptor(_savedChildNodeDescriptor);
                    parent.InternalAddChildSorted(child, false);

                    parent.UpdateVersion(false);

                    TreeViewService.NotifyTranslationValidChanged();
                }
                else
                {
                    UndoManager.RemoveAllForModelElement(ChildElementKey);
                }
            }
        }

        protected override UndoCommandCompleted InternalCompleted(UndoCommandKind undoKind)
        {
            var result = base.InternalCompleted(undoKind);

            if (Validate(out TreeElementViewModel<TModelElement> parent))
            {
                ITreeElementViewModel child = FindChild();

                if (child != null)
                {
                    // when child was removed
                    if (child.IsDisposed && SavedChildModelElement != null)
                    {
                        parent.RestoreSelectedElement(parent);
                    }
                    else
                    {
                        parent.RestoreSelectedElement(child);
                    }
                }
                else
                {
                    parent.RestoreSelectedElement(parent);
                    UndoManager.RemoveAllForModelElement(ChildElementKey);
                }
            }

            return result;
        }
    }
}
