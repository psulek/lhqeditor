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
using System.IO;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.PluginSystem;
using LHQ.Core.DependencyInjection;
using LHQ.Core.Interfaces;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Utils.Extensions;

namespace LHQ.App.Services.Implementation
{
    public sealed class AppContext : ServiceBase, IAppContext
    {
        public bool AppStartedFirstTime { get; private set; }

        public UpdateCheckInfo UpdateInfo { get; private set; }

        public ModelFileFromCmdLine ModelFileFromCmdLine { get; private set; }

        public IServicesRegistrator ServicesRegistrator { get; private set; }
        
        private IUIService _uiService;
        public IUIService UIService => _uiService ?? (_uiService = Container.Get<IUIService>());

        private ILogger _logger;
        public ILogger Logger => _logger ?? (_logger = Container.Get<ILogger>());

        private IUtilityHelper _utilityHelper;
        public IUtilityHelper UtilityHelper => _utilityHelper ?? (_utilityHelper = Container.Get<IUtilityHelper>());

        private IFileSystemService _fileSystemService;
        public IFileSystemService FileSystemService => _fileSystemService ?? (_fileSystemService = Container.Get<IFileSystemService>());

        private IUpdateService _updateService;
        public IUpdateService UpdateService => _updateService ?? (_updateService = Container.Get<IUpdateService>());

        private IPluginManager _pluginManager;
        public IPluginManager PluginManager => _pluginManager ?? (_pluginManager = Container.Get<IPluginManager>());

        private IAppConfigStorage _appConfigStorage;
        public IAppConfigStorage AppConfigStorage => _appConfigStorage ?? (_appConfigStorage = Container.Get<IAppConfigStorage>());

        private IAppConfigFactory _appConfigFactory;
        public IAppConfigFactory AppConfigFactory => _appConfigFactory ?? (_appConfigFactory = Container.Get<IAppConfigFactory>());

        private IModelFileStorage _modelFileStorage;
        public IModelFileStorage ModelFileStorage => _modelFileStorage ?? (_modelFileStorage = Container.Get<IModelFileStorage>());

        private IPluginServiceContainer _pluginServiceContainer;
        public IPluginServiceContainer PluginServiceContainer => _pluginServiceContainer ?? (_pluginServiceContainer = Container.Get<IPluginServiceContainer>());

        private IRecentFilesService _recentFilesService;
        public IRecentFilesService RecentFilesService => _recentFilesService ?? (_recentFilesService = Container.Get<IRecentFilesService>());

        private IAppStateStorageService _appStateStorageService;
        public IAppStateStorageService AppStateStorageService => _appStateStorageService ?? (_appStateStorageService = Container.Get<IAppStateStorageService>());

        private IApplicationService _applicationService;
        public IApplicationService ApplicationService => _applicationService ?? (_applicationService = Container.Get<IApplicationService>());

        private ITranslationService _translationService;
        public ITranslationService TranslationService => _translationService ?? (_translationService = Container.Get<ITranslationService>());

        private IStandaloneCodeGeneratorService _standaloneCodeGeneratorService;
        public IStandaloneCodeGeneratorService StandaloneCodeGeneratorService => _standaloneCodeGeneratorService ?? (_standaloneCodeGeneratorService = Container.Get<IStandaloneCodeGeneratorService>());

        private IDialogService _dialogService;
        public IDialogService DialogService => _dialogService ?? (_dialogService = Container.Get<IDialogService>());

        public bool RunInVsPackage { get; private set; }

        public string AppFolder { get; private set; }
        
        public string[] CmdArgs { get; private set; }

        public string DataFolder { get; private set; }

        public string AppFileName { get; private set; }

        public Version AppCurrentVersion { get; private set; }

        public event EventHandler<ApplicationEventArgs> OnAppEvent;

        public static AppContext Initialize(IServicesRegistrator servicesRegistrator, bool runInVsPackage,
            string appFileName, string[] cmdArgs = null, string extensionPath = null)
        {
            string dataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            dataFolder = Path.Combine(dataFolder, "ScaleHQSolutions\\LHQ\\");
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            var modelFileFromCmdLine = new ModelFileFromCmdLine();
            if (cmdArgs?.Length > 1)
            {
                if (cmdArgs[0].ToLower() == "-m" && !cmdArgs[1].IsNullOrEmpty())
                {
                    modelFileFromCmdLine = new ModelFileFromCmdLine(cmdArgs[1]);
                }
            }
            
            var appCurrentVersion = typeof(AppContext).Assembly.GetName().Version;
            var appContext = new AppContext
            {
                RunInVsPackage = runInVsPackage,
                AppFileName = appFileName,
                CmdArgs = cmdArgs,
                AppFolder = Path.GetDirectoryName(appFileName),
                DataFolder = dataFolder,
                ServicesRegistrator = servicesRegistrator,
                AppCurrentVersion = appCurrentVersion,
                ModelFileFromCmdLine = modelFileFromCmdLine
            };

            void OnServiceCreated(IService service)
            {
                if (service is IAppContextService appContextService)
                {
                    // ReSharper disable once AccessToModifiedClosure
                    appContextService.AppContext = appContext;
                }
            }

            ServiceContainer serviceContainer = ServiceContainer.Create(OnServiceCreated);
            servicesRegistrator.RegisterServices(serviceContainer, ServicesRegistratorType.AppContextService);

            // eager initialization of ILogger before any other service!
            ILogger logger = serviceContainer.LoadByType<ILogger>();
            string appFolder = appContext.AppFolder;
            string logsFolder = Path.Combine(appFolder, "logs");
            logger.Initialize(appFolder, logsFolder);
            
            ((IService)appContext).SetContainer(serviceContainer);
            serviceContainer.Load(obj =>
                {
                    obj.SetContainer(serviceContainer);
                    var hasInitialize = obj as IHasInitialize;
                    hasInitialize?.Initialize();
                });

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
