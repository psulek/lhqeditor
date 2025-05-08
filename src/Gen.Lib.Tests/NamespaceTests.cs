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
using System.Text;
using LHQ.Gen.Lib;
using LHQ.Utils;
using Shouldly;

namespace LHQ.Cmd.Tests;

[TestFixture]
public sealed class NamespaceTests: TestBase
{
    private const string TestDir = @"TestData\csprojs\";
    
    public NamespaceTests(): base(GetVerifySettings("namespaces"))
    { }

    
    private Task DoTest([CallerMemberName] string methodName = "")
    {
        var rootNamespace = GetRootNamespace(methodName);
        
        rootNamespace.ShouldNotBeNullOrEmpty();
        
        return Verify(rootNamespace).UseFileName(methodName);
    }

    private string GetRootNamespace([CallerMemberName] string methodName = "")
    {
        string fileName = methodName.Split("_")[0].ToLowerInvariant() + ".csproj";
        return CsProjectUtils.GetRootNamespace("Strings.lhq", TestDir + fileName, null).Namespace;
    }

    [Test]
    public Task Project01_Test() => DoTest();

    [Test]
    public Task Project02_Test() => DoTest();
    
    [Test]
    public Task Project03_Test() => DoTest();
    
    [Test]
    public Task Project04_Test() => DoTest();
    
    [Test]
    public Task Project05_Test() => DoTest();

    [Test]
    public Task Project06_Test() => DoTest();

    [Test]
    public void Project07_Test() => DoTest();
    
    [Test]
    public Task Project08_Test() => DoTest();

    private Task TestGeneratedFileContent(LineEndings lineEndings, [CallerMemberName] string methodName = "")
    {
        string fileName = Path.Combine("TestData", "Encodings", $"test_{lineEndings.ToString().ToLowerInvariant()}.txt");
        string content = File.ReadAllText(fileName);
        content = new GeneratedFile(fileName, content, false, lineEndings).GetContent(true);
        var buffer = Encoding.UTF8.GetBytes(content);
        return Verify(buffer).UseFileName(methodName);
    }

    [Test]
    public Task LineEndings_CRLF() => TestGeneratedFileContent(LineEndings.CRLF);
    
    [Test]
    public Task LineEndings_LF() => TestGeneratedFileContent(LineEndings.LF);
}