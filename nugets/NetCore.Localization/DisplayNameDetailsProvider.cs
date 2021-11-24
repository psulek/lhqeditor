// /* Copyright (C) 2020 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
#if !ASP_NET_CORE1
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Localization;

namespace ScaleHQ.AspNetCore.LHQ
{
    /// <summary>
    /// Metadata provider for display attribute(s).
    /// </summary>
    public class DisplayNameDetailsProvider : IDisplayMetadataProvider
    {
        private static IStringLocalizer _stringLocalizer;

        /// <summary>
        /// Registers LHQ strings localizer.
        /// </summary>
        /// <param name="stringLocalizer"></param>
        public static void SetStringLocalizer(IStringLocalizer stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        /// <summary>
        /// Creates display metadata from provided <paramref name="context"/>.
        /// Display metadata will contains LHQ registration types.
        /// </summary>
        /// <param name="context">Context from which to create display metadata.</param>
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            var displayAttribute = context.Attributes.OfType<DisplayAttribute>().FirstOrDefault();
            if (displayAttribute != null)
            {
                context.DisplayMetadata.DisplayName = () => GetLocalizedDisplayName(displayAttribute);
            }
        }

        private string GetLocalizedDisplayName(DisplayAttribute displayAttribute)
        {
            return _stringLocalizer[displayAttribute.Name];
        }
    }
}
#endif