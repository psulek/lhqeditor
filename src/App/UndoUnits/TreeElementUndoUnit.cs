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
using LHQ.App.Model;
using LHQ.App.Services.Implementation;
using LHQ.App.Services.Implementation.Undo;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.App.ViewModels.Elements;
using LHQ.Data;

namespace LHQ.App.UndoUnits
{
    internal abstract class TreeElementUndoUnit<TModelElement> : UndoCommandUnit, IModelElementUndoCommand
        where TModelElement : ModelElementBase, new()
    {
        private readonly string _parentElementKey;

        protected TreeElementUndoUnit(TreeElementViewModel<TModelElement> parent) : base(Guid.NewGuid().ToString())
        {
            _parentElementKey = parent?.ElementKey;
        }

        protected TransactionUndoManager UndoManager => (TransactionUndoManager)Manager;

        protected IShellViewContext ShellViewContext => Manager.ShellViewContext;

        protected IShellService ShellService => ShellViewContext.ShellService;

        protected ITreeViewService TreeViewService => ShellViewContext.TreeViewService;

        protected IMessenger Messenger => ShellViewContext.Messenger;

        public virtual bool CanRemove(string keyToRemove)
        {
            return _parentElementKey == keyToRemove;
        }

        protected TreeElementViewModel<TModelElement> FindParent()
        {
            return TreeViewService.FindElementByKey(_parentElementKey) as TreeElementViewModel<TModelElement>;
        }

        protected bool Validate(out TreeElementViewModel<TModelElement> parent)
        {
            // if current tree element is disposed, it means it was removed so remove current undo cmd!
            parent = FindParent();
            bool isValid = parent != null && !parent.IsDisposed;

            if (parent == null || parent.IsDisposed)
            {
                UndoManager.RemoveAllForModelElement(_parentElementKey);
            }

            return isValid;
        }

        protected override UndoCommandCompleted InternalCompleted(UndoCommandKind undoKind)
        {
            if (Validate(out TreeElementViewModel<TModelElement> parent))
            {
                RootModelViewModel rootModel = parent.GetRootModel();
                rootModel.RefreshAllResourcesCount();
                rootModel.InvalidateTranslationCountInfo();
            }

            return UndoCommandCompleted.Default;
        }
    }
}
