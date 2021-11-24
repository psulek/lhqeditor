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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using EnvDTE;
using LHQ.App.Code;
using LHQ.App.Model;
using LHQ.App.Services.Implementation;
using LHQ.VsExtension.Code.Extensions;

namespace LHQ.VsExtension.Code
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class VsApplicationService : ApplicationService
    {
        public override Dictionary<string, string> GetEnvironmentInfo()
        {
            var info = base.GetEnvironmentInfo();

            string vsProductVersion = DteExtensions.GetVsProductVersion();
            DTE dte = DteExtensions.GetDTE();
            string vsEdition = dte.Edition;

            info.Add("Visual Studio Version", vsProductVersion);
            info.Add("Visual Studio Edition", vsEdition);

            return info;
        }

        public override void Exit(bool restartApp)
        {
            VsPackageService.CloseOpenedEditors();
        }

        public override AppVisualTheme GetInitialTheme()
        {
            bool detectTheme = AppContext.AppConfigFactory.Current.DetectTheme.GetValueOrDefault(true);

            if (detectTheme)
            {
                return VisualStudioThemeManager.Instance.Theme.ToAppVisualTheme();
            }

            return base.GetInitialTheme();
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            OpenReadmeHtml();
        }

        private void OpenReadmeHtml()
        {
            if (AppContext.AppStartedFirstTime)
            {
                try
                {
                    string htmlFile = VsPackageService.GetExtensionPath("readme.html");
                    if (File.Exists(htmlFile))
                    {
                        VsPackageService.DTE.ItemOperations.Navigate(htmlFile, vsNavigateOptions.vsNavigateOptionsNewWindow);

                        ShowAppSettings();
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("Error updating readme file!", e);
                }
            }
        }
    }
}