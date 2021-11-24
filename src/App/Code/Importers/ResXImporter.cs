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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using LHQ.Core.Interfaces;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Core.Model;
using LHQ.Core.Model.Importer;
using LHQ.Data;
using LHQ.Data.Extensions;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.Code.Importers
{
    public sealed class ResXImporter : IResourceImporter
    {
        private readonly Regex _paramsRegex = new Regex(@"\{([0-9]+)\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string FilesFilter => ".NET Framework Resource Files (*.resx)|*.resx";

        public string[] FileExtensions => new[] { ".resx" };

        public string Key { get; } = "NET.ResX.Importer";

        public string DisplayName { get; } = "NET resources (resx) files importer";

        public string Version { get; } = "1.0";

        public string Help { get; } = "";

        public IPluginSystemContext PluginSystemContext { get; set; }

        public bool AllowMultipleFilesAtOnce { get; } = true;

        public bool CategoryHasDescription { get; } = false;

        public bool ResourceHasDescription { get; } = true;

        public void Initialize()
        { }

        public Task<ResourceImportResult> Import(string[] fileNames, CancellationToken cancellationToken)
        {
            if (fileNames == null)
            {
                fileNames = new string[0];
            }

            var importResult = new ResourceImportResult
            {
                Status = OperationResultStatus.Success
            };
            var modelContext = importResult.ModelContext;

            var fileLanguages = new Dictionary<string, string>();

            var errors = new StringBuilder();
            foreach (string fileName in fileNames)
            {
                var languageCode = "en";
                string baseName = Path.GetFileName(fileName) ?? string.Empty;
                int idx0 = baseName.IndexOf(".", StringComparison.Ordinal);
                int idx1 = baseName.IndexOf(".", idx0 + 1, StringComparison.Ordinal);
                if (idx0 > -1 && idx1 > -1 && idx1 > idx0)
                {
                    languageCode = baseName.Substring(idx0 + 1, idx1 - idx0 - 1);
                    CultureInfo cultureInfo = CultureCache.Instance.GetCulture(languageCode);
                    if (cultureInfo == null)
                    {
                        // TODO: Localize
                        errors.Append($"Could not found culture '{languageCode}' for file!");
                        errors.AppendLine();
                    }
                }

                if (errors.Length == 0)
                {
                    if (!modelContext.Model.Languages.ContainsByName(languageCode, true))
                    {
                        modelContext.AddLanguage(languageCode);
                    }

                    fileLanguages.Add(fileName, languageCode);
                }
            }

            if (errors.Length > 0)
            {
                importResult.SetError(errors.ToString());
            }

            try
            {
                if (importResult.Status == OperationResultStatus.Success)
                {
                    foreach (string fileName in fileNames)
                    {
                        string languageCode = fileLanguages[fileName];
                        ImportFile(importResult, fileName, languageCode);

                        if (importResult.Status == OperationResultStatus.Failed)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                importResult.Status = OperationResultStatus.Failed;
                DebugUtils.Error($"{nameof(ResXImporter)}.{nameof(Import)}({string.Join(",", fileNames)}) failed", e);
            }
            finally
            {
                if (importResult.Status == OperationResultStatus.Failed && importResult.Message.IsNullOrEmpty())
                {
                    importResult.Message = "Failed to import resources from selected file(s)";
                }
            }

            return Task.FromResult(importResult);
        }

        private void ImportFile(ResourceImportResult importResult, string fileName, string languageCode)
        {
            if (File.Exists(fileName))
            {
                var modelContext = importResult.ModelContext;
                var model = modelContext.Model;

                var doc = new XmlDocument();
                doc.Load(fileName);

                XmlNode resheader = doc.SelectSingleNode("/root/resheader[contains(@name,'resmimetype')]");
                if (resheader != null && resheader.FirstChild.InnerText == "text/microsoft-resx")
                {
                    XmlNodeList dataList = doc.SelectNodes("//root/data");
                    if (dataList != null)
                    {
                        string categoryName = Path.GetFileName(fileName);
                        int idxExt = categoryName.IndexOf(".", StringComparison.Ordinal);
                        if (idxExt > -1)
                        {
                            categoryName = categoryName.Substring(0, idxExt);
                        }

                        CategoryElement fileCategoryHolder = modelContext.AddCategory(categoryName);

                        foreach (XmlNode data in dataList)
                        {
                            XmlNode nodeComment = data.ChildNodes.OfType<XmlNode>().SingleOrDefault(x => x.Name == "comment");
                            XmlNode nodeValue = data.ChildNodes.OfType<XmlNode>().SingleOrDefault(x => x.Name == "value");
                            XmlAttribute attrName = data.Attributes?["name"];
                            if (nodeValue != null && !string.IsNullOrEmpty(attrName?.Value))
                            {
                                string resKey = attrName.Value;

                                string resourceValue = nodeValue.InnerText.IsNullOrEmpty() ? string.Empty : nodeValue.InnerText.Trim();
                                string resourceDescription = string.Empty;
                                if (nodeComment != null)
                                {
                                    resourceDescription = nodeComment.InnerText.IsNullOrEmpty() ? string.Empty : nodeComment.InnerText.Trim();
                                }

                                ResourceElement resourceElement = model.Resources.FindByName(resKey, true, CultureInfo.InvariantCulture) ??
                                    modelContext.AddResource(resKey, fileCategoryHolder);

                                if (!resourceDescription.IsNullOrEmpty() && resourceElement.Description.IsNullOrEmpty())
                                {
                                    resourceElement.Description = resourceDescription;
                                }

                                ResourceValueElement resourceValueElement = resourceElement.FindValueByLanguage(languageCode);
                                if (resourceValueElement == null)
                                {
                                    modelContext.AddResourceValue(languageCode, resourceValue, resourceElement);
                                }
                                else
                                {
                                    resourceValueElement.Value = resourceValue;
                                }
                                
                                if (!resourceValue.IsNullOrEmpty())
                                {
                                    var paramsIndexes = new List<int>();

                                    foreach (Match match in _paramsRegex.Matches(resourceValue))
                                    {
                                        if (match.Groups.Count == 2)
                                        {
                                            Group matchGroup = match.Groups[1];
                                            if (!matchGroup.Value.IsNullOrEmpty())
                                            {
                                                if (int.TryParse(matchGroup.Value, out int i))
                                                {
                                                    if (!paramsIndexes.Contains(i))
                                                    {
                                                        paramsIndexes.Add(i);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (paramsIndexes.Count > 0)
                                    {
                                        foreach (int paramIndex in paramsIndexes.OrderBy(x => x))
                                        {
                                            var paramName = "p" + paramIndex;
                                            var parameterElement = resourceElement.Parameters.FindByName(paramName, true, CultureInfo.InvariantCulture);
                                            if (parameterElement == null)
                                            {
                                                modelContext.AddResourceParameter(paramName, resourceElement);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    importResult.SetError($"File '{fileName}' does not seem to be valid .NET Framework Resource File !");
                }
            }
            else
            {
                importResult.SetError($"Could not find file '{fileName}' !");
            }
        }
    }
}
