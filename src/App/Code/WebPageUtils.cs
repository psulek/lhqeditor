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

using System.Collections.Generic;
using System.Diagnostics;

namespace LHQ.App.Code
{
    public static class WebPageUtils
    {
        private static readonly Dictionary<WebPageDocSection, string> _docQueryStrings = new Dictionary<WebPageDocSection, string>
        {
            { WebPageDocSection.MainWindow, "ui_main_window" },
            { WebPageDocSection.CreateNewProject, "ui_new_lhq_file" },
            { WebPageDocSection.ProjectSettings, "ui_project_settings" },
            { WebPageDocSection.ImportResources, "ui_import_resources" },
            { WebPageDocSection.ExportResources, "ui_export_resources" },
            { WebPageDocSection.LanguageSettings, "ui_language_settings" },
            { WebPageDocSection.ResourceParameters, "ui_resource_parameters" },
            { WebPageDocSection.PreferencesGeneral, "ui_preferences_general" },
            { WebPageDocSection.PreferencesTranslator, "ui_preferences_translator" },
            { WebPageDocSection.PreferencesPlugins, "ui_preferences_plugins" }
        };

        public static void ShowDoc(WebPageDocSection? docSection = null)
        {
            string url = AppConstants.WebSiteUrls.Documentation;
            if (docSection != null)
            {
                url += "?page=" + _docQueryStrings[docSection.Value];
            }

            ShowUrl(url);
        }

        public static void ShowProductWebPage()
        {
            ShowUrl(AppConstants.WebSiteUrls.HomePage);
        }

        public static void ShowUrl(string url)
        {
            Process.Start(url);
        }
    }

    public enum WebPageDocSection
    {
        MainWindow,
        CreateNewProject,
        ProjectSettings,
        ImportResources,
        ExportResources,
        LanguageSettings,
        ResourceParameters,
        PreferencesGeneral,
        PreferencesTranslator,
        PreferencesPlugins,
        //LicenseActivation
    }
}
