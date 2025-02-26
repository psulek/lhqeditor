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