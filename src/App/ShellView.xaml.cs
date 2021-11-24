// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
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
