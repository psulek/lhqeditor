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
