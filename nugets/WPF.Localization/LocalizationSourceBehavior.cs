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

// ReSharper disable UnusedMember.Global

namespace ScaleHQ.WPF.LHQ
{
    /// <summary>
    /// WPF source behaviour for LHQ strings context.
    /// </summary>
    public static class LocalizationSourceBehavior
    {
        /// <summary>
        /// Localization source dependency property.
        /// </summary>
        public static readonly DependencyProperty LocalizationSourceProperty =
            DependencyProperty.RegisterAttached("LocalizationSource", typeof(IFormattable), typeof(LocalizationSourceBehavior));

        /// <summary>
        /// Gets localization source from dependency object.
        /// </summary>
        /// <param name="element">LHQ strings context singleton.</param>
        /// <returns></returns>
        public static IFormattable GetLocalizationSource(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return element.GetValue(LocalizationSourceProperty) as IFormattable;
        }

        /// <summary>
        /// Sets LHQ strings context singleton on dependency object.
        /// </summary>
        /// <param name="element">Dependency object where to set localization source as LHQ strings context.</param>
        /// <param name="value"></param>
        public static void SetLocalizationSource(DependencyObject element, IFormattable value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(LocalizationSourceProperty, value);
        }
    }
}
