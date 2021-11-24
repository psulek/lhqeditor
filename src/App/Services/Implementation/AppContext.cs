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
using System.IO;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.PluginSystem;
using LHQ.Core.DependencyInjection;
using LHQ.Core.Interfaces;
using LHQ.Core.Interfaces.PluginSystem;

namespace LHQ.App.Services.Implementation
{
    public sealed class AppContext : ServiceBase, IAppContext
    {
        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        {
            ServiceContainer = serviceContainer ?? throw new ArgumentNullException(nameof(serviceContainer));

            Logger = ServiceContainer.Get<ILogger>();
            UtilityHelper = ServiceContainer.Get<IUtilityHelper>();
            UIService = ServiceContainer.Get<IUIService>();
            FileSystemService = ServiceContainer.Get<IFileSystemService>();
            PluginManager = ServiceContainer.Get<IPluginManager>();
            AppConfigStorage = ServiceContainer.Get<IAppConfigStorage>();
            AppConfigFactory = ServiceContainer.Get<IAppConfigFactory>();
            ModelFileStorage = ServiceContainer.Get<IModelFileStorage>();
            PluginServiceContainer = ServiceContainer.Get<IPluginServiceContainer>();
            RecentFilesService = ServiceContainer.Get<IRecentFilesService>();
            AppStateStorageService = ServiceContainer.Get<IAppStateStorageService>();
            ApplicationService = ServiceContainer.Get<IApplicationService>();
            TranslationService = ServiceContainer.Get<ITranslationService>();
            DialogService = ServiceContainer.Get<IDialogService>();
            UpdateService = ServiceContainer.Get<IUpdateService>();
        }

        public bool AppStartedFirstTime { get; private set; }

        public UpdateCheckInfo UpdateInfo { get; private set; }

        public IServicesRegistrator ServicesRegistrator { get; private set; }

        public IServiceContainer ServiceContainer { get; private set; }

        public IUIService UIService { get; private set; }

        public ILogger Logger { get; private set; }

        public IUtilityHelper UtilityHelper { get; private set; }

        public IFileSystemService FileSystemService { get; private set; }

        public IUpdateService UpdateService { get; private set; }

        public IPluginManager PluginManager { get; private set; }

        public IAppConfigStorage AppConfigStorage { get; private set; }

        public IAppConfigFactory AppConfigFactory { get; private set; }

        public IModelFileStorage ModelFileStorage { get; private set; }

        public IPluginServiceContainer PluginServiceContainer { get; private set; }

        public IRecentFilesService RecentFilesService { get; private set; }

        public IAppStateStorageService AppStateStorageService { get; private set; }

        public IApplicationService ApplicationService { get; private set; }

        public ITranslationService TranslationService { get; private set; }

        public IDialogService DialogService { get; private set; }

        public bool RunInVsPackage { get; private set; }

        public string AppFolder { get; private set; }

        public string DataFolder { get; private set; }

        public string AppFileName { get; private set; }

        public Version AppCurrentVersion { get; private set; }

        public event EventHandler<ApplicationEventArgs> OnAppEvent;

        public static AppContext Initialize(IServicesRegistrator servicesRegistrator, bool runInVsPackage,
            string appFileName, string extensionPath = null)
        {
            string dataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            dataFolder = Path.Combine(dataFolder, "ScaleHQSolutions\\LHQ\\");
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            var appCurrentVersion = typeof(AppContext).Assembly.GetName().Version;
            var appContext = new AppContext
            {
                RunInVsPackage = runInVsPackage,
                AppFileName = appFileName,
                AppFolder = Path.GetDirectoryName(appFileName),
                DataFolder = dataFolder,
                ServicesRegistrator = servicesRegistrator,
                AppCurrentVersion = appCurrentVersion
            };

            void OnServiceCreated(IService service)
            {
                if (service is IAppContextService appContextService)
                {
                    // ReSharper disable once AccessToModifiedClosure
                    appContextService.AppContext = appContext;
                }
            }

            ServiceContainer serviceContainer = Implementation.ServiceContainer.Create(OnServiceCreated);
            servicesRegistrator.RegisterServices(serviceContainer, ServicesRegistratorType.AppContextService);

            serviceContainer.Load(obj =>
                {
                    var hasInitialize = obj as IHasInitialize;
                    hasInitialize?.Initialize();
                });

            appContext.ConfigureDependencies(serviceContainer);
            appContext.AppStartedFirstTime = appContext.AppStateStorageService.Current.AppStartedFirstTime;

            return appContext;
        }

        public void FireAppEvent(ApplicationEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            try
            {
                OnAppEvent?.Invoke(this, eventArgs);
            }
            catch (Exception e)
            {
                Logger.Error($"{nameof(ApplicationService)}.{nameof(FireAppEvent)} '{eventArgs.GetType().FullName}' failed", e);
            }
        }

        public void UnsetAppStartedFirstTime()
        {
            AppStartedFirstTime = false;
        }

        public void SetUpdateInfo(UpdateCheckInfo info)
        {
            UpdateInfo = info;
            FireAppEvent(new ApplicationEventCheckForUpdateCompleted(this, UpdateInfo));
        }

        public string GetTitleForDialog(string title)
        {
            return RunInVsPackage ? $"{Strings.App.Title} - {title}" : title;
        }
    }
}
