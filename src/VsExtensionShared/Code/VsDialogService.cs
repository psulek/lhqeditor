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
using System.Diagnostics.CodeAnalysis;
using LHQ.App.Model;
using LHQ.App.Services.Implementation;
using LHQ.App.ViewModels.Dialogs.AppSettings;
using LHQ.Utils.Extensions;

namespace LHQ.VsExtension.Code
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class VsDialogService: DialogService
    {
        public override DialogResultInfo ShowError(DialogShowInfo dialogShowInfo, AppMessageDisplayType displayType = AppMessageDisplayType.ModalDialog)
        {
            if (displayType == AppMessageDisplayType.HostDialog)
            {
                var msg = dialogShowInfo.Message;
                if (!dialogShowInfo.Detail.IsNullOrEmpty())
                {
                    msg = $"{msg}, {dialogShowInfo.Detail}";
                }
                
                VsPackageService.AddMessageToOutput(msg, OutputMessageType.Error);
                
                return DialogResultInfo.OK;
            }

            return base.ShowError(dialogShowInfo, displayType);
        }

        public override AppSettingsDialogResult ShowAppSettings(AppSettingsDialogPage activePage = AppSettingsDialogPage.General)
        {
            if (AppContext.AppStartedFirstTime)
            {
                AppContext.UIService.DispatchActionOnUI(() =>
                    {
                        AppContext.UnsetAppStartedFirstTime();

                        /*string caption = App.Localization.Strings.VsExtension.FirstTimeOpenSettings.Caption;
                        string message = App.Localization.Strings.VsExtension.FirstTimeOpenSettings.Message;
                        string detail = App.Localization.Strings.VsExtension.FirstTimeOpenSettings.Detail;

                        var dialogShowInfo = new DialogShowInfo(caption, message, detail);
                        
                        if (AppContext.DialogService.ShowConfirm(dialogShowInfo).DialogResult == DialogResult.Yes)
                        {
                            VsPackageService.CloseOpenedEditors();
                            VsPackageService.ShowOptionsPage(activePage, AppContext.AppStartedFirstTime);
                        }*/
                    });
            }
            else
            {
                VsPackageService.ShowOptionsPage(activePage, false);
            }

            return new AppSettingsDialogResult(false, null);
        }
    }
}
