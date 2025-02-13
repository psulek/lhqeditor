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
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using LHQ.App.Code;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using LHQ.VsExtension.Code;
using Microsoft.VisualStudio.TemplateWizard;
using Newtonsoft.Json;
using RestSharp;

namespace LHQ.VsExtension.Wizard
{
    public abstract class WizardBase<TWizardData> : IWizard where TWizardData: WizardData
    {
        private const string WizardData = "$wizarddata$";

        private readonly ThreadSafeSingleShotGuard _installingPackages = ThreadSafeSingleShotGuard.Create(false);

        protected TWizardData _wizardData;
        protected DTE2 _dte;
        protected Version _dteVersion;
        protected string _vsProductVersion;

        private readonly Dictionary<int, string> _vsVersions = new Dictionary<int, string>
        {
            {14, "2015"},
            {15, "2017"},
            {16, "2019"}
        };

        private readonly TimeSpan _webRequestCallTimeout = TimeSpan.FromSeconds(10);

        protected abstract bool WizardDataIsRequired { get; }

        protected abstract bool AllowInstallNugetPackages { get; }

        protected abstract WizardType WizardType { get; }

        protected string GetProjectName(DTE2 dte)
        {
            string result = string.Empty;

            //Adding by template is unavailable in the context menu when selecting more than one item -> use index 0. 
            var selectedItems = dte.ToolWindows.SolutionExplorer.SelectedItems as UIHierarchyItem[];
            UIHierarchyItem selectedUiHierarchyItem = selectedItems?[0];

            //not null when selecting project node (when adding an item) or when selecting a solution folder (when adding a project)
            //Determine which kind of project node: 
            if (selectedUiHierarchyItem?.Object is Project selectedProject)
            {
                if (selectedProject.Kind == Constants.vsProjectKindSolutionItems)
                {
                    //solution folder
                }
                else
                {
                    //project
                    result = selectedProject.Name;
                }
            }

            return result;
        }

        protected string GetVsProductVersion(Version version)
        {
            return _vsVersions.ContainsKey(version.Major) ? _vsVersions[version.Major] : version.Major.ToString();
        }

        public virtual void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            _dte = (DTE2)automationObject;

            if (!replacementsDictionary.ContainsKey(WizardData) && WizardDataIsRequired)
            {
                throw new InvalidOperationException("LHQ Editor - Wizard data is missing, cant install project template!");
            }

            string wizardDataStr = replacementsDictionary.ContainsKey(WizardData) ? replacementsDictionary[WizardData] : null;
            if (wizardDataStr.IsNullOrEmpty() && WizardDataIsRequired)
            {
                throw new InvalidOperationException("LHQ Editor - Wizard data is empty, cant install project template!");
            }

            try
            {
                _wizardData = JsonUtils.FromJsonString<TWizardData>(wizardDataStr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _wizardData = null;
            }
            
            if (_wizardData == null && WizardDataIsRequired)
            {
                throw new InvalidOperationException("LHQ Editor - Invalid wizard data, cant install " + WizardType.ToLowerInvariant() + " template!");
            }

            _dteVersion = Version.Parse(_dte.Version);
            _vsProductVersion = GetVsProductVersion(_dteVersion);
        }

        protected void InstallNugetPackages(Project project, bool openReadmeTxt)
        {
            if (_installingPackages.TrySetSignal())
            {
                Task.Run(async () =>
                    {
                        try
                        {
                            if (_wizardData?.Packages?.Count > 0)
                            {
                                foreach (var package in _wizardData.Packages)
                                {
                                    string packageVersion = await GetAvailableNugetVersion(package);

                                    await project.InstallPackage(package.Id, packageVersion, openReadmeTxt);
                                }
                            }
                        }
                        finally
                        {
                            _installingPackages.ResetSignal();
                        }
                    });
            }
            else
            {
                DebugUtils.Log($"{GetType().Namespace}.{nameof(InstallNugetPackages)} skipped, already installing...");
            }
        }

        // Find highest available version on nuget.org, but only on 3rd number on version
        // so if param in package.id is '1.0.0', and on nuget.org there are other versions
        // like: 1.0.1, 1.0.2 and 2.0.0 that this method will return 1.0.2.
        private async Task<string> GetAvailableNugetVersion(WizardDataPackage package)
        {
            string result = package.Version;

            try
            {
                var client = new RestClient(AppConstants.WebSiteUrls.GetNugetPackageVersionsUrl(package.Id));

                //_webRequestCallTimeout = TimeSpan.FromMilliseconds(100);
                var response = await TaskAsyncExtensions.WaitUntil(_webRequestCallTimeout, cancellationToken =>
                    client.ExecuteGetTaskAsync(new RestRequest(), cancellationToken));

                if (response != null && !response.Content.IsNullOrEmpty())
                {
                    var packageVersions = JsonUtils.FromJsonString<NugetPackageVersionsResponse>(response.Content);
                    if (packageVersions?.Versions?.Count > 0)
                    {
                        var packageVersion = ParseVersion(package.Version);
                        if (packageVersion != null)
                        {
                            Version availableVersion = packageVersions.Versions.Select(ParseVersion)
                                .Where(x => x != null && x.Major == packageVersion.Major && x.Minor == packageVersion.Minor &&
                                    x.Build > packageVersion.Build)
                                .OrderByDescending(x => x.Build)
                                .FirstOrDefault();

                            result = availableVersion == null
                                ? package.Version
                                : $"{availableVersion.Major}.{availableVersion.Minor}.{availableVersion.Build}";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }

        private Version ParseVersion(string version)
        {
            Version result = null;
            string[] entries = version.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (entries.Length > 2 && 
                int.TryParse(entries[0], out var major) &&
                int.TryParse(entries[1], out var minor) &&
                int.TryParse(entries[2], out var build))
            {
                result = new Version(major, minor, build);
            }

            return result;
        }

        public virtual void ProjectFinishedGenerating(Project project)
        {}

        public virtual void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {}

        public virtual bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        public virtual void BeforeOpeningFile(ProjectItem projectItem)
        {}

        public virtual void RunFinished()
        { }
    }

    internal class NugetPackageVersionsResponse
    {
        [JsonProperty("versions")]
        public List<string> Versions { get; set; }

        public NugetPackageVersionsResponse()
        {
            Versions = new List<string>();
        }
    }
}
