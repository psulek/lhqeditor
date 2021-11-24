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
using LHQ.App.Extensions;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Implementation.Messaging;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.App.ViewModels.Elements;
using LHQ.Data;
using LHQ.Utils.Extensions;

namespace LHQ.App.UndoUnits
{
    internal class AddSortedChildUndoUnit<TModelElement> : TreeElementChildUndoUnit<TModelElement>
        where TModelElement : ModelElementBase, new()
    {
        private readonly Func<ITreeElementViewModel> _recreateNewChild;

        public AddSortedChildUndoUnit(TreeElementViewModel<TModelElement> parent, ITreeElementViewModel child)
            : base(parent, child)
        {
            TreeElementType childElementType = child.ElementType;
            string childName = child.Name;
            TreeElementType parentElementType = parent.ElementType;
            string parentElementName = parent.Name;

            InternalUpdateGetName(() => Strings.Services.UndoUnits.AddSortedChild.ActionName(
                childElementType.ToDisplayName(), childName, parentElementType.ToDisplayName(), parentElementName));

            // save child viewmodel to model element to be able re-add it as viewmodel on Undo action..
            SavedChildModelElement = child.SaveToModelElement();
            if (SavedChildModelElement == null)
            {
                throw new InvalidOperationException("Save to model element failed!");
            }

            _recreateNewChild = () =>
                {
                    LastCreatedChild = child.CreateFromModelElement(SavedChildModelElement, parent);
                    return LastCreatedChild;
                };
        }

        internal ITreeElementViewModel LastCreatedChild { get; private set; }

        protected override void InternalDo()
        {
            if (Validate(out TreeElementViewModel<TModelElement> parent))
            {
                ITreeElementViewModel child = FindChild() ?? _recreateNewChild();

                // if child was not found, this can be case when Undo for this cmd was executed and newly added element was removed by undo,
                // so we must recreate from savedChildModelElement..

                if (child != null)
                {
                    if (child.IsDisposed)
                    {
                        throw new ObjectDisposedException(
                            "AddSortedChildUndoUnit.InternalDo child (key: {0} is disposed!".FormatWith(ChildElementKey));
                    }

                    parent.InternalAddChildSorted(child, true);

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

                if (child != null)
                {
                    if (SavedChildModelElement == null)
                    {
                        throw new InvalidOperationException("AddSortedChildUndoUnit.InternalUndo savedChildModelElement is null!");
                    }

                    if (child.IsDisposed)
                    {
                        throw new ObjectDisposedException(
                            "AddSortedChildUndoUnit.InternalUndo child (key: {0} is disposed!".FormatWith(ChildElementKey));
                    }

                    parent.InternalRemoveChild(child);

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
                    if (child.IsDisposed)
                    {
                        throw new ObjectDisposedException(
                            "AddSortedChildUndoUnit.InternalCompleted child (key: {0} is disposed!".FormatWith(ChildElementKey));
                    }

                    if (undoKind == UndoCommandKind.Do)
                    {
                        Messenger.MultiDispatch(MessengerDefinitions.SelectNodeInTree(child),
                            MessengerDefinitions.FocusPrimaryResourceValue);
                    }
                }
                else
                {
                    parent.RestoreSelectedElement(parent);
                }
            }

            return result;
        }
    }
}
