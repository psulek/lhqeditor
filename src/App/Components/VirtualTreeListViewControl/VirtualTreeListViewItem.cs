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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.App.Components.VirtualTreeListViewControl
{
    /// <summary>
    ///     A tree node
    /// </summary>
    public class VirtualTreeListViewItem : ListViewItem, INotifyPropertyChanged
    {
        private VirtualTreeListViewNode _node;
        private Visibility _dropDownVisibility;
        private Thickness _nodeIndent;
        private bool _isExpanded;

        /// <summary>
        ///     Static constructor
        /// </summary>
        static VirtualTreeListViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VirtualTreeListViewItem), new FrameworkPropertyMetadata(typeof(VirtualTreeListViewItem)));
        }

        /// <summary>
        ///     Get or set the node
        /// </summary>
        public VirtualTreeListViewNode Node
        {
            get => _node;
            set
            {
                if (_node != null)
                {
                    _node.PropertyChanged -= OnNodePropertyChanged;
                }

                _node = value;

                if (_node == null)
                {
                    return;
                }

                _node.PropertyChanged += OnNodePropertyChanged;

                NodeIndent = new Thickness((_node.NodeDepth - 1) * 15, 0, 0, 0);
                DropDownVisibility = _node.IsExpandable ? Visibility.Visible : Visibility.Hidden;
                IsExpanded = _node.IsExpanded;
                OnPropertyChanged("Node");
            }
        }

        /// <summary>
        ///     Get or set the tree
        /// </summary>
        public VirtualTreeListView Tree { get; set; }

        /// <summary>
        ///     Get the node indentation
        /// </summary>
        public Thickness NodeIndent
        {
            get => _nodeIndent;
            set
            {
                if (_nodeIndent == value)
                {
                    return;
                }

                _nodeIndent = value;
                OnPropertyChanged("NodeIndent");
            }
        }

        /// <summary>
        ///     Get or set the dropdown visibility
        /// </summary>
        public Visibility DropDownVisibility
        {
            get => _dropDownVisibility;
            set
            {
                if (value == _dropDownVisibility)
                {
                    return;
                }

                _dropDownVisibility = value;
                OnPropertyChanged("DropDownVisibility");
            }
        }

        /// <summary>
        ///     Get whether expanded
        /// </summary>
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
                _node.IsExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     A property changed
        /// </summary>
        /// <param name="name"></param>
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        ///     A node property changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnNodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ChildCount":
                {
                    DropDownVisibility = _node.IsExpandable ? Visibility.Visible : Visibility.Hidden;
                    break;
                }
            }

            OnPropertyChanged("Node." + e.PropertyName);
        }
    }
}
