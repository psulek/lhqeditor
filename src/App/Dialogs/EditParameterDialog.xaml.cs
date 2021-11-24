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

using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels;
using LHQ.App.ViewModels.Dialogs;
using LHQ.Utils;

namespace LHQ.App.Dialogs
{
    /// <summary>
    ///     Interaction logic for EditParameterDialog.xaml
    /// </summary>
    public partial class EditParameterDialog
    {
        public EditParameterDialog()
        {
            InitializeComponent();
        }

        public static Result DialogShow(IAppContext appContext, ObservableCollectionExt<EditResourceParameterViewModel> parameters,
            EditResourceParameterViewModel parameter)
        {
            using (var viewModel = new EditParameterDialogViewModel(appContext, parameters, parameter))
            {
                bool submitted = DialogShow<EditParameterDialogViewModel, EditParameterDialog>(viewModel) == true;
                return new Result(submitted, viewModel.Name?.Trim(), viewModel.Description?.Trim(), viewModel.Order);
            }
        }

        public class Result
        {
            public Result(bool submitted, string name, string description, int order)
            {
                Submitted = submitted;
                Name = name;
                Description = description;
                Order = order;
            }

            public bool Submitted { get; }

            public string Name { get; }

            public string Description { get; }

            public int Order { get; }
        }
    }
}
