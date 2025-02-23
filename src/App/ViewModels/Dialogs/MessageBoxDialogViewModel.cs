#region License
// Copyright (c) 2021 Peter �ulek / ScaleHQ Solutions s.r.o.
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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Utils.Extensions;

namespace LHQ.App.ViewModels.Dialogs
{
    public class MessageBoxDialogViewModel : DialogViewModelBase
    {
        private readonly DialogButtons _buttons;
        private bool _isChecked;
        private readonly string _caption;
        private readonly Action _extraButtonAction;

        public MessageBoxDialogViewModel(IAppContext appContext,
            string message, string caption, string detail, DialogIcon icon,
            DialogButtons buttons, bool? checkValue, string checkHeader, string checkHint,
            string cancelButtonHeader = null, string yesButtonHeader = null, string noButtonHeader = null,
            string extraButtonHeader = null, Action extraButtonAction = null)
            : base(appContext)
        {
            _caption = caption;
            Message = message;
            Detail = detail;
            Icon = icon;
            _buttons = buttons;

            _extraButtonAction = extraButtonAction;
            ExtraButtonVisibility = string.IsNullOrEmpty(extraButtonHeader) ? Visibility.Collapsed : Visibility.Visible;
            ExtraButtonVisibilityHeader = extraButtonHeader;
            if (!string.IsNullOrEmpty(extraButtonHeader))
            {
                ExtraButtonVisibilityCommand = new DelegateCommand(ExtraButtonExecute);
            }
            
            IsChecked = checkValue.HasValue && checkValue.Value;
            CheckPanelVisibility = checkValue.HasValue ? Visibility.Visible : Visibility.Collapsed;
            CheckHeader = checkValue.HasValue ? checkHeader : string.Empty;
            CheckHintText = checkValue.HasValue ? checkHint : string.Empty;
            CheckHintVisibility = checkValue.HasValue && !checkHint.IsNullOrEmpty()
                ? Visibility.Visible
                : Visibility.Collapsed;

            DetailVisibility = Detail.IsNullOrEmpty() ? Visibility.Collapsed : Visibility.Visible;

            bool isYesNoCancel = buttons == DialogButtons.YesNoCancel;
            NoButtonVisibility = isYesNoCancel
                ? Visibility.Visible
                : Visibility.Collapsed;

            if (isYesNoCancel)
            {
                NoButtonCommand = new DelegateCommand(NoButtonExecute);
            }

            CancelButtonVisibility = buttons == DialogButtons.Ok ? Visibility.Collapsed : Visibility.Visible;

            if (cancelButtonHeader.IsNullOrEmpty())
            {
                CancelButtonHeader = buttons == DialogButtons.YesNo
                    ? Strings.ViewModels.MessageBox.LabelNo
                    : Strings.ViewModels.MessageBox.LabelCancel;
            }
            else
            {
                CancelButtonHeader = cancelButtonHeader;
            }

            bool hasYesNo = buttons.In(DialogButtons.YesNo, DialogButtons.YesNoCancel);

            if (yesButtonHeader.IsNullOrEmpty())
            {
                YesOkButtonHeader = hasYesNo
                    ? Strings.ViewModels.MessageBox.LabelYes
                    : Strings.ViewModels.MessageBox.LabelOk;
            }
            else
            {
                YesOkButtonHeader = yesButtonHeader;
            }

            NoButtonHeader = noButtonHeader.IsNullOrEmpty() ? Strings.ViewModels.MessageBox.LabelNo : noButtonHeader;

            Result = Model.DialogResult.None;
        }

        private void ExtraButtonExecute(object obj)
        {
            _extraButtonAction?.Invoke();
        }

        public DialogResult Result { get; private set; }

        public string Message { get; }

        public string Detail { get; }

        public DialogIcon Icon { get; }

        public string ExtraButtonVisibilityHeader { get; }
        
        public ICommand ExtraButtonVisibilityCommand { get; }
        
        public Visibility ExtraButtonVisibility { get; }
        
        public Visibility NoButtonVisibility { get; }

        public string YesOkButtonHeader { get; }

        public string NoButtonHeader { get; }

        public ICommand NoButtonCommand { get; }

        public Visibility CancelButtonVisibility { get; }

        public string CancelButtonHeader { get; }

        public Visibility DetailVisibility { get; }

        public Visibility CheckPanelVisibility { get; }

        public string CheckHeader { get; }

        public string CheckHintText { get; }

        public Visibility CheckHintVisibility { get; }

        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }

        private void NoButtonExecute(object obj)
        {
            Result = Model.DialogResult.No;
            CloseDialog();
        }

        protected override void OnClosingWindow(CancelEventArgs e)
        {
            SetCancelResult();
        }

        private void SetCancelResult()
        {
            if (DialogResult == null && Result == Model.DialogResult.None)
            {
                Result = Model.DialogResult.Cancel;
            }
        }

        protected override void CancelDialog()
        {
            SetCancelResult();
            base.CancelDialog();
        }

        protected override string GetTitle() => _caption;

        public override bool SubmitDialog()
        {
            switch (_buttons)
            {
                case DialogButtons.Ok:
                case DialogButtons.OkCancel:
                {
                    Result = Model.DialogResult.OK;
                    break;
                }
                case DialogButtons.YesNo:
                case DialogButtons.YesNoCancel:
                {
                    Result = Model.DialogResult.Yes;
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            return true;
        }
    }
}
