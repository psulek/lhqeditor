// /* Copyright (C) 2020 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace ScaleHQ.WPF.LHQ
{
    /// <summary>
    /// LHQ localization converted responsible for returning valid string resource with optional binding parameters.
    /// </summary>
    public class LocalizationConverter : IValueConverter, IMultiValueConverter
    {
        private readonly IFormattable _localizationContext;
        private readonly Collection<BindingBase> _bindings;

        /// <summary>
        /// Creates new instance of <see cref="LocalizationConverter"/> class.
        /// </summary>
        /// <param name="localizationContext">LHQ strings context singleton (returned by <see cref="LocalizationContextFactoryBase"/> instance) or direct reference.</param>
        /// <param name="key">Localization key.</param>
        /// <param name="bindings">Parameters bindings.</param>
        public LocalizationConverter(IFormattable localizationContext, string key, Collection<BindingBase> bindings = null)
        {
            _localizationContext = localizationContext ?? throw new ArgumentNullException(nameof(localizationContext));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            _bindings = bindings;
        }

        /// <summary>
        /// Localization key.
        /// </summary>
        internal string Key { get; set; }

        /// <inheritdoc />
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _localizationContext.ToString(Key, culture);
        }

        /// <inheritdoc />
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        /// <inheritdoc />
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string translatedString = _localizationContext.ToString(Key, culture);

            int parametersCount = values.Length;
            if (parametersCount > 1) // must be more than 1 because 1st value is 'binding' itself.
            {
                parametersCount--;

                List<object> parameters = new List<object>();
                for (int i = 0; i < parametersCount; i++)
                {
                    object param = values[i + 1];
                    string stringFormat = _bindings[i].StringFormat;
                    if (!string.IsNullOrEmpty(stringFormat))
                    {
                        param = string.Format(culture, stringFormat, param);
                    }
                    parameters.Add(param);
                }

                try
                {
                    translatedString = string.Format(culture, translatedString, parameters.ToArray());
                }
                catch (FormatException fex)
                {
                    Debug.WriteLine($"[LHQ] LocalizationConverter failed to format text {translatedString}");
                    translatedString = fex.Message;
                }
            }

            return translatedString;
        }

        /// <inheritdoc />
        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[0];
        }
    }
}
