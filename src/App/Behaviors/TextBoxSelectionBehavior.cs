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
using System.Windows.Controls;
using System.Windows.Threading;

namespace LHQ.App.Behaviors
{
    public static class TextBoxSelectionBehavior
    {
        public static readonly DependencyProperty IsAllSelectedProperty =
            DependencyProperty.RegisterAttached("IsAllSelected", typeof(bool?), typeof(TextBoxSelectionBehavior),
                new FrameworkPropertyMetadata(IsAllSelectedChanged));

        public static bool? GetIsAllSelected(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (bool?)element.GetValue(IsAllSelectedProperty);
        }

        public static void SetIsAllSelected(DependencyObject element, bool? value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(IsAllSelectedProperty, value);
        }

        private static void IsAllSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox && ((bool?)e.NewValue ?? false))
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new ThreadStart(() => { textBox.SelectAll(); }), DispatcherPriority.ContextIdle);
            }
        }
    }
}
