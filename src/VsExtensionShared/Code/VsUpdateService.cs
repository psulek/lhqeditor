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
using System.Linq;
using System.Threading.Tasks;
using LHQ.App.Code;
using LHQ.App.Model;
using LHQ.App.Model.VsGallery;
using LHQ.App.Services.Implementation;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using RestSharp;
using Version = System.Version;

namespace LHQ.VsExtension.Code
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class VsUpdateService : UpdateService
    {
        public override async Task<UpdateCheckInfo> CheckForUpdate()
        {
            UpdateCheckInfo result = new UpdateCheckInfo(null, null, null);

            try
            {
                var client = new RestClient(AppConstants.WebSiteUrls.VsGalleryApiUrl);
                var restRequest = new RestRequest();

                var extensionName = VsPackageService.VsVersion.Major >= 17 ? "scalehqsolutions.lhqeditorvs2022" : "scalehqsolutions.lhqeditor";
                var request = new VsGalleryRequest
                {
                    Filters = new[]
                    {
                        new Filter
                        {
                            Criteria = new[] { new Criterion { FilterType = 7, Value = extensionName } },
                            PageNumber = 1,
                            PageSize = 1,
                            SortBy = 0,
                            SortOrder = 0
                        }
                    },
                    AssetTypes = new[] { "Microsoft.VisualStudio.Services.Content.Details" },
                    Flags = 513
                };

                // https://github.com/microsoft/vscode/blob/main/src/vs/platform/extensionManagement/common/extensionGalleryService.ts#L113-L122
                restRequest.AddJsonBody(request);
                var restResponse = await client.ExecutePostTaskAsync(restRequest);
                var response = JsonUtils.FromJsonString<VsGalleryResponse>(restResponse.Content);

                if (response?.Results?.Length > 0 && response.Results[0].Extensions?.Length == 1)
                {
                    Extension extension = response.Results[0].Extensions.Single();
                    if (extension?.Versions?.Length == 1)
                    {
                        string[] extensionFlags = extension.Flags.ToLowerInvariant()
                            .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .ToArray();

                        if (extensionFlags.Contains("validated") && extensionFlags.Contains("public"))
                        {
                            var versionObj = extension.Versions.Single();
                            if (versionObj != null && versionObj.Flags.ToLowerInvariant().Contains("validated"))
                            {
                                string versionStr = versionObj.VersionVersion;
                                if (Version.TryParse(versionStr, out var serverVersionObj))
                                {
                                    var serverVersion = new Version(serverVersionObj.Major, serverVersionObj.Minor,
                                        serverVersionObj.Build == -1 ? 0 : serverVersionObj.Build,
                                        serverVersionObj.Revision == -1 ? 0 : serverVersionObj.Revision);

                                    if (serverVersion > AppContext.AppCurrentVersion)
                                    {
                                        var msg2 =
                                            $"Visual Studio Marketplace contains newer version: {serverVersion} than current version: {AppContext.AppCurrentVersion}\n" +
                                            "Check https://marketplace.visualstudio.com/items?itemName=" + extensionName;
                                        VsPackageService.AddMessageToOutput(msg2, OutputMessageType.Info, false);

                                        var currentVersionDescriptions = ReadCurrentVersionDescriptions();
                                        result = new UpdateCheckInfo(serverVersion, Array.Empty<string>(), currentVersionDescriptions);
                                    }
                                    else
                                    {
                                        int compareValue = serverVersion.CompareTo(AppContext.AppCurrentVersion);
                                        string verIdent = compareValue == 0 ? "same" : "lower";

                                        if (compareValue == 0)
                                        {
                                            VsPackageService.AddMessageToOutput(
                                                $"Visual Studio Marketplace contains {verIdent} version ({serverVersion}) than current version ({AppContext.AppCurrentVersion}).",
                                                OutputMessageType.Info, false);
                                        }

                                        DebugUtils.Log(
                                            $"Last extension version (from vsgallery): {serverVersionObj} has {verIdent} than current version {AppContext.AppCurrentVersion}");
                                    }

                                    VsPackageService.AddMessageToOutput(
                                        "Visual Studio Marketplace - https://marketplace.visualstudio.com/items?itemName=" + extensionName,
                                        OutputMessageType.Info, false);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Error("Check for update failed", e);
            }

            return result;
        }

        private string[] ReadCurrentVersionDescriptions()
        {
            string releaseNotesFile = VsPackageService.GetExtensionPath("ReleaseNotes.txt");
            try
            {
                if (File.Exists(releaseNotesFile))
                {
                    List<string> readedNotes = new List<string>();
                    bool foundVersion = false;
                    foreach (var line in File.ReadAllLines(releaseNotesFile))
                    {
                        if (!line.IsNullOrEmpty() && line[0] != '-' && Version.TryParse(line, out var version) &&
                            version == AppContext.AppCurrentVersion)
                        {

                            foundVersion = true;
                            continue;
                        }

                        if (foundVersion)
                        {
                            if (line.Length > 0 && line[0] == '-')
                            {
                                readedNotes.Add(line.Substring(1).Trim());
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (readedNotes.Count > 0)
                    {
                        return readedNotes.ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error reading release notes from '{releaseNotesFile}'", e);
            }

            return null;
        }
    }
}
