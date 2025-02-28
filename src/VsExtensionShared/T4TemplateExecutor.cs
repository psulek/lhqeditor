#region License
// Copyright (c) 2025 Peter Šulek / ScaleHQ Solutions s.r.o.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

// #region License
// // Copyright (c) 2021 Peter Šulek / ScaleHQ Solutions s.r.o.
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
// #endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LHQ.Utils.Extensions;
using LHQ.VsExtension.Code;
using Microsoft.VisualStudio.TextTemplating.VSHost;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace LHQ.VsExtension
{
    // public static class T4Utils
    // {
    //     public static void SetCallContextData(string key, object value)
    //     {
    //         System.Runtime.Remoting.Messaging.CallContext.LogicalSetData(key, value);
    //     }
    //
    //     private static T GetCallContextData<T>(string key, T defaultValue)
    //     {
    //         T result = defaultValue;
    //         object o = System.Runtime.Remoting.Messaging.CallContext.LogicalGetData(key);
    //         if (o != null)
    //         {
    //             result = (T)o;
    //         }
    //
    //         return result;
    //     }
    //
    //     private static void RemoveCallContextData(string key)
    //     {
    //         System.Runtime.Remoting.Messaging.CallContext.FreeNamedDataSlot(key);
    //     }
    //
    //     public static bool HasCallContextData(string key)
    //     {
    //         return System.Runtime.Remoting.Messaging.CallContext.LogicalGetData(key) != null;
    //     }
    //
    //     public static void SetLastError(Exception e)
    //     {
    //         SetCallContextData("LastError", e);
    //     }
    //
    //     public static Dictionary<string, byte> GetOutputMessages()
    //     {
    //         return GetCallContextData<Dictionary<string, byte>>("OutputMessages", null);
    //     }
    //
    //     public static Exception GetLastError()
    //     {
    //         return GetCallContextData<Exception>("LastError", null);
    //     }
    //
    //     public static void RemoveLastError()
    //     {
    //         RemoveCallContextData("LastError");
    //     }
    //
    //     public static bool HasLastError()
    //     {
    //         return HasCallContextData("LastError");
    //     }
    //
    //     public static bool IsCalledFromEditor()
    //     {
    //         return GetCallContextData("CalledFromEditor", false);
    //     }
    //
    //     public static void SetCalledFromEditor()
    //     {
    //         SetCallContextData("CalledFromEditor", true);
    //     }
    //
    //     public static void RemoveCalledFromEditor()
    //     {
    //         RemoveCallContextData("CalledFromEditor");
    //     }
    //
    //     public static void RemoveOutputMessages()
    //     {
    //         RemoveCallContextData("OutputMessages");
    //     }
    // }

    /*
    public class T4TemplateExecutor
    {
        public static void RunTemplate(ITextTemplating t4, string templateFile, string templateContent = null)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    templateContent = string.IsNullOrEmpty(templateFile) ? templateContent : File.ReadAllText(templateFile);
                    var cb = new T4Callback();

                    T4Utils.SetCalledFromEditor();
                    VsPackageService.ClearOutputMessages();

#if DEBUG
                    bool genTeplate = false;
                    if (genTeplate)
                    {
                        string[] references;
                        string generatedTeplate = t4.PreprocessTemplate(templateFile, templateContent, cb, "LhqT4Generator", "LHQ", out references);
                    }
#endif

                    string result = t4.ProcessTemplate(templateFile, templateContent, cb);
                    string resultFileName =
                        Path.Combine(Path.GetDirectoryName(templateFile),
                            Path.GetFileNameWithoutExtension(templateFile))
                        + ".txt";

                    var lastError = T4Utils.GetLastError();

                    if (lastError != null || cb.HasErrors)
                    {
                        string baseError = "Error generating code from localization HQ model." + Environment.NewLine;

                        if (lastError is ApplicationException || lastError is IOException)
                        {
                            ShowError(baseError + lastError.Message);
                        }
                        else
                        {
                            File.WriteAllLines(resultFileName, cb.errorMessages);

                            ShowError(baseError + $"See '{resultFileName}' for more info.");
                        }
                    }
                    else
                    {
                        if (!File.Exists(resultFileName))
                        {
                            File.WriteAllText(resultFileName, result, cb.outputEncoding);
                        }

                        var outputMessages = T4Utils.GetOutputMessages();
                        if (outputMessages != null)
                        {
                            foreach (var outputMessage in outputMessages)
                            {
                                VsPackageService.AddMessageToOutput(outputMessage.Key, (OutputMessageType)outputMessage.Value);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugUtils.Error("[T4Generator] RunTemplate failed", e);
                }
                finally
                {
                    T4Utils.RemoveCalledFromEditor();
                    T4Utils.RemoveLastError();
                    T4Utils.RemoveOutputMessages();
                }
            });
        }

        private static void ShowError(string error)
        {
            VsPackageService.AddMessageToOutput(error, OutputMessageType.Error);
        }

        private class T4Callback : ITextTemplatingCallback
        {
            public readonly List<string> errorMessages = new List<string>();
            public string fileExtension = ".txt";
            public Encoding outputEncoding = Encoding.UTF8;

            public List<string> ErrorMessages => errorMessages;

            public bool HasErrors => ErrorMessages.Count > 0;

            public void ErrorCallback(bool warning, string message, int line, int column)
            {
                if (!warning)
                {
                    errorMessages.Add(message);
                }
            }

            public void SetFileExtension(string extension)
            {
                fileExtension = extension;
            }

            public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
            {
                outputEncoding = encoding;
            }

        }
    }
*/
}