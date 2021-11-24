// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System.Reflection;
using System.Runtime.InteropServices;
using LHQ.Core.Attributes;
using LHQ.Plugin.YandexTranslator;

[assembly: AssemblyTitle("LHQ Yandex Translator Plugin")]
[assembly: AssemblyProduct("LHQ Yandex Translator Plugin")]

[assembly: ComVisible(false)]
[assembly: Guid("920eb8f4-19f7-4c0a-b6aa-0aaddf3c35a0")]

[assembly: PluginModuleRegistration(typeof(PluginModule))]