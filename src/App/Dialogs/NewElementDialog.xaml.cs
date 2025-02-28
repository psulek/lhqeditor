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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Dialogs;

namespace LHQ.App.Dialogs
{
    /// <summary>
    ///     Interaction logic for NewCategoryDialog.xaml
    /// </summary>
    public partial class NewElementDialog
    {
        public NewElementDialog()
        {
            InitializeComponent();
        }

        private NewElementDialogViewModel DialogViewModel => (NewElementDialogViewModel)DataContext;

        public static string DialogShow(IShellViewContext shellViewContext, ICategoryLikeViewModel parentElement,
            List<string> parentNames, bool createResource)
        {
            using (var viewModel =
                new NewElementDialogViewModel(shellViewContext, parentElement, parentNames, createResource))
            {
                bool? dialogResult = DialogShow<NewElementDialogViewModel, NewElementDialog>(viewModel);
                return dialogResult == true ? viewModel.ElementName.Trim() : null;
            }
        }

        public override void OnApplyTemplate()
        {
            List<string> parentRecursiveNames = DialogViewModel.ParentNames;
            TextBlockParentNames.Inlines.Clear();
            if (parentRecursiveNames != null && parentRecursiveNames.Count > 0)
            {
                for (var i = 0; i < parentRecursiveNames.Count; i++)
                {
                    TextBlockParentNames.Inlines.Add(parentRecursiveNames[i]);

                    if (i + 1 < parentRecursiveNames.Count)
                    {
                        TextBlockParentNames.Inlines.Add(new Run(" • ") { FontWeight = FontWeights.Bold });
                    }
                }
            }
        }
    }
}
