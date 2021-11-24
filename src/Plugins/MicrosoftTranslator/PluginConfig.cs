// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
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
