// using System.Reflection;
//
// namespace LhqHandleBarsTest.Code;
//
// public static class TemplateManager
// {
//     private static Dictionary<string, Type> Templates { get; } = new();
//
//     public static void Initialize()
//     {
//         foreach (var templateType in AppDomain.CurrentDomain.GetAssemblies()
//                      .SelectMany(assembly => assembly.GetTypes())
//                      .Select(type => new {type, attribute = type.GetCustomAttribute<GeneratorTemplateAttribute>()})
//                      .Where(x=> x.attribute != null))
//         {
//             Register(templateType.type, templateType.attribute!.ItemTemplateName);
//         }
//     }
//     
//     private static void Register(Type type, string itemTemplateName)
//     {
//         if (string.IsNullOrEmpty(itemTemplateName))
//         {
//             throw new ApplicationException(
//                 $"Could not register type '{type.FullName}' as template, missing 'ItemTemplateName' attribute value!");
//         }
//
//         if (!Templates.TryAdd(itemTemplateName, type))
//         {
//             throw new ApplicationException(
//                 $"Could not register type '{type.FullName}' as template, '{itemTemplateName}' is already registered!");
//         }
//     }
//
//     public static GeneratorTemplate GetTemplate(string itemTemplateName)
//     {
//         if (string.IsNullOrEmpty(itemTemplateName))
//         {
//             throw new ArgumentNullException(nameof(itemTemplateName));
//         }
//
//         if (!Templates.TryGetValue(itemTemplateName, out var templateType))
//         {
//             throw new ApplicationException($"Could not find template for '{itemTemplateName}'!");
//         }
//
//         return (GeneratorTemplate)Activator.CreateInstance(templateType)!;
//     }
// }