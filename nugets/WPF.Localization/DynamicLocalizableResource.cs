// /* Copyright (C) 2020 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
namespace ScaleHQ.WPF.LHQ
{
    /// <summary>
    /// Dynamic localizable resource base class
    /// </summary>
    public abstract class DynamicLocalizableResource
    {
        /// <summary>
        /// Gets localized string based on <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">Value from <c>ConverterParameter</c> in XAML binding which will be used to get proper localized string.</param>
        /// <returns></returns>
        public abstract string GetLocalizedString(object parameter);
    }
}
