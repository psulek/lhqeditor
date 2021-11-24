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
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using LHQ.App.Code;
using LHQ.App.Extensions;
using LHQ.Utils.Extensions;

namespace LHQ.App.Components.VirtualTreeListViewControl
{
    public class VirtualTreeListView : ListView
    {
        private ScrollViewer _scrollViewer;
        private VirtualTreeListViewNode _pendingFocusNode;
      
        static VirtualTreeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VirtualTreeListView), new FrameworkPropertyMetadata(typeof(VirtualTreeListView)));
        }

        public VirtualTreeListView()
        {
            this.AddStyleFromResources(ResourcesConsts.VirtualTreeListViewControl);

            Loaded += OnLoaded;
            ItemContainerGenerator.StatusChanged += ItemContainerGeneratorStatusChanged;
        }

        /// <summary>
        ///     Ensure the specified node's parents are all expanded and the node is scrolled into view
        /// </summary>
        /// <param name="nodePath"></param>
        private VirtualTreeListViewNode ExposeNode(NodePath nodePath)
        {
            if (nodePath?.IntPath == null || nodePath.IntPath.Length == 0)
            {
                return null;
            }

            var treeCollectionView = ItemsSource as TreeCollectionView;
            TreeNodeCollection collection = treeCollectionView?.Collection;

            if (collection == null)
            {
                return null;
            }

            VirtualTreeListViewNode curNode = collection.RootNode;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var depth = 0; depth < nodePath.IntPath.Length; depth++)
            {
                int childIdx = nodePath.IntPath[depth];

                curNode.IsExpanded = true;

                if (curNode.ChildCount == 0)
                {
                    return curNode;
                }

                if (childIdx < 0)
                {
                    curNode = curNode.Children[0];
                    break;
                }

                if (childIdx >= curNode.ChildCount)
                {
                    curNode = curNode.Children[curNode.ChildCount - 1];
                    break;
                }

                curNode = curNode.Children[childIdx];
            }

            if (curNode.IsRoot)
            {
                return null;
            }

            int margin = (int)_scrollViewer.ViewportHeight / 3;
            int nodeIdx = curNode.FlatIndex;
            var topVisible = (int)_scrollViewer.VerticalOffset;
            int bottomVisible = topVisible + (int)_scrollViewer.ViewportHeight;
            int maxOffset = collection.RootNode.VisibleDescendantCount - (int)_scrollViewer.ViewportHeight;

            if (nodeIdx < topVisible)
            {
                _scrollViewer.ScrollToVerticalOffset(Math.Max(0, nodeIdx - margin));
            }
            else if (nodeIdx >= bottomVisible)
            {
                _scrollViewer.ScrollToVerticalOffset(Math.Max(0, Math.Min(maxOffset, nodeIdx - (int)_scrollViewer.ViewportHeight + margin)));
            }

            return curNode;
        }

        private VirtualTreeListViewNode SelectNode(NodePath nodePath)
        {
            VirtualTreeListViewNode node = ExposeNode(nodePath);

            if (node == null || node.TextPath != nodePath.TextPath)
            {
                SelectedIndex = -1;
            }
            else
            {
                SelectedIndex = node.FlatIndex;
            }

            return node;
        }

        // ReSharper disable once UnusedMember.Global
        public VirtualTreeListViewNode SelectNode(VirtualTreeListViewNode node)
        {
            if (node == null)
            {
                return null;
            }

            return SelectNode(new NodePath { TextPath = node.TextPath });
        }

        private void SetRangeOfInterest(double start, double count)
        {
            var collection = ItemsSource as TreeCollectionView;
            collection?.SetRangeOfInterest((int)Math.Round(start), (int)Math.Round(count));
        }

        private void ItemContainerGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated && _pendingFocusNode != null)
            {
                var item = ItemContainerGenerator.ContainerFromItem(_pendingFocusNode) as VirtualTreeListViewItem;
                item?.Focus();

                _pendingFocusNode = null;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new VirtualTreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is VirtualTreeListViewItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is VirtualTreeListViewItem ti && item is VirtualTreeListViewNode node)
            {
                ti.Node = node;
                ti.Tree = this;
            }

            base.PrepareContainerForItemOverride(element, item);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is VirtualTreeListViewItem ti && item is VirtualTreeListViewNode)
            {
                ti.Node = null;
                ti.Tree = null;
            }

            base.ClearContainerForItemOverride(element, item);
        }

        private void ChangeFocus(VirtualTreeListViewNode node)
        {
            if (ItemContainerGenerator.ContainerFromItem(node) is VirtualTreeListViewItem item)
            {
                item.Focus();
            }
            else
            {
                _pendingFocusNode = node;
            }
        }

        public bool IsUserVisible(VirtualTreeListViewNode node)
        {
            var itemDepObj = ItemContainerGenerator.ContainerFromItem(node);
            if (itemDepObj is VirtualTreeListViewItem item)
            {
                if (!item.IsVisible)
                {
                    return false;
                }

                if (!(VisualTreeHelper.GetParent(item) is FrameworkElement container))
                {
                    throw new ArgumentNullException(nameof(container));
                }

                Rect bounds = item.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, item.RenderSize.Width, item.RenderSize.Height));
                Rect rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
                return rect.IntersectsWith(bounds);
            }

            return false;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = this.FindChild<ScrollViewer>();

            if (_scrollViewer == null)
            {
                return;
            }

            _scrollViewer.ScrollChanged += OnScrollChanged;
            SetRangeOfInterest(_scrollViewer.VerticalOffset, _scrollViewer.ViewportHeight);
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            SetRangeOfInterest(_scrollViewer.VerticalOffset, _scrollViewer.ViewportHeight);
        }

        protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (SelectedItem is VirtualTreeListViewNode node)
            {
                if (node.IsExpandable)
                {
                    node.IsExpanded = !node.IsExpanded;
                }
            }

            base.OnPreviewMouseDoubleClick(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (SelectedItem is VirtualTreeListViewNode node)
            {
                // NOTE - experimental for fix on tree search exit (key down press) does not have focused value node
                //ChangeFocus(node);

                switch (e.Key)
                {
                    case Key.Right:
                    {
                        e.Handled = true;
                        node.IsExpanded = true;
                        ChangeFocus(node);
                        break;
                    }
                    case Key.Left:
                    {
                        e.Handled = true;

                        if (node.IsExpanded)
                        {
                            node.IsExpanded = false;
                            ChangeFocus(node);
                            break;
                        }

                        if (!node.Parent.IsRoot)
                        {
                            node.Parent.IsExpanded = false;
                            ChangeFocus(node.Parent);
                        }

                        break;
                    }
                    case Key.Subtract:
                    {
                        e.Handled = true;
                        
                        bool isControl = e.KeyboardDevice.Modifiers.IsFlagSet(ModifierKeys.Control);
                        bool isShift = e.KeyboardDevice.Modifiers.IsFlagSet(ModifierKeys.Shift);
                        bool currentExpand = isControl && !isShift;
                        bool allExpand = isControl && isShift;

                        if (currentExpand || allExpand)
                        {
                            var nodeToCollapse = currentExpand ? node : GetRootNode(node);
                            if (nodeToCollapse != null)
                            {
                                nodeToCollapse.SetExpand(false, true);

                                if (allExpand && nodeToCollapse.Children?.Count == 1)
                                {
                                    var modelRoot = nodeToCollapse.Children[0];
                                    modelRoot.SetExpand(true, false);
                                    ChangeFocus(modelRoot);
                                    SetSelectedItems(new[] { modelRoot });
                                }
                                else
                                {
                                    ChangeFocus(nodeToCollapse);
                                }
                            }
                        }
                        else
                        {
                            node.IsExpanded = false;
                            ChangeFocus(node);
                        }
                        
                        break;
                    }
                    case Key.Add:
                    {
                        e.Handled = true;

                        bool isControl = e.KeyboardDevice.Modifiers.IsFlagSet(ModifierKeys.Control);
                        bool isShift = e.KeyboardDevice.Modifiers.IsFlagSet(ModifierKeys.Shift);
                        bool currentExpand = isControl && !isShift;
                        bool allExpand = isControl && isShift;

                        if (currentExpand || allExpand)
                        {
                            var nodeToExpand = currentExpand ? node : GetRootNode(node);
                            nodeToExpand?.SetExpand(true, true);
                        }
                        else
                        {
                            node.IsExpanded = true;
                            ChangeFocus(node);
                        }

                        break;
                    }
                    case Key.Down:
                    {
                        if (e.KeyboardDevice.Modifiers == ModifierKeys.None)
                        {
                            ChangeFocus(node);
                        }
                        break;
                    }
                    case Key.Up:
                    {
                        if (e.KeyboardDevice.Modifiers == ModifierKeys.None)
                        {
                            ChangeFocus(node);
                        }
                        break;
                    }
                }
            }

            if (!e.Handled)
            {
                base.OnKeyDown(e);
            }
        }

        private VirtualTreeListViewNode GetRootNode(VirtualTreeListViewNode node)
        {
            VirtualTreeListViewNode root = node;
            while (root.Parent != null)
            {
                root = root.Parent;
            }

            return root;
        }

        // NOTE: Do not remove, else it tries to enumerate ALL items
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        { }
    }
}
