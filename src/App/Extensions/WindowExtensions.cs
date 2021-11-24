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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
// ReSharper disable All

namespace LHQ.App.Extensions
{
    internal static class WindowExtensions
    {
        // ReSharper disable once InconsistentNaming
        private const int GWL_STYLE = -16;

        // ReSharper disable once InconsistentNaming
        private const int WS_MAXIMIZEBOX = 0x10000;

        // ReSharper disable once InconsistentNaming
        private const int WS_MINIMIZEBOX = 0x20000;

        // ReSharper disable once InconsistentNaming
        private const int WS_SYSMENU = 0x80000;


        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hwnd, int index, int value);

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hwnd, int index);

        public static void HideMinimizeAndMaximizeButtons(this Window window)
        {
            HideMinimizeAndMaximizeButtons(window, true, true, false);
        }

        internal static void HideMinimizeAndMaximizeButtons(this Window window, bool hideMinimize, bool hideMaximize, bool hideClose)
        {
            bool sourceInitializedCalled = false;
            if (window.Tag != null)
            {
                sourceInitializedCalled = (bool)window.Tag;
            }

            if (sourceInitializedCalled)
            {
                InternalHideMinimizeAndMaximizeButtons(window, hideMinimize, hideMaximize, hideClose);
            }
            else
            {
                window.Tag = true;
                window.SourceInitialized += (sender, args) => InternalHideMinimizeAndMaximizeButtons(window, hideMinimize, hideMaximize, hideClose);
            }
        }

        private static void InternalHideMinimizeAndMaximizeButtons(Window window, bool hideMinimize, bool hideMaximize, bool hideClose)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            int currentStyle = GetWindowLong(hwnd, GWL_STYLE);

            int value = currentStyle;

            if (hideMinimize)
            {
                value = value & ~WS_MINIMIZEBOX;
            }

            if (hideMaximize)
            {
                value = value & ~WS_MAXIMIZEBOX;
            }

            if (hideClose)
            {
                value = value & ~WS_SYSMENU;
            }

            SetWindowLong(hwnd, GWL_STYLE, value);
        }
    }
}
