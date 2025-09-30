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
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Components.VirtualTreeListViewControl;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using Syncfusion.Windows.Controls.Input;
using Syncfusion.Windows.Tools.Controls;

namespace LHQ.App
{
    public partial class ShellView
    {
        private const string LayoutFileName = "layout.json";
        private string _storageFileName;
        private int _lastValidationErrorCount = -1;

        public ShellView()
        {
            InitializeComponent();

            EventManager.RegisterClassHandler(
                typeof(UIElement),
                Keyboard.PreviewGotKeyboardFocusEvent,
                (KeyboardFocusChangedEventHandler)HandlePreviewGotKeyboardFocus);

            VisualManager.Instance.ApplyTheme(this);

            DataContextChanged += OnDataContextChanged;
        }

        private void HandlePreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (DataContext is ShellViewModel shellViewModel)
            {
                shellViewModel.IsTreeViewFocused = e.NewFocus is VirtualTreeListViewItem;
            }
        }

        private string StorageFileName => _storageFileName ?? (_storageFileName = AppContext.FileSystemService.GetDataFileName(LayoutFileName));

        protected override bool HandleDispatcherShutdown => true;

        public void AppClose()
        {
            SimulateAppClose();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is ShellViewModel shellViewModel)
            {
                IValidatorContext validatorContext = shellViewModel.ValidatorContext;
                validatorContext.PropertyChanged += ValidatorContextOnPropertyChanged;
            }
        }

        private void ValidatorContextOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (sender is IValidatorContext validatorContext)
            {
                int errorsCount = validatorContext.Errors.Count;
                if (errorsCount != _lastValidationErrorCount)
                {
                    DockingManager.SetHeader(DocItemValidationPanelView, $"Validations errors ({errorsCount})");

                    if (validatorContext.HasErrors)
                    {
                        DockingManager.SetState(DocItemValidationPanelView, DockState.Dock);
                        DockingManager.SetCanClose(DocItemValidationPanelView, false);
                    }
                    else
                    {
                        DockingManager.SetCanClose(DocItemValidationPanelView, true);
                        DockingManager.SetState(DocItemValidationPanelView, DockState.Hidden);
                    }

                    _lastValidationErrorCount = errorsCount;
                }
            }
        }

        protected override void OnAppClosing()
        {
            try
            {
                ShellViewLayout shellViewLayout = CreateShellViewLayout();
                //DebugUtils.Log("Saving dockingManager state to: " + StorageFileName);
                FileUtils.WriteAllText(StorageFileName, JsonUtils.ToJsonString(shellViewLayout));

                string msg = AppContext.RunInVsPackage
                    ? "Editor Instance Ended"
                    : "Application Ended.";
                AppContext?.Logger?.Info(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void HandleLoaded(object sender, RoutedEventArgs args)
        {
            if (File.Exists(StorageFileName))
            {
                try
                {
                    //DebugUtils.Log("Loading dockingManager state from: " + StorageFileName);
                    string json = FileUtils.ReadAllText(StorageFileName);

                    var shellViewLayout = JsonUtils.FromJsonString<ShellViewLayout>(json);
                    if (shellViewLayout != null)
                    {
                        DockingManager.SetDesiredWidthInDockedMode(DockItemTreeView, shellViewLayout.TreeViewWidth);

                        double validationPanelHeight = shellViewLayout.ValidationPanelHeight;
                        if (validationPanelHeight <= 0)
                        {
                            validationPanelHeight = 150;
                        }
                        DockingManager.SetDesiredHeightInDockedMode(DocItemValidationPanelView, validationPanelHeight);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private ShellViewLayout CreateShellViewLayout()
        {
            double treeViewWidth = DockingManager.GetDesiredWidthInDockedMode(DockItemTreeView);
            double validationPanelHeight = DockingManager.GetDesiredHeightInDockedMode(DocItemValidationPanelView);

            return new ShellViewLayout
            {
                TreeViewWidth = treeViewWidth,
                ValidationPanelHeight = validationPanelHeight
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            EventManager.RegisterClassHandler(typeof(SfTextBoxExt), UnloadedEvent,
                new RoutedEventHandler(HandleTextBoxUnloadedEvent));
        }

        private void HandleTextBoxUnloadedEvent(object sender, RoutedEventArgs args)
        {
            args.Handled = true;
        }
    }

    public class ShellViewLayout
    {
        public double TreeViewWidth { get; set; }

        public double ValidationPanelHeight { get; set; }
    }
}
