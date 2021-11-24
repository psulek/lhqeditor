// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System.Reflection;
using System.Runtime.InteropServices;
using LHQ.Core.Attributes;
using LHQ.Plugin.ExportImportPack;

[assembly: AssemblyTitle("LHQ default exporters and importers pack plugin")]
[assembly: AssemblyProduct("ExportImportPack")]
[assembly: AssemblyCopyright("Copyright © ScaleHQ Solutions 2020")]

[assembly: ComVisible(false)]
[assembly: Guid("1a43af51-9d19-40ea-a87a-5b36e24e2e69")]

[assembly: PluginModuleRegistration(typeof(PluginModule))]