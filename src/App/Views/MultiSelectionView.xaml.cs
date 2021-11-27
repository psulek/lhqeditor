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
