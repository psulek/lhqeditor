using System.Reflection;

namespace CmdGen.Code;

public static class Extensions
{
    public static string GetAssemblyDefaultNamespace(this Assembly assembly)
    {
        return (assembly.EntryPoint?.DeclaringType?.Namespace ??
                assembly.GetName().Name) ?? string.Empty;
    }
}

public enum CSharpLanguageVersion
{
    CSharp7 = 7,
    CSharp8,
    CSharp9,
    CSharp10,
    CSharp11,
    CSharp12
}

public enum CsharpVisibilityModifier
{
    Public,
    Private,
    Protected,
    Internal,
    ProtectedInternal
}