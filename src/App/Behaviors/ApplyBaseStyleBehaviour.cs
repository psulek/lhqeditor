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
using System.Windows;
using LHQ.App.Extensions;

namespace LHQ.App.Behaviors
{
    public static class ApplyBaseStyleBehaviour
    {
        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.RegisterAttached("Style", typeof(Style), typeof(ApplyBaseStyleBehaviour),
                new PropertyMetadata(OnStyleChanged));

        public static Style GetStyle(IInputElement element)
        {
            if (element is FrameworkElement fe)
            {
                return (Style)fe.GetValue(StyleProperty);
            }

            if (element is FrameworkContentElement fce)
            {
                return (Style)fce.GetValue(StyleProperty);
            }

            throw new InvalidOperationException($"Unknown element type '{element.GetType().Name}'");
        }

        public static void SetStyle(IInputElement element, Style value)
        {
            if (element is FrameworkElement fe)
            {
                fe.SetValue(StyleProperty, value);
            }
            else if (element is FrameworkContentElement fce)
            {
                fce.SetValue(StyleProperty, value);
            }
        }

        private static void OnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (FrameworkElementExtensions.IsInDesignMode)
            {
                return;
            }

            if (e.OldValue != null)
            {
                return;
            }

            if (d is FrameworkElement fe)
            {
                fe.Loaded += OnElementLoaded;
            }
            else if (d is FrameworkContentElement fce)
            {
                fce.Loaded += OnElementLoaded;
            }
        }

        private static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            if (FrameworkElementExtensions.IsInDesignMode)
            {
                return;
            }

            if (sender is FrameworkElement fe)
            {
                fe.Loaded -= OnElementLoaded;
                Style baseStyle = fe.Style;
                Style behaviourStyle = GetStyle(fe);
                if (behaviourStyle.BasedOn == null)
                {
                    behaviourStyle.BasedOn = baseStyle;
                }

                if (fe.Style != behaviourStyle)
                {
                    fe.Style = behaviourStyle;
                }
            }
            else if (sender is FrameworkContentElement fce)
            {
                fce.Loaded -= OnElementLoaded;
                Style baseStyle = fce.Style;
                Style behaviourStyle = GetStyle(fce);
                if (behaviourStyle.BasedOn == null)
                {
                    behaviourStyle.BasedOn = baseStyle;
                }

                if (fce.Style != behaviourStyle)
                {
                    fce.Style = behaviourStyle;
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"{nameof(ApplyBaseStyleBehaviour)} is valid only for type " +
                    $"'{nameof(FrameworkElement)}' or '{nameof(FrameworkContentElement)}' descendants.");
            }
        }
    }
}
