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
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using LHQ.App.Code;
using LHQ.VsExtension.Code;
using LHQ.VsExtension.Commands;
using LHQ.VsExtension.Options;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ScaleHQ.WPF.LHQ;
using Task = System.Threading.Tasks.Task;

namespace LHQ.VsExtension
{
    [Guid(PackageGuids.guidPackageString)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    [InstalledProductRegistration("#110", "#112", VsExtensionConstants.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideEditorExtension(typeof(EditorFactory), ".lhq", 1000, ProjectGuid = "{A2FE74E1-B743-11d0-AE1A-00A0C90FFFC3}",
        TemplateDir = "Templates", NameResourceID = 110, DefaultName = "LocalizationHQ")]
    [ProvideEditorLogicalView(typeof(EditorFactory), VSConstants.LOGVIEWID.TextView_string, IsTrusted = true)]
    [ProvideBindingPath]
    [ProvideOptionPage(typeof(VsOptions), "Localization HQ", "General", 0, 0, true)]
    public sealed class VsPackage : AsyncPackage
    {
        private static VsOptions _options;
        private static readonly object _syncRoot = new object();

        public static VsOptions Options
        {
            get
            {
                if (_options == null)
                {
                    lock (_syncRoot)
                    {
                        if (_options == null)
                        {
                            EnsurePackageLoaded();
                        }
                    }
                }

                return _options;
            }
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            DateTimeHelper.Initialize();
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            LocalizationExtension.Initialize(typeof(LocalizationContextFactory));
            _options = (VsOptions)GetDialogPage(typeof(VsOptions));

            var editorFactory = new EditorFactory(this);
            RegisterEditorFactory(editorFactory);

            await VsPackageService.InitializeAsync(this, cancellationToken);

            if (await GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                CommandsManager.Instance.Initialize(this, commandService);
            }

            Version appCurrentVersion = VsPackageService.AppContext?.AppCurrentVersion;
            string appVersion = appCurrentVersion == null ? VsPackageService.ExtensionVersion : appCurrentVersion.ToString();

            VsPackageService.AddMessageToOutput($"Localization HQ Editor (version {appVersion}) was successfully initialized.\n",
                OutputMessageType.Info, false);
        }

        protected override void Dispose(bool disposing)
        {
            VsPackageService.Unload();
            DateTimeHelper.Stop();
            base.Dispose(disposing);
        }

        private static void EnsurePackageLoaded()
        {
            var shell = (IVsShell)GetGlobalService(typeof(SVsShell));

            if (shell.IsPackageLoaded(ref PackageGuids.guidPackage, out var package) != VSConstants.S_OK)
            {
                ErrorHandler.Succeeded(shell.LoadPackage(ref PackageGuids.guidPackage, out package));
            }
        }
    }
}
