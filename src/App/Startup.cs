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
using System.Reflection;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.ViewModels;
using ScaleHQ.WPF.LHQ;
using Application = System.Windows.Application;

namespace LHQ.App
{
    public class Startup
    {
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

            var application = new App();
            //application.InitializeComponent();
            application.Run();
        }

        private static void OnAppUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            //ExceptionViewerDialog.DialogShow(args.ExceptionObject as Exception);
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
