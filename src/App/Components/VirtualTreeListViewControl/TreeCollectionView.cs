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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using GongSolutions.Wpf.DragDrop;
using LHQ.App.Model;
using LHQ.App.ViewModels.Elements;
using LHQ.Data;
using LHQ.Utils.Extensions;

namespace LHQ.App.Components.VirtualTreeListViewControl
{
    public class TreeCollectionView :
        DispatcherObject,
        IList,
        IComparer,
        ICollectionView,
        INotifyPropertyChanged,
        IEditableCollectionViewAddNewItem,
        IItemProperties,
        IDropTarget
    {
        private TreeNodeCollection _treeNodeCollection;

        // ReSharper disable once MemberCanBePrivate.Global
        public TreeCollectionView(TreeNodeCollection collection)
        {
            Collection = collection ?? new TreeNodeCollection();
        }

        public TreeCollectionView() : this(null)
        { }

        public TreeNodeCollection Collection
        {
            get => _treeNodeCollection;
            set
            {
                if (_treeNodeCollection != null)
                {
                    _treeNodeCollection.CollectionChanged -= OnCollectionChanged;
                }

                _treeNodeCollection = value;
                _treeNodeCollection.CollectionChanged += OnCollectionChanged;
            }
        }

        public ReadOnlyCollection<ItemPropertyInfo> ItemProperties => throw new NotImplementedException();

        public bool CanAddNew => false;

        public bool CanCancelEdit => false;

        public bool CanRemove => false;

        public object CurrentAddItem => throw new NotImplementedException();

        public object CurrentEditItem => throw new NotImplementedException();

        public bool IsAddingNew => false;

        public bool IsEditingItem => false;

        public NewItemPlaceholderPosition NewItemPlaceholderPosition
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public bool CanAddNewItem => false;

        public bool CanFilter => false;

        public bool CanGroup => false;

        public bool CanSort => false;

        public CultureInfo Culture
        {
            get => CultureInfo.CurrentCulture;
            set { }
        }

        /// <summary>
        ///     Get the current item
        /// </summary>
        public object CurrentItem
        {
            get
            {
                if (CurrentPosition < 0 || CurrentPosition >= _treeNodeCollection.Count)
                {
                    return null;
                }

                return _treeNodeCollection[CurrentPosition];
            }
        }

        /// <summary>
        ///     Get the current position
        /// </summary>
        public int CurrentPosition { get; private set; } = -1;

        public Predicate<object> Filter
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public ObservableCollection<GroupDescription> GroupDescriptions { get; } = new ObservableCollection<GroupDescription>();

        public ReadOnlyObservableCollection<object> Groups { get; } = new ReadOnlyObservableCollection<object>(new ObservableCollection<object>());

        public bool IsCurrentAfterLast => CurrentPosition >= _treeNodeCollection.Count;

        public bool IsCurrentBeforeFirst => CurrentPosition < 0;

        public bool IsEmpty => _treeNodeCollection.Count == 0;

        public SortDescriptionCollection SortDescriptions => new SortDescriptionCollection();

        public IEnumerable SourceCollection => _treeNodeCollection;

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public object this[int index]
        {
            get => _treeNodeCollection[index];
            set => throw new NotImplementedException();
        }

        public int Count => _treeNodeCollection.Count;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot { get; } = new object();

        private List<VirtualTreeListViewNode> ExtractDropData(object data)
        {
            if (data == null)
            {
                return null;
            }

            if (data is IEnumerable enumerable)
            {
                return enumerable.Cast<VirtualTreeListViewNode>().ToList();
            }

            return Enumerable.Repeat((VirtualTreeListViewNode)data, 1).ToList();
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            List<VirtualTreeListViewNode> sourceNodes = ExtractDropData(dropInfo.Data);
            var targetNode = dropInfo.TargetItem as VirtualTreeListViewNode;

            bool canMove = targetNode != null && sourceNodes.Select(x => x.NodeDepth).Distinct().Count() == 1;
            if (canMove)
            {
                Dictionary<VirtualTreeListViewNode, ITreeElementViewModel> sourceElements =
                    sourceNodes.ToDictionary(x => x, x => (ITreeElementViewModel)x.Tag);
                canMove = sourceElements.Values.Select(x => x.ElementType).Distinct().Count() == 1;

                if (canMove)
                {
                    if (targetNode.Tag is ITreeElementViewModel targetElement)
                    {
                        if (targetElement.ElementType == TreeElementType.Model)
                        {
                            var root = targetElement as RootModelViewModel;
                            bool resourcesUnderRoot = root?.ModelOptions.Resources == ModelOptionsResources.All;

                            if (!resourcesUnderRoot && sourceElements.Values.Any(x => x.ElementType == TreeElementType.Resource))
                            {
                                canMove = false;
                            }
                        }

                        if (canMove)
                        {
                            TreeElementType targetElementType = targetElement.ElementType;
                            bool targetElementCanContainChilds = targetElementType.In(TreeElementType.Category, TreeElementType.Model);

                            foreach (KeyValuePair<VirtualTreeListViewNode, ITreeElementViewModel> pair in sourceElements)
                            {
                                VirtualTreeListViewNode sourceNode = pair.Key;
                                ITreeElementViewModel sourceElement = pair.Value;
                                TreeElementType sourceElementType = sourceElement.ElementType;

                                canMove &= sourceNode != targetNode &&
                                    sourceNode.Parent != targetNode &&
                                    sourceElementType != TreeElementType.Model &&
                                    targetNode.Parent != sourceNode &&
                                    sourceNode.FlatIndex != targetNode.FlatIndex &&
                                    (
                                        sourceElementType == TreeElementType.Resource && targetElementCanContainChilds
                                        ||
                                        sourceElementType == TreeElementType.Category && targetElementCanContainChilds
                                    );

                                if (!canMove)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (canMove)
            {
                dropInfo.Effects = DragDropEffects.Move;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            List<VirtualTreeListViewNode> sourceNodes = ExtractDropData(dropInfo.Data);
            if (sourceNodes != null && sourceNodes.Count > 0 && dropInfo.TargetItem is VirtualTreeListViewNode targetNode)
            {
                var targetElement = targetNode.Tag as ITreeElementViewModel;
                List<ITreeElementViewModel> movedChilds = sourceNodes.Select(x => x.Tag).Cast<ITreeElementViewModel>().ToList();
                targetElement?.Move(movedChilds);
            }
        }

        /// <summary>
        ///     The collection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(sender, e);
        }

        /// <summary>
        ///     A property changed
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Set the range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public void SetRangeOfInterest(int start, int count)
        {
            _treeNodeCollection.SetRangeOfInterest(start, count);
        }

        public object AddNew()
        {
            throw new NotImplementedException();
        }

        public void CancelEdit()
        {
            throw new NotImplementedException();
        }

        public void CancelNew()
        {
            throw new NotImplementedException();
        }

        public void CommitEdit()
        {
            throw new NotImplementedException();
        }

        public void CommitNew()
        {
            throw new NotImplementedException();
        }

        public void EditItem(object item)
        {
            throw new NotImplementedException();
        }

        public void Remove(object item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public object AddNewItem(object newItem)
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public bool Contains(object item)
        {
            return _treeNodeCollection.Contains(item);
        }

        public IDisposable DeferRefresh()
        {
            return new DeferRefreshProxy();
        }

        public bool MoveCurrentTo(object item)
        {
            if (!(item is VirtualTreeListViewNode node))
            {
                return false;
            }

            CurrentPosition = node.FlatIndex;
            return CurrentPosition >= 0;
        }

        public bool MoveCurrentToFirst()
        {
            return MoveCurrentToPosition(0);
        }

        public bool MoveCurrentToLast()
        {
            return MoveCurrentToPosition(_treeNodeCollection.Count - 1);
        }

        public bool MoveCurrentToNext()
        {
            CurrentPosition = Math.Min(_treeNodeCollection.Count, CurrentPosition + 1);

            return CurrentPosition >= 0 && CurrentPosition < _treeNodeCollection.Count;
        }

        public bool MoveCurrentToPrevious()
        {
            CurrentPosition = Math.Max(-1, CurrentPosition - 1);

            return CurrentPosition >= 0 && CurrentPosition < _treeNodeCollection.Count;
        }

        public bool MoveCurrentToPosition(int position)
        {
            CurrentPosition = Math.Max(-1, Math.Min(_treeNodeCollection.Count, position));

            return CurrentPosition >= 0 && CurrentPosition < _treeNodeCollection.Count;
        }

        public void Refresh()
        { }

        public IEnumerator GetEnumerator()
        {
            return _treeNodeCollection.GetEnumerator();
        }

        public int Compare(object x, object y)
        {
            return 0;
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            return ((VirtualTreeListViewNode)value).FlatIndex;
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        private class DeferRefreshProxy : IDisposable
        {
            public void Dispose()
            { }
        }

#pragma warning disable CS0067
        public event EventHandler CurrentChanged;

        public event CurrentChangingEventHandler CurrentChanging;
#pragma warning restore CS0067
    }
}
