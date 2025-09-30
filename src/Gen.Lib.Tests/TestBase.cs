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

using System.Runtime.CompilerServices;

namespace LHQ.Cmd.Tests;

public abstract class TestBase: VerifyBase
{
    protected readonly string _sourceFile;
    protected static string _shapshotDir = null!;
    protected readonly string _shapshotFullDir;

    protected TestBase(VerifySettings? settings = null, [CallerFilePath] string sourceFile = "") : base(settings, sourceFile)
    {
        _sourceFile = sourceFile;
        _shapshotFullDir = Path.Combine(Path.GetDirectoryName(sourceFile), _shapshotDir);
    }

    internal static VerifySettings GetVerifySettings(string className)
    {
        _shapshotDir = Path.Combine("_shapshots", className);
        var verifySettings = new VerifySettings();
        verifySettings.UseDirectory(_shapshotDir);
        return verifySettings;
    }
}