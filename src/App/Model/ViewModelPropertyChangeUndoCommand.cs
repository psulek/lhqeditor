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
using LHQ.App.Services.Implementation;
using LHQ.App.Services.Implementation.Undo;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.App.ViewModels;
using LHQ.App.ViewModels.Elements;
using LHQ.Data;
using LHQ.Utils.Utilities;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.App.Model
{
    public class ViewModelPropertyChangeUndoCommand<TModelElement> : UndoCommand, IModelElementUndoCommand
        where TModelElement : ModelElementBase, new()
    {
        public ViewModelPropertyChangeUndoCommand(TreeElementViewModel<TModelElement> treeViewModelElement,
            ITreeElementViewModel parentTreeElement, string propertyName, object newValue, object oldValue,
            Func<string> transactionDescription, string undoCommandUID)
            : this(treeViewModelElement as ITreeElementViewModel, parentTreeElement, propertyName,
                newValue, oldValue, transactionDescription, undoCommandUID)
        { }

        private ViewModelPropertyChangeUndoCommand(ITreeElementViewModel treeViewModelElement,
            ITreeElementViewModel parentTreeElement, string propertyName, object newValue, object oldValue,
            Func<string> transactionDescription, string undoCommandUID)
            : this(treeViewModelElement as ViewModelElementBase<TModelElement>, parentTreeElement, propertyName,
                newValue, oldValue, transactionDescription, undoCommandUID)
        { }

        public ViewModelPropertyChangeUndoCommand(ViewModelElementBase<TModelElement> viewModelElement,
            ITreeElementViewModel parentTreeElement, string propertyName, object newValue, object oldValue,
            Func<string> transactionDescription, string undoCommandUID) : base(undoCommandUID)
        {
            ArgumentValidator.EnsureArgumentNotNull(viewModelElement, "viewModelElement");

            ViewModelElement = viewModelElement;
            ModelElementType = viewModelElement.GetModelElementType();
            ModelElementKey = viewModelElement.ElementKey;

            ParentTreeElement = parentTreeElement;
            ParentTreeElementKey = parentTreeElement?.ElementKey;

            PropertyName = propertyName;
            NewValue = newValue;
            OldValue = oldValue;

            Name = transactionDescription;
            Do = InternalDo;
            Undo = InternalUndo;
            Completed = InternalCompleted;
        }

        public ViewModelElementBase<TModelElement> ViewModelElement { get; private set; }

        public ITreeElementViewModel ParentTreeElement { get; private set; }

        public ModelElementType ModelElementType { get; }

        public string ModelElementKey { get; }

        public string ParentTreeElementKey { get; }

        public string PropertyName { get; }

        public object NewValue { get; private set; }

        public object OldValue { get; }

        public ITreeViewService TreeViewService => UndoManager.ShellViewContext.TreeViewService;

        public ShellViewModel ShellViewModel => TreeViewService.RootModel.ShellViewModel;

        public ITransactionManager TransactionManager => UndoManager.ShellViewContext.TransactionManager;

        protected TransactionUndoManager UndoManager => (TransactionUndoManager)Manager;

        private void InternalDo()
        {
            ViewModelElementBase<TModelElement> viewModel = GetViewModel();
            if (viewModel != null)
            {
                viewModel.InternalSetPropertyValue(PropertyName, NewValue);

                viewModel.UpdateVersion(true);
            }
            else
            {
                UndoManager.RemoveAllForModelElement(ModelElementKey);
            }
        }

        private void InternalUndo()
        {
            ViewModelElementBase<TModelElement> viewModel = GetViewModel();
            if (viewModel != null)
            {
                viewModel.InternalSetPropertyValue(PropertyName, OldValue);

                viewModel.UpdateVersion(false);
            }
            else
            {
                UndoManager.RemoveAllForModelElement(ModelElementKey);
            }
        }

        private UndoCommandCompleted InternalCompleted(UndoCommandKind undoCommandKind)
        {
            GetViewModel(out ITreeElementViewModel treeElement);
            treeElement?.RestoreSelectedElement(treeElement);
            return UndoCommandCompleted.Default;
        }

        public bool CanRemove(string keyToRemove)
        {
            return ModelElementKey == keyToRemove;
        }

        private ViewModelElementBase<TModelElement> GetViewModel()
        {
            return GetViewModel(out _);
        }

        private ITreeElementViewModel GetParentElement()
        {
            return ParentTreeElementKey == null
                ? TreeViewService.RootModel
                : TreeViewService.FindElementByKey(ParentTreeElementKey);
        }

        private ViewModelElementBase<TModelElement> GetViewModel(out ITreeElementViewModel treeElement)
        {
            treeElement = null;
            ViewModelElementBase<TModelElement> result = null;

            if (ViewModelElement != null && !ViewModelElement.IsDisposed)
            {
                result = ViewModelElement;
                if (ModelElementType != ModelElementType.ResourceValue)
                {
                    treeElement = (ITreeElementViewModel)result;
                }
            }

            if (treeElement == null && ParentTreeElement != null && !ParentTreeElement.IsDisposed)
            {
                treeElement = ParentTreeElement;
            }

            if (result == null)
            {
                switch (ModelElementType)
                {
                    case ModelElementType.Model:
                    {
                        treeElement = TreeViewService.RootModel;
                        result = treeElement as ViewModelElementBase<TModelElement>;
                        break;
                    }
                    case ModelElementType.Category:
                    case ModelElementType.Resource:
                    {
                        treeElement = TreeViewService.FindElementByKey(ModelElementKey);
                        result = treeElement as ViewModelElementBase<TModelElement>;
                        break;
                    }
                    case ModelElementType.Language:
                    {
                        result = TreeViewService.RootModel.FindLanguageByKey(ModelElementKey) as ViewModelElementBase<TModelElement>;
                        break;
                    }
                    case ModelElementType.ResourceParameter:
                    {
                        throw new NotSupportedException("ResourceParameter");
                    }
                    case ModelElementType.ResourceValue:
                    {
                        treeElement = GetParentElement();
                        var resource = treeElement as ResourceViewModel;
                        result = resource?.FindValueByKey(ModelElementKey) as ViewModelElementBase<TModelElement>;
                        break;
                    }
                    default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }

                if (result != null)
                {
                    ViewModelElement = result;
                }
            }
            else if (treeElement == null)
            {
                if (ModelElementType == ModelElementType.ResourceValue)
                {
                    treeElement = GetParentElement();
                    ParentTreeElement = treeElement;

                    var resource = treeElement as ResourceViewModel;
                    result = resource?.FindValueByKey(ModelElementKey) as ViewModelElementBase<TModelElement>;
                    ViewModelElement = result;
                }
            }

            return result;
        }

        public override void Reuse(IUndoCommand otherCommand)
        {
            if (otherCommand is ViewModelPropertyChangeUndoCommand<TModelElement> other)
            {
                NewValue = other.NewValue;
                Name = other.Name;
            }
        }
    }
}
