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

namespace LHQ.App.Code
{
    public static class AppConstants
    {
        private const string ModelFileExtension = "lhq";

        public const string ModernGeneratorMinVersion = "2025.1";
        public const string ModernGeneratorNoT4MinVersion = "2025.2";

        public static class WebSiteUrls
        {
            public const string HomePage = "https://psulek.github.io/lhqeditor/";
            public const string Documentation = HomePage + "/help/";

            public const string ReportBug = "https://github.com/psulek/lhqeditor/issues/new?labels=bug&";

            public const string VsGalleryApiUrl =
                "https://marketplace.visualstudio.com/_apis/public/gallery/extensionquery?api-version=3.0-preview.1";

            private const string NugetApiServer = "https://api.nuget.org/v3-flatcontainer/";

            public const string ModelV2_Info = "https://github.com/psulek/lhqeditor/wiki/LHQ-Model-v2-Changes";
            public const string ModelV2_T4Obsolete = "https://github.com/psulek/lhqeditor/wiki/LHQ-Model-v2-Changes#obsolete-t4-template-files-lhqtt";

            public static string GetReportBug(string title, string body)
            {
                return ReportBug + "title=" + title + "&body=" + body;
            }

            public static string GetNugetPackageVersionsUrl(string packageId)
            {
                return NugetApiServer + packageId + "/index.json";
            }
        }

        
        public static string GetModelFileExtension(bool includeDot)
        {
            return includeDot ? "." + ModelFileExtension : ModelFileExtension;
        }
    }
}
