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
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using LHQ.App.Code;
using LHQ.App.Dialogs;
using LHQ.App.Dialogs.AppSettings;
using LHQ.App.Extensions;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Dialogs;
using LHQ.App.ViewModels.Dialogs.AppSettings;
using LHQ.App.ViewModels.Dialogs.Export;
using LHQ.App.ViewModels.Elements;
using LHQ.Core.DependencyInjection;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Core.Model;
using LHQ.Data;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation
{
    public class DialogService : AppContextServiceBase, IDialogService
    {
        /// <summary>
        /// indicates that 'exception dialog is visible' or not
        /// </summary>
        private readonly ThreadSafeSingleShotGuard _exceptionDialogIsVisible = ThreadSafeSingleShotGuard.Create(false);

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        public DialogResultInfo ShowConfirm(DialogShowInfo dialogShowInfo,
            DialogButtons buttons = DialogButtons.YesNo, DialogIcon dialogIcon = DialogIcon.Question)
        {
            return MessageBoxDialog.DialogShow(AppContext, dialogIcon, buttons, dialogShowInfo);
        }

        public bool ShowPrompt(string caption, string message, ref string input)
        {
            PromptDialog.Result result = PromptDialog.DialogShow(AppContext, caption, message, input);
            if (result.Submitted)
            {
                input = result.Input;
            }

            return result.Submitted;
        }

        public virtual DialogResultInfo ShowError(DialogShowInfo dialogShowInfo, AppMessageDisplayType displayType = AppMessageDisplayType.ModalDialog)
        {
            return MessageBoxDialog.DialogShow(AppContext, DialogIcon.Error, DialogButtons.Ok, dialogShowInfo);
        }

        public DialogResultInfo ShowInfo(DialogShowInfo dialogShowInfo)
        {
            return MessageBoxDialog.DialogShow(AppContext, DialogIcon.Info, DialogButtons.Ok, dialogShowInfo);
        }

        public void ShowOperationResult(string caption, OperationResult operationResult)
        {
            var dialogShowInfo = new DialogShowInfo(caption, operationResult.Message, operationResult.Detail);
            if (operationResult.IsWarning)
            {
                // ShowWarning(caption, operationResult.Message, operationResult.Detail, delayTimeout);
                ShowWarning(dialogShowInfo);
            }
            else if (operationResult.IsSuccess)
            {
                ShowInfo(dialogShowInfo);
            }
            else
            {
                ShowError(dialogShowInfo);
            }
        }

        public DialogResultInfo ShowWarning(DialogShowInfo dialogShowInfo)
        {
            return MessageBoxDialog.DialogShow(AppContext, DialogIcon.Warning, DialogButtons.Ok, dialogShowInfo);
        }

        public virtual AppSettingsDialogResult ShowAppSettings(AppSettingsDialogPage activePage = AppSettingsDialogPage.General)
        {
            return AppSettingsDialog.DialogShow(AppContext, activePage);
        }

        public void ShowImportResourcesDialog(IShellViewContext shellViewContext)
        {
            if (shellViewContext.ShellService.CheckProjectIsDirty())
            {
                using (var viewModel = new ImportDialogViewModel(shellViewContext))
                {
                    DialogWindow.DialogShow<ImportDialogViewModel, ImportDialog>(viewModel);
                }
            }
        }

        public void ShowExportResourcesDialog(IShellViewContext shellViewContext, bool recursive,
            List<string> resourceKeysToExport = null)
        {
            RootModelViewModel rootModel = shellViewContext.ShellViewModel.RootModel;
            if (rootModel.Children.Count == 0)
            {
                ShowError(new DialogShowInfo(Strings.Services.Export.ExportResourcesCaption,
                    Strings.Services.Export.NoResourcesToExport));
            }
            else
            {
                var fromRootModel = resourceKeysToExport == null;
                using (var viewModel = new ExportDialogViewModel(shellViewContext, fromRootModel, recursive, resourceKeysToExport))
                {
                    if (DialogWindow.DialogShow<ExportDialogViewModel, ExportDialog>(viewModel) == true)
                    { }
                }
            }
        }

        public void ShowProgressDialog<T>(T asyncOperation)
            where T : IAsyncOperation
        {
            if (asyncOperation == null)
            {
                throw new ArgumentNullException(nameof(asyncOperation));
            }

            using (var viewModel = new ProgressDialogViewModel(AppContext, asyncOperation))
            {
                DialogWindow.DialogShow<ProgressDialogViewModel, ProgressDialog>(viewModel);
            }
        }

        public void ShowExceptionDialog(Exception exception)
        {
            if (_exceptionDialogIsVisible.TrySetSignal())
            {
                try
                {
                    Logger.Error("Global exception", exception);
                    
#if DEBUG0
                    ExceptionDebugDialog.DialogShow(AppContext, exception);
#else
                    string caption = $"{Strings.App.Title} - {Strings.Common.Error}";
                    string message = Strings.Dialogs.UnhandledError.Message;
                    string detail = Strings.Dialogs.UnhandledError.Detail;
                    var dialogResultInfo = ShowConfirm(new DialogShowInfo(caption, message, detail), DialogButtons.YesNo, DialogIcon.Error);
                    if (dialogResultInfo.DialogResult == DialogResult.Yes)
                    {
                        ReportErrorToWeb(exception);
                    }
#endif
                }
                finally
                {
                    _exceptionDialogIsVisible.ResetSignal();
                }
            }
        }

        public bool ShowUpgradeModelDialog(string caption  = null, string title  = null)
        {
            caption = caption ?? Strings.Dialogs.UpgradeModel.UpgradeModelCaption;
            var latestVersion = ModelConstants.CurrentModelVersion;
            var latestVersionStr = $"v{latestVersion}";
            var message = Strings.Dialogs.UpgradeModel.UpgradeModelMessage(latestVersionStr);
            if (!string.IsNullOrEmpty(title))
            {
                message = $"{title}\n\n{message}";
            }
            string detail = Strings.Dialogs.UpgradeModel.UpgradeModelDetail(latestVersionStr, AppConstants.ModernGeneratorMinVersion);

            var dialogShowInfo = new DialogShowInfo(caption, message, detail)
            {
                ExtraButtonHeader = Strings.Common.ReadMore,
                ExtraButtonAction = () =>
                    {
                        WebPageUtils.ShowUrl(AppConstants.WebSiteUrls.GetLatestModelChanges());
                    }
            };
            
            return DialogService.ShowConfirm(dialogShowInfo).DialogResult == DialogResult.Yes;
        }

        private void ReportErrorToWeb(Exception exception)
        {
            string title = Uri.EscapeDataString("[BUG] [UE] " + exception.Message);
            var time = DateTimeHelper.DateTimeToUtcIsoString(DateTime.UtcNow);

            StringBuilder bodyBuilder = new StringBuilder();
            bodyBuilder.AppendLine("### Unhandled Exception");
            bodyBuilder.AppendLine();
            bodyBuilder.AppendLine($"`{exception.Message}`");
            bodyBuilder.AppendLine();

            bodyBuilder.AppendLine("**Occurred at**");
            bodyBuilder.AppendLine();
            bodyBuilder.AppendLine($"`{time}` (UTC)");
            bodyBuilder.AppendLine();
            
            bodyBuilder.AppendLine("**Full Exception**");
            bodyBuilder.AppendLine();
            bodyBuilder.Append(@"```");
            bodyBuilder.AppendLine(exception.GetFullExceptionDetails());
            bodyBuilder.AppendLine(@"```");
            bodyBuilder.AppendLine();
            bodyBuilder.AppendLine("**Environment**");

            foreach (var pair in ApplicationService.GetEnvironmentInfo())
            {
                bodyBuilder.AppendLine();
                bodyBuilder.AppendLine($"**{pair.Key}**");

                bodyBuilder.AppendLine();
                bodyBuilder.AppendLine($"{pair.Value}");
            }

            bodyBuilder.AppendLine();
            bodyBuilder.AppendLine("**Extension Version**");
            bodyBuilder.AppendLine();
            bodyBuilder.AppendLine($"`{AppContext.AppCurrentVersion}`");
            bodyBuilder.AppendLine();

            string body = Uri.EscapeDataString(bodyBuilder.ToString());
            string url = AppConstants.WebSiteUrls.GetReportBug(title, body);

            WebPageUtils.ShowUrl(url);
        }

        public void ShowPluginHelp(IPluginModule pluginModule)
        {
            var helpItems = pluginModule.GetHelpItems();
            string header = Strings.Dialogs.PluginHelp.Title(pluginModule.DisplayName);
            string dialogTitle = Strings.Dialogs.PluginHelp.DialogTitle;

            using (var viewModel = new HelpItemsDialogViewModel(AppContext, helpItems, dialogTitle, header))
            {
                DialogWindow.DialogShow<HelpItemsDialogViewModel, HelpItemsDialog>(viewModel);
            }
        }
    }
}
