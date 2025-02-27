﻿#region License
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
using System.Linq;
using System.Windows;
using LHQ.App.Code;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Dialogs;

namespace LHQ.App.Dialogs
{
    /// <summary>
    ///     Interaction logic for LanguageSettingsDialog.xaml
    /// </summary>
    public partial class LanguageSettingsDialog
    {
        public LanguageSettingsDialog()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (FindResource("ListItemStyle") is Style listItemStyle)
            {
                listItemStyle.BasedOn = VisualManager.Instance.DefaultListViewItemStyle;
            }

            if (FindResource("GridViewColumnHeaderStyle") is Style gridViewColumnHeaderStyle)
            {
                gridViewColumnHeaderStyle.BasedOn = VisualManager.Instance.DefaultGridViewColumnHeaderStyle;
            }
        }

        public static bool DialogShow(IShellViewContext shellViewContext, out List<string> allLanguages,
            out Dictionary<string, string> replacedLanguages, out string primaryLanguageName)
        {
            allLanguages = null;
            replacedLanguages = null;
            primaryLanguageName = null;

            using (var viewModel = new LanguageSettingsDialogViewModel(shellViewContext))
            {
                bool? dialogResult = DialogShow<LanguageSettingsDialogViewModel, LanguageSettingsDialog>(viewModel);
                bool result = dialogResult == true;
                if (result)
                {
                    allLanguages = viewModel.Languages.Select(x => x.Name).ToList();
                    replacedLanguages = viewModel.GetReplacedLanguages();
                    primaryLanguageName =
                        viewModel.Languages.Where(x => x.IsPrimary).Select(x => x.Name).Single();
                }

                return result;
            }
        }
    }
}
