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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LHQ.Utils.Extensions;

namespace LHQ.App.Code
{
    public static class ResourceHelper
    {
        private const string PackPrefix = "pack://application:,,,";
        private const string AppComponentPrefix = "/LHQ.App";

        public static string BuildEmbedResourcePath(string resourcePath, string assemblyName = null)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(resourcePath));
            }

            if (!resourcePath.StartsWith("/"))
            {
                throw new ArgumentException("Parameter 'resourcePath' must starts with /");
            }

            if (assemblyName.IsNullOrEmpty())
            {
                assemblyName = AppComponentPrefix;
            }

            if (!assemblyName.StartsWith("/"))
            {
                throw new ArgumentException("Parameter 'assemblyName' must starts with /");
            }

            return $"{assemblyName};component{resourcePath}";
        }

        public static Uri GetPackUri(string resourcePath)
        {
            return new Uri(GetPackUrl(resourcePath), UriKind.Absolute);
        }

        public static string GetPackUrl(string resourcePath)
        {
            if (resourcePath.StartsWith(PackPrefix))
            {
                return resourcePath;
            }

            if (!resourcePath.StartsWith(@"/"))
            {
                resourcePath = @"/" + resourcePath;
            }
            return PackPrefix + resourcePath;
        }

        public static ImageSource GetImageSource(string imagePath)
        {
            return new BitmapImage(new Uri(GetPackUrl(imagePath)));
        }

        public static Uri GetComponentUri(string componentPath)
        {
            string packUrl = AppComponentPrefix + ";component" + componentPath;
            return new Uri(packUrl, UriKind.Relative);
        }

        public static string GetImageComponentUri(string imageName)
        {
            return GetComponentUri(imageName).ToString();
        }
    }
}
