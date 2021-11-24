// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System.Collections.Generic;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Core.UI;
using LHQ.Plugin.ExportImportPack.Exporters;
using LHQ.Plugin.ExportImportPack.Importers;

namespace LHQ.Plugin.ExportImportPack
{
    public class PluginModule : IPluginModule
    {
        public string Key { get; } = PluginConsts.PluginModuleKey;

        public string DisplayName { get; } = "Default Exporters/Importers Pack";

        public string Version { get; } = "1.0";

        public string Description { get; } = "Default exporters and importers pack (Excel, etc..)";

        public List<BulletListTextItem> HelpItems => GetHelpItems();

        public bool IsConfigurable { get; } = false;

        public IPluginConfig PluginConfig { get; set; }

        public IPluginConfig CreateDefaultConfig()
        {
            return new PluginConfig();
        }

        private List<BulletListTextItem> GetHelpItems()
        {
            var help = new List<BulletListTextItem>
            {
                BulletListTextItem.Item("Default Exporters/Importers Pack includes:", true),
                BulletListTextItem.Separator(),
                BulletListTextItem.BulletItem("Exports resources to Microsoft Excel (xlsx) file"),
                BulletListTextItem.Separator(),
                BulletListTextItem.BulletItem("Imports resources from Microsoft Excel (xlsx) file")
            };

            return help;
        }

        public void Initialize(IPluginServiceContainer serviceContainer)
        {
            serviceContainer.Register<ExcelExporter>(this);
            serviceContainer.Register<ExcelImporter>(this);
        }
    }
}
