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

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LHQ.App.Code;
using LHQ.App.Components.VirtualTreeListViewControl;
using LHQ.App.Extensions;
using LHQ.App.Model;
using LHQ.App.Model.Messaging;
using LHQ.App.Services.Implementation.Messaging;

namespace LHQ.App.Views
{
    public partial class TreeView : IMessageHandler
    {
        public TreeView()
        {
            InitializeComponent();

            this.RegisterMessengerHandler();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (FindResource("TreeItemStyle") is Style treeItemStyle)
            {
                treeItemStyle.BasedOn = VisualManager.Instance.DefaultListViewItemStyle;
            }
        }

        public string HandlerName { get; } = "TreeView";

        void IMessageHandler.HandleMessage(IMessengerData data)
        {
            if (data == MessengerDefinitions.FocusTree)
            {
                TreeListView.Focus();
            }
            else if (data == MessengerDefinitions.FocusSearchPanel)
            {
                SearchPanel.Focus();
            }
            else if (data is MessengerDataTreeNodeScrollIntoView msgData)
            {
                var listViewNode = msgData.Data.ListViewNode;
                if (listViewNode != null)
                {
                    TreeListView.ScrollIntoView(listViewNode);
                }
            }
            else if (data is MessengerDataTreeSelection treeSelection)
            {
                MessengerDataTreeSelection.Mode actionMode = treeSelection.ActionMode;
                IEnumerable<ITreeElementViewModel> viewModels = treeSelection.Data;

                if (viewModels != null)
                {
                    List<VirtualTreeListViewNode> nodes = viewModels
                        .Where(x => x.ListViewNode != null)
                        .Select(x => x.ListViewNode)
                        .ToList();

                    void SelectNodes()
                    {
                        TreeListView.SelectedItems.Clear();
                        nodes.ForEach(x => TreeListView.SelectedItems.Add(x));
                    }

                    void ScrollIntoView()
                    {
                        if (TreeListView.SelectedItems.Count > 0)
                        {
                            var firstItem = TreeListView.SelectedItems[0] as VirtualTreeListViewNode;
                            bool isUserVisible = TreeListView.IsUserVisible(firstItem);

                            if (!isUserVisible && firstItem?.Parent != null)
                            {
                                TreeListView.ScrollIntoView(firstItem);
                            }
                        }
                    }

                    if (actionMode == MessengerDataTreeSelection.Mode.Select)
                    {
                        SelectNodes();

                        ScrollIntoView();
                    }
                    else
                    {
                        IEnumerable<VirtualTreeListViewNode> currentSelection = TreeListView.SelectedItems.Cast<VirtualTreeListViewNode>();
                        List<VirtualTreeListViewNode> toRemove = currentSelection.Where(x => nodes.Any(y => y == x) && !x.IsRoot).ToList();
                        if (toRemove.Count > 0)
                        {
                            foreach (VirtualTreeListViewNode node in toRemove)
                            {
                                TreeListView.SelectedItems.Remove(node);
                            }
                        }

                        if (actionMode == MessengerDataTreeSelection.Mode.ReSelect)
                        {
                            SelectNodes();
                        }

                        if (TreeListView.SelectedItems.Count == 0 && TreeListView.Items.Count > 0)
                        {
                            var rootNode = TreeListView.Items[0] as VirtualTreeListViewNode;
                            TreeListView.SelectedItems.Add(rootNode);
                        }

                        ScrollIntoView();
                    }
                }
            }
        }
    }
}
