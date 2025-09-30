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
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.App.ViewModels;
using LHQ.Core.DependencyInjection;
using LHQ.Data.Interfaces.Key;
using LHQ.Utils.Extensions;
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.App.Services.Implementation
{
    public sealed class ShellViewContext : ServiceBase, IShellViewContext
    {
        private ShellViewContext(IAppContext appContext)
        {
            AppContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
        }

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        {
            ServiceContainer = serviceContainer ?? throw new ArgumentNullException(nameof(serviceContainer));
            if (AppContext.ServiceContainer == serviceContainer)
            {
                throw new InvalidOperationException("shellServiceContainer must be different to AppServiceContainer!");
            }

            ShellService = ServiceContainer.Get<IShellService>();
            Messenger = ServiceContainer.Get<IMessenger>();
            ShellViewCache = ServiceContainer.Get<IShellViewCache>();
            ClipboardManager = ServiceContainer.Get<ITreeElementClipboardManager>();
            TreeViewService = ServiceContainer.Get<ITreeViewService>();
            PropertyChangeObserver = ServiceContainer.Get<IPropertyChangeObserver>();
            ValidatorContext = ServiceContainer.Get<IValidatorContext>();
            UndoManager = ServiceContainer.Get<IUndoManager>();
            TransactionManager = ServiceContainer.Get<ITransactionManager>();
            ResourceImportExportService = ServiceContainer.Get<IResourceImportExportService>();
        }

        public IAppContext AppContext { get; }

        public IShellService ShellService { get; private set; }

        public IServiceContainer ServiceContainer { get; private set; }

        public IMessenger Messenger { get; private set; }

        public ShellViewModel ShellViewModel { get; set; }

        public IShellViewCache ShellViewCache { get; private set; }

        public ITreeElementClipboardManager ClipboardManager { get; private set; }

        public ITreeViewService TreeViewService { get; private set; }

        public IPropertyChangeObserver PropertyChangeObserver { get; private set; }

        public IValidatorContext ValidatorContext { get; private set; }

        public ITransactionManager TransactionManager { get; private set; }

        public IUndoManager UndoManager { get; private set; }

        public IResourceImportExportService ResourceImportExportService { get; private set; }

        public IModelElementKeyProvider ModelElementKeyProvider => ShellViewModel.ModelContext.KeyProvider;

        public static ShellViewContext Initialize(IAppContext appContext, IServicesRegistrator servicesRegistrator, bool runInVsPackage)
        {
            var shellViewContext = new ShellViewContext(appContext);

            void OnServiceCreated(IService service)
            {
                if (service is IShellViewContextService shellViewContextService)
                {
                    // ReSharper disable once AccessToModifiedClosure
                    shellViewContextService.AppContext = appContext;
                    shellViewContextService.ShellViewContext = shellViewContext;
                }
            }

            ServiceContainer serviceContainer = Implementation.ServiceContainer.Create(OnServiceCreated);
            servicesRegistrator.RegisterServices(serviceContainer, ServicesRegistratorType.ShellViewContextService);

            serviceContainer.Load(obj =>
                {
                    var hasInitialize = obj as IHasInitialize;
                    hasInitialize?.Initialize();
                });

            shellViewContext.ConfigureDependencies(serviceContainer);

            return shellViewContext;
        }
    }
}