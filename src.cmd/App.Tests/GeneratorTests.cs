using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using Shouldly;
// ReSharper disable InconsistentNaming

namespace LHQ.Cmd.Tests;

[TestFixture]
public sealed class GeneratorTests : TestBase
{
    public GeneratorTests() : base(GetVerifySettings("generators"))
    {
    }

    private const char LF = '\n';

    private static string FixNewlines(string str)
    {
        return str.Replace("\r\n", "\n").Replace('\r', '\n');
    }

    private Task Generate(string? lhqModelFileName = null, string? csProjectFileName = null,
        Dictionary<string, object>? hostData = null, [CallerMemberName] string methodName = "")
    {
        if (lhqModelFileName == null)
        {
            lhqModelFileName = Path.Combine("TestData", methodName, "Strings.lhq");
        }

        if (csProjectFileName == null)
        {
            csProjectFileName = Path.Combine("TestData", methodName, $"{methodName}.csproj");
        }

        // create verified files if not exist (based on content from testdata\{methodname}\Resources\...)
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

                    File.WriteAllText(verifiedFile, verifiedFileContent, Encoding.UTF8);

                    foreach (var file in resourceFiles.OrderBy(x => x))
                    {
                        var verifiedFileName = Path.Combine(ShapshotFullDir, methodName,
                            $"file#{Path.GetFileName(file)}.verified.txt");
                        if (!File.Exists(verifiedFileName))
                        {
                            File.WriteAllText(verifiedFileName, FixNewlines(File.ReadAllText(file)), Encoding.UTF8);
                        }
                    }
                }
            }
        }

        using var generator = new Generator();
        var generatedFiles = generator.Generate(lhqModelFileName, csProjectFileName, hostData);

        generatedFiles.ShouldNotBeNull();
        generatedFiles.ShouldNotBeEmpty();

        string allFilesInfo = string.Join(LF,
            generatedFiles.Select(x => x.Key).Order(StringComparer.InvariantCultureIgnoreCase));
        var result = Verify(allFilesInfo).UseDirectory(Path.Combine(ShapshotDir, methodName))
            .UseFileName("file"); //methodName);

        foreach (var (file, content) in generatedFiles)
        {
            result.AppendContentAsFile(FixNewlines(content), name: Path.GetFileName(file));
        }

        return result;
    }

    [Test]
    public Task WpfResxCsharp01() => Generate();
    
    [Test]
    public Task WpfResxCsharp01v2() => Generate();
}