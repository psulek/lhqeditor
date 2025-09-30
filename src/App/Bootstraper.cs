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

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using LHQ.App.Code;
using LHQ.App.Model;
using LHQ.App.Services.Implementation;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels;
using LHQ.Data;
using LHQ.Utils.Extensions;
using AppContext = LHQ.App.Services.Implementation.AppContext;

namespace LHQ.App
{
    public sealed class Bootstraper
    {
        private readonly string _appFileName;
        private readonly string[] _cmdArgs;
        private AppView _appView;

        public Bootstraper(string appFileName, string[] cmdArgs)
        {
            _appFileName = appFileName;
            _cmdArgs = cmdArgs;
            
            var list = new List<string>(new [] {"AWP"});
            int idx = list.GetIndexForAddSorted("Appllication");
            int compare = string.Compare(list[0], "Application");
            DebugUtils.Log(idx.ToString());
        }

        public static async Task<AppView> CreateAppView(string appFileName, string[] cmdArgs)
        {
            var servicesRegistrator = new DefaultServicesRegistrator();
            AppContext appContext = CreateAppContext(appFileName, servicesRegistrator, false, cmdArgs);
            ShellViewContext shellViewContext = CreateShellViewContext(appContext);

            var appView = new AppView();
            ModelContext startupModel = CreateEmptyModel(appContext);
            bool showMainMenu = !appContext.RunInVsPackage;
            var shellViewModel = new ShellViewModel(shellViewContext, startupModel, showMainMenu);

            appView.DataContext = shellViewModel;
            appView.Closing += (sender, args) => shellViewModel.OnClosing(args);
            appView.Closed += (sender, args) => shellViewModel.OnClosed();

            shellViewModel.MainWindow = appView;

            await appContext.ApplicationService.Initialize();

            shellViewModel.ShellView = appView.ShellView;

            return appView;
        }

        public static ShellView CreateShellView(IAppContext appContext, IServicesRegistrator servicesRegistrator)
        {
            var shellViewContext = CreateShellViewContext(appContext);
            var shellView = new ShellView();

            ModelContext startupModel = CreateEmptyModel(appContext);
            bool showMainMenu = !appContext.RunInVsPackage;
            var shellViewModel = new ShellViewModel(shellViewContext, startupModel, showMainMenu);
            shellView.DataContext = shellViewModel;
            shellViewModel.ShellView = shellView;
            return shellView;
        }

        public static AppContext CreateAppContext(string appFileName, IServicesRegistrator servicesRegistrator, bool runInVsPackage,
            string[] cmdArgs)
        {
            return AppContext.Initialize(servicesRegistrator, runInVsPackage, appFileName, cmdArgs);
        }

        public static ShellViewContext CreateShellViewContext(IAppContext appContext)
        {
            ShellViewContext shellViewContext = ShellViewContext.Initialize(appContext, appContext.ServicesRegistrator, appContext.RunInVsPackage);
            AppConfig appConfig = appContext.ApplicationService.AppConfig;
            appContext.UIService.ChangeUiLanguage(appConfig.UILanguage);

            AppVisualTheme initialTheme = appContext.ApplicationService.GetInitialTheme();

            VisualManager.Instance.SetTheme(initialTheme);
            return shellViewContext;
        }

        public async Task Run()
        {
            _appView = await CreateAppView(_appFileName, _cmdArgs);

#if DEBUG
            /*var resetAppStorage = 0;
            if (resetAppStorage == 1)
            {
                new AppStateStorageService().Reset();
                new LicenseStateStorageService().Reset();
                MessageBox.Show("App Storage was reseted!");
                return;
            }*/
#endif

            Application.Current.MainWindow = _appView;
            Application.Current.MainWindow.Show();
        }

        private static ModelContext CreateEmptyModel(IAppContext appContext)
        {
            AppConfig appConfig = appContext.AppConfigFactory.Current;

            var startupModel = new ModelContext(ModelContextOptions.Default);
            startupModel.RecreateModel("Default");
            startupModel.AddLanguage(appConfig.DefaultProjectLanguage, true);
            return startupModel;
        }
    }
}
