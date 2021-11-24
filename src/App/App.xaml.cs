// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.ViewModels;
using LHQ.Utils.Extensions;
using ScaleHQ.WPF.LHQ;

namespace LHQ.App
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static string _appFileName;

        [STAThread]
        public static void Main()
        {
            DateTimeHelper.Initialize();

            LocalizationExtension.Initialize(new LocalizationContextFactoryDefault(() => StringsContext.Instance));
            AppDomain.CurrentDomain.UnhandledException += OnAppUnhandledException;

            _appFileName = Assembly.GetExecutingAssembly().Location;

            var application = new App();
            application.InitializeComponent();
            application.Run();
        }

        private static void OnAppUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            //ExceptionViewerDialog.DialogShow(args.ExceptionObject as Exception);
            ShowExceptionDialog(args.ExceptionObject as Exception);
            Process.GetCurrentProcess().Kill();
        }

        private static void ShowExceptionDialog(Exception exception)
        {
            if (Current.MainWindow != null && exception != null && Current.MainWindow.DataContext is ShellViewModel shellViewModel)
            {
                shellViewModel.AppContext?.DialogService?.ShowExceptionDialog(exception);
            }
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Ensure the current culture passed into bindings 
            // is the OS culture. By default, WPF uses en-US 
            // as the culture, regardless of the system settings.
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            int argCount = e.Args.Length;
            if (argCount > 0)
            {
                string arg0 = e.Args[0];
                if (!arg0.IsNullOrEmpty() && arg0 == "-GenerateAppConfig" && argCount > 1)
                {
                    string folder = e.Args[1];
                    bool targetIsVS = argCount > 2 && e.Args[2] == "-TargetIsVS";
                    if (!folder.IsNullOrEmpty() && Directory.Exists(folder))
                    {
                        AppTools.GenerateAppConfig(folder, targetIsVS);
                        Shutdown(0);
                        return;
                    }
                }
            }

            ToolTipService.ShowOnDisabledProperty.OverrideMetadata(typeof(Control),
                new FrameworkPropertyMetadata(true));

#if (DEBUG)
            await RunInDebugModeAsync();
#else
            await RunInReleaseModeAsync();
#endif
            ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        private static Task RunInDebugModeAsync()
        {
            var bootstrapper = new Bootstraper(_appFileName);
            return bootstrapper.Run();
        }

        // ReSharper disable once UnusedMember.Local
        private static async Task RunInReleaseModeAsync()
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
            var bootstrapper = new Bootstraper(_appFileName);
            await bootstrapper.Run();
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //HandleException(e.ExceptionObject as Exception);
            ShowExceptionDialog(e.ExceptionObject as Exception);
        }

        // private static void HandleException(Exception ex)
        // {
        //     if (ex == null)
        //     {
        //         return;
        //     }
        //
        //     //TODO: log exception
        //     MessageBox.Show($"UnhandledException ({ex})");
        //     Environment.Exit(1);
        // }
    }
}
