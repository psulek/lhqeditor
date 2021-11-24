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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using LHQ.App.Code;
using LHQ.App.Extensions;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.App.Components
{
    public class DropDownButton : ToggleButton
    {
        /// <summary>
        ///     The dropdown context menu dependency property
        /// </summary>
        public static readonly DependencyProperty DropDownContextMenuProperty =
            DependencyProperty.Register("DropDownContextMenu", typeof(ContextMenu), typeof(DropDownButton), new UIPropertyMetadata(null));

        /// <summary>
        ///     The image dependency property
        /// </summary>
        public static readonly DependencyProperty ImageProperty = 
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(DropDownButton));

        public static readonly DependencyProperty ContentControlVisibleProperty =
            DependencyProperty.Register("ContentControlVisible", typeof(bool), typeof(DropDownButton));

        /// <summary>
        ///     The text dependency property
        /// </summary>
        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register("Text", typeof(string), typeof(DropDownButton));

        /// <summary>
        ///     The target dependency property
        /// </summary>
        public static readonly DependencyProperty TargetProperty = 
            DependencyProperty.Register("Target", typeof(UIElement),
            typeof(DropDownButton));

        public static readonly DependencyProperty ArrowVisibilityProperty = 
            DependencyProperty.Register("ArrowVisibility",
            typeof(Visibility), typeof(DropDownButton), new UIPropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty ImageVisibleProperty = 
            DependencyProperty.Register("ImageVisible",
            typeof(bool), typeof(DropDownButton), new UIPropertyMetadata(true));

        /// <summary>
        ///     The drop down button command dependency property
        /// </summary>
        public static readonly DependencyProperty DropDownButtonCommandProperty =
            DependencyProperty.Register("DropDownButtonCommand", typeof(ICommand), typeof(DropDownButton), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty EllipsisVisibilityProperty =
            DependencyProperty.Register("EllipsisVisibility", typeof(Visibility), typeof(DropDownButton),
                new FrameworkPropertyMetadata(Visibility.Collapsed));

        /// <summary>
        ///     Constructor
        /// </summary>
        public DropDownButton()
        {
            this.AddStyleFromResources(ResourcesConsts.DropDownButtonStyle);

            // Bind the ToogleButton.IsChecked property to the drop-down's IsOpen property 
            var binding = new Binding("DropDownContextMenu.IsOpen") { Source = this };
            SetBinding(IsCheckedProperty, binding);
        }

        /// <summary>
        ///     The dropdown context menu property
        /// </summary>
        public ContextMenu DropDownContextMenu
        {
            get => GetValue(DropDownContextMenuProperty) as ContextMenu;
            set => SetValue(DropDownContextMenuProperty, value);
        }

        /// <summary>
        ///     The image property
        /// </summary>
        public ImageSource Image
        {
            get => GetValue(ImageProperty) as ImageSource;
            set => SetValue(ImageProperty, value);
        }

        /// <summary>
        ///     The text property
        /// </summary>
        public string Text
        {
            get => GetValue(TextProperty) as string;
            set => SetValue(TextProperty, value);
        }

        public bool ContentControlVisible
        {
            get => (bool)GetValue(ContentControlVisibleProperty);
            set => SetValue(ContentControlVisibleProperty, value);
        }

        public Visibility ArrowVisibility
        {
            get => (Visibility)GetValue(ArrowVisibilityProperty);
            set => SetValue(ArrowVisibilityProperty, value);
        }

        public bool ImageVisible
        {
            get => (bool)GetValue(ImageVisibleProperty);
            set => SetValue(ImageVisibleProperty, value);
        }

        public Visibility EllipsisVisibility
        {
            get => (Visibility)GetValue(EllipsisVisibilityProperty);
            set => SetValue(EllipsisVisibilityProperty, value);
        }

        /// <summary>
        ///     The target property
        /// </summary>
        public UIElement Target
        {
            get => GetValue(TargetProperty) as UIElement;
            set => SetValue(TargetProperty, value);
        }

        /// <summary>
        ///     The dropdown button command property
        /// </summary>
        public ICommand DropDownButtonCommand
        {
            get => GetValue(DropDownButtonCommandProperty) as ICommand;
            set => SetValue(DropDownButtonCommandProperty, value);
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == DropDownButtonCommandProperty)
            {
                Command = DropDownButtonCommand;
            }
        }

        /// <inheritdoc />
        protected override void OnClick()
        {
            if (DropDownContextMenu == null)
            {
                return;
            }

            DropDownButtonCommand?.Execute(null);

            // If there is a drop-down assigned to this button, then position and display it 
            DropDownContextMenu.PlacementTarget = this;
            DropDownContextMenu.Placement = PlacementMode.Bottom;
            DropDownContextMenu.IsOpen = !DropDownContextMenu.IsOpen;
        }
    }
}
