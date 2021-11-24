// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System;
using System.Resources;

namespace LHQ.App.Localization
{
    public static class StringsExt
    {
        public static string CopyrightNow => Strings.App.Copyright(DateTime.Now.Year);
    }

    public partial class StringsContext
    {
        public string GetLocalizedString(string name, bool ignoreCase = true)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            string result;
            try
            {
                if (ResourceManager.IgnoreCase != ignoreCase)
                {
                    ResourceManager.IgnoreCase = ignoreCase;
                }

                result = Culture == null ? ResourceManager.GetString(name) : ResourceManager.GetString(name, Culture);
                if (string.IsNullOrEmpty(result) && FallbackCulture != null)
                {
                    result = ResourceManager.GetString(name, FallbackCulture);
                }
            }
            catch (MissingManifestResourceException)
            {
                result = null;
            }

            return result;
        }
    }
}