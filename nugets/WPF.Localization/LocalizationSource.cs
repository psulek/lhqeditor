// /* Copyright (C) 2020 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System;
using System.Windows;

namespace ScaleHQ.WPF.LHQ
{
    /// <summary>
    ///     Source binding for LHQ localization context.
    /// </summary>
    /// <seealso cref="System.Windows.Freezable" />
    public class LocalizationContextSource : Freezable
    {
        /// <summary>
        /// Source dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(object), typeof(LocalizationContextSource), null);

        /// <summary>
        /// Source binding to <c>StringsContext.Instance</c>.
        /// </summary>
        public IFormattable Source
        {
            get => GetValue(SourceProperty) as IFormattable;
            set => SetValue(SourceProperty, value);
        }

        /// <inheritdoc />
        protected override Freezable CreateInstanceCore()
        {
            return new LocalizationContextSource();
        }
    }
}
