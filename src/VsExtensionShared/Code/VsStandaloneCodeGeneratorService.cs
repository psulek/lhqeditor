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
using System.IO;
using System.Threading.Tasks;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Implementation;
using LHQ.Data;
using LHQ.Gen.Lib;
using LHQ.Utils.Extensions;
using NLog;

namespace LHQ.VsExtension.Code
{
    public class VsStandaloneCodeGeneratorService : StandaloneCodeGeneratorService
    {
        // private NLog.Logger _logger;

        public override async Task<StandaloneCodeGenerateResult> GenerateCodeAsync(string modelFileName, ModelContext modelContext)
        {
            // using (var inMemoryLogger = new InMemoryLogger(Logger.GetSourceLogger() as NLog.Logger, OnLogMessage))
            // {
            //     _logger = inMemoryLogger.Logger;

                var result = await base.GenerateCodeAsync(modelFileName, modelContext);

                if (result.Success)
                {
                    int modelVersion = modelContext.Model.Version;
                    var isModernGenerator = modelVersion > 1;
                    if (isModernGenerator)
                    {
                        var dir = Path.GetDirectoryName(modelFileName);
                        var t4File = Path.Combine(dir, Path.GetFileNameWithoutExtension(modelFileName) + ".lhq.tt");
                        if (File.Exists(t4File))
                        {
                            bool flagUnsupportedT4Template = AppContext.AppConfigFactory.Current.AppHints.IsFlagSet(AppHintType.UnsupportedT4Template);
                            
                            if (flagUnsupportedT4Template)
                            {
                                var message = Strings.Dialogs.CodeGenerator.T4NotUsedAnymore(modelVersion, 
                                    Path.GetFileName(t4File),
                                    AppConstants.ModernGeneratorNoT4MinVersion);
                                
                                var dialogShowInfo = new DialogShowInfo(Strings.Dialogs.CodeGenerator.CodeGeneratorTitle, message)
                                {
                                    CheckValue = false,
                                    CheckHeader = Strings.Common.DontShowAgain,
                                    ExtraButtonHeader = Strings.Common.ReadMore,
                                    ExtraButtonAction = () =>
                                        {
                                            WebPageUtils.ShowUrl(AppConstants.WebSiteUrls.GetT4_ObsoleteUrl());
                                        }
                                };
                                
                                AppContext.UIService.DispatchActionOnUI(() =>
                                    {
                                        var dialogResultInfo = AppContext.DialogService.ShowWarning(dialogShowInfo);
                                        
                                        if (dialogResultInfo.DialogResult == DialogResult.OK)
                                        {
                                            AppContext.AppConfigFactory.Current.UpdateAppHint(AppHintType.UnsupportedT4Template, !dialogResultInfo.IsChecked);
                                            AppContext.ApplicationService.SaveAppConfig();
                                        }
                                    }, TimeSpan.FromSeconds(1));
                            }
                        }
                    }
                }

                return result;
            //}

        }

        private void OnLogMessage(Tuple<LogLevel, string> logItem)
        {
            VsPackageService.AddMessageToOutput(logItem.Item2, logItem.Item1.ToOutputMessageType());
        }

        protected override (NLog.ILogger logger, IDisposable loggerRegion) GetLoggerForGenerator()
        {
            var inMemoryLogger = new InMemoryLogger(Logger.GetSourceLogger() as NLog.Logger, OnLogMessage);
            return (inMemoryLogger.Logger, inMemoryLogger);
        }

        // protected override ILogger GetLoggerForGenerator()
        // {
        //     return _logger;
        // }
    }
}
