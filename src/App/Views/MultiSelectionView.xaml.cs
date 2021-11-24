// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System;
using System.Windows;
using LHQ.App.Code;
using LHQ.App.Extensions;

namespace LHQ.App.Views
{
    public partial class MultiSelectionView
    {
        private PropertyChangeNotifier _propertyChangeNotifier;

        public MultiSelectionView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            UpdateMaxHeight(this.FindParent<DetailsView>());

            _propertyChangeNotifier = new PropertyChangeNotifier(this.FindParent<DetailsView>(), ActualHeightProperty);
            _propertyChangeNotifier.ValueChanged += OnDetailsViewHeightChanged;
        }

        private void OnDetailsViewHeightChanged(object sender, EventArgs e)
        {
            var notifier = sender as PropertyChangeNotifier;
            var detailsView = notifier?.PropertySource as DetailsView;
            UpdateMaxHeight(detailsView);
        }

        private void UpdateMaxHeight(DetailsView detailsView)
        {
            if (detailsView != null)
            {
                double height = detailsView.ActualHeight - 50;
                if (height < 50)
                {
                    height = 50;
                }
                MainBorder.MaxHeight = height;
            }
        }
    }
}
