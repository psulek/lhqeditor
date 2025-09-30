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

using System.Collections.Generic;
using LHQ.Utils.GenericTree;
using LHQ.Utils.Utilities;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.App.Components.VirtualTreeListViewControl
{
    public class TreeNodeDescriptor
    {
        public TreeNodeDescriptor(VirtualTreeListViewNode node)
        {
            ArgumentValidator.EnsureArgumentNotNull(node, "node");

            Tag = node.Tag;
            NodeTextPath = node.TextPath;
        }

        public string NodeTextPath { get; }

        public object Tag { get; private set; }

        public List<TreeNodeDescriptor> Childs { get; private set; }

        public void UpdateTag(object tag)
        {
            ArgumentValidator.EnsureArgumentNotNull(tag, "tag");
            Tag = tag;
        }

        public TreeNodeDescriptor AddChild(VirtualTreeListViewNode node)
        {
            if (Childs == null)
            {
                Childs = new List<TreeNodeDescriptor>();
            }

            var nodeItem = new TreeNodeDescriptor(node);
            Childs.Add(nodeItem);
            return nodeItem;
        }

        public IEnumerable<TreeNodeDescriptor> GetChildsAsFlatList()
        {
            var treeIterator = new GenericTreeIterator<TreeNodeDescriptor>(arg => arg.Childs ?? new List<TreeNodeDescriptor>());
            return treeIterator.IterateTree(this);
        }
    }
}
