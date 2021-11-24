// /* Copyright (C) 2020 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;

namespace ScaleHQ.WPF.LHQ
{
    /// <summary>
    /// WPF Bindings behaviour for LHQ strings context.
    /// </summary>
    public static class LocalizationBindingsBehavior
    {
        /// <summary>
        /// Bindings dependency property.
        /// </summary>
        public static readonly DependencyProperty BindingsProperty =
            DependencyProperty.RegisterAttached(nameof(BindingsProperty),
                typeof(Collection<BindingBase>), typeof(LocalizationBindingsBehavior),
                new PropertyMetadata());

        /// <summary>
        /// Gets collection of bindings.
        /// </summary>
        /// <param name="element">Element to get bindings from.</param>
        /// <returns>Collection of bindings.</returns>
        public static Collection<BindingBase> GetBindings(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return element.GetValue(BindingsProperty) as Collection<BindingBase>;
        }

        /// <summary>
        /// Sets collection of bindings to dependency object.
        /// </summary>
        /// <param name="element">Dependency object where to set bindings on.</param>
        /// <param name="value">Collection of bindings.</param>
        public static void SetBindings(DependencyObject element, Collection<BindingBase> value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(BindingsProperty, value);
        }
    }
}