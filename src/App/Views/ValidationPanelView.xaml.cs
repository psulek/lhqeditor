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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LHQ.App.Code;

namespace LHQ.App.Views
{
    public partial class ValidationPanelView
    {
        public ValidationPanelView()
        {
            InitializeComponent();

            ValidationListView.DataContextChanged += ListViewOnDataContextChanged;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (FindResource("GridViewColumnHeaderStyle") is Style gridViewColumnHeaderStyle)
            {
                gridViewColumnHeaderStyle.BasedOn = VisualManager.Instance.DefaultGridViewColumnHeaderStyle;
            }
        }

        private void ListViewOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == null && e.NewValue != null)
            {
                ValidationListView.ItemContainerGenerator.StatusChanged += ItemContainerGeneratorOnStatusChanged;
            }
        }

        private void ItemContainerGeneratorOnStatusChanged(object sender, EventArgs eventArgs)
        {
            ReadOnlyCollection<object> items = ValidationListView.ItemContainerGenerator.Items;
            if (items.Count > 0)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    if (ValidationListView.ItemContainerGenerator.ContainerFromIndex(i) is ListViewItem item && item.BorderThickness.Left == 1)
                    {
                        item.BorderThickness = new Thickness(0);
                    }
                }
            }
        }
    }
}
