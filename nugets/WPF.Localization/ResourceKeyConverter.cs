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
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ScaleHQ.WPF.LHQ
{
    /// <summary>
    /// Resource key converter.
    /// </summary>
    public class ResourceKeyConverter : Freezable, IValueConverter
    {
        private LocalizationConverter _localizationConverter;

        /// <summary>
        /// Localization source dependency property.
        /// </summary>
        public static readonly DependencyProperty LocalizationSourceProperty =
            DependencyProperty.Register(
                nameof(LocalizationSource), 
                typeof(IFormattable),
                typeof(ResourceKeyConverter), 
                new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets LHQ strings context singleton.
        /// </summary>
        public IFormattable LocalizationSource
        {
            get => GetValue(LocalizationSourceProperty) as IFormattable;
            set => SetValue(LocalizationSourceProperty, value);
        }

        /// <inheritdoc />
        protected override Freezable CreateInstanceCore()
        {
            return new ResourceKeyConverter();
        }

        private void Initialize(string resourceKey)
        {
            if (_localizationConverter == null)
            {
                _localizationConverter = new LocalizationConverter(LocalizationSource, resourceKey);
            }
            else
            {
                _localizationConverter.Key = resourceKey;
            }
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Initialize(value as string);
            return (_localizationConverter as IValueConverter).Convert(value, targetType, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}