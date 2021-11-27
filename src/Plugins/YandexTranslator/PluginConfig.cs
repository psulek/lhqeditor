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

using System.ComponentModel;
using LHQ.Core.Interfaces.PluginSystem;

namespace LHQ.Plugin.YandexTranslator
{
    public class PluginConfig: IPluginConfig
    {
        private const string CategoryYandexTranslator = "Yandex Translator";
        private const string HelpUrl = "https://translate.yandex.com/developers/keys";

        [Category(CategoryYandexTranslator)]
        [DisplayName("API Key")]
        [Description("API key to Yandex Translator.")]
        public string YandexTranslatorApiKey { get; set; }

        [Category(CategoryYandexTranslator)]
        [DisplayName("API Key Help")]
        [ReadOnly(true)]
        [Description("For info how to obtain API key, read instruction on provider URL.")]
        public string YandexTranslatorApiKeyHelp { get; set; }

        public PluginConfig()
        {
            YandexTranslatorApiKey = string.Empty;
            YandexTranslatorApiKeyHelp = HelpUrl;
        }
    }
}
