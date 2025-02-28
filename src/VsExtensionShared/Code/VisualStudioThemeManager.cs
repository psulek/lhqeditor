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
using System.Drawing;
using System.Linq;
using EnvDTE;
using LHQ.App.ViewModels;
using Microsoft.VisualStudio.PlatformUI;

namespace LHQ.VsExtension.Code
{
    internal sealed class VisualStudioThemeManager
    {
        private readonly TimeSpan _dialogDelayTimeout = TimeSpan.FromMilliseconds(500);

        private static readonly Lazy<VisualStudioThemeManager> _instance = new Lazy<VisualStudioThemeManager>(() => new VisualStudioThemeManager());

        public static VisualStudioThemeManager Instance => _instance.Value;

        public VisualStudioThemeManager()
        {
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;
        }

        public static void Initialize()
        {
            Instance.Theme = Instance.CalculateTheme();
        }

        public VisualStudioTheme Theme { get; private set; }

        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            VisualStudioTheme newTheme = CalculateTheme();

            if (Theme != newTheme)
            {
                Theme = newTheme;

                ShellViewModel shellViewModel = VsPackageService.GetOpenedEditors().FirstOrDefault();

                if (shellViewModel != null)
                {
                    var showMessage = shellViewModel.AppContext.RunInVsPackage && shellViewModel.AppConfig.DetectTheme.GetValueOrDefault(true);
                    if (showMessage)
                    {
                        shellViewModel.AppContext.UIService.DispatchActionOnUI(() =>
                                {
                                    VsPackageService.CloseAndReopenEditors(vsSaveChanges.vsSaveChangesPrompt);
                                },
                            _dialogDelayTimeout);
                    }
                }
            }
        }

        private VisualStudioTheme CalculateTheme()
        {
            Color backgroundColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
            switch (backgroundColor.Name)
            {
                case "ff252526":
                {
                    return VisualStudioTheme.Dark;
                }
                case "ffffffff":
                {
                    return VisualStudioTheme.Blue;
                }
                default:
                {
                    return VisualStudioTheme.Light;
                }
            }
        }
    }

    public enum VisualStudioTheme
    {
        Dark,
        Light,
        Blue,
        Unknown
    }
}
