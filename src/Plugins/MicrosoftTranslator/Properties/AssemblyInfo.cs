// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System.Reflection;
using System.Runtime.InteropServices;
using LHQ.Core.Attributes;
using LHQ.Plugin.MicrosoftTranslator;

[assembly: AssemblyTitle("LHQ Microsoft Translator Plugin")]
[assembly: AssemblyProduct("LHQ Microsoft Translator Plugin")]

[assembly: ComVisible(false)]
[assembly: Guid("d1e7efca-c3d2-4e79-b6d9-021f9e730464")]

[assembly: PluginModuleRegistration(typeof(PluginModule))]