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
using System.Threading;
using System.Threading.Tasks;
using LHQ.Core.Interfaces;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Core.Model;
using LHQ.Core.Model.Importer;
using LHQ.Data;
using LHQ.Data.Extensions;
using LHQ.Data.Interfaces;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using OfficeOpenXml;

namespace LHQ.Plugin.ExportImportPack.Importers
{
    public class ExcelImporter: IResourceImporter
    {
        public string FilesFilter => "Microsoft Excel (*.xlsx)|*.xlsx";

        public string[] FileExtensions => new[] { ".xlsx" };

        public string Key { get; } = "Excel.Importer";

        public string DisplayName { get; } = "Microsoft Excel Resources Importer";

        public string Version { get; } = "1.0";

        public string Help { get; } = string.Empty;

        public IPluginSystemContext PluginSystemContext { get; set; }

        public bool AllowMultipleFilesAtOnce { get; } = false;

        public bool CategoryHasDescription { get; } = false;

        public bool ResourceHasDescription { get; } = false;

        public void Initialize()
        { }

        private void AddError(StringBuilder errors, string error)
        {
            // TODO: Localize
            errors.Append(error);
            errors.AppendLine();
        }

        public Task<ResourceImportResult> Import(string[] fileNames, CancellationToken cancellationToken)
        {
            var importResult = new ResourceImportResult
            {
                Status = OperationResultStatus.Success
            };
            var modelContext = importResult.ModelContext;
            Model model = modelContext.Model;

            string fileName = fileNames[0];
            var fileInfo = new FileInfo(fileName);

            var errors = new StringBuilder();
            using (var excel = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet ws = excel.Workbook.Worksheets[ExcelConsts.WorksheetName];
                if (ws == null && excel.Workbook.Worksheets.Count > 0)
                {
                    ws = excel.Workbook.Worksheets.First();
                }

                if (ws == null)
                {
                    // TODO: Localize
                    AddError(errors, $"Could not found sheet with name '{ExcelConsts.WorksheetName}'.");
                }
                else
                {
                    bool ValidateCell(char colChar, int row)
                    {
                        int col = ((byte)colChar) - 64;

                        var cell = ws.Cells[row, col];
                        if (cell?.Value == null || cell.Value.ToString().IsNullOrEmpty())
                        {
                            AddError(errors, $"Could not found data on cell {colChar}:{row} !");
                            return false;
                        }

                        return true;
                    }

                    if (ws.Cells.Rows < 2 || ws.Cells.Columns < 3)
                    {
                        AddError(errors, "Could not found any data to import!");
                    }
                    // check if there are at least 2 columns, 'A:1' -> Resource Key, 'A:2' -> Primary Language Name
                    else if (ValidateCell('A', 1) && ValidateCell('B', 1))
                    {
                        // key - column, value - language code
                        var languageCodes = new Dictionary<int, string>();

                        // read language codes starting from B:1
                        for (int col = 2; col <= ws.Cells.Columns; col++)
                        {
                            ExcelRange cell = ws.Cells[1, col];
                            string cellValue = cell.Value as string;
                            string[] values = cellValue?.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            if (values?.Length > 0)
                            {
                                string langCode = values[0].ToLowerInvariant();
                                var cultureInfo = CultureCache.Instance.GetCulture(langCode);
                                if (cultureInfo != null && languageCodes.All(x => x.Value != langCode))
                                {
                                    languageCodes.Add(col, cultureInfo.Name);
                                }
                            }
                        }

                        if (languageCodes.Count == 0)
                        {
                            AddError(errors, "Missing or unknown language codes to import data for.");
                        }
                        else
                        {
                            foreach (string languageCode in languageCodes.Values)
                            {
                                if (!model.Languages.ContainsByName(languageCode, true))
                                {
                                    modelContext.AddLanguage(languageCode);
                                }
                            }

                            const int startRow = 2;
                            var excelResources = new Dictionary<string, ExcelResourceLineItem>();
                            for (int row = startRow; row < ws.Cells.Rows; row++)
                            {
                                ExcelRange cell = ws.Cells[row, 1];
                                string resourceKey = cell.Value as string;
                                if (!resourceKey.IsNullOrEmpty())
                                {
                                    if (!excelResources.ContainsKey(resourceKey))
                                    {
                                        var excelResourceLineItem = new ExcelResourceLineItem(resourceKey);
                                        excelResources.Add(resourceKey, excelResourceLineItem);


                                        foreach (var pair in languageCodes)
                                        {
                                            int column = pair.Key;
                                            string languageCode = pair.Value;

                                            cell = ws.Cells[row, column];
                                            string resourceValue = cell.Value as string;

                                            if (!languageCode.IsNullOrEmpty())
                                            {
                                                excelResourceLineItem.AddValue(languageCode, resourceValue);
                                            }
                                        }
                                    }
                                }
                            }

                            var lineItems = excelResources.Values.OrderByDescending(x => x.ModelPath.PathPartsCount).ToArray();
                            var categoryCache = new Dictionary<string, CategoryElement>();

                            CategoryElement FindCategory(string categoryName, ICategoryLikeElement parent)
                            {
                                return parent.Categories.FindByName(categoryName, true, CultureInfo.InvariantCulture);
                            }

                            foreach (var lineItem in lineItems)
                            {
                                ModelPath modelPath = lineItem.ModelPath;
                                int partsCount = modelPath.PathPartsCount;
                                bool isResource = partsCount <= 1;

                                ICategoryLikeElement categoryForNewResource = model;
                                string newResourceName;

                                if (isResource)
                                {
                                    newResourceName = lineItem.FullKey;
                                }
                                else
                                {
                                    var categoryPath = lineItem.ModelPath.GetPath(partsCount - 1);
                                    var category = categoryCache.ContainsKey(categoryPath) 
                                        ? categoryCache[categoryPath]
                                        : modelContext.FindCategory(modelPath, partsCount - 1);

                                    if (category == null)
                                    {
                                        var categoriesList = modelPath.PathParts.Take(partsCount - 1).ToArray();
                                        ICategoryLikeElement parentCategory = model;
                                        foreach (string categoryName in categoriesList)
                                        {
                                            parentCategory = FindCategory(categoryName, parentCategory) ??
                                                modelContext.AddCategory(categoryName, parentCategory);
                                        }

                                        if (parentCategory != model)
                                        {
                                            category = parentCategory as CategoryElement;
                                            categoryCache.Add(categoryPath, category);
                                        }
                                    }

                                    categoryForNewResource = category;
                                    newResourceName = modelPath.PathParts.Last();
                                }

                                if (!categoryForNewResource.Resources.ContainsByName(newResourceName, true))
                                {
                                    var newResource = modelContext.AddResource(newResourceName, categoryForNewResource);
                                    foreach (var valuePair in lineItem.Values)
                                    {
                                        string languageCode = valuePair.Key;
                                        string value = valuePair.Value;
                                        modelContext.AddResourceValue(languageCode, value, newResource);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (errors.Length > 0)
            {
                importResult.SetError(errors.ToString());
            }

            return Task.FromResult(importResult);
        }

        private class ExcelResourceLineItem
        {
            public string FullKey { get; }
            public ModelPath ModelPath { get; }

            private const string PathSeparator = "/";

            public Dictionary<string, string> Values { get; }

            public ExcelResourceLineItem(string fullKey)
            {
                FullKey = fullKey;
                ModelPath = new ModelPath(PathSeparator, fullKey);
                Values = new Dictionary<string, string>();
            }

            public void AddValue(string languageCode, string resourceValue)
            {
                if (!Values.ContainsKey(languageCode))
                {
                    Values.Add(languageCode, resourceValue);
                }
            }
        }
    }
}
