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

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using LHQ.Gen.Lib;
using NLog;
using Shouldly;

// ReSharper disable InconsistentNaming

namespace LHQ.Cmd.Tests;

[TestFixture]
public sealed class GeneratorTests() : TestBase(GetVerifySettings("generators"))
{
    private const char LF = '\n';

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Console.WriteLine($"[OneTimeSetUp] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
        
        if (!Directory.Exists(_shapshotFullDir))
        {
            Directory.CreateDirectory(_shapshotFullDir);
        }
        
        var subdirs = Directory.GetDirectories(_shapshotFullDir, "*.*", SearchOption.TopDirectoryOnly);
        if (subdirs.Length == 0)
        {
            var testMethods = GetType().GetMethods()
                .Where(m => m.GetCustomAttribute<TestAttribute>() != null)
                .Select(m => m.Name)
                .ToArray();

            foreach (var testMethod in testMethods)
            {
                CreateVerifiedFiles(testMethod);
            }
            
            //throw new Exception($"{nameof(GeneratorTests)} was stopped after initial verification files was created. Please restart unit test(s)!");
        }
    }

    #region Generate

    private async Task<VerifyResult> Generate(string? lhqModelFileName = null, string? csProjectFileName = null,
        Dictionary<string, object>? hostData = null, [CallerMemberName] string methodName = "")
    {
        //Console.WriteLine($"[Generate] {methodName} at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
        
        if (lhqModelFileName == null)
        {
            lhqModelFileName = Path.Combine("TestData", methodName, "Strings.lhq");
        }

        if (csProjectFileName == null)
        {
            csProjectFileName = Path.Combine("TestData", methodName, $"{methodName}.csproj");
        }

        using var generator = new Generator(new NullLogger(new LogFactory()));
        var generateResult = generator.Generate(lhqModelFileName, csProjectFileName, string.Empty, hostData);
        var generatedFiles = generateResult.GeneratedFiles; 

        generatedFiles.ShouldNotBeNull();
        generatedFiles.ShouldNotBeEmpty();

        string allFilesInfo = string.Join(LF,
            generatedFiles.Select(x => x.FileName).Order(StringComparer.InvariantCultureIgnoreCase));

        var verifySettings = new VerifySettings(GetVerifySettings("generators"));

        var result = Verify(allFilesInfo, verifySettings)
            .UseDirectory(Path.Combine(_shapshotDir, methodName))
            .UseFileName("file")
            .AutoVerify();

        foreach (var file in generatedFiles)
        {
            string content = file.GetContent(false);
            await result.AppendContentAsFile(ToLinuxLineEndings(content), name: Path.GetFileName(file.FileName));
        }

        return await result;
    }

    #endregion

    private static readonly Encoding NoBomEncoding = new UTF8Encoding(false);

    public static string ToLinuxLineEndings(string str)
    {
        return str.Replace("\r\n", "\n").Replace('\r', '\n');
    }

    public static void WriteAllText(string fileName, string content, bool useBOM = false, bool linuxLineEndings = false)
    {
        if (linuxLineEndings)
        {
            content = ToLinuxLineEndings(content);
        }

        File.WriteAllText(fileName, content, useBOM ? Encoding.UTF8 : NoBomEncoding);
    }

    private void CreateVerifiedFiles(string methodName)
    {
        var verifiedFile = Path.Combine(_shapshotFullDir, methodName, "file.verified.txt");
        if (!File.Exists(verifiedFile))
        {
            var resourcesDir = Path.Combine("TestData", methodName, "Resources");
            if (Directory.Exists(resourcesDir))
            {
                var resourceFiles = Directory.GetFiles(resourcesDir, "Strings.*", SearchOption.TopDirectoryOnly);
                if (resourceFiles.Length > 0)
                {
                    var verifiedFileContent = string.Join(LF,
                        resourceFiles.Select(x => Path.Combine("Resources", Path.GetFileName(x)))
                            .Order(StringComparer.InvariantCultureIgnoreCase));

                    var dir = Path.GetDirectoryName(verifiedFile);
                    if (!Directory.Exists(verifiedFile))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    WriteAllText(verifiedFile, verifiedFileContent);

                    foreach (var file in resourceFiles.OrderBy(x => x))
                    {
                        var verifiedFileName = Path.Combine(_shapshotFullDir, methodName,
                            $"file#{Path.GetFileName(file)}.verified.txt");
                        if (!File.Exists(verifiedFileName))
                        {
                            WriteAllText(verifiedFileName, File.ReadAllText(file), linuxLineEndings: true);
                        }
                    }
                }
            }
        }
    }

    // WPF - start
    [Test]
    public Task WpfResxCsharp01() => Generate();

    [Test]
    public Task WpfResxCsharp01v2() => Generate();

    [Test]
    public Task WpfResxCsharp01v3() => Generate();
    // WPF - end

    // NetCore
    [Test]
    public Task NetCoreResxCsharp01() => Generate();

    // Typescript
    [Test]
    public Task TypescriptJson01() => Generate();

    [Test]
    public Task TypescriptJson01v2() => Generate();

    [Test]
    public Task TypescriptJson01v3() => Generate();

    // WinForms
    [Test]
    public Task WinFormsResxCsharp01() => Generate();
}