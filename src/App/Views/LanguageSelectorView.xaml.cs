// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using LHQ.App.ViewModels;

namespace LHQ.App.Views
{
    public partial class LanguageSelectorView
    {
        public LanguageSelectorView()
        {
            InitializeComponent();
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // if (sender is ComboBox comboBox)
            // {
            //     var viewModel = (LanguageSelectorViewModel)comboBox.DataContext;
            //     var searchEdit = (TextBox)comboBox.Template.FindName("PART_EditableTextBox", comboBox);
            //     string searchText = string.Empty;
            //     int selectionStart = searchEdit.SelectionStart;
            //     if (selectionStart > 0)
            //     {
            //         searchText = searchEdit.Text.Substring(0, selectionStart) + e.Text;
            //     }
            //
            //     if (!viewModel.Cultures.Any(
            //         x => x.CultureDisplayName.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) > -1))
            //     {
            //         e.Handled = true;
            //     }
            // }
        }
    }
}
