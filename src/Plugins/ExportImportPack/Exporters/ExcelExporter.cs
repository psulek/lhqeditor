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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LHQ.Core.Interfaces;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Core.Model;
using LHQ.Core.Model.Exporter;
using LHQ.Data;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace LHQ.Plugin.ExportImportPack.Exporters
{
    public class ExcelExporter : IResourceExporter
    {
        public string FilesFilter => "Microsoft Excel (*.xlsx)|*.xlsx";

        public string[] FileExtensions => new[] { ".xlsx" };

        public string Key { get; } = "Excel.Exporter";

        public string DisplayName { get; } = "Microsoft Excel Resources Exporter";

        public string Version { get; } = "1.0";

        public string Help { get; } = string.Empty;

        public IPluginSystemContext PluginSystemContext { get; set; }

        public void Initialize()
        {}

        public Task<ResourceExportResult> Export(string fileName, Model model, CultureInfo[] culturesToExport, List<string> resourceKeysToExport)
        {
            LanguageElement primaryLanguage = model.Languages.Single(x => x.IsPrimary);
            CultureInfo primaryCulture = CultureCache.Instance.GetCulture(primaryLanguage.Name);

            var fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            using (var excel = new ExcelPackage(fileInfo))
            {
                var headerRowIndex = 1;
                var headerCol = 1;
                ExcelWorksheet ws = excel.Workbook.Worksheets.Add(ExcelConsts.WorksheetName);

                ExcelRange cellKey = ws.Cells[headerRowIndex, headerCol];
                cellKey.Value = "Resource Key";
                cellKey.Style.Font.Bold = true;

                headerCol++;
                ExcelRange cellPrimaryLanguage = ws.Cells[headerRowIndex, headerCol];
                cellPrimaryLanguage.Value = $"{primaryCulture.Name.ToUpper()} ({primaryCulture.EnglishName}) [Primary]";
                cellPrimaryLanguage.Style.Font.Bold = true;

                foreach (CultureInfo otherLanguage in culturesToExport)
                {
                    CultureInfo otherCulture = CultureCache.Instance.GetCulture(otherLanguage.Name);
                    headerCol++;
                    ExcelRange cellOtherLanguage = ws.Cells[headerRowIndex, headerCol];
                    cellOtherLanguage.Value = $"{otherCulture.Name.ToUpper()} ({otherCulture.EnglishName})";
                    cellOtherLanguage.Style.Font.Bold = true;
                }

                List<CultureInfo> allLanguages = EnumerableExtensions.One(primaryCulture).Concat(culturesToExport).ToList();

                string parentPath = string.Empty;
                if (model.Categories.Count > 0)
                {
                    ExportCategories(model.Categories, resourceKeysToExport, allLanguages, parentPath, ref headerRowIndex, ws);
                }

                ExportResources(model.Resources, resourceKeysToExport, allLanguages, parentPath, ref headerRowIndex, ws);

                // column 'key'
                ExcelColumn columnKey = ws.Column(1);
                columnKey.AutoFit(0, 50);
                columnKey.Style.Fill.PatternType = ExcelFillStyle.Solid;
                columnKey.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
                columnKey.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                columnKey.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // column 'primary language'
                ExcelColumn columnPrimaryLang = ws.Column(2);
                columnPrimaryLang.Style.Fill.PatternType = ExcelFillStyle.Solid;
                columnPrimaryLang.Style.Fill.BackgroundColor.SetColor(Color.Beige);
                columnPrimaryLang.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                columnPrimaryLang.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                for (var i = 2; i <= ws.Dimension.Columns; i++)
                {
                    ws.Column(i).AutoFit(0, 70);
                }

                if (ws.Dimension.Rows > 1)
                {
                    for (var rowIdx = 1; rowIdx <= ws.Dimension.Rows; rowIdx++)
                    {
                        ws.Row(rowIdx).Style.Border.Bottom.Style =
                            rowIdx == 1 ? ExcelBorderStyle.Medium : ExcelBorderStyle.Thin;

                        for (var colIdx = 2; colIdx <= ws.Dimension.Columns; colIdx++)
                        {
                            ExcelRange cell = ws.Cells[rowIdx, colIdx];
                            cell.Style.WrapText = true;
                        }
                    }
                }

                if (ws.Dimension.Columns > 2)
                {
                    for (var i = 2; i <= ws.Dimension.Columns; i++)
                    {
                        ExcelColumn column = ws.Column(i);
                        column.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        if (column.Width < 50)
                        {
                            column.AutoFit(50, 70);
                        }
                    }
                }

                // header row
                ExcelRow headerRow = ws.Row(1);
                headerRow.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerRow.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
                headerRow.Style.Font.Size = 13;

                ws.View.FreezePanes(2, 3);

                excel.Save();
            }

            var status = File.Exists(fileName) ? OperationResultStatus.Success : OperationResultStatus.Failed;
            if (status == OperationResultStatus.Success)
            {
                Process.Start(fileName);
            }

            var result = new ResourceExportResult
            {
                Status = status,
                Message = $"Successfully exported resources to excel file '{fileName}'."
            };

            return Task.FromResult(result);
        }

        private void ExportCategories(CategoryElementList categories, List<string> resourceKeysToExport, 
            List<CultureInfo> allLanguages, string parentPath, ref int cellRow, ExcelWorksheet ws)
        {
            foreach (CategoryElement category in categories)
            {
                string categoryPath = $"{parentPath}/{category.Name}";

                if (category.Resources.Count > 0)
                {
                    ExportResources(category.Resources, resourceKeysToExport, allLanguages, categoryPath, ref cellRow, ws);
                }

                if (category.Categories.Count > 0)
                {
                    ExportCategories(category.Categories, resourceKeysToExport, allLanguages, categoryPath, ref cellRow, ws);
                }
            }
        }

        private void ExportResources(ResourceElementList resources, List<string> resourceKeysToExport, 
            List<CultureInfo> allLanguages, string categoryPath, ref int cellRow, ExcelWorksheet ws)
        {
            var resourcesQuery = resourceKeysToExport == null || resourceKeysToExport.Count == 0
                ? resources
                : resources.Where(x => resourceKeysToExport.Contains(x.Key.Serialize()));

            foreach (ResourceElement resource in resourcesQuery)
            {
                cellRow++;
                string resourcePath = categoryPath.IsNullOrEmpty()
                    ? resource.Name
                    : $"{categoryPath}/{resource.Name}";
                ExcelRange cellKey = ws.Cells[cellRow, 1];
                cellKey.Value = resourcePath;
                cellKey.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                cellKey.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                var cellValueCol = 1;
                foreach (CultureInfo language in allLanguages)
                {
                    cellValueCol++;
                    ExcelRange cellResource = ws.Cells[cellRow, cellValueCol];
                    cellResource.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    cellResource.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    ResourceValueElement resourceValue = resource.FindValueByLanguage(language.Name);
                    cellResource.Value = resourceValue?.Value;
                }
            }
        }
    }
}
