// #region License
//
// // Copyright (c) 2025 Peter Šulek / ScaleHQ Solutions s.r.o.
// // 
// // Permission is hereby granted, free of charge, to any person
// // obtaining a copy of this software and associated documentation
// // files (the "Software"), to deal in the Software without
// // restriction, including without limitation the rights to use,
// // copy, modify, merge, publish, distribute, sublicense, and/or sell
// // copies of the Software, and to permit persons to whom the
// // Software is furnished to do so, subject to the following
// // conditions:
// // 
// // The above copyright notice and this permission notice shall be
// // included in all copies or substantial portions of the Software.
// // 
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// // EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// // OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// // NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// // HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// // WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// // FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// // OTHER DEALINGS IN THE SOFTWARE.
//
// #endregion
//
// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Text.RegularExpressions;
// using System.Xml;
// using System.Xml.Linq;
// using System.Xml.XPath;
// using JetBrains.Annotations;
// using NLog;
//
// namespace LHQ.Utils
// {
//     [PublicAPI]
//     public static class CsProjectUtils
//     {
//         private const string XpathRootNamespace = "//ns:Project/ns:PropertyGroup/ns:RootNamespace";
//         private const string XpathAssemblyName = "//ns:Project/ns:PropertyGroup/ns:AssemblyName";
//
//         private static readonly string[] _itemGroupTypes = { "Content", "None", "Compile", "EmbeddedResource" };
//         private static readonly string[] _itemGroupTypesAttrs = { "Include", "Update" };
//
//         private const string CsProjectXPath =
//             "//ns:Project/ns:ItemGroup/ns:##TYPE##[@##ATTR##='##FILE##']";
//
//         public static CsProjectInfo FindOwnerCsProjectFile(string lhqModelFileName, ILogger logger)
//         {
//             try
//             {
//                 if (!File.Exists(lhqModelFileName))
//                 {
//                     return new CsProjectInfo();
//                 }
//
//                 var dir = Path.GetDirectoryName(lhqModelFileName);
//                 var csProjectFiles = dir == null ? Array.Empty<string>() : Directory.GetFiles(dir, "*.csproj", SearchOption.TopDirectoryOnly);
//
//                 NamespaceInfo result = null;
//                 var namespaceResults = new List<NamespaceInfo>();
//
//                 foreach (var csProj in csProjectFiles)
//                 {
//                     var rootNamespace = GetRootNamespace(lhqModelFileName, csProj, logger);
//                     if (rootNamespace != null)
//                     {
//                         namespaceResults.Add(rootNamespace);
//                     }
//                 }
//
//                 if (namespaceResults.Count > 1)
//                 {
//                     var multipleRefs = namespaceResults.Count(x => x.ReferencedLhqFile || x.ReferencedT4File);
//                     if (multipleRefs > 1)
//                     {
//                         var lhq = Path.GetFileName(lhqModelFileName);
//                         var t4 = Path.GetFileName(lhqModelFileName) + ".tt";
//                         throw new Exception($"Multiple C# project files found in directory '{dir}' that reference either '{lhq}' or a '{t4}' file.\n" +
//                             "Specify which C# project file to use with the '--project' argument.");
//                     }
//
//                     result = namespaceResults.FirstOrDefault(x => (x.ReferencedLhqFile || x.ReferencedT4File) && !string.IsNullOrEmpty(x.Namespace)) ??
//                         namespaceResults.FirstOrDefault(x => !string.IsNullOrEmpty(x.Namespace)) ??
//                         namespaceResults[0];
//                 }
//                 else if (namespaceResults.Count == 1)
//                 {
//                     result = namespaceResults[0];
//                 }
//
//                 if (result != null && result.NamespaceDynamicExpression)
//                 {
//                     result.Namespace = "";
//                     logger?.Warn(
//                         "Warning: \nValue in 'RootNamespace' or 'AssemblyName' element contains dynamic expression which is not supported.\n" +
//                         "This value will not be used for 'Namespace' in generator.\n" +
//                         "Set namespace directly in the lhq file in C# template setting 'Namespace' or provide namespace via cmd '--data namespace=<value>'.");
//                 }
//
//                 return new CsProjectInfo
//                 {
//                     CsProjectFile = result?.CsProjectFile ?? string.Empty,
//                     Namespace = result?.Namespace ?? string.Empty
//                 };
//             }
//             catch (Exception e)
//             {
//                 return new CsProjectInfo();
//             }
//         }
//         
//         public static NamespaceInfo GetRootNamespace(string lhqModelFileName, string csProjectFileName, ILogger logger)
//         {
//             var referencedLhqFile = false;
//             var referencedT4File = false;
//             var namespaceDynamicExpression = false;
//
//             if (string.IsNullOrEmpty(csProjectFileName) || !File.Exists(csProjectFileName))
//             {
//                 return null;
//             }
//
//             lhqModelFileName = Path.GetFileName(lhqModelFileName);
//             var lhqFileT4 = lhqModelFileName + ".tt";
//
//             string rootNamespace;
//
//             try
//             {
//                 var doc = XDocument.Parse(File.ReadAllText(csProjectFileName));
//                 var ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;
//
//                 var manager = new XmlNamespaceManager(new NameTable());
//                 manager.AddNamespace("ns", ns.NamespaceName);
//
//                 XElement FindFileElement(string fileName)
//                 {
//                     foreach (var itemGroupType in _itemGroupTypes)
//                     {
//                         foreach (var attr in _itemGroupTypesAttrs)
//                         {
//                             var xpath = CsProjectXPath.Replace("##TYPE##", itemGroupType)
//                                 .Replace("##ATTR##", attr).Replace("##FILE##", fileName);
//
//                             var element = doc.XPathSelectElement(xpath, manager);
//                             if (element != null)
//                             {
//                                 return element;
//                             }
//                         }
//                     }
//
//                     return null;
//                 }
//
//                 // 1st: try to find <RootNamespace>
//                 var rootNamespaceElem = doc.XPathSelectElement(XpathRootNamespace, manager);
//                 rootNamespace = rootNamespaceElem?.Value;
//
//                 // if csproj has imported lhq file (lhqModelFileName) explicitly eg: /ItemGroup/None[@Update="Strings.lhq"] (and other variants)
//                 // we can look at CustomToolNamespace element under '{lhqModelFileName}.tt' element (if it exists)
//                 referencedLhqFile = FindFileElement(lhqModelFileName) != null;
//                 var t4FileElement = FindFileElement(lhqFileT4);
//                 if (t4FileElement != null)
//                 {
//                     referencedT4File = true;
//                     var dependentUpon = t4FileElement.Descendants(ns + "DependentUpon").FirstOrDefault()?.Value;
//                     if (!string.IsNullOrEmpty(dependentUpon) && dependentUpon == lhqModelFileName)
//                     {
//                         referencedLhqFile = true;
//                     }
//
//                     var customToolNamespace =
//                         t4FileElement.Descendants(ns + "CustomToolNamespace").FirstOrDefault()?.Value;
//                     if (!string.IsNullOrEmpty(customToolNamespace))
//                     {
//                         rootNamespace = customToolNamespace;
//                     }
//                 }
//
//                 if (string.IsNullOrEmpty(rootNamespace))
//                 {
//                     // 2st: try to find <AssemblyName>
//                     var assemblyNameElem = doc.XPathSelectElement(XpathAssemblyName, manager);
//                     rootNamespace = assemblyNameElem?.Value;
//
//                     if (string.IsNullOrEmpty(rootNamespace) && !string.IsNullOrEmpty(csProjectFileName))
//                     {
//                         rootNamespace = Path.GetFileNameWithoutExtension(csProjectFileName).Replace(" ", "_");
//                     }
//                 }
//
//                 if (!string.IsNullOrEmpty(rootNamespace))
//                 {
//                     string pattern = @"\$\((.*?)(?:\)(?!\)))";
//
//                     if (Regex.Matches(rootNamespace, pattern).Count > 0)
//                     {
//                         namespaceDynamicExpression = true;
//                     }
//                 }
//             }
//             catch (Exception e)
//             {
//                 logger?.Error(e, "Error getting root namespace.");
//                 rootNamespace = null;
//             }
//
//             return new NamespaceInfo
//             {
//                 CsProjectFile = csProjectFileName,
//                 Namespace = rootNamespace ?? string.Empty,
//                 NamespaceDynamicExpression = namespaceDynamicExpression,
//                 ReferencedLhqFile = referencedLhqFile,
//                 ReferencedT4File = referencedT4File
//             };
//         }
//     }
//     
//     public sealed class CsProjectInfo
//     {
//         public string CsProjectFile { get; set; } = string.Empty;
//
//         public string Namespace { get; set; } = string.Empty;
//     }
//     
//     public sealed class NamespaceInfo
//     {
//         public string CsProjectFile { get; set; }
//         
//         public string Namespace { get; set; }
//
//         public bool ReferencedLhqFile { get; set; }
//
//         public bool ReferencedT4File { get; set; }
//
//         public bool NamespaceDynamicExpression { get; set; }
//     }
// }
