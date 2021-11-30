﻿#region License
// Copyright (c) 2021 Peter Šulek / ScaleHQ Solutions s.r.o.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

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