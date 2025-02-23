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
// using Microsoft.VisualStudio.Shell;
//
// namespace LHQ.VsExtension
// {
//     [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
//     public sealed class CodeGeneratorRegistrationNewAttribute : RegistrationAttribute
//     {
//         public CodeGeneratorRegistrationNewAttribute(Type generatorType, string generatorName, string contextGuid)
//         {
//             ContextGuid = contextGuid ?? throw new ArgumentNullException(nameof(contextGuid));
//             GeneratorType = generatorType ?? throw new ArgumentNullException(nameof(generatorType));
//             GeneratorName = generatorName ?? throw new ArgumentNullException(nameof(generatorName));
//             GeneratorRegKeyName = generatorType.Name;
//             GeneratorGuid = generatorType.GUID;
//         }
//
//         /// <summary> 
//         /// Get the generator Type 
//         /// </summary> 
//         public Type GeneratorType { get; }
//
//         /// <summary> 
//         /// Get the Guid representing the project type 
//         /// </summary> 
//         public string ContextGuid { get; }
//
//         /// <summary> 
//         /// Get the Guid representing the generator type 
//         /// </summary> 
//         public Guid GeneratorGuid { get; }
//
//         /// <summary> 
//         /// Get or Set the GeneratesDesignTimeSource value 
//         /// </summary> 
//         public bool GeneratesDesignTimeSource { get; set; }
//
//         /// <summary> 
//         /// Get or Set the GeneratesSharedDesignTimeSource value 
//         /// </summary> 
//         public bool GeneratesSharedDesignTimeSource { get; set; }
//
//         /// <summary> 
//         /// Gets the Generator name  
//         /// </summary> 
//         public string GeneratorName { get; }
//
//         /// <summary> 
//         /// Gets the Generator reg key name under  
//         /// </summary> 
//         public string GeneratorRegKeyName { get; set; }
//
//         /// <summary> 
//         /// Property that gets the generator base key name 
//         /// </summary> 
//         private string GeneratorRegKey => $@"Generators\{ContextGuid}\{GeneratorRegKeyName}";
//
//         /// <summary> 
//         ///     Called to register this attribute with the given context.  The context 
//         ///     contains the location where the registration information should be placed. 
//         ///     It also contains other information such as the type being registered and path information. 
//         /// </summary> 
//         public override void Register(RegistrationContext context)
//         {
//             using (var childKey = context.CreateKey(GeneratorRegKey))
//             {
//                 childKey.SetValue(string.Empty, GeneratorName);
//                 childKey.SetValue("CLSID", GeneratorGuid.ToString("B"));
//
//                 if (GeneratesDesignTimeSource)
//                     childKey.SetValue("GeneratesDesignTimeSource", 1);
//
//                 if (GeneratesSharedDesignTimeSource)
//                     childKey.SetValue("GeneratesSharedDesignTimeSource", 1);
//             }
//         }
//
//         /// <summary> 
//         /// Un-register this file extension. 
//         /// </summary> 
//         /// <param name="context"></param> 
//         public override void Unregister(RegistrationContext context)
//         {
//             context.RemoveKey(GeneratorRegKey);
//         }
//     }
// }
