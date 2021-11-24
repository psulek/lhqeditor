// /* Copyright (C) 2020 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System;

namespace ScaleHQ.WPF.LHQ
{
    /// <summary>
    /// Factory which provide LHQ strings context as singleton.
    /// </summary>
    public abstract class LocalizationContextFactoryBase
    {
        /// <summary>
        /// Gets LHQ strings context singleton.
        /// </summary>
        /// <returns></returns>
        public abstract IFormattable GetSingleton();
    }

    /// <summary>
    /// Default implementation of localization context factory which late-bound return of strings context singleton.
    /// </summary>
    public class LocalizationContextFactoryDefault : LocalizationContextFactoryBase
    {
        private readonly Func<IFormattable> _stringsContextFactory;

        /// <summary>
        /// Creates new instance of <see cref="LocalizationContextFactoryDefault"/> class.
        /// </summary>
        /// <param name="stringsContextFactory">Delegate which returns LHQ strings context singleton when required.</param>
        public LocalizationContextFactoryDefault(Func<IFormattable> stringsContextFactory)
        {
            _stringsContextFactory = stringsContextFactory ?? throw new ArgumentNullException(nameof(stringsContextFactory));
        }

        /// <summary>
        /// Gets LHQ strings context singleton.
        /// </summary>
        /// <returns></returns>
        public override IFormattable GetSingleton()
        {
            return _stringsContextFactory();
        }
    }
}