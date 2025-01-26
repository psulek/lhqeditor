// using System.Text.RegularExpressions;
// using LHQ.Data;
// using LHQ.Data.Templating.Templates;
//
// namespace LhqHandleBarsTest.Code;
//
// public partial class ModelManager: IDisposable
// {
//     private bool _disposed;
//
//     [GeneratedRegex(@"^<\#@ LHQDirective .*?ItemTemplate=""([^""]+)""", RegexOptions.Multiline)]
//     private static partial Regex FindItemTemplate();
//
//     public ModelManager(string lhqModelFileName)
//     {
//         LhqModelFileName = lhqModelFileName;
//         
//         ModelContext = new ModelContext(ModelContextOptions.Default);
//         var loadResult = LHQ.Data.ModelStorage.ModelSerializerManager.DeserializeFrom(lhqModelFileName, ModelContext);
//         if (loadResult.Status != LHQ.Data.ModelStorage.ModelLoadStatus.Success)
//         {
//             throw new ApplicationException($"Error loading strings file, {loadResult.Status}");
//         }
//
//         string ttFile = Path.GetFileName(lhqModelFileName) + ".tt";
//         if (!File.Exists(ttFile))
//         {
//             throw new ApplicationException($"Could not find template file: {ttFile} !");
//         }
//
//         var match = FindItemTemplate().Match(File.ReadAllText(ttFile));
//         if (!match.Success)
//         {
//             throw new ApplicationException($"Could not find 'ItemTemplate' in file: {ttFile}!");
//         }
//
//         ItemTemplate = match.Groups[1].Value;
//         if (string.IsNullOrEmpty(ItemTemplate))
//         {
//             throw new ApplicationException($"'ItemTemplate' in file: {ttFile} has empty value!");
//         }
//         
//         CodeGeneratorTemplate = LHQ.Data.Extensions.ModelContextExtensions.GetCodeGeneratorTemplate(ModelContext, ItemTemplate);
//         if (CodeGeneratorTemplate == null)
//         {
//             throw new ApplicationException("Could not find code generator template!");
//         }
//     }
//
//     public string LhqModelFileName { get; }
//
//     public ModelContext ModelContext { get; }
//
//     public CodeGeneratorTemplate CodeGeneratorTemplate { get; }
//
//     public string ItemTemplate { get; }
//
//     public void Dispose()
//     {
//         if (!_disposed)
//         {
//             ModelContext.Dispose();
//             _disposed = true;
//         }
//     }
// }