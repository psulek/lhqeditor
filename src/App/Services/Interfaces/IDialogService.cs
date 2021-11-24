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
using LHQ.App.Code;
using LHQ.App.Model;
using LHQ.App.ViewModels.Dialogs.AppSettings;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Core.Model;
// ReSharper disable UnusedMember.Global

namespace LHQ.App.Services.Interfaces
{
    public interface IDialogService : IAppContextService
    {
        DialogResult ShowConfirm(string caption, string message, string detail,
            DialogButtons buttons = DialogButtons.YesNo, DialogIcon dialogIcon = DialogIcon.Question,
            string cancelButtonHeader = null, string yesButtonHeader = null, string noButtonHeader = null);

        DialogResult ShowConfirmRemember(string caption, string message, string detail,
            DialogButtons buttons, ref bool rememberChecked, string rememberText,
            string rememberHint = null, DialogIcon dialogIcon = DialogIcon.Question,
            string cancelButtonHeader = null);

        bool ShowPrompt(string caption, string message, ref string input);

        void ShowError(string caption, string message, string detail, TimeSpan? delayTimeout = null, 
            AppMessageDisplayType displayType = AppMessageDisplayType.ModalDialog);

        void ShowInfo(string caption, string message, string detail, TimeSpan? delayTimeout = null);

        void ShowOperationResult(string caption, OperationResult operationResult, TimeSpan? delayTimeout = null);

        void ShowWarning(string caption, string message, string detail, TimeSpan? delayTimeout = null);

        AppSettingsDialogResult ShowAppSettings(AppSettingsDialogPage activePage = AppSettingsDialogPage.General);

        void ShowImportResourcesDialog(IShellViewContext shellViewContext);

        void ShowExportResourcesDialog(IShellViewContext shellViewContext, bool recursive,
            List<string> resourceKeysToExport = null);

        void ShowProgressDialog<T>(T asyncOperation)
            where T : IAsyncOperation;

        void ShowPluginHelp(IPluginModule pluginModule);

        void ShowExceptionDialog(Exception exception);
    }
}
