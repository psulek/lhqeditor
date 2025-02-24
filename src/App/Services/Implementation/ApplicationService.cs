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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using LHQ.App.Code;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Dialogs.AppSettings;
using LHQ.Core.DependencyInjection;

namespace LHQ.App.Services.Implementation
{
    public class ApplicationService : AppContextServiceBase, IApplicationService
    {
        private bool _disposed;

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        public virtual async Task Initialize()
        {
            // load and init plugins
            AppContext.PluginManager.Initialize();

            // when app started for 1st time, mark flag to false
            if (AppContext.AppStartedFirstTime)
            {
                AppContext.AppStateStorageService.Update(storage => { storage.AppStartedFirstTime = false; });
            }

            // notify that application was initialized
            AppContext.FireAppEvent(new ApplicationEventAppInitialized(AppContext));

            await CheckForUpdates();
        }

        private async Task CheckForUpdates()
        {
            UpdateCheckInfo checkResult = await AppContext.UpdateService.CheckForUpdate();
            if (checkResult != null)
            {
                AppContext.SetUpdateInfo(checkResult);
            }
            else
            {
                Logger.Info("Update check for new version resulted in: NULL");
            }
        }

        public void ShowAppSettings(AppSettingsDialogPage activePage = AppSettingsDialogPage.General)
        {
            var dialogResult = DialogService.ShowAppSettings(activePage);
            if (dialogResult.Submitted)
            {
                AppVisualTheme oldTheme = VisualManager.Instance.Theme;

                AppConfig newAppConfig = dialogResult.AppConfig;
                AppContext.AppConfigFactory.Set(newAppConfig);

                AppVisualTheme newTheme = newAppConfig.Theme;
                bool themeChanged = newTheme != VisualManager.Instance.Theme;

                if (UIService.ChangeUiLanguage(AppConfig.UILanguage))
                {
                    SaveAppConfig();
                }
                else
                {
                    AppContext.FireAppEvent(new ApplicationEventOptionsChanged(AppContext));
                }

                if (themeChanged)
                {
                    SetTheme(newTheme, oldTheme);
                }
            }
        }

        public void SetTheme(AppVisualTheme newTheme, AppVisualTheme oldTheme)
        {
            VisualManager.Instance.SetTheme(newTheme);

            AppContext.FireAppEvent(new ApplicationEventThemeChanged(AppContext, oldTheme, newTheme));
        }

        public virtual Dictionary<string, string> GetEnvironmentInfo()
        {
            return new Dictionary<string, string>
            {
                { "OS", Environment.OSVersion.ToString() }
            };
        }
        
        public virtual void Exit(bool restartApp)
        {
            if (restartApp)
            {
                var startInfo = new ProcessStartInfo
                {
                    Arguments = "/C choice /C Y /N /D Y /T 0 & START \"\" \"" + Assembly.GetExecutingAssembly().Location + "\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = "cmd.exe"
                };
                Process.Start(startInfo);
            }

            Application current = Application.Current;
            if (current != null)
            {
                current.Shutdown(0);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        public virtual AppVisualTheme GetInitialTheme()
        {
            return AppConfig.Theme;
        }

        public void SaveAppConfig()
        {
            AppContext.AppConfigFactory.Save();
            AppContext.FireAppEvent(new ApplicationEventOptionsChanged(AppContext));
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    DateTimeHelper.Stop();
                }
                finally
                {
                    _disposed = true;
                }
            }
        }
    }
}
