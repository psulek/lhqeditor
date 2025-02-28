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

using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels;
using LHQ.Core.Interfaces;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.App.Services.Implementation
{
    public abstract class ShellViewContextServiceBase : ServiceBase, IShellViewContextService
    {
        private IAppContext _appContext;

        public IAppContext AppContext
        {
            get => _appContext;
            set
            {
                _appContext = value;
                AppContextUpdated();
            }
        }

        public IShellViewContext ShellViewContext { get; set; }

        public ILogger Logger => AppContext.Logger;

        public AppConfig AppConfig => AppContext.AppConfigFactory.Current;

        public AppStateStorage AppState => AppContext.AppStateStorageService.Current;

        protected ShellViewModel ShellViewModel => ShellViewContext.ShellViewModel;

        protected ITreeViewService TreeViewService => ShellViewContext.TreeViewService;

        protected virtual void AppContextUpdated()
        {}
    }
}