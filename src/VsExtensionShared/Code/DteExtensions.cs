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
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using LHQ.App.ViewModels;
using LHQ.Utils.Extensions;
using Microsoft.VisualStudio.Shell;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.VsExtension.Code
{
    public static class DteExtensions
    {
        private static readonly Dictionary<int, string> _vsVersions = new Dictionary<int, string>
        {
            {14, "2015"},
            {15, "2017"},
            {16, "2019"}
        };

        public static DTE GetDTE()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return Package.GetGlobalService(typeof(DTE)) as DTE;
        }

        public static string GetVsProductVersion()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (Package.GetGlobalService(typeof(DTE)) is DTE dte)
            {
                var dteVersion = Version.Parse(dte.Version);
                return _vsVersions.TryGetValue(dteVersion.Major, out string version) ? version : dteVersion.Major.ToString();
            }

            return string.Empty;
        }

        public static List<ShellViewModel> GetOpenedShellViews(this DTE dte, out List<ProjectItem> projectItems)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return GetOpenedShellViews(dte.Windows, out projectItems);
        }

        public static List<EditorPane> GetOpenedEditorPanes(this EnvDTE.Windows windows, out List<ProjectItem> projectItems)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            List<Window> shellWindows = GetOpenedShellWindows(windows, out projectItems);
#pragma warning disable VSTHRD010
            return shellWindows.Select(x => x.Object as EditorPane).ToList();
#pragma warning restore VSTHRD010
        }

        public static List<ShellViewModel> GetOpenedShellViews(this EnvDTE.Windows windows, out List<ProjectItem> projectItems)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            List<Window> shellWindows = GetOpenedShellWindows(windows, out projectItems);
#pragma warning disable VSTHRD010
            return shellWindows.Where(x => x.Object is EditorPane).Select(x => ((EditorPane)x.Object).ShellViewModel).ToList();
#pragma warning restore VSTHRD010
        }

        public static Window GetWindowForShellView(this EnvDTE.Windows windows, ShellViewModel shellViewModel)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
#pragma warning disable VSTHRD010
            return GetOpenedShellWindows(windows, out _)
                .Where(x => x.Object is EditorPane)
                .SingleOrDefault(x => ((EditorPane)x.Object).ShellViewModel == shellViewModel);
#pragma warning restore VSTHRD010
        }

        public static List<Window> GetOpenedShellWindows(this EnvDTE.Windows windows, out List<ProjectItem> projectItems)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            List<Window> result = new List<Window>();
            projectItems = new List<ProjectItem>();

            foreach (Window window in windows)
            {
                if (!window.ObjectKind.IsNullOrEmpty())
                {
                    if (Guid.TryParse(window.ObjectKind, out var editorKidUid))
                    {
                        if (editorKidUid.EqualsTo(PackageGuids.guidLhqEditor))
                        {
                            if (window.Object is EditorPane)
                            {
                                projectItems.Add(window.ProjectItem);
                                result.Add(window);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static ShellViewModel GetActiveShellView(this DTE dte)
        {
            return GetActiveEditorPane(dte)?.ShellViewModel;
        }
        
        public static EditorPane GetActiveEditorPane(this DTE dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Window window = dte.ActiveWindow;
            if (window != null && !window.ObjectKind.IsNullOrEmpty() && 
                Guid.TryParse(window.ObjectKind, out var editorKidUid) &&
                editorKidUid.EqualsTo(PackageGuids.guidLhqEditor) &&
                window.Object is EditorPane editorPane)
            {
                return editorPane;
            }

            return null;
        }
    }
}
