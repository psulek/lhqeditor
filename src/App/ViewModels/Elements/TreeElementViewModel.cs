#region License
// Copyright (c) 2021 Peter �ulek / ScaleHQ Solutions s.r.o.
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using LHQ.App.Code;
using LHQ.App.Components.VirtualTreeListViewControl;
using LHQ.App.Extensions;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.App.UndoUnits;
using LHQ.Core.Model.Translation;
using LHQ.Data;
using LHQ.Utils;
using LHQ.Utils.Extensions;

namespace LHQ.App.ViewModels.Elements
{
    public abstract class TreeElementViewModel<TModelElement> : ViewModelElementBase<TModelElement>, ITreeElementViewModel
        where TModelElement : ModelElementBase, new()
    {
        private bool _isTranslationValid;
        private ObservableCollectionExt<ITreeElementViewModel> _children;
        private bool _isMarkedForCut;
        private bool _isMarkedForExport;

        private bool _isExpandable;

        private bool _isVisible;

        private bool _flushCurrentChange;

        private bool _hasHighlightInlines;
        private bool _focusName;
        private string _name;

        protected ITreeElementViewModel parent;

        protected TreeElementViewModel(IShellViewContext shellViewContext, TModelElement modelElement,
            ITreeElementViewModel parentViewModel)
            : base(shellViewContext, modelElement, false)
        {
            parent = parentViewModel;

            UpdateNameOnEnterCommand = new DelegateCommand(UpdateNameOnEnterCommandExecute, UpdateNameOnEnterCommandCanExecute);
        }

        public string DisplayName => Name;

        public string ElementTypeDisplayName => ElementType.ToDisplayName();

        public bool IsExpanded => this.GetIsExpanded();

        public bool FocusName
        {
            get => _focusName;
            set => SetProperty(ref _focusName, value);
        }

        public override string Name
        {
            get => _name;
            set => SetPropertyWithUndo(ref _name, value, Strings.Common.Properties.Name);
        }

        public bool IsExpandable
        {
            get => _isExpandable;
            set => SetProperty(ref _isExpandable, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        public bool FlushCurrentChange
        {
            get => _flushCurrentChange;
            set
            {
                if (_flushCurrentChange != value && !IsDisposed)
                {
                    _flushCurrentChange = value;
                    RaisePropertyChanged(nameof(FlushCurrentChange));
                }
            }
        }

        public VirtualTreeListViewNode ListViewNode { get; set; }

        public bool IsMarkedForCut
        {
            get => _isMarkedForCut;
            set
            {
                if (value != _isMarkedForCut)
                {
                    _isMarkedForCut = value;
                    RaisePropertyChanged(nameof(IsMarkedForCut));
                    RaiseObjectPropertiesChanged();
                }
            }
        }

        public bool IsMarkedForExport
        {
            get => _isMarkedForExport;
            set
            {
                if (value != _isMarkedForExport)
                {
                    _isMarkedForExport = value;
                    RaisePropertyChanged(nameof(IsMarkedForExport));
                    RaiseObjectPropertiesChanged();
                }
            }
        }

        public bool IsTranslationValid
        {
            get => _isTranslationValid;
            set => SetProperty(ref _isTranslationValid, value);
        }

        public TreeElementType ElementType => GetElementType();

        public bool ElementIsModel => ElementType == TreeElementType.Model;

        public bool ElementIsCategory => ElementType == TreeElementType.Category;

        public bool ElementIsResource => ElementType == TreeElementType.Resource;

        public bool HasHighlightInlines
        {
            get => _hasHighlightInlines;
            set => SetProperty(ref _hasHighlightInlines, value);
        }

        public ITreeElementViewModel Parent => parent;

        public ObservableCollectionExt<ITreeElementViewModel> Children
        {
            get => _children;
            private set => SetProperty(ref _children, value);
        }

        public abstract bool AllowDrop { get; }

        public abstract bool AllowDrag { get; }

        public bool AllowInsert => false;

        protected ITreeViewService TreeViewService => ShellViewContext.TreeViewService;

        public DelegateCommand UpdateNameOnEnterCommand { get; }

        protected void SetPropertyWithUndo<T>(ref T storage, T value, string propertyKindName, [CallerMemberName] string propertyName = "")
        {
            if (Equals(storage, value))
            {
                return;
            }

            if (IsBuildModelInProgress)
            {
                storage = value;
            }
            else
            {
                if (!Equals(storage, value))
                {
                    var clonable = value as IClonable;
                    T oldValue = clonable == null ? storage : (T)clonable.Clone();

                    string GetInfo()
                    {
                        return "{0} {1}".FormatWith(ElementTypeDisplayName, propertyKindName);
                    }

                    string GetDescription()
                    {
                        bool isValueType = typeof(T).IsValueType;
                        return isValueType
                            ? Strings.Operations.PropertyChangeTransactionInfo1(GetInfo(), value)
                            : Strings.Operations.PropertyChangeTransactionInfo2(GetInfo());
                    }

                    IUndoCommand command = CreatePropertyChangeUndoCommand(this, Parent, propertyName, value, oldValue, GetDescription);
                    TransactionManager.Execute(command);
                }
            }
        }

        protected override void BuildModelInternal(TModelElement modelElement)
        {
            IsTranslationValid = true;
            IsVisible = true;

            Children = new ObservableCollectionExt<ITreeElementViewModel>();
            Children.AddRange(PopulateChilds(modelElement));

            IsExpandable = GetIsExpandable();
        }

        public void RestoreSelectedElement(ITreeElementViewModel treeElementViewModel)
        {
            if (!TransactionManager.IsBlockedUndoRedo && !TreeViewService.IsTempraryBlocked(TemporaryBlockType.Selection))
            {
                ITreeElementViewModel element = treeElementViewModel ?? GetRootModel();
                if (element.IsDisposed)
                {
                    throw new ObjectDisposedException("Could not restore selection with disposed element!");
                }

                TreeViewService.Select(element);
            }
        }

        ITreeElementViewModel ITreeElementViewModel.CreateFromModelElement(ModelElementBase modelElement,
            ITreeElementViewModel parentElement)
        {
            return CreateFromModelElement(modelElement, parentElement);
        }

        ModelElementBase ITreeElementViewModel.SaveToModelElement()
        {
            return SaveToModelElement();
        }

        protected abstract ITreeElementViewModel CreateFromModelElement(ModelElementBase modelElement,
            ITreeElementViewModel parentElement);

        protected abstract bool GetIsExpandable();

        protected abstract TreeElementType GetElementType();

        protected abstract IEnumerable<ITreeElementViewModel> PopulateChilds(TModelElement modelElement);

        public bool GetHasChildren()
        {
            return Children.Count > 0;
        }

        public void UpdateParent(ITreeElementViewModel newParent)
        {
            parent = newParent;
            ParentElementKey = newParent?.ElementKey;
        }

        void ITreeElementViewModel.PasteFromClipboard(TreeElementClipboardData clipboardData)
        {
            TransactionManager.Execute(new PasteFromClipboardUndoUnit<TModelElement>(this, clipboardData));
        }

        void ITreeElementViewModel.AddChildsElements<T>(IEnumerable<T> elements)
        {
            var undoUnit = new AddChildsUndoUnit<TModelElement>(this, elements);
            TransactionManager.Execute(undoUnit);
        }

        ITreeElementViewModel ITreeElementViewModel.AddChildSorted(ITreeElementViewModel childViewModel)
        {
            var undoUnit = new AddSortedChildUndoUnit<TModelElement>(this, childViewModel);
            TransactionManager.Execute(undoUnit);
            return undoUnit.LastCreatedChild;
        }

        internal void InternalAddChildSorted(ITreeElementViewModel childViewModel, bool addLinkedNode)
        {
            if (childViewModel.ElementType != TreeElementType.Model)
            {
                Children.AddSorted(childViewModel, TreeElementViewModelComparer.Default);

                if (addLinkedNode)
                {
                    int insertIndex = Children.GetIndexForAddSorted(childViewModel,
                        TreeElementViewModelComparer.Default);
                    ListViewNode.InsertChild(insertIndex, childViewModel);
                }
            }
        }

        public void RemoveChild(ITreeElementViewModel child)
        {
            TransactionManager.Execute(new RemoveChildUndoUnit<TModelElement>(this, child));
        }

        public void AutoTranslateResource(TranslationResult translationResult, ResourceViewModel resourceViewModel)
        {
            if (ElementType == TreeElementType.Resource)
            {
                throw new InvalidOperationException("Parent of 'Resource' cannot be resource itself!");
            }

            TransactionManager.Execute(new AutoTranslateResourceUndoUnit<TModelElement>(this, resourceViewModel, translationResult));
        }

        public void LockResourceValues(ResourceViewModel resourceViewModel, bool lockAll)
        {
            TransactionManager.Execute(new LockResourceValuesUndoUnit<TModelElement>(this, resourceViewModel, lockAll));
        }

        internal void InternalRemoveChild(ITreeElementViewModel childViewModel)
        {
            if (childViewModel.ElementType != TreeElementType.Model)
            {
                TreeViewService.Unselect(childViewModel);
                Children.Remove(childViewModel);
                ListViewNode.RemoveChild(childViewModel.ListViewNode);

                ValidatorContext.RemoveValidationErrors(childViewModel);

                childViewModel.Dispose();
            }
        }

        public void Move(List<ITreeElementViewModel> movedChilds)
        {
            List<ITreeElementViewModel> childsToMove = movedChilds?.Where(x => x.ElementType != TreeElementType.Model).ToList();
            if (childsToMove?.Count > 0)
            {
                var moveAllowed = true;

                List<ITreeElementViewModel> duplicates = FindPossibleDuplicates(movedChilds);
                if (duplicates.Count > 0)
                {
                    string dialogCaption = Strings.Operations.Move.MoveElementsCaption;

                    if (duplicates.Count == childsToMove.Count)
                    {
                        moveAllowed = false;
                        DialogService.ShowError(dialogCaption, Strings.Operations.Move.MoveElementsStoppedErrorMessage, null);
                    }
                    else
                    {
                        var message = Strings.Operations.Move.MoveElementsSomeDuplicatesMessage;
                        string detail = Strings.Operations.Move.MoveElementsSomeDuplicatesDetail;

                        moveAllowed = DialogService.ShowConfirm(dialogCaption, message, detail) == DialogResult.Yes;
                        if (moveAllowed)
                        {
                            childsToMove = childsToMove.Except(duplicates).ToList();
                        }
                    }
                }

                if (moveAllowed)
                {
                    TransactionManager.Execute(new MoveChildsUndoUnit<TModelElement>(this, childsToMove));
                }
            }
        }

        void ITreeElementViewModel.InternalMove(List<ITreeElementViewModel> movedChilds)
        {
            if (ElementType == TreeElementType.Model)
            {
                bool resourcesUnderRoot = (this as RootModelViewModel).ModelOptions.Resources == ModelOptionsResources.All;
                if (!resourcesUnderRoot && movedChilds.Any(x => x.ElementType == TreeElementType.Resource))
                {
                    //DebugUtils.Log($"[{ElementType}.InternalMove()] Could not move childs into Root element, model options does not allow resources under root");
                    return;
                }
            }

            foreach (ITreeElementViewModel child in movedChilds)
            {
                child.Parent.Children.Remove(child);
                child.Parent.ListViewNode.Refresh(child.Parent.Children);
                child.UpdateParent(this);
                InternalAddChildSorted(child, false);
            }

            RefreshListViewNode();
            RaiseObjectPropertiesChanged();

            TreeViewService.NotifyTranslationValidChanged();

            if (IsExpandable && !this.GetIsExpanded())
            {
                TreeViewService.Expand(this, false);
            }

            TreeViewService.SelectMultiple(movedChilds);
        }

        public void RefreshListViewNode()
        {
            ListViewNode.Refresh(Children);
        }

        // ReSharper disable once UnusedMember.Local
        private void DumpTreeInfoToDebug()
        {
            VirtualTreeListViewNode rootNode = ListViewNode.NodeCollection.RootNode;
            var sb = new StringBuilder(rootNode.ToString());

            void Loop(VirtualTreeListViewNode node)
            {
                if (node.Children != null)
                {
                    foreach (KeyValuePair<int, VirtualTreeListViewNode> child in node.Children.RealizedChildren)
                    {
                        var ident = new string(' ', child.Value.NodeDepth);
                        sb.AppendLine(ident + " " + child.Value);

                        Loop(child.Value);
                    }
                }
            }

            Loop(rootNode);

            DebugUtils.Log("[TreeDump] start");
            DebugUtils.Log(sb.ToString());
            DebugUtils.Log("[TreeDump] end");
        }

        private List<ITreeElementViewModel> FindPossibleDuplicates(List<ITreeElementViewModel> childs)
        {
            var result = new List<ITreeElementViewModel>();

            foreach (ITreeElementViewModel child in childs)
            {
                string uniqueName = child.GenerateUniqueName(this as ICategoryLikeViewModel);
                if (child.Name != uniqueName)
                {
                    result.Add(child);
                }
            }

            return result;
        }

        public override string ToString()
        {
            string parentType = Parent == null ? "-" : parent.ElementType.ToString();
            string parentName = Parent == null ? "-" : parent.Name;
            return "{0} '{1}', Parent: {2} '{3}'".FormatWith(ElementType, Name, parentType, parentName);
        }

        private bool UpdateNameOnEnterCommandCanExecute(object arg)
        {
            return true;
        }

        private void UpdateNameOnEnterCommandExecute(object parameter)
        {
            // code inspiration from http://stackoverflow.com/a/26777735/316886
            if (parameter is TextBox tBox)
            {
                DependencyProperty prop = TextBox.TextProperty;
                BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
                binding?.UpdateSource();
            }
        }

        internal override bool InternalSetPropertyValue(string propertyName, object value)
        {
            var hasChanges = false;

            if (propertyName == nameof(Name))
            {
                hasChanges = true;
                _name = value as string;

                RaisePropertyChanged(nameof(Name));
                RaisePropertyChanged(nameof(DisplayName));
            }

            return hasChanges;
        }

        protected internal override void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.RaisePropertyChanged(propertyName);

            if (propertyName == nameof(Version))
            {
                TreeViewService.NotifyPropertyValidator();
            }
        }

        protected override string ValidateProperty(string propertyName)
        {
            IValidationError error = ValidatorContext.Errors.SingleOrDefault(x => x.TreeElement == this && x.PropertyName == propertyName);
            return error?.Error;
        }
    }
}
