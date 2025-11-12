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
using Velopack.Locators;
using Velopack.Windows;

namespace LHQ.App
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
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
            var bootstrapper = new Bootstraper(LHQ.App.Startup._appFileName, LHQ.App.Startup._cmdArgs);
            return bootstrapper.Run();
        }

        // ReSharper disable once UnusedMember.Local
        private static async Task RunInReleaseModeAsync()
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
            var bootstrapper = new Bootstraper(LHQ.App.Startup._appFileName, LHQ.App.Startup._cmdArgs);
            await bootstrapper.Run();
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //HandleException(e.ExceptionObject as Exception);
            LHQ.App.Startup.ShowExceptionDialog(e.ExceptionObject as Exception);
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
