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

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LHQ.App.Code;
// ReSharper disable UnusedMember.Global

namespace LHQ.App.Components
{
    // source from: https://stackoverflow.com/a/11356845/316886
    /// <summary>
    ///     Class used to have an image that is able to be gray when the control is not enabled.
    ///     Author: Thomas LEBRUN (http://blogs.developpeur.org/tom)
    /// </summary>
    public class AutoGreyableImage : Image
    {
        public static readonly DependencyProperty SourceExtProperty =
            DependencyProperty.RegisterAttached(nameof(SourceExt), typeof(ImageSource), typeof(AutoGreyableImage),
                new UIPropertyMetadata(null, OnSourceExtChanged));

        public AutoGreyableImage()
        {
            ToolTipService.SetShowOnDisabled(this, true);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AutoGreyableImage" /> class.
        /// </summary>
        static AutoGreyableImage()
        {
            // Override the metadata of the IsEnabled property.
            IsEnabledProperty.OverrideMetadata(typeof(AutoGreyableImage), new FrameworkPropertyMetadata(true, OnIsEnabledPropertyChanged));
        }

        public ImageSource SourceExt
        {
            get => (ImageSource)GetValue(SourceExtProperty);
            set => SetValue(SourceExtProperty, value);
        }

        private static void OnSourceExtChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            if (source is AutoGreyableImage autoGreyScaleImg)
            {
                UpdateImage(autoGreyScaleImg, autoGreyScaleImg.IsEnabled);
            }
        }

        /// <summary>
        ///     Called when [auto grey scale image is enabled property changed].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        private static void OnIsEnabledPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var autoGreyScaleImg = source as AutoGreyableImage;
            bool isEnabled = Convert.ToBoolean(args.NewValue);
            UpdateImage(autoGreyScaleImg, isEnabled);
        }

        private static void UpdateImage(AutoGreyableImage autoGreyScaleImg, bool isEnabled)
        {
            if (autoGreyScaleImg != null)
            {
                if (isEnabled)
                {
                    var isFormatConvertedBitmap = autoGreyScaleImg.Source is FormatConvertedBitmap;
                    if (autoGreyScaleImg.Source == null || !isFormatConvertedBitmap)
                    {
                        if (autoGreyScaleImg.SourceExt !=null)
                        {
                            autoGreyScaleImg.Source = new BitmapImage(new Uri(autoGreyScaleImg.SourceExt.ToString()));
                        }
                    }
                    else
                    {
                        // Set the Source property to the original value.
                        autoGreyScaleImg.Source = ((FormatConvertedBitmap)autoGreyScaleImg.Source).Source;
                        // Reset the Opcity Mask
                        autoGreyScaleImg.OpacityMask = null;
                    }
                }
                else if (autoGreyScaleImg.SourceExt != null)
                {
                    var bitmapImage = new BitmapImage(new Uri(autoGreyScaleImg.SourceExt.ToString()));
                    autoGreyScaleImg.Source = new FormatConvertedBitmap(bitmapImage, PixelFormats.Gray32Float, null, 0);
                    autoGreyScaleImg.OpacityMask = new ImageBrush(bitmapImage);
                }
            }
        }

        public static ImageSource ConvertToGray(string imagePath)
        {
            var imageSource = ResourceHelper.GetImageSource(imagePath) as BitmapSource;
            return imageSource == null ? null : new FormatConvertedBitmap(imageSource, PixelFormats.Gray32Float, null, 0);
        }

        public static Image ConvertToGray(ImageSource imageSource)
        {
            if (imageSource is DrawingImage drawingImage)
            {
                DrawingVisual vis = new DrawingVisual();
                using (DrawingContext cont = vis.RenderOpen())
                {
                    cont.DrawImage(drawingImage, new Rect(new Size(120d, 100d)));
                    cont.Close();

                    double width = drawingImage.Width;
                    double height = drawingImage.Height;
                    RenderTargetBitmap bitmapImage = new RenderTargetBitmap((int)width,
                        (int)height, 96d, 96d, PixelFormats.Default);
                    bitmapImage.Render(vis);

                    return new Image
                    {
                        Source = new FormatConvertedBitmap(bitmapImage, PixelFormats.Gray32Float, null, 0),
                        SnapsToDevicePixels = true,
                        OpacityMask = new ImageBrush(bitmapImage)
                    };
                }
            }

            return null;
        }

        public static Image GetGrayscaleImage(string imagePath)
        {
            if (ResourceHelper.GetImageSource(imagePath) is BitmapSource bitmapImage)
            {
                var imageSource = new FormatConvertedBitmap(bitmapImage, PixelFormats.Gray32Float, null, 0);
                return new Image
                {
                    Source = imageSource,
                    SnapsToDevicePixels = true,
                    OpacityMask = new ImageBrush(bitmapImage)
                };
            }

            return null;
        }
    }
}
