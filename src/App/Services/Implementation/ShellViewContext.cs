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
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.App.Services.Implementation
{
    public sealed class ShellViewContext : ServiceBase, IShellViewContext
    {
        private ShellViewContext(IAppContext appContext)
        {
            AppContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
        }

        public IAppContext AppContext { get; }

        private IShellService _shellService;
        public IShellService ShellService => _shellService ?? (_shellService = Container.Get<IShellService>());

        private IMessenger _messenger;
        public IMessenger Messenger => _messenger ?? (_messenger = Container.Get<IMessenger>());

        public ShellViewModel ShellViewModel { get; set; }

        private IShellViewCache _shellViewCache;
        public IShellViewCache ShellViewCache => _shellViewCache ?? (_shellViewCache = Container.Get<IShellViewCache>());

        private ITreeElementClipboardManager _clipboardManager;
        public ITreeElementClipboardManager ClipboardManager => _clipboardManager ?? (_clipboardManager = Container.Get<ITreeElementClipboardManager>());

        private ITreeViewService _treeViewService;
        public ITreeViewService TreeViewService => _treeViewService ?? (_treeViewService = Container.Get<ITreeViewService>());

        private IPropertyChangeObserver _propertyChangeObserver;
        public IPropertyChangeObserver PropertyChangeObserver => _propertyChangeObserver ?? (_propertyChangeObserver = Container.Get<IPropertyChangeObserver>());

        private IValidatorContext _validatorContext;
        public IValidatorContext ValidatorContext => _validatorContext ?? (_validatorContext = Container.Get<IValidatorContext>());

        private ITransactionManager _transactionManager;
        public ITransactionManager TransactionManager => _transactionManager ?? (_transactionManager = Container.Get<ITransactionManager>());

        private IUndoManager _undoManager;
        public IUndoManager UndoManager => _undoManager ?? (_undoManager = Container.Get<IUndoManager>());

        private IResourceImportExportService _resourceImportExportService;
        public IResourceImportExportService ResourceImportExportService =>
            _resourceImportExportService ?? (_resourceImportExportService = Container.Get<IResourceImportExportService>());

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

            ServiceContainer serviceContainer = ServiceContainer.Create(OnServiceCreated);
            servicesRegistrator.RegisterServices(serviceContainer, ServicesRegistratorType.ShellViewContextService);

            serviceContainer.Load(obj =>
                {
                    var hasInitialize = obj as IHasInitialize;
                    hasInitialize?.Initialize();
                });

            ((IService)shellViewContext).SetContainer(serviceContainer);

            return shellViewContext;
        }
    }
}