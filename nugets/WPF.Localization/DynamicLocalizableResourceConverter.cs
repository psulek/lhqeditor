// /* Copyright (C) 2020 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
// ReSharper disable UnusedMember.Global

namespace ScaleHQ.WPF.LHQ
{
    /// <summary>
    /// Value convertor for dynamic localizable resource.
    /// </summary>
    public class DynamicLocalizableResourceConverter : IValueConverter
    {
        private static readonly Type SupportedType = typeof(DynamicLocalizableResource);

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            Type valueType = value.GetType();
            if (!SupportedType.IsAssignableFrom(valueType))
            {
                var inDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

                if (!inDesignMode)
                {
                    throw new InvalidOperationException(
                        $"Converter '{GetType().Name}' can only be applied to object derived from '{SupportedType.FullName}' where " +
                        $"'{valueType.FullName}' is not!");
                }

                return string.Empty;
            }

            DynamicLocalizableResource resource = (DynamicLocalizableResource)value;
            return resource.GetLocalizedString(parameter as string);
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}