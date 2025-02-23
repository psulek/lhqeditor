#region License
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

using System;
using System.Collections.Generic;
using LHQ.App.Model;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using Newtonsoft.Json.Linq;

namespace LHQ.App.Code
{
    public class AppConfigStorageV1000 : AppConfigFileStorageBase
    {
        private const string AttrVersion = "version";
        private const string AttrUILanguage = "uiLanguage";
        private const string AttrDefaultProjectLanguage = "defaultProjectLanguage";
        private const string AttrOpenLastProjectOnStartup = "openLastProjectOnStartup";
        private const string AttrAppHints = "appHints";
        private const string AttrEnableTranslation = "enableTranslation";
        private const string AttrTranslatorProviderKey = "translatorProviderKey";
        private const string AttrLockTranslationsWithResource = "lockTranslationsWithResource";
        private const string AttrTheme = "theme";
        private const string AttrData = "data";
        private const string AttrDetectTheme = "detectTheme";
        private const string AttrRunTemplateAfterSave = "runTemplateAfterSave";

        protected override AppConfig LoadFromJson(JObject jsonRoot)
        {
            var result = new AppConfig();

            if (TryGetJsonValue(jsonRoot, AttrVersion, JTokenType.String, out string version))
            {
                result.Version = version;
            }

            if (TryGetJsonValue(jsonRoot, AttrUILanguage, JTokenType.String, out string uiLanguage))
            {
                result.UILanguage = uiLanguage.IsNullOrEmpty() ? CultureCache.English.Name : uiLanguage;
            }

            if (TryGetJsonValue(jsonRoot, AttrDefaultProjectLanguage, JTokenType.String, out string defaultProjectLanguage))
            {
                result.DefaultProjectLanguage = defaultProjectLanguage.IsNullOrEmpty() ? CultureCache.English.Name : defaultProjectLanguage;
            }

            if (TryGetJsonValue(jsonRoot, AttrOpenLastProjectOnStartup, JTokenType.Boolean, out bool openLastProjectOnStartup))
            {
                result.OpenLastProjectOnStartup = openLastProjectOnStartup;
            }

            if (TryGetJsonValue(jsonRoot, AttrAppHints, JTokenType.String, out string appHints))
            {
                result.AppHints = Enum<AppHintType>.TryParse(appHints, AppHintType.All);
            }

            if (TryGetJsonValue(jsonRoot, AttrEnableTranslation, JTokenType.Boolean, out bool enableTranslation))
            {
                result.EnableTranslation = enableTranslation;
            }

            if (TryGetJsonValue(jsonRoot, AttrTranslatorProviderKey, JTokenType.String, out string translatorProviderKey))
            {
                result.TranslatorProviderKey = translatorProviderKey;
            }

            if (TryGetJsonValue(jsonRoot, AttrLockTranslationsWithResource, JTokenType.Boolean, out bool lockTranslationsWithResource))
            {
                result.LockTranslationsWithResource = lockTranslationsWithResource;
            }

            if (TryGetJsonValue(jsonRoot, AttrRunTemplateAfterSave, JTokenType.Boolean, out bool runTemplateAfterSave))
            {
                result.RunTemplateAfterSave = runTemplateAfterSave;
            }

            if (TryGetJsonValue(jsonRoot, AttrTheme, JTokenType.String, out string theme))
            {
                result.Theme = Enum.TryParse(theme, out AppVisualTheme visualTheme) ? visualTheme : AppVisualTheme.Light;

                // could not set 'Automatic' directly!
                if (result.Theme == AppVisualTheme.Automatic)
                {
                    result.Theme = AppVisualTheme.Light;
                }
            }

            if (TryGetJsonValue(jsonRoot, AttrDetectTheme, JTokenType.Boolean, out bool detectTheme))
            {
                result.DetectTheme = detectTheme;
            }

            JToken data = jsonRoot[AttrData];
            if (data != null)
            {
                result.Data = data.ToObject<Dictionary<string, string>>();
            }

            return result;
        }

        protected override JObject SaveToJson(AppConfig config)
        {
            AppVisualTheme appVisualTheme = config.Theme;
            if (appVisualTheme == AppVisualTheme.Automatic)
            {
                appVisualTheme = AppVisualTheme.Light;
                config.DetectTheme = true;
            }

            var json = new JObject
            {
                [AttrVersion] = config.Version,
                [AttrUILanguage] = config.UILanguage,
                [AttrDefaultProjectLanguage] = config.DefaultProjectLanguage,
                [AttrOpenLastProjectOnStartup] = config.OpenLastProjectOnStartup,
                [AttrAppHints] = config.AppHints.ToString(),
                [AttrEnableTranslation] = config.EnableTranslation,
                [AttrTranslatorProviderKey] = config.TranslatorProviderKey,
                [AttrTheme] = appVisualTheme.ToString(),
                [AttrData] = JToken.FromObject(config.Data),
                [AttrRunTemplateAfterSave] = config.RunTemplateAfterSave
            };

            if (config.LockTranslationsWithResource != null)
            {
                json[AttrLockTranslationsWithResource] = config.LockTranslationsWithResource.Value;
            }

            if (config.DetectTheme != null)
            {
                json[AttrDetectTheme] = config.DetectTheme;
            }

            return json;
        }
    }
}
