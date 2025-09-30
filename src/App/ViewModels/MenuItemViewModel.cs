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
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FontAwesome.WPF;
using LHQ.App.Code;
using LHQ.App.Components;
using LHQ.App.Model;
using LHQ.Utils;
using LHQ.Utils.Extensions;

namespace LHQ.App.ViewModels
{
    public class MenuItemViewModel : ObservableObject
    {
        private ObservableCollectionExt<MenuItemViewModel> _menuItems;
        private string _header;
        private string _imageUri;
        private string _gestureText;
        private bool _isCheckable;
        private ICommand _command;
        private object _commandParameter;
        private bool _isEnabled;
        private Image _normalImage;
        private Image _disabledImage;
        private bool _visible;
        private Func<bool> _getVisibleFn;
        private Func<ImageSource> _getImageSourceFn;
        private object _tag;
        private bool _imageVisible;
        private string _toolTip;

        public MenuItemViewModel()
        {
            MenuItems = new ObservableCollectionExt<MenuItemViewModel>();
            IsEnabled = true;
            Visible = true;
            ImageVisible = true;
        }

        public ObservableCollectionExt<MenuItemViewModel> MenuItems
        {
            get => _menuItems;
            set => SetProperty(ref _menuItems, value);
        }

        public Func<bool> GetVisible
        {
            get => _getVisibleFn;
            set
            {
                _getVisibleFn = value;
                RaisePropertyChanged(nameof(Visible));
            }
        }

        public Func<ImageSource> GetImageSource
        {
            get => _getImageSourceFn;
            set
            {
                _getImageSourceFn = value;
                RaisePropertyChanged(nameof(Image));
            }
        }

        public bool Visible
        {
            get => _getVisibleFn?.Invoke() ?? _visible;
            set => SetProperty(ref _visible, value);
        }

        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }

        public string ToolTip
        {
            get => _toolTip;
            set => SetProperty(ref _toolTip, value);
        }

        public string ImageUri
        {
            get => _imageUri;
            set => SetProperty(ref _imageUri, value);
        }

        public Image Image => GetImage();

        public string GestureText
        {
            get => _gestureText;
            set => SetProperty(ref _gestureText, value);
        }

        public bool IsCheckable
        {
            get => _isCheckable;
            set => SetProperty(ref _isCheckable, value);
        }

        public bool ImageVisible
        {
            get => _imageVisible;
            set => SetProperty(ref _imageVisible, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            private set => SetProperty(ref _isEnabled, value);
        }

        public object Tag
        {
            get => _tag;
            set => SetProperty(ref _tag, value);
        }

        public ICommand Command
        {
            get => _command;
            set
            {
                if (SetProperty(ref _command, value))
                {
                    _command.CanExecuteChanged += CommandOnCanExecuteChanged;
                }
            }
        }

        public object CommandParameter
        {
            get => _commandParameter;
            set => SetProperty(ref _commandParameter, value);
        }

        public MenuItemViewModel Clone()
        {
            var cloned = new MenuItemViewModel
            {
                Header = Header,
                Command = Command,
                GestureText = GestureText,
                ImageUri = ImageUri,
                Visible = Visible,
                GetVisible = GetVisible,
                CommandParameter = CommandParameter,
                IsCheckable = IsCheckable,
                IsEnabled = IsEnabled,
                GetImageSource = GetImageSource
            };

            if (MenuItems != null)
            {
                cloned.MenuItems.AddRange(MenuItems.Select(x => x.Clone()));
            }

            return cloned;
        }

        public void SetAwesomeImage(FontAwesomeIcon awesomeIcon, SolidColorBrush solidColorBrush)
        {
            GetImageSource = () => ImageAwesome.CreateImageSource(awesomeIcon, solidColorBrush);
            ImageUri = null;
        }

        private Image GetImage(bool force = false)
        {
            if (_imageUri.IsNullOrEmpty() && _getImageSourceFn == null)
            {
                return null;
            }

            Image image = IsEnabled ? _normalImage : _disabledImage;

            if (image == null || force)
            {
                if (IsEnabled)
                {
                    image = new Image
                    {
                        Height = 16,
                        Width = 16,
                        SnapsToDevicePixels = true,
                        Source = _getImageSourceFn == null 
                        ? ResourceHelper.GetImageSource(_imageUri)
                        : _getImageSourceFn()
                    };
                }
                else
                {
                    image = _getImageSourceFn == null
                        ? AutoGreyableImage.GetGrayscaleImage(_imageUri)
                        : AutoGreyableImage.ConvertToGray(_getImageSourceFn());
                    image.Height = 16;
                    image.Width = 16;
                }

                if (IsEnabled)
                {
                    _normalImage = image;
                }
                else
                {
                    _disabledImage = image;
                }
            }

            return image;
        }

        private void CommandOnCanExecuteChanged(object sender, EventArgs eventArgs)
        {
            IsEnabled = _command.CanExecute(CommandParameter);
            GetImage(true);
            RaisePropertyChanged(nameof(Image));
        }

        protected override void OnDispose()
        {
            if (_command != null)
            {
                _command.CanExecuteChanged -= CommandOnCanExecuteChanged;
                _command = null;
            }
        }
    }
}
