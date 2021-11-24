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
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace LHQ.App.Behaviors
{
    public static class IsFocusBehavior
    {
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached("IsFocused", typeof(bool?), typeof(IsFocusBehavior),
                new FrameworkPropertyMetadata(IsFocusedChanged));

        public static bool? GetIsFocused(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (bool?)element.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject element, bool? value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(IsFocusedProperty, value);
        }

        private static void IsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)d;

            if (e.OldValue == null)
            {
                element.GotFocus += FrameworkElement_GotFocus;
                element.LostFocus += FrameworkElement_LostFocus;
            }

            if (!element.IsVisible)
            {
                element.IsVisibleChanged += fe_IsVisibleChanged;
            }

            if (!element.IsFocused)
            {
                if ((bool?)e.NewValue ?? false)
                {
                    Dispatcher.CurrentDispatcher.BeginInvoke(new ThreadStart(() =>
                        {
                            element.Focus();
                        }), DispatcherPriority.ContextIdle);
                }
            }
        }

        private static void fe_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            if (fe.IsVisible && (bool)((FrameworkElement)sender).GetValue(IsFocusedProperty))
            {
                fe.IsVisibleChanged -= fe_IsVisibleChanged;
                fe.Focus();
            }
        }

        private static void FrameworkElement_GotFocus(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).SetValue(IsFocusedProperty, true);
        }

        private static void FrameworkElement_LostFocus(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).SetValue(IsFocusedProperty, false);
        }
    }
}
