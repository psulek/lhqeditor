// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
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
