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

using Velopack.Locators;
using Velopack.Logging;
using Velopack.Windows;

namespace LHQ.App
{
    public static class VelopackService
    {
        public static void AfterInstall()
        {
            try
            {
                var location = ShortcutLocation.AppRoot;
                new Shortcuts().CreateShortcutForThisExe(location);
            }
            catch
            {
                // NOTE: Ignore exceptions
            }
        }

        public static void FirstRun()
        {
            var logger = VelopackLocator.Current.Log;
            var shortcutsApi = new Shortcuts();
            var location = ShortcutLocation.AppRoot;
            logger.Log(VelopackLogLevel.Information, $"Creating shortcut for: {VelopackLocator.Current.ThisExeRelativePath} on {location} (OnFirstRun)");
            shortcutsApi.CreateShortcutForThisExe(location);
        }
    }
}
