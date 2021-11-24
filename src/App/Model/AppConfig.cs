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
using LHQ.Utils.Extensions;

namespace LHQ.App.Model
{
    [Serializable]
    public class AppConfig
    {
        public AppConfig()
        {
            DetectTheme = null;
            Data = new Dictionary<string, string>();
        }

        public string Version { get; set; }

        public bool EnableTranslation { get; set; }

        public string TranslatorProviderKey { get; set; }

        public string UILanguage { get; set; }

        public string DefaultProjectLanguage { get; set; }

        public bool OpenLastProjectOnStartup { get; set; }

        public bool CheckUpdatesOnAppStart { get; set; }

        public bool? LockTranslationsWithResource { get; set; }

        public AppVisualTheme Theme { get; set; }

        public bool? DetectTheme { get; set; }

        public Dictionary<string, string> Data { get; set; }

        public AppHintType AppHints { get; set; }

        public void RemoveAppHint(AppHintType appHintType)
        {
            AppHints = AppHints.ClearFlags(appHintType);
        }

        public void SetAllAppHints()
        {
            AppHints = AppHintType.All;
        }

        public void SetNoneAppHints()
        {
            AppHints = AppHintType.None;
        }

        public void UpdateVersion(Version version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            Version = version.ToString();
        }
    }
}
