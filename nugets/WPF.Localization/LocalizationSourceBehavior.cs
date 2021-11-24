// /* Copyright (C) 2020 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
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
