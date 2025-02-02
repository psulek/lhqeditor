using System.Runtime.CompilerServices;
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
        return Generator.GetRootNamespace("Strings.lhq", TestDir + fileName);
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
    public void Project07_Test()
    {
        var rootNamespace = GetRootNamespace();
        rootNamespace.ShouldBeNullOrEmpty();
    }
}