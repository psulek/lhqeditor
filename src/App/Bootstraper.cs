// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System.Threading.Tasks;
using System.Windows;
using LHQ.App.Code;
using LHQ.App.Model;
using LHQ.App.Services.Implementation;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels;
using LHQ.Data;
using AppContext = LHQ.App.Services.Implementation.AppContext;

namespace LHQ.App
{
    public sealed class Bootstraper
    {
        private readonly string _appFileName;
        private AppView _appView;

        public Bootstraper(string appFileName)
        {
            _appFileName = appFileName;
        }

        public static async Task<AppView> CreateAppView(string appFileName)
        {
            var servicesRegistrator = new DefaultServicesRegistrator();
            AppContext appContext = CreateAppContext(appFileName, servicesRegistrator, false);
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

        public static AppContext CreateAppContext(string appFileName, IServicesRegistrator servicesRegistrator, bool runInVsPackage)
        {
            return AppContext.Initialize(servicesRegistrator, runInVsPackage, appFileName);
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
            _appView = await CreateAppView(_appFileName);

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
