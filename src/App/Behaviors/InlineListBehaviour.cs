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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.App.Behaviors
{
    public static class InlineListBehaviour
    {
        public static readonly DependencyProperty InlineListProperty =
            DependencyProperty.RegisterAttached(
                "InlineList",
                typeof(List<Inline>),
                typeof(InlineListBehaviour),
                new FrameworkPropertyMetadata(null, OnInlineListPropertyChanged));

        public static List<Inline> GetInlineList(TextBlock element)
        {
            return element?.GetValue(InlineListProperty) as List<Inline>;
        }

        public static void SetInlineList(TextBlock element, List<Inline> value)
        {
            element?.SetValue(InlineListProperty, value);
        }

        private static void OnInlineListPropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            if (obj is TextBlock textBlock)
            {
                textBlock.Inlines.Clear();

                var list = e.NewValue as List<Inline>;
                list?.ForEach(inl => textBlock.Inlines.Add(inl));
            }
        }
    }
}
