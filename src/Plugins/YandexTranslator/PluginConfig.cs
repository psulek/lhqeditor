// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
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
