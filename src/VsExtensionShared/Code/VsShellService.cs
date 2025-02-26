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
using System.IO;
using EnvDTE;
using LHQ.App.Model;
using LHQ.App.Services.Implementation;
using LHQ.Data;
using LHQ.Utils.Extensions;
// ReSharper disable ClassNeverInstantiated.Global

namespace LHQ.VsExtension.Code
{
    public class VsShellService : ShellService
    {
        public override bool SaveProject(string fileName = null, SaveProjectFlags flags = SaveProjectFlags.None)
        {
            if (flags.IsFlagSet(SaveProjectFlags.HostEnvironmentSave))
            {
                Window window = VsPackageService.GetWindowForShellView(ShellViewModel);
                if (window?.Document != null)
                {
                    try
                    {
                        return window.Document.Save() == vsSaveStatus.vsSaveSucceeded;
                    }
                    catch (Exception e)
                    {
                        Logger.Info("Document save failed", e);
                        return false;
                    }
                }
            }

            return base.SaveProject(fileName, flags);
        }
        
        protected override void HandleShellViewLoadedEvent(ShellViewLoadedEventArgs eventArgs)
        {
            // In VS, do nothing here...
        }
    }
}