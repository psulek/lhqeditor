using System.Runtime.CompilerServices;

namespace LHQ.Cmd.Tests;

public abstract class TestBase: VerifyBase
{
    protected readonly string _sourceFile;
    protected static string ShapshotDir = null!;
    protected string ShapshotFullDir;

    protected TestBase(VerifySettings? settings = null, [CallerFilePath] string sourceFile = "") : base(settings,
        sourceFile)
    {
        _sourceFile = sourceFile;
        ShapshotFullDir = Path.Combine(Path.GetDirectoryName(sourceFile), ShapshotDir);
    }

    internal static VerifySettings GetVerifySettings(string className)
    {
        ShapshotDir = Path.Combine("_shapshots", className);
        var verifySettings = new VerifySettings();
        verifySettings.UseDirectory(ShapshotDir);
        return verifySettings;
    }
}