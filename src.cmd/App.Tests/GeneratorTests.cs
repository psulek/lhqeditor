using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Shouldly;
using TestContext = NUnit.Framework.TestContext;

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
        
        if (!Directory.Exists(ShapshotFullDir))
        {
            Directory.CreateDirectory(ShapshotFullDir);
        }
        
        var subdirs = Directory.GetDirectories(ShapshotFullDir, "*.*", SearchOption.TopDirectoryOnly);
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

        using var generator = new Generator();
        var generatedFiles = generator.Generate(lhqModelFileName, csProjectFileName, hostData);

        generatedFiles.ShouldNotBeNull();
        generatedFiles.ShouldNotBeEmpty();

        string allFilesInfo = string.Join(LF,
            generatedFiles.Select(x => x.Key).Order(StringComparer.InvariantCultureIgnoreCase));

        var verifySettings = new VerifySettings(GetVerifySettings("generators"));

        var result = Verify(allFilesInfo, verifySettings)
            .UseDirectory(Path.Combine(ShapshotDir, methodName))
            .UseFileName("file")
            .AutoVerify();

        foreach (var (file, content) in generatedFiles)
        {
            result.AppendContentAsFile(FileHelper.ToLinuxLineEndings(content), name: Path.GetFileName(file));
        }

        return await result;
    }

    #endregion

    private void CreateVerifiedFiles(string methodName)
    {
        var verifiedFile = Path.Combine(ShapshotFullDir, methodName, "file.verified.txt");
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

                    FileHelper.WriteAllText(verifiedFile, verifiedFileContent);

                    foreach (var file in resourceFiles.OrderBy(x => x))
                    {
                        var verifiedFileName = Path.Combine(ShapshotFullDir, methodName,
                            $"file#{Path.GetFileName(file)}.verified.txt");
                        if (!File.Exists(verifiedFileName))
                        {
                            FileHelper.WriteAllText(verifiedFileName, File.ReadAllText(file), linuxLineEndings: true);
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