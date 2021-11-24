// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System;
using System.IO;
using LHQ.App.Code;
using LHQ.App.Model;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App
{
    public static class AppTools
    {
        public static void GenerateAppConfig(string folder, bool targetIsVs)
        {
            try
            {
                string version = typeof(AppTools).Assembly.GetName().Version.ToString();
                var appConfig = new AppConfig
                {
                    Version = version,
                    UILanguage = CultureCache.English.Name,
                    AppHints = AppHintType.All,
                    DefaultProjectLanguage = CultureCache.English.Name,
                    EnableTranslation = false,
                    OpenLastProjectOnStartup = targetIsVs == false,
                    CheckUpdatesOnAppStart = targetIsVs == false,
                    LockTranslationsWithResource = null,
                };

                var configStorage = new AppConfigStorageV1000();
                string targetFileName = Path.Combine(folder, configStorage.GetFileName());
                configStorage.SaveToFile(appConfig, targetFileName);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error generating app config file to output folder '{folder}', error: " + e.GetFullExceptionDetails());
            }
        }
    }
}
