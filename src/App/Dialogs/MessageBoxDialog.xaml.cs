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

using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Dialogs;

namespace LHQ.App.Dialogs
{
    /// <summary>
    ///     Interaction logic for MessageBoxDialog.xaml
    /// </summary>
    public partial class MessageBoxDialog
    {
        public MessageBoxDialog()
        {
            InitializeComponent();
        }

        public static Result DialogShow(IAppContext appContext, string message, string caption,
            DialogIcon icon, DialogButtons buttons, string detail, bool? checkValue, 
            string checkHeader, string checkHint, string cancelButtonHeader = null, 
            string yesButtonHeader = null, string noButtonHeader = null)
        {
            using (var viewModel = new MessageBoxDialogViewModel(appContext, message,
                caption, detail, icon, buttons, checkValue, checkHeader, checkHint, 
                cancelButtonHeader, yesButtonHeader, noButtonHeader))
            {
                DialogShow<MessageBoxDialogViewModel, MessageBoxDialog>(viewModel);
                return new Result(viewModel.Result, viewModel.IsChecked);
            }
        }

        public class Result
        {
            public Result(DialogResult dialogResult, bool @checked)
            {
                DialogResult = dialogResult;
                Checked = @checked;
            }

            public DialogResult DialogResult { get; }

            public bool Checked { get; }
        }
    }
}
