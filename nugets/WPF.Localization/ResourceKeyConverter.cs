// /* Copyright (C) 2020 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
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