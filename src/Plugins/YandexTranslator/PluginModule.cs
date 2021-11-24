// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System.Collections.Generic;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Core.UI;

namespace LHQ.Plugin.YandexTranslator
{
    public class PluginModule : IPluginModule
    {
        private const string HowToSignUpUrl = "https://translate.yandex.com/developers/keys";

        public const string PluginModuleKey = "LHQ.YandexTranslator";

        public string Key { get; } = PluginModuleKey;

        public string DisplayName { get; } = "Yandex Translations";

        public string Version { get; } = "1.0";

        public string Description { get; } = "Yandex Translator.";

        public List<BulletListTextItem> HelpItems => GetHelpItems();

        public bool IsConfigurable { get; } = true;

        public IPluginConfig PluginConfig { get; set; }

        public IPluginConfig CreateDefaultConfig()
        {
            return new PluginConfig();
        }

        private List<BulletListTextItem> GetHelpItems()
        {
            var help = new List<BulletListTextItem>
            {
                BulletListTextItem.Item("Yandex Translator requires:", true),
                BulletListTextItem.BulletItem("Free API Key"),
                BulletListTextItem.Separator(),
                BulletListTextItem.Item("Guide can be found on official Yandex page: ", true),
                BulletListTextItem.Hyperlink("Request Free API Key", HowToSignUpUrl, true),
                BulletListTextItem.Separator(),
                BulletListTextItem.Item("Official Yandex Translate page:"),
                BulletListTextItem.Hyperlink("Overview ", "https://tech.yandex.com/translate/doc/dg/concepts/api-overview-docpage/")
            };

            return help;
        }

        public void Initialize(IPluginServiceContainer serviceContainer)
        {
            serviceContainer.Register<TranslationProvider>(this);
        }
    }
}
