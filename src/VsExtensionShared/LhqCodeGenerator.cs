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
// using System.Diagnostics;
// using System.IO;
// using System.Runtime.InteropServices;
// using EnvDTE;
// using Microsoft.VisualStudio.Shell;
// using Microsoft.VisualStudio.Shell.Interop;
// using VSLangProj80;
//
// namespace LHQ.VsExtension
// {
//     [ComVisible(true)]
//     [Guid("9237AA46-264B-4D58-9E1F-77F6C0D9CB09")]
//     [ProvideObject(typeof(LhqCodeGenerator))]
//     // https://github.com/JamesW75/visual-studio-project-type-guid
//     [CodeGeneratorRegistration(typeof(LhqCodeGenerator), nameof(LhqCodeGenerator), vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true)]
//     public class LhqCodeGenerator : IVsSingleFileGenerator
//     {
//         public int DefaultExtension(out string pbstrDefaultExtension)
//         {
//             pbstrDefaultExtension = ".lhq";
//             return pbstrDefaultExtension.Length;
//         }
//         
//         public int Generate(string wszInputFilePath, string bstrInputFileContents,
//             string wszDefaultNamespace, IntPtr[] rgbOutputFileContents,
//             out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
//         {
//             rgbOutputFileContents[0] = IntPtr.Zero;
//             pcbOutput = 0;
//             return 0;
//         }
//
//         // protected override string GetDefaultExtension()
//         // {
//         //     return ".lhq";
//         // }
//         //
//         // protected override BaseCodeGenerator.GenerateCodeResult GenerateCode(string inputFileContent)
//         // {
//         //     ProjectItem projectItem = GetProjectItem();
//         //
//         //     string file = Path.Combine(@"c:\tmp", Path.GetRandomFileName());
//         //     System.IO.File.WriteAllText(file, InputFilePath);
//         //     Trace.WriteLine("Saved to " + file);
//         //
//         //     return new BaseCodeGenerator.GenerateCodeResult { Buffer = null, EmptyBufferIsValid = true };
//         // }
//     }
// }
