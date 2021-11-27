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
