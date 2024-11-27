using System.Text.RegularExpressions;
using FluentAssertions;
using LHQ.Data.Templating.Templates.NetCore;
using LHQ.Generators;
using LHQ.Generators.NetCore;

namespace Generators.Tests;

[TestFixture]
public sealed class Test1: VerifyBase
{
    public Test1(): base(GetVerifySettings())
    { }

    private static VerifySettings GetVerifySettings()
    {
        var verifySettings = new VerifySettings();
        verifySettings.UseDirectory("_shapshots");
        return verifySettings;
    }

    [Test]
    public Task NetCoreResxCsharp01Template_CSharp()
    {
        // var ok = GetStr(true);
        // var notok = GetStr(false);
        
        string input = """
                       abc
                          
                       efg
                       """;
        string output = Regex.Replace(input, @"^\s+\n", "\n", RegexOptions.Multiline);
        
        var modelContext = TemplateGeneratorFactory.LoadModel("TestData\\Strings.lhq", out var settings, out _);
        modelContext.Should().NotBeNull();

        var template = (NetCoreResxCsharp01Template)settings;

        var renderToString = TemplateGenerator.RenderToString<CSharpTemplateViewModel>(modelContext, template.CSharp);

        return Verify(renderToString);
    }

//     private string GetStr(bool ok)
//     {
//         string NamespaceName = "MyNamespace";
//         string Other = "abc";
//         bool UseExpressionBodySyntax = true;
//         
//         string RenderCSharp_Property()
//         {
//             return ok
//                 ? $$"""
//                     {{Other}}
//                     """
//                 : $$"""
//                     
//                     {{Other}}
//                     """;
//         }
//         
//         
//
//         
//         return 
// $$"""
// namespace {{NamespaceName}}
// {
//     {{RenderCSharp_Property()}}
// }
// """;
//         
//     }
}