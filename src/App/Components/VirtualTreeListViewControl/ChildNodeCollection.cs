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

namespace LHQ.App.Components.VirtualTreeListViewControl
{
    public class ChildNodeCollection : IDisposable
    {
        private static readonly SortedNodeComparer _sortedNodeComparer;
        private readonly SortedDictionary<int, VirtualTreeListViewNode> _realizedChildren;
        private bool _isDisposed;
        private SortedSet<VirtualTreeListViewNode> _expandedChildren;
        private VirtualTreeListViewNode _parent;
        private int _count;
        private int _totalDescendantCount;
        private int _visibleDescendantCount;

        /// <summary>
        ///     Static constructor
        /// </summary>
        static ChildNodeCollection()
        {
            _sortedNodeComparer = new SortedNodeComparer();
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="parent"></param>
        public ChildNodeCollection(VirtualTreeListViewNode parent)
        {
            _parent = parent;
            _realizedChildren = new SortedDictionary<int, VirtualTreeListViewNode>();
        }

        /// <summary>
        ///     Get or set the number of child nodes
        /// </summary>
        public int Count
        {
            get
            {
                CheckDisposed();
                return _count;
            }
            set
            {
                CheckDisposed();

                int immediateDelta = value - _count;
                int totalDelta = immediateDelta;
                //var visibleDelta = 0;

                if (immediateDelta == 0)
                {
                    return;
                }

                _count = value;

                if (immediateDelta < 0)
                {
                    _expandedChildren?.RemoveWhere(node => node.SiblingIndex >= _count);

                    if (_realizedChildren != null)
                    {
                        KeyValuePair<int, VirtualTreeListViewNode>[] childs = _realizedChildren.ToArray();
                        for (int i = childs.Length - 1; i >= 0; i--)
                        {
                            KeyValuePair<int, VirtualTreeListViewNode> last = childs[i];

                            if (last.Key >= _count)
                            {
                                last.Value.Dispose();
                                _realizedChildren.Remove(last.Key);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                TotalDescendantCount += totalDelta;
                VisibleDescendantCount += immediateDelta;
            }
        }

        /// <summary>
        ///     Get the child at the specified index
        /// </summary>
        /// <param name="childIdx"></param>
        /// <returns></returns>
        public VirtualTreeListViewNode this[int childIdx] => GetOrCreate(childIdx, null, false);

        /// <summary>
        ///     Get the realized children
        /// </summary>
        public IDictionary<int, VirtualTreeListViewNode> RealizedChildren => _realizedChildren;

        public int RealizedChildrenCount => _realizedChildren.Count;

        /// <summary>
        ///     Get the expanded children
        /// </summary>
        public ISet<VirtualTreeListViewNode> ExpandedChildren => _expandedChildren;

        /// <summary>
        ///     Get or set the total descendant count
        /// </summary>
        public int TotalDescendantCount
        {
            get => _totalDescendantCount;
            set
            {
                int delta = value - _totalDescendantCount;

                if (delta == 0)
                {
                    return;
                }

                _totalDescendantCount = value;

                if (_parent.IsRoot)
                {
                    _parent.NodeCollection.CountChanged();
                }
                else
                {
                    _parent.Parent.Children.TotalDescendantCount += delta;
                }
            }
        }

        /// <summary>
        ///     Get or set the total visible child count
        /// </summary>
        public int VisibleDescendantCount
        {
            get => _visibleDescendantCount;
            set
            {
                int delta = value - _visibleDescendantCount;

                if (delta == 0)
                {
                    return;
                }

                _visibleDescendantCount = value;

                if (_parent.IsRoot)
                {
                    _parent.NodeCollection.CountChanged();
                }
                else if (_parent.IsExpanded)
                {
                    _parent.Parent.Children.VisibleDescendantCount += delta;
                }
            }
        }

        /// <summary>
        ///     Dispose
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (_realizedChildren != null)
                {
                    foreach (VirtualTreeListViewNode child in _realizedChildren.Values)
                    {
                        child.Dispose();
                    }
                }

                _expandedChildren = null;
                _parent = null;
                _isDisposed = true;
            }
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException("ChildNodeCollection was disposed!");
            }
        }

        private void UpdateTotalCounts(int oldCount, int newCount)
        {
            int immediateDelta = newCount - oldCount;
            int totalDelta = immediateDelta;

            TotalDescendantCount += totalDelta;
            VisibleDescendantCount += immediateDelta;
        }

        public VirtualTreeListViewNode GetOrCreate(int childIdx, object initNodeData, bool replaceExistingNodeData)
        {
            //if (childIdx < 0 || childIdx >= _count)
            if (childIdx < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(childIdx));
            }

            if (!_realizedChildren.TryGetValue(childIdx, out VirtualTreeListViewNode childNode))
            {
                childNode = new VirtualTreeListViewNode(_parent.NodeCollection)
                {
                    SiblingIndex = childIdx,
                    Parent = _parent
                };
                _realizedChildren[childIdx] = childNode;

                if (childNode.Tag == null || replaceExistingNodeData)
                {
                    childNode.Tag = initNodeData;
                }

                _parent.NodeCollection.OnRealizeNode(childNode);
            }
            else
            {
                _parent.NodeCollection.OnUpdateNode(childNode);
            }

            return childNode;
        }

        public VirtualTreeListViewNode Add(object initNodeData)
        {
            //Count++;
            //return GetOrCreate(Count - 1, initNodeData, true);
            var countBkp = Count;
            var childIdx = countBkp;
            var result = GetOrCreate(childIdx, initNodeData, true);
            Count++;
            return result;
        }

        public void Exchange(int index1, int index2)
        {
            if (index1 < _count && index2 < _count)
            {
                VirtualTreeListViewNode node1;
                VirtualTreeListViewNode node2;
                _realizedChildren.TryGetValue(index1, out node1);
                _realizedChildren.TryGetValue(index2, out node2);

                if (node1 != null && node2 != null)
                {
                    _realizedChildren[index1] = node2;
                    node2.SiblingIndex = index1;

                    _realizedChildren[index2] = node1;
                    node1.SiblingIndex = index2;
                }
            }
        }

        /// <summary>
        ///     Add a child
        /// </summary>
        /// <param name="insertIdx"></param>
        /// <param name="initNodeData"></param>
        public VirtualTreeListViewNode Insert(int insertIdx, object initNodeData)
        {
            bool canInsert = insertIdx < _count;
            VirtualTreeListViewNode result;

            if (canInsert)
            {
                int lastIndex = _count - 1;
                Count++;

                // sample items: 0 - 1 - 2
                // insert at 1:
                // count will be 4 (after count++)
                // for 'idx' starting at '2' downto '1'
                for (int idx = lastIndex; idx >= insertIdx; idx--)
                {
                    int newIdx = idx + 1;
                    if (_realizedChildren.TryGetValue(idx, out VirtualTreeListViewNode currentNode))
                    {
                        _realizedChildren[newIdx] = currentNode;
                        currentNode.SiblingIndex = newIdx;

                        //realizedChildren[idx] = null; // fail safe..
                        _realizedChildren.Remove(idx);
                    }
                    else
                    {
                        GetOrCreate(newIdx, initNodeData, true);
                    }
                }

                if (_realizedChildren.ContainsKey(insertIdx))
                {
                    _realizedChildren.Remove(insertIdx);
                }

                result = GetOrCreate(insertIdx, initNodeData, true);
            }
            else // add to end
            {
                result = Add(initNodeData);
            }

            return result;
        }

        /// <summary>
        ///     Remove a child
        /// </summary>
        /// <param name="child"></param>
        public void Remove(VirtualTreeListViewNode child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            RemoveAt(child.SiblingIndex);
        }

        /// <summary>
        ///     Remove the child at the specified index
        /// </summary>
        /// <param name="childIdx"></param>
        public void RemoveAt(int childIdx)
        {
            if (childIdx < 0 || childIdx >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(childIdx));
            }

            RemoveChildIsExpanded(childIdx, true);

            // sample items: 0 - 1 - 2 - 3  (count: 4)
            // remove at 1:
            // count will be 3 (after remove)
            // for 'idx' starting at 1..2..3

            int lastIndex = _count - 1;
            for (int idx = childIdx; idx < lastIndex; idx++)
            {
                int nextIdx = idx + 1;
                if (_realizedChildren.TryGetValue(nextIdx, out VirtualTreeListViewNode nextNode))
                {
                    _realizedChildren[idx] = nextNode;
                    nextNode.SiblingIndex = idx;

                    _realizedChildren.Remove(nextIdx);
                }
                else
                {
                    _realizedChildren.Remove(idx);
                }
            }

            int oldCount = _count;
            _count--;
            UpdateTotalCounts(oldCount, _count);
        }

        private void RemoveChildIsExpanded(int childIdx, bool callDispose)
        {
            if (_realizedChildren.TryGetValue(childIdx, out VirtualTreeListViewNode currentNode))
            {
                if (currentNode.IsExpanded)
                {
                    currentNode.IsExpanded =
                        false; // this call is responsible to remove self node from 'expandedChildren' and update counts like 'VisibleDescendantCount'
                }

                if (callDispose)
                {
                    currentNode.Dispose();
                }
            }
        }

        /// <summary>
        ///     A child's expanded state changed
        /// </summary>
        /// <param name="child"></param>
        public void OnChildExpandChanged(VirtualTreeListViewNode child)
        {
            if (child.IsExpanded)
            {
                if (_expandedChildren == null)
                {
                    _expandedChildren = new SortedSet<VirtualTreeListViewNode>(_sortedNodeComparer);
                }

                _expandedChildren.Add(child);

                VisibleDescendantCount += child.VisibleDescendantCount;
            }
            else
            {
                if (_expandedChildren == null || !_expandedChildren.Contains(child))
                {
                    return;
                }

                _expandedChildren.Remove(child);

                if (_expandedChildren.Count == 0)
                {
                    _expandedChildren = null;
                }

                VisibleDescendantCount -= child.VisibleDescendantCount;
            }
        }

        /// <summary>
        ///     Compare sorted nodes
        /// </summary>
        private class SortedNodeComparer : IComparer<VirtualTreeListViewNode>
        {
            public int Compare(VirtualTreeListViewNode x, VirtualTreeListViewNode y)
            {
                return x.SiblingIndex.CompareTo(y.SiblingIndex);
            }
        }
    }
}
