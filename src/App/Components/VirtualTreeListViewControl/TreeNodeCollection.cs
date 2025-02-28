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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using LHQ.Utils.Utilities;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.App.Components.VirtualTreeListViewControl
{
    public class TreeNodeCollection : IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private int _freezeCount;
        private bool _pendingCollectionChanged;

        private VirtualTreeListViewNode _lastIndexedNode;
        private int _lastIndexedIndex;

        public TreeNodeCollection()
        {
            RootNode = new VirtualTreeListViewNode(this) { IsExpanded = true };
        }

        public int Count => RootNode.VisibleDescendantCount;

        public object this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (_lastIndexedNode != null)
                {
                    if (index == _lastIndexedIndex)
                    {
                        return _lastIndexedNode;
                    }

                    if (index == _lastIndexedIndex + 1)
                    {
                        _lastIndexedNode = _lastIndexedNode.NextFlatNode;
                    }
                    else
                    {
                        _lastIndexedNode = null;
                        _lastIndexedIndex = 0;
                    }
                }

                if (_lastIndexedNode == null)
                {
                    _lastIndexedNode = RootNode.GetNodeAtFlatIndex(index, 0);
                }

                _lastIndexedIndex = _lastIndexedNode != null ? index : 0;

                return _lastIndexedNode;
            }
            set => throw new NotImplementedException();
        }

        public VirtualTreeListViewNode RootNode { get; }

        public int StartRange { get; private set; }

        public int RangeCount { get; private set; }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            FreezeUpdate();
            RootNode.ChildCount = 0;
            ThawUpdate();
        }

        public VirtualTreeListViewNode ChangeParent(VirtualTreeListViewNode sourceNode, VirtualTreeListViewNode newParentNode)
        {
            ArgumentValidator.EnsureArgumentNotNull(sourceNode, "sourceNode");
            ArgumentValidator.EnsureArgumentNotNull(newParentNode, "newParentNode");

            TreeNodeDescriptor rootSourceNodeItem = sourceNode.CreateDescriptor();

            sourceNode.ChildCount = 0;
            sourceNode.Parent.RemoveChild(sourceNode);

            return newParentNode.AddFromDescriptor(rootSourceNodeItem);
        }

        public bool Contains(object value)
        {
            return value is VirtualTreeListViewNode node && node.IsInExpandedPath;
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void SetRangeOfInterest(int start, int count)
        {
            if (start == StartRange && count == RangeCount)
            {
                return;
            }

            StartRange = start;
            RangeCount = count;
        }

        public IEnumerable<VirtualTreeListViewNode> EnumFlatNodes(int startIdx)
        {
            VirtualTreeListViewNode curNode = RootNode.GetNodeAtFlatIndex(startIdx, 0);

            while (curNode != null)
            {
                yield return curNode;

                curNode = curNode.NextFlatNode;
            }
        }

        public void CountChanged()
        {
            _lastIndexedNode = null;
            _lastIndexedIndex = 0;
            OnCollectionChanged();
        }

        public virtual void OnRealizeNode(VirtualTreeListViewNode node)
        { }

        public virtual void OnUpdateNode(VirtualTreeListViewNode node)
        { }

        public IDisposable DelayedInvalidate()
        {
            FreezeUpdate();
            return new Disposabler(ThawUpdate);
        }

        public void FreezeUpdate()
        {
            _freezeCount++;
        }

        public void ThawUpdate()
        {
            _freezeCount--;

            if (_freezeCount < 0)
            {
                throw new Exception("FreezeUpdate/ThawUpdate mismatch");
            }

            if (_freezeCount > 0)
            {
                return;
            }

            if (_pendingCollectionChanged)
            {
                OnCollectionChanged();
            }
        }

        protected void OnCollectionChanged()
        {
            if (_freezeCount > 0)
            {
                _pendingCollectionChanged = true;
                return;
            }

            _pendingCollectionChanged = false;

            if (CollectionChanged != null)
            {
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

                CollectionChanged(this, args);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        private struct Enumerator : IEnumerator<VirtualTreeListViewNode>
        {
            private readonly TreeNodeCollection _nodeCollection;

            public VirtualTreeListViewNode Current { get; private set; }

            object IEnumerator.Current => Current;

            public Enumerator(TreeNodeCollection nodeCollection)
            {
                _nodeCollection = nodeCollection;
                Current = _nodeCollection.RootNode;
            }

            public void Dispose()
            { }

            public bool MoveNext()
            {
                if (Current == null)
                {
                    return false;
                }

                if (Current.IsExpanded && Current.ChildCount > 0)
                {
                    Current = Current.Children[0];
                }
                else
                {
                    // Next child
                    VirtualTreeListViewNode nextSibling = Current.NextSibling;

                    if (nextSibling != null)
                    {
                        // Next child
                        Current = nextSibling;
                    }
                    else
                    {
                        // Keep stepping out until the next ancestor sibling is not null
                        while (true)
                        {
                            if (Current.IsRoot)
                            {
                                Current = null;
                                break;
                            }

                            nextSibling = Current.Parent.NextSibling;

                            if (nextSibling != null)
                            {
                                break;
                            }

                            Current = Current.Parent;
                        }

                        Current = nextSibling;
                    }
                }

                return Current != null;
            }

            public void Reset()
            {
                Current = _nodeCollection.RootNode;
            }
        }
    }
}
