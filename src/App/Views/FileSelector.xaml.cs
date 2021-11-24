// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System.Windows;
using System.Windows.Input;

namespace LHQ.App.Views
{
    public partial class FileSelector
    {
        public static DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(FileSelector));

        public static DependencyProperty BrowseCommandProperty =
            DependencyProperty.Register("BrowseCommand", typeof(ICommand), typeof(FileSelector));

        public static DependencyProperty BrowseButtonTooltipProperty =
            DependencyProperty.Register("BrowseButtonTooltip", typeof(string), typeof(FileSelector));

        public static DependencyProperty IsReadonlyProperty =
            DependencyProperty.Register("IsReadonly", typeof(bool), typeof(FileSelector));

        public FileSelector()
        {
            InitializeComponent();
        }

        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }

        public ICommand BrowseCommand
        {
            get => (ICommand)GetValue(BrowseCommandProperty);
            set => SetValue(BrowseCommandProperty, value);
        }

        public string BrowseButtonTooltip
        {
            get => (string)GetValue(BrowseButtonTooltipProperty);
            set => SetValue(BrowseButtonTooltipProperty, value);
        }

        public bool IsReadonly
        {
            get => (bool)GetValue(IsReadonlyProperty);
            set => SetValue(IsReadonlyProperty, value);
        }
    }
}
