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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Services.Implementation;
using LHQ.App.ViewModels;
using NLog;
using NLog.Config;
using ScaleHQ.WPF.LHQ;
using Velopack;
using Velopack.Locators;
using Velopack.Logging;
using Velopack.Windows;
using Application = System.Windows.Application;
using ILogger = LHQ.Core.Interfaces.ILogger;

namespace LHQ.App
{
    public class Startup
    {
        private static readonly ILogger _logger = new DefaultLogger();
        
        public static string _appFileName;
        public static string[] _cmdArgs;

        [STAThread]
        public static void Main(string[] args)
        {
            _cmdArgs = args;
            DateTimeHelper.Initialize();

            LocalizationExtension.Initialize(new LocalizationContextFactoryDefault(() => StringsContext.Instance));
            AppDomain.CurrentDomain.UnhandledException += OnAppUnhandledException;

            _appFileName = Assembly.GetExecutingAssembly().Location;
            
            var appFolder = Path.GetDirectoryName(_appFileName);
            _logger.Initialize(appFolder, Path.Combine(appFolder, "logs"));
            
            _logger.Info($"Initializing velopack ... {_appFileName}");

            VelopackApp.Build()
                .OnAfterInstallFastCallback(_ => VelopackService.AfterInstall())
                .OnFirstRun(_ => VelopackService.FirstRun())
                .OnAfterUpdateFastCallback(_ => VelopackService.AfterUpdate())
                .OnBeforeUninstallFastCallback(_ => VelopackService.BeforeUninstall())
                .Run();

            _logger.Info("Velopack finished, starting application...");
            
            var application = new App();
            application.Run();
        }


        private static void OnAppUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            try
            {
                _logger.Error("Unhandled exception", args.ExceptionObject as Exception);
            }
            catch
            {
                // Do nothing
            }
            ShowExceptionDialog(args.ExceptionObject as Exception);
            Process.GetCurrentProcess().Kill();
        }

        internal static void ShowExceptionDialog(Exception exception)
        {
            Application application = Application.Current;
            if (application.MainWindow != null && exception != null && application.MainWindow.DataContext is ShellViewModel shellViewModel)
            {
                shellViewModel.AppContext?.DialogService?.ShowExceptionDialog(exception);
            }
        }
    }
}
