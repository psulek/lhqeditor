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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LHQ.App.Behaviors;
using LHQ.App.Code;
using LHQ.App.Extensions;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Dialogs;
using LHQ.Utils.Utilities;
using Microsoft.Xaml.Behaviors;

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.App.Dialogs
{
    public class DialogWindow : Window
    {
        public DialogWindow()
        {
            Loaded += OnLoaded;
        }

        protected virtual bool HideMinimizeButton => true;

        protected virtual bool HideMaximizeButton => true;

        protected virtual bool HideCloseButton => false;

        protected DialogViewModelBase ViewModel => DataContext as DialogViewModelBase;

        protected IAppContext AppContext => ViewModel?.AppContext;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.HideMinimizeAndMaximizeButtons(HideMinimizeButton, HideMaximizeButton, HideCloseButton);

            ShowInTaskbar = false;
            WindowStyle = WindowStyle.ToolWindow;

            VisualManager.Instance.ApplyTheme(this);
        }

        protected void ReapplyButtons()
        {
            this.HideMinimizeAndMaximizeButtons(HideMinimizeButton, HideMaximizeButton, HideCloseButton);
        }

        private void OnLoaded(object o, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            InternalOnLoaded();

            FontSize = VisualManager.Instance.FontSize;
            FontFamily = VisualManager.Instance.FontFamily;

            if (!(VisualTreeHelper.GetChild(this, 0) is Border mainBorder))
            {
                throw new InvalidOperationException("Dialog content must be wrapped in top level Border!");
            }

            mainBorder.Padding = new Thickness(15);
            mainBorder.Background = VisualManager.Instance.ContentBackgroundBrush;
        }

        protected virtual void InternalOnLoaded()
        { }

        public static bool? DialogShow<TDialogViewModel, TDialogWindow>(TDialogViewModel dialogViewModel, 
            bool manualBehaviors = false)
            where TDialogViewModel : DialogViewModelBase
            where TDialogWindow : DialogWindow, new()
        {
            ArgumentValidator.EnsureArgumentNotNull(dialogViewModel, "dialogViewModel");

            Application application = Application.Current;
            Window ownerWindow = application?.MainWindow != null ? GetWindow(application.MainWindow) : null;
            var dialog = new TDialogWindow
            {
                Owner = ownerWindow,
                DataContext = dialogViewModel
            };

            if (manualBehaviors)
            {
                var behaviors = Interaction.GetBehaviors(dialog);
                behaviors.Add(new EventToCommandBehavior
                {
                    Event = "Loaded",
                    PassArguments = true,
                    Command = dialogViewModel.LoadedWindowCommand
                });
            }

            dialog.Closed += (sender, args) => { dialog.DataContext = null; };

            return dialog.ShowDialog();
        }
    }
}
