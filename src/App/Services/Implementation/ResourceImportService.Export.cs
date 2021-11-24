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
using System.Threading.Tasks;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Services.Interfaces;
using LHQ.Core.Interfaces;
using LHQ.Core.Model.Exporter;
using LHQ.Data;
using LHQ.Utils.Extensions;

namespace LHQ.App.Services.Implementation
{
    public partial class ResourceImportService
    {
        private class ExporterAsyncOperation : AsyncOperation
        {
            private readonly IResourceExporter _exporter;
            private readonly string _fileName;
            private readonly CultureInfo[] _cultures;
            private readonly List<string> _resourceKeysToExport;
            private ResourceExportResult _exportResult;
            private readonly ModelContext _modelContextToExport;

            public ExporterAsyncOperation(IAppContext appContext, IResourceExporter exporter, string fileName, CultureInfo[] cultures,
                ModelContext modelContextToExport, List<string> resourceKeysToExport)
                : base(appContext)
            {
                _exporter = exporter ?? throw new ArgumentNullException(nameof(exporter));
                _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
                _cultures = cultures ?? throw new ArgumentNullException(nameof(cultures));
                _resourceKeysToExport = resourceKeysToExport;
                _modelContextToExport = modelContextToExport ?? throw new ArgumentNullException(nameof(modelContextToExport));
            }

            public override string ProgressMessage => Strings.Services.Export.ExportResourcesInProgress;

            public override bool ShowCancelButton { get; } = true;

            protected override async Task InternalStart()
            {
                _exportResult = await Export(_exporter, _modelContextToExport, _fileName, _cultures, _resourceKeysToExport);
            }

            protected override Task InternalCompleted(bool userCancelled)
            {
                IDialogService dialogService = AppContext.DialogService;

                if (userCancelled)
                {
                    dialogService.ShowError(Strings.Services.Import.ImportResourcesCaption, Strings.Services.Import.ImportWasCancelledByUser, null);
                }
                else
                {
                    if (_exportResult.IsSuccess)
                    {
                        string message = _exportResult.Message.IsNullOrEmpty()
                            ? Strings.Services.Export.ExportingResourcesWasSuccessfull
                            : _exportResult.Message;

                        dialogService.ShowInfo(Strings.Services.Export.ExportResourcesCaption, message, null);
                    }
                    else
                    {
                        string message = _exportResult.Message.IsNullOrEmpty()
                            ? Strings.Services.Export.ExportingResourcesFailed
                            : _exportResult.Message;

                        dialogService.ShowError(Strings.Services.Export.ExportResourcesCaption, message, null);
                    }
                }

                return Task.CompletedTask;
            }

            private Task<ResourceExportResult> Export(IResourceExporter exporter, ModelContext modelContextToExport, 
                string fileName, CultureInfo[] culturesToExport, List<string> resourceKeysToExport)
            {
                //ShellViewModel shellViewModel = AppContext.ShellViewModel;
                IModelFileStorage modelFileStorage = AppContext.ModelFileStorage;
                //ModelContext modelContext = modelFileStorage.Clone(shellViewModel.ModelContext, true);
                ModelContext modelContext = modelFileStorage.Clone(modelContextToExport, true);
                return exporter.Export(fileName, modelContext.Model, culturesToExport, resourceKeysToExport);
            }
        }
    }
}
