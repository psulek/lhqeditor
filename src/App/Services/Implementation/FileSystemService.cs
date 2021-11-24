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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using LHQ.App.Services.Interfaces;
using LHQ.Core.DependencyInjection;

namespace LHQ.App.Services.Implementation
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class FileSystemService : AppContextServiceBase, IFileSystemService
    {
        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        public string GetDataFileName(string fileName)
        {
            // for vs extension, it is possible to write into app folder (from where is extension loaded)
            string folder = AppContext.RunInVsPackage ? AppContext.AppFolder : AppContext.DataFolder;
            return Path.Combine(folder, fileName);
        }
    }
}
