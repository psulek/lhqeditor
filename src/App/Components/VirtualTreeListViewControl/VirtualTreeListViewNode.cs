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
using System.ComponentModel;
using System.Linq;
using System.Text;
using LHQ.App.Model;
using LHQ.Utils;
using LHQ.Utils.Utilities;
// ReSharper disable UnusedMember.Global

namespace LHQ.App.Components.VirtualTreeListViewControl
{
    public class VirtualTreeListViewNode : IDisposable, INotifyPropertyChanged
    {
        private VirtualTreeListViewNode _parent;
        private object _tag;
        private bool _isExpanded;
        private string _textPath;
        private bool _isDisposed;

        public VirtualTreeListViewNode(TreeNodeCollection nodeCollection)
        {
            NodeCollection = nodeCollection;
        }

        public TreeNodeCollection NodeCollection { get; private set; }

        public VirtualTreeListViewNode Parent
        {
            get => _parent;
            set
            {
                _parent = value;

                if (_parent == null)
                {
                    NodeDepth = 0;
                }
                else
                {
                    NodeDepth = _parent.NodeDepth + 1;
                }
            }
        }

        public object Tag
        {
            get => _tag;
            set
            {
                _tag = value;
                OnPropertyChanged("Tag");
            }
        }

        public bool IsRoot => _parent == null;

        public bool IsTranslationValid { get; } = false;

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value)
                {
                    return;
                }

                _isExpanded = value;

                Parent?.OnChildExpandChanged(this);
            }
        }

        public int ChildCount
        {
            get => Children?.Count ?? 0;
            set
            {
                if (ChildCount == value)
                {
                    return;
                }

                int prevCount = ChildCount;

                if (value == 0)
                {
                    if (Children == null)
                    {
                        return;
                    }

                    Children.Count = 0;
                    Children = null;
                }
                else
                {
                    EnsureChildrenCreated();

                    Children.Count = value;
                }

                if (prevCount == 0 || value == 0)
                {
                    OnPropertyChanged("IsExpandable");
                }

                OnPropertyChanged("ChildCount");
            }
        }

        /// <summary>
        ///     Get or set the sibling index
        /// </summary>
        public int SiblingIndex { get; set; }

        /// <summary>
        ///     Get the children, which could be null
        /// </summary>
        public ChildNodeCollection Children { get; private set; }

        /// <summary>
        ///     Get the total child count
        /// </summary>
        public int TotalDescendantCount => Children?.TotalDescendantCount ?? 0;

        /// <summary>
        ///     Get the total visible descendant count
        /// </summary>
        public int VisibleDescendantCount => Children?.VisibleDescendantCount ?? 0;

        /// <summary>
        ///     Get the node depth, with the RootNode at 0
        /// </summary>
        public int NodeDepth { get; private set; }

        /// <summary>
        ///     Get whether the node can expand
        /// </summary>
        public bool IsExpandable => ChildCount > 0;

        /// <summary>
        ///     Get the next sibling
        /// </summary>
        public VirtualTreeListViewNode NextSibling
        {
            get
            {
                if (IsRoot)
                {
                    return null;
                }

                if (SiblingIndex + 1 >= Parent.ChildCount)
                {
                    return null;
                }

                return Parent.Children[SiblingIndex + 1];
            }
        }

        /// <summary>
        ///     Get the next flat node
        /// </summary>
        public VirtualTreeListViewNode NextFlatNode
        {
            get
            {
                if (IsExpanded && ChildCount > 0)
                {
                    return Children[0];
                }

                VirtualTreeListViewNode nextNode = NextSibling;

                if (nextNode != null)
                {
                    return nextNode;
                }

                VirtualTreeListViewNode curNode = Parent;

                while (true)
                {
                    nextNode = curNode.NextSibling;

                    if (nextNode != null)
                    {
                        return nextNode;
                    }

                    if (curNode.IsRoot)
                    {
                        return null;
                    }

                    curNode = curNode.Parent;
                }
            }
        }

        /// <summary>
        ///     Get the text path
        /// </summary>
        public string TextPath
        {
            get
            {
                if (_textPath == null)
                {
                    if (_parent == null)
                    {
                        _textPath = "";
                    }
                    else
                    {
                        string parentPath = _parent.TextPath;

                        if (parentPath.Length > 0)
                        {
                            _textPath = parentPath + "." + SiblingIndex;
                        }
                        else
                        {
                            _textPath = SiblingIndex.ToString();
                        }
                    }
                }

                return _textPath;
            }
        }

        /// <summary>
        ///     Get the node's flat index
        /// </summary>
        public int FlatIndex
        {
            get
            {
                // Root is never visible
                if (IsRoot)
                {
                    return -1;
                }

                int flatIdx = SiblingIndex;

                if (!_parent.IsRoot)
                {
                    flatIdx += _parent.FlatIndex + 1;
                }

                if (_parent.Children.ExpandedChildren != null)
                {
                    // Add all expanded earlier siblings' visible descendant count
                    foreach (VirtualTreeListViewNode expandedSibling in _parent.Children.ExpandedChildren)
                    {
                        if (expandedSibling.SiblingIndex >= SiblingIndex)
                        {
                            break;
                        }

                        flatIdx += expandedSibling.VisibleDescendantCount;
                    }
                }

                return flatIdx;
            }
        }

        /// <summary>
        ///     Get whether this node is within an expanded path
        /// </summary>
        public bool IsInExpandedPath
        {
            get
            {
                if (_isDisposed)
                {
                    return false;
                }

                VirtualTreeListViewNode curNode = this;

                while (true)
                {
                    curNode = curNode.Parent;

                    if (curNode == null)
                    {
                        return true;
                    }

                    if (!curNode.IsExpanded)
                    {
                        return false;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Dispose
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                PropertyChanged = null;
                ChildCount = 0;

                if (_parent?.Children != null)
                {
                    IDictionary<int, VirtualTreeListViewNode> realizedChildren = _parent.Children.RealizedChildren;
                    if (realizedChildren.ContainsKey(SiblingIndex))
                    {
                        realizedChildren.Remove(SiblingIndex);
                    }
                }

                _parent = null;
                _tag = null;
                NodeCollection = null;

                _isDisposed = true;
            }
        }

        /// <summary>
        ///     Get the visible node at the specified flat index
        /// </summary>
        /// <param name="findIdx"></param>
        /// <param name="searchIdx"></param>
        /// <returns></returns>
        public VirtualTreeListViewNode GetNodeAtFlatIndex(int findIdx, int searchIdx)
        {
            if (findIdx < searchIdx)
            {
                return null;
            }

            int curSearchRowIdx = searchIdx;

            if (IsRoot)
            {
                if (findIdx >= VisibleDescendantCount)
                {
                    return null;
                }
            }
            else
            {
                if (curSearchRowIdx == findIdx)
                {
                    return this;
                }

                int nodeRange = 1 + (IsExpanded ? VisibleDescendantCount : 0);

                if (findIdx > curSearchRowIdx + nodeRange)
                {
                    return null;
                }

                // One of our nested children
                curSearchRowIdx++;
            }

            if (Children == null)
            {
                return null;
            }

            int startChildIdx;

            if (Children.ExpandedChildren != null)
            {
                foreach (VirtualTreeListViewNode expChild in Children.ExpandedChildren)
                {
                    int childRowIdx = curSearchRowIdx + expChild.SiblingIndex;

                    if (childRowIdx > findIdx)
                    {
                        // In one of the previous collapsed sibling nodes
                        startChildIdx = findIdx - curSearchRowIdx;

                        return Children[startChildIdx];
                    }
                    VirtualTreeListViewNode foundNode = expChild.GetNodeAtFlatIndex(findIdx, childRowIdx);

                    if (foundNode != null)
                    {
                        return foundNode;
                    }

                    curSearchRowIdx += expChild.VisibleDescendantCount;
                }
            }

            startChildIdx = findIdx - curSearchRowIdx;

            if (startChildIdx < 0 || startChildIdx >= ChildCount)
            {
                return null;
            }

            return Children[startChildIdx];
        }

        /// <summary>
        ///     A child's expanded state changed
        /// </summary>
        /// <param name="child"></param>
        private void OnChildExpandChanged(VirtualTreeListViewNode child)
        {
            if (Children == null)
            {
                throw new Exception("Children can't expand if there are no children");
            }

            Children.OnChildExpandChanged(child);
        }

        /// <summary>
        ///     A property changed
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetExpand(bool expanded, bool recursive)
        {
            if (!IsExpandable)
            {
                return;
            }

            // Root must always stay expanded
            if (!IsRoot)
            {
                IsExpanded = expanded;
            }

            if (recursive)
            {
                if (Children?.RealizedChildren == null)
                {
                    return;
                }

                foreach (KeyValuePair<int, VirtualTreeListViewNode> pair in Children.RealizedChildren)
                {
                    pair.Value.SetExpand(expanded, true);
                }
            }
        }

        public void InsertChild(int insertIdx, object initNodeData)
        {
            EnsureChildrenCreated();
            Children.Insert(insertIdx, initNodeData);
        }

        public VirtualTreeListViewNode AddChild(object initNodeData)
        {
            EnsureChildrenCreated();
            return Children.Add(initNodeData);
        }

        public void RemoveChild(VirtualTreeListViewNode child)
        {
            Children?.Remove(child);
        }

        private void EnsureChildrenCreated()
        {
            if (Children == null)
            {
                Children = new ChildNodeCollection(this);
            }
        }

        /// <summary>
        ///     String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder(TextPath);

            if (Tag != null)
            {
                sb.AppendFormat(" '{0}'", Tag);
            }

            return sb.ToString();
        }

        public VirtualTreeListViewNode AddFromDescriptor(TreeNodeDescriptor descriptor)
        {
            ArgumentValidator.EnsureArgumentNotNull(descriptor, "descriptor");

            return RecursiveAddFromDescriptor(descriptor, this);
        }

        private VirtualTreeListViewNode RecursiveAddFromDescriptor(TreeNodeDescriptor descriptor, VirtualTreeListViewNode parentNode)
        {
            VirtualTreeListViewNode newChildNode = parentNode.AddChild(descriptor.Tag);

            if (descriptor.Childs != null && descriptor.Childs.Count > 0)
            {
                foreach (TreeNodeDescriptor childNodeItem in descriptor.Childs)
                {
                    RecursiveAddFromDescriptor(childNodeItem, newChildNode);
                }
            }

            return newChildNode;
        }

        public TreeNodeDescriptor CreateDescriptor(Action<VirtualTreeListViewNode> onNodeProcessed = null)
        {
            var rootSourceNodeItem = new TreeNodeDescriptor(this);
            // store child items
            RecursiveCreateDescriptor(this, rootSourceNodeItem, onNodeProcessed);
            return rootSourceNodeItem;
        }

        private void RecursiveCreateDescriptor(VirtualTreeListViewNode node, TreeNodeDescriptor descriptor,
            Action<VirtualTreeListViewNode> onNodeProcessed)
        {
            if (node.Children != null)
            {
                foreach (VirtualTreeListViewNode childNode in node.Children.RealizedChildren.Values.ToArray())
                {
                    TreeNodeDescriptor newChildNodeItem = descriptor.AddChild(childNode);
                    RecursiveCreateDescriptor(childNode, newChildNodeItem, onNodeProcessed);
                }
            }

            if (onNodeProcessed != null && !node._isDisposed)
            {
                onNodeProcessed(node);
            }
        }

        internal void Refresh(ObservableCollectionExt<ITreeElementViewModel> children)
        {
            ChildCount = 0; // NOTE: clear old > experimental 

            if (children != null)
            {
                int childCount = children.Count;
                ChildCount = childCount;

                for (var childIdx = 0; childIdx < childCount; childIdx++)
                {
                    ITreeElementViewModel childViewModel = children[childIdx];
                    VirtualTreeListViewNode childNode = Children.GetOrCreate(childIdx, childViewModel, false);

                    childNode.Refresh(childViewModel.Children);
                }
            }
        }
    }
}
