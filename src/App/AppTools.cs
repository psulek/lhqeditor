#region License
// Copyright (c) 2025 Peter Šulek / ScaleHQ Solutions s.r.o.
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

using System;
using System.Diagnostics;
using System.IO;
using LHQ.App.Code;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using Velopack.Locators;
using Velopack.Windows;

namespace LHQ.App
{
    public static class AppTools
    {
        public static void CreateDesktopShortcut(IAppContext appContext)
        {
            try
            {
                // Only create shortcut if StartMeUp is installed (from software center)
                if (VelopackLocator.Current.CurrentlyInstalledVersion == null)
                {
                    return;
                }
                
                var processModule = Process.GetCurrentProcess().MainModule;
                var appExeFileName = Path.GetFileName(processModule.FileName);
                string appContextAppFileName = appContext.AppFileName;

                var shortcutsApi = new Shortcuts();
                var exePath = appExeFileName;
                var shortcuts = shortcutsApi.FindShortcuts(exePath, ShortcutLocation.Desktop);
        
                appContext.Logger.Info($"[DesktopShortcut] Found {shortcuts.Count} shortcuts to {exePath}.");
                foreach (var shortcut in shortcuts)
                {
                    appContext.Logger.Info("[DesktopShortcut] Found shortcut at: " + shortcut.Key + " -> " + shortcut.Value.Target);
                }
        
                if (shortcuts.Count == 0)
                {
                    appContext.Logger.Info($"[DesktopShortcut] Creating desktop shortcut for: {exePath}");
                    shortcutsApi.CreateShortcutForThisExe(ShortcutLocation.Desktop);
                }
            }
            catch (Exception e)
            {
                appContext.Logger.Error($"[DesktopShortcut] Error creating desktop shortcut: {e.Message}", e);
            }
        }
        
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
                    RunTemplateAfterSave = targetIsVs,
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
