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

using System.Windows;
using System.Windows.Controls;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.App.Behaviors
{
    public static class BindableToolTipBehavior
    {
        public static readonly DependencyProperty ToolTipProperty = DependencyProperty.RegisterAttached(
            "ToolTip", typeof(FrameworkElement), typeof(BindableToolTipBehavior), new PropertyMetadata(null, OnToolTipChanged));

        public static readonly DependencyProperty DataContextProperty = DependencyProperty.RegisterAttached(
            "DataContext", typeof(object), typeof(BindableToolTipBehavior), new PropertyMetadata(null, OnDataContextChanged));

        public static void SetToolTip(DependencyObject element, FrameworkElement value)
        {
            element.SetValue(ToolTipProperty, value);
        }

        public static FrameworkElement GetToolTip(DependencyObject element)
        {
            return (FrameworkElement)element.GetValue(ToolTipProperty);
        }

        private static void OnToolTipChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            ToolTipService.SetToolTip(element, e.NewValue);

            if (e.NewValue != null)
            {
                ((ToolTip)e.NewValue).DataContext = GetDataContext(element);
            }
        }

        public static void SetDataContext(DependencyObject element, object value)
        {
            element.SetValue(DataContextProperty, value);
        }

        public static object GetDataContext(DependencyObject element)
        {
            return element.GetValue(DataContextProperty);
        }

        private static void OnDataContextChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement toolTip = GetToolTip(element);
            if (toolTip != null)
            {
                toolTip.DataContext = e.NewValue;
            }
        }
    }
}
