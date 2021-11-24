// /* Copyright (C) 2020 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System;
using Microsoft.Extensions.Localization;

namespace ScaleHQ.AspNetCore.LHQ
{
    /// <summary>
    /// Factory for <see cref="IStringLocalizer"/> using LHQ types.
    /// </summary>
    public class StringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IStringLocalizer _stringLocalizer;

        /// <summary>
        /// Creates new instance of <see cref="StringLocalizerFactory"/>.
        /// </summary>
        /// <param name="stringLocalizer">Source LHQ string localizer.</param>
        public StringLocalizerFactory(IStringLocalizer stringLocalizer)
        {
            _stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
        }

        /// <inheritdoc />
        public IStringLocalizer Create(Type resourceSource)
        {
            return _stringLocalizer;
        }

        /// <inheritdoc />
        public IStringLocalizer Create(string baseName, string location)
        {
            return _stringLocalizer;
        }
    }
}
