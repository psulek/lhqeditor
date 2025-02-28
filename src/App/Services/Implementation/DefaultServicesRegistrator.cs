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
using LHQ.App.Code;
using LHQ.App.Model;
using LHQ.App.Services.Implementation.Messaging;
using LHQ.App.Services.Implementation.PluginSystem;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.PluginSystem;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.Core.Interfaces;
using LHQ.Core.Interfaces.PluginSystem;

namespace LHQ.App.Services.Implementation
{
    public class DefaultServicesRegistrator : IServicesRegistrator
    {
        public virtual void RegisterServices(ServiceContainer serviceContainer, ServicesRegistratorType registratorType)
        {
            switch (registratorType)
            {
                case ServicesRegistratorType.AppContextService:
                {
                    serviceContainer.Register<IUIService, UIService>();
                    serviceContainer.Register<IDialogService, DialogService>();
                    serviceContainer.Register<IRecentFilesService, RecentFilesService>();
                    serviceContainer.Register<IAppStateStorageService, AppStateStorageService>();
                    serviceContainer.Register<IApplicationService, ApplicationService>();
                    serviceContainer.Register<IAppConfigStorage, AppConfigStorageV1000>();
                    serviceContainer.Register<IAppConfigFactory, AppConfigFactory>();
                    serviceContainer.Register<ITranslationService, TranslationService>();
                    serviceContainer.Register<IPluginManager, PluginManager>();
                    serviceContainer.Register<IPluginServiceContainer, PluginServiceContainer>();
                    serviceContainer.Register<ILogger, DefaultLogger>();
                    serviceContainer.Register<IUtilityHelper, UtilityHelper>();
                    serviceContainer.Register<IFileSystemService, FileSystemService>();
                    serviceContainer.Register<IUpdateService, UpdateService>();
                    serviceContainer.Register<IModelFileStorage, ModelFileStorage>();
                    serviceContainer.Register<IStandaloneCodeGeneratorService, StandaloneCodeGeneratorService>();
                    break;
                }
                case ServicesRegistratorType.ShellViewContextService:
                {
                    serviceContainer.Register<IMessenger, Messenger>();
                    serviceContainer.Register<IUndoManager, TransactionUndoManager>();
                    serviceContainer.Register<ITransactionManager, TransactionManager>();
                    serviceContainer.Register<IShellViewCache, ShellViewCache>();
                    serviceContainer.Register<ITreeElementClipboardManager, TreeElementClipboardManager>();
                    serviceContainer.Register<ITreeViewService, TreeViewService>();
                    serviceContainer.Register<IPropertyChangeObserver, PropertyChangeObserver>();
                    serviceContainer.Register<IValidatorContext, ValidatorContext>();
                    serviceContainer.Register<IShellService, ShellService>();
                    serviceContainer.Register<IResourceImportExportService, ResourceImportService>();
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(registratorType), registratorType, null);
                }
            }
        }
    }
}
