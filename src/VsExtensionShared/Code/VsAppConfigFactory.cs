#region License
// Copyright (c) 2025 Peter Šulek / ScaleHQ Solutions s.r.o.
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
using LHQ.App.Model;
using LHQ.App.Services.Implementation;
using LHQ.Utils.Utilities;
using LHQ.VsExtension.Code;

namespace LHQ.VsExtension.Code
{
    public class VsAppConfigFactory : AppConfigFactory
    {
        private static readonly Type _thisType = typeof(VsAppConfigFactory);
        private static readonly Version _appVersion = _thisType.Assembly.GetName().Version;

        public override AppConfig CreateDefault()
        {
            var newConfig = new AppConfig
            {
                UILanguage = CultureCache.English.Name,
                OpenLastProjectOnStartup = true,
                AppHints = AppHintType.All,
                DefaultProjectLanguage = CultureCache.English.Name,
                CheckUpdatesOnAppStart = false,
                EnableTranslation = false,
                TranslatorProviderKey = string.Empty,
                RunTemplateAfterSave = true
            };
            newConfig.UpdateVersion(_appVersion);
            return newConfig;
        }

        protected override void Loaded(AppConfig appConfig)
        {
            bool? detectVsTheme = appConfig.DetectTheme;
            if (detectVsTheme == null || detectVsTheme.Value)
            {
                var appVisualTheme = VisualStudioThemeManager.Instance.Theme.ToAppVisualTheme();
                appConfig.Theme = appVisualTheme;
            }
        }

        protected override bool UpgradeIfRequired()
        {
            var result = base.UpgradeIfRequired();
            if (!result && !Current.RunTemplateAfterSave)
            {
                Current.RunTemplateAfterSave = true;
                result = true;
            }
            return result;
        }
    }
}