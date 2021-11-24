// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System.Collections.Generic;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Core.UI;

namespace LHQ.Plugin.MicrosoftTranslator
{
    public class PluginModule : IPluginModule
    {
        private const string HowToSignUpUrl = "https://docs.microsoft.com/en-us/azure/cognitive-services/translator/translator-text-how-to-signup";

        public const string PluginModuleKey = "LHQ.MicrosoftTranslator";

        public string Key { get; } = PluginModuleKey;

        public string DisplayName { get; } = "Microsoft Translator";

        public string Version { get; } = "1.0";

        public string Description { get; } = "Microsoft Translator using Azure Cognitive Services.";

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
                BulletListTextItem.Item("Microsoft Translator requires:", true),
                BulletListTextItem.BulletItem("Azure Account (Free account can be used)"),
                BulletListTextItem.BulletItem("Personalized unique subscription key"),
                BulletListTextItem.Separator(),
                BulletListTextItem.Item("Guide can be found on official Microsoft page: ", true),
                BulletListTextItem.Hyperlink("How to sign up for the Translator Text API", HowToSignUpUrl, true),
            };

            return help;
        }

        public void Initialize(IPluginServiceContainer serviceContainer)
        {
            serviceContainer.Register<TranslationProvider>(this);
        }
    }
}
