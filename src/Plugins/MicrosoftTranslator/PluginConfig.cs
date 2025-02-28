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

using System.ComponentModel;
using LHQ.Core.Interfaces.PluginSystem;

namespace LHQ.Plugin.MicrosoftTranslator
{
    public class PluginConfig: IPluginConfig
    {
        private const string CategoryMicrosoftTranslator = "Microsoft Translator";
        private const string HelpUrl = "https://docs.microsoft.com/en-us/azure/cognitive-services/translator/translator-text-how-to-signup";

        [Category(CategoryMicrosoftTranslator)]
        [DisplayName("Subscription Key")]
        [Description("Subscription key to Microsoft Translator, using Azure Cognitive Services.")]
        public string MicrosoftTranslatorSubscriptionKey { get; set; }

        [Category(CategoryMicrosoftTranslator)]
        [DisplayName("Subscription Key Help")]
        [ReadOnly(true)]
        [Description("For info how to obtain key, read instruction on provider URL.")]
        public string MicrosoftTranslatorSubscriptionKeyHelp { get; set; }

        public PluginConfig()
        {
            MicrosoftTranslatorSubscriptionKey = string.Empty;
            MicrosoftTranslatorSubscriptionKeyHelp = HelpUrl;
        }
    }
}
