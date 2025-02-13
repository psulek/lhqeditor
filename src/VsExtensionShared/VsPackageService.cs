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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using EnvDTE;
using LHQ.App;
using LHQ.App.Code;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels;
using LHQ.App.ViewModels.Dialogs.AppSettings;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using LHQ.VsExtension.Code;
using LHQ.VsExtension.Options;
using Microsoft.VisualStudio.Shell;
using ScaleHQ.WPF.LHQ;
using AppContext = LHQ.App.Services.Implementation.AppContext;
using Task = System.Threading.Tasks.Task;

namespace LHQ.VsExtension
{
    public static class VsPackageService
    {
        private static AsyncPackage _asyncPackage;
        private const string JsonAssemblyName = "Newtonsoft.Json";
        private static IServicesRegistrator _servicesRegistrator;
        private const string LhqOutputMessagesPaneName = "LHQ Editor Code Generator";

        private static Events _events;
        private static SolutionEvents _solutionEvents;

        private static readonly ThreadSafeSingleShotGuard _autoShowOutputPaneDisabled = ThreadSafeSingleShotGuard.Create(false);

        public static DTE DTE { get; private set; }

        public static async Task InitializeAsync(AsyncPackage asyncPackage, CancellationToken cancellationToken)
        {
            _asyncPackage = asyncPackage;
            DTE = (DTE)await asyncPackage.GetServiceAsync(typeof(DTE));

            VsVersion = Version.Parse(DTE.Version);

            VisualStudioThemeManager.Initialize();
            // TODO: input bindings keys does not work well!
            //VsCommandsManager.Initialize(this);

            _events = DTE.Events;
            _solutionEvents = _events.SolutionEvents;
            _solutionEvents.AfterClosing += SolutionClosed;

            var uri = new Uri(typeof(VsPackageService).Assembly.CodeBase, UriKind.Absolute);
            ExtensionPath = Path.GetDirectoryName(uri.LocalPath);

            AppDomain.CurrentDomain.AssemblyResolve += DomainOnAssemblyResolve;

            string appFullPath = Assembly.GetExecutingAssembly().Location;

            _servicesRegistrator = new VsServicesRegistrator();
            AppContext = AppContext.Initialize(_servicesRegistrator, true, appFullPath, null, ExtensionPath);
            await AppContext.ApplicationService.Initialize();

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        public static bool CanAutoShowOutputPane()
        {
            bool canAutoShowOutputPane = _autoShowOutputPaneDisabled.TrySetSignal();
            return canAutoShowOutputPane;
        }

        private static void SolutionClosed()
        {
            _autoShowOutputPaneDisabled.ResetSignal();
        }

        public static AppContext AppContext { get; set; }

        public static string ExtensionPath { get; private set; }

        public static Version VsVersion { get; private set; }

        public static string ExtensionVersion => VsExtensionConstants.Version;

        public static void ShowExceptionDialog(Exception exception)
        {
            if (exception != null)
            {
                AppContext?.DialogService?.ShowExceptionDialog(exception);
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowExceptionDialog(e.ExceptionObject as Exception);
        }

        public static string GetExtensionPath(string fileOrFolder)
        {
            return Path.Combine(ExtensionPath, fileOrFolder);
        }

        public static ShellView CreateShellView()
        {
            return Bootstraper.CreateShellView(AppContext, _servicesRegistrator);
        }

        private static Assembly DomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains(JsonAssemblyName))
            {
                Assembly latestJsonAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => x.FullName.Contains(JsonAssemblyName))
                    .OrderByDescending(x => x.GetName().Version.Major)
                    .FirstOrDefault();

                return latestJsonAssembly;
            }

            return null;
        }

        public static void ShowOptionsPage(AppSettingsDialogPage activePage, bool appStartedFirstTime)
        {
            VsOptions.InitActivePage = activePage;
            _asyncPackage.ShowOptionPage(typeof(VsOptions));
        }

        public static ShellViewModel GetActiveShellView()
        {
            return DTE.GetActiveShellView();
        }

        public static IEnumerable<ShellViewModel> GetOpenedEditors()
        {
            return DTE.GetOpenedShellViews(out _);
        }

        internal static bool CloseOpenedEditors()
        {
            return CloseOpenedEditors(out _, vsSaveChanges.vsSaveChangesNo);
        }

        internal static bool HasEditorWithUnsavedChanges()
        {
            var shellViews = DTE.Windows.GetOpenedShellViews(out _);
            return shellViews.Any(x => x.ProjectIsDirty);
        }

        internal static Window GetWindowForShellView(ShellViewModel shellViewModel)
        {
            return DTE.Windows.GetWindowForShellView(shellViewModel);
        }

        internal static bool CloseOpenedEditors(out List<ProjectItem> projectItems, vsSaveChanges saveChangesType)
        {
            var windows = DTE.Windows.GetOpenedShellWindows(out projectItems);
            windows.Run(win => win.Close(saveChangesType));
            bool result = windows.Any();

            return result;
        }

        internal static void ReopenEditors(IEnumerable<string> fileNames)
        {
            foreach (string fileName in fileNames)
            {
                DTE.ItemOperations.OpenFile(fileName, Constants.vsViewKindTextView);
            }
        }

        internal static void CloseAndReopenEditors(vsSaveChanges saveChangesType)
        {
            CloseOpenedEditors(out var projectItems, saveChangesType);
            if (projectItems != null)
            {
                var closedEditorFileNames = projectItems.Select(x => x.FileNames[0]).ToList();
                if (closedEditorFileNames.Count > 0)
                {
                    ReopenEditors(closedEditorFileNames);
                }
            }
        }

        public static void Unload()
        {
            _solutionEvents.AfterClosing -= SolutionClosed;
            AppDomain.CurrentDomain.AssemblyResolve -= DomainOnAssemblyResolve;
        }

        public static void ShowIsolatedAppWindow(Action<ShellViewModel> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                LocalizationExtension.Initialize(typeof(LocalizationContextFactory));

                ShellView shellView = CreateShellView();
                using (var shellViewModel = (ShellViewModel)shellView.DataContext)
                {
                    action(shellViewModel);
                }
            }
            catch (Exception ex)
            {
                VsShellUtilities.ShowMessageBox(
                    _asyncPackage,
                    ex.Message,
                    VsExtensionConstants.Name,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGICON.OLEMSGICON_WARNING,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
                );
            }
        }

        public static void ClearOutputMessages()
        {
            Window window = DTE.Windows.Item(Constants.vsWindowKindOutput);
            var outputWindow = (OutputWindow)window.Object;

            OutputWindowPane pane = null;
            foreach (OutputWindowPane item in outputWindow.OutputWindowPanes)
            {
                if (item.Name == LhqOutputMessagesPaneName)
                {
                    pane = item;
                    break;
                }
            }

            if (pane != null)
            {
                pane.Clear();
                pane.OutputString($"Localization HQ Editor (version {ExtensionVersion})\n");
            }
        }

        /// <summary>
        /// Add message into output pane and task list pane
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="messageType">0 - info, 1 - warning, 2 - error</param>
        public static void AddMessageToOutput(string message, OutputMessageType messageType, bool autoShowAllowed = true)
        {
            try
            {
                Window window = DTE.Windows.Item(Constants.vsWindowKindOutput);
                var outputWindow = (OutputWindow)window.Object;

                OutputWindowPane pane = null;
                bool activate = messageType != OutputMessageType.Info;
                foreach (OutputWindowPane item in outputWindow.OutputWindowPanes)
                {
                    if (item.Name == LhqOutputMessagesPaneName)
                    {
                        pane = item;
                        break;
                    }
                }

                if (pane == null)
                {
                    pane = outputWindow.OutputWindowPanes.Add(LhqOutputMessagesPaneName);
                }

                if (activate || (autoShowAllowed && CanAutoShowOutputPane()))
                {
                    window.Activate();
                    pane.Activate();
                }

                string prefix;
                switch (messageType)
                {
                    case OutputMessageType.Info:
                    {
                        prefix = "INFO";
                        break;
                    }
                    case OutputMessageType.Warn:
                    {
                        prefix = "WARN";
                        break;
                    }
                    case OutputMessageType.Error:
                    {
                        prefix = "ERR";
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
                }

                pane.OutputString($"[{prefix}] {message}\n");
            }
            catch (Exception e)
            {
                DebugUtils.Error("[VsPackageService] AddToErrorPane() failed", e);
            }
        }
    }
}
