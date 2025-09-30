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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JavaScriptEngineSwitcher.Core;
using JetBrains.Annotations;
using LHQ.Utils;
using Newtonsoft.Json;
using NLog;
// using JsEngine = JavaScriptEngineSwitcher.ChakraCore.ChakraCoreJsEngine;
// using JsEngineSettings = JavaScriptEngineSwitcher.ChakraCore.ChakraCoreSettings;
using JsEngine = JavaScriptEngineSwitcher.V8.V8JsEngine;
using JsEngineSettings = JavaScriptEngineSwitcher.V8.V8Settings;

// ReSharper disable InconsistentNaming

namespace LHQ.Gen.Lib
{
    /// <summary>
    /// Code generator that generates files from *.lhq model files.
    /// </summary>
    public class Generator : IDisposable
    {
        private ILogger _logger;
        private bool _disposed;
        private readonly JsEngine _engine;
        private readonly List<GeneratedFile> _generatedFiles = new List<GeneratedFile>();

        private static class DataKeys
        {
            public const string Namespace = "namespace";
        }

        /// <summary>
        /// Initializes new instance of <see cref="Generator"/>
        /// </summary>
        /// <param name="logger">Logger that implements <see cref="NLog.ILogger"/> which will be used to log messages.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="logger"/> is null.
        /// </exception>
        public Generator(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _engine = new JsEngine(new JsEngineSettings
            {
                AllowReflection = false,
                //EnableDebugging = true // Jint
            });

            var type = typeof(Generator);
            var thisAssembly = type.Assembly;
            var rootNamespace = type.Namespace;

            var queryRoot = $"{rootNamespace}.content";
            string[] allManigestResourceNames = thisAssembly.GetManifestResourceNames();

            try
            {
                var queryBrowser = $"{queryRoot}.browser.";
                var jsFiles = allManigestResourceNames.Where(x => x.StartsWith(queryBrowser)).ToList();
                if (jsFiles.Count == 0)
                {
                    throw new ApplicationException("Could not find lhq engine implementation files in assembly.");
                }

                foreach (var jsFile in jsFiles)
                {
                    _engine.ExecuteResource(jsFile, thisAssembly);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var handlebarFiles = new Dictionary<string, string>();
            //var queryTemplates = $"{queryRoot}.templates.";
            var queryTemplates = $"{queryRoot}.hbs.";
            var hbsFiles = allManigestResourceNames.Where(x => x.StartsWith(queryTemplates) && x.EndsWith(".hbs")).ToList();
            if (hbsFiles.Count == 0)
            {
                throw new ApplicationException("Could not find any lhq code templates (*.hbs) in assembly.");
            }

            foreach (var hbsFile in hbsFiles)
            {
                var content = JavaScriptEngineSwitcher.Core.Utilities.Utils.GetResourceAsString(hbsFile, thisAssembly);
                var key = hbsFile.Replace(queryTemplates, "").Replace(".hbs", "");
                handlebarFiles.Add(key, content);
            }

            string metadataFileName = allManigestResourceNames.Single(x => x.EndsWith("metadata.json"));
            var templatesMetadata = JavaScriptEngineSwitcher.Core.Utilities.Utils.GetResourceAsString(metadataFileName, thisAssembly);

            var hbsTemplatesJson = JsonConvert.SerializeObject(handlebarFiles);
            _engine.SetVariableValue("__hbsTemplates", hbsTemplatesJson);
            _engine.SetVariableValue("__templatesMetadata", templatesMetadata);
            _engine.EmbedHostObject("__hostEnvironment", new JSHostEnvironment());
            var initJson = "{hbsTemplates: JSON.parse(__hbsTemplates), templatesMetadata: JSON.parse(__templatesMetadata), hostEnvironment: __hostEnvironment}";
            _engine.Execute($"LhqGenerators.Generator.initialize({initJson});");
        }

        /// <summary>
        /// Updates current logger to new one.
        /// </summary>
        /// <param name="logger">new <see cref="NLog.ILogger"/> to replace current one.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="logger"/> is null.
        /// </exception>
        public void UpdateLogger(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public string AutodetectNamespace(string lhqModelFileName)
        {
            var namespaceInfo = FindNamespaceForModel(lhqModelFileName);
            var csProjectFileName = namespaceInfo?.CsProjectFileName;
            if (!string.IsNullOrEmpty(csProjectFileName))
            {
                _logger?.Info($"Found '{csProjectFileName}' associated with '{lhqModelFileName}'.");
            }

            if (namespaceInfo?.NamespaceDynamicExpression == true)
            {
                namespaceInfo.Namespace = string.Empty;
                _logger?.Warn(
                    "Warning: \nValue in 'RootNamespace' or 'AssemblyName' element contains dynamic expression which is not supported.\n" +
                    "This value will not be used for 'Namespace' in generator.\n" +
                    "Set namespace directly in the lhq file in C# template setting 'Namespace' or provide namespace via cmd '--data namespace=<value>'.");
            }

            return namespaceInfo?.Namespace ?? string.Empty;
        }

        public CSharpNamespaceInfo FindNamespaceForModel(string lhqModelFileName, string[] csProjectFileNames = null)
        {
            CSharpNamespaceInfo result = null;

            var lhqFileInfo = new FileInfo(lhqModelFileName);
            var lhqModelFile = new JSFileInfo
            {
                Exist = File.Exists(lhqModelFileName),
                Full = lhqFileInfo.FullName,
                Ext = lhqFileInfo.Extension,
                Extless = lhqFileInfo.Name.Replace(lhqFileInfo.Extension, ""),
                Relative = lhqFileInfo.Name,
                Dirname = lhqFileInfo.DirectoryName,
                Basename = Path.GetFileName(lhqFileInfo.FullName)
            };

            var csProjectFiles = new List<JSFileInfo>();

            if (csProjectFileNames == null || csProjectFileNames.Length == 0)
            {
                csProjectFileNames = string.IsNullOrEmpty(lhqFileInfo.DirectoryName)
                    ? Array.Empty<string>()
                    : Directory.GetFiles(lhqFileInfo.DirectoryName, "*.csproj", SearchOption.TopDirectoryOnly);
            }

            foreach (string csProject in csProjectFileNames)
            {
                var csProjectFileInfo = new FileInfo(csProject);
                bool exists = File.Exists(csProject);
                var csProjectFile = new JSFileInfo
                {
                    Exist = exists,
                    Full = csProjectFileInfo.FullName,
                    Ext = csProjectFileInfo.Extension,
                    Extless = csProjectFileInfo.Name.Replace(csProjectFileInfo.Extension, ""),
                    Relative = csProjectFileInfo.Name,
                    Dirname = csProjectFileInfo.DirectoryName,
                    Basename = Path.GetFileName(csProjectFileInfo.FullName),
                    Content = exists ? File.ReadAllText(csProjectFileInfo.FullName) : null
                };

                csProjectFiles.Add(csProjectFile);
            }

            var options = new JSFindNamespaceOptions
            {
                LhqModelFile = lhqModelFile,
                CsProjectFiles = csProjectFiles.ToArray()
            };
            _engine.SetVariableValue("__options", JsonConvert.SerializeObject(options));

            try
            {
                var generateCall = "LhqGenerators.namespaceUtils.findNamespaceForModel(JSON.parse(__options))";
                var resultJson = _engine.Evaluate<string>($"JSON.stringify({generateCall})");
                var callResult = JsonConvert.DeserializeObject<JSCSharpNamespaceInfo>(resultJson);
                if (callResult != null)
                {
                    result = new CSharpNamespaceInfo(callResult);
                }
            }
            catch (Exception e)
            {
                HandleJsRuntimeException(e);
                throw;
            }
            finally
            {
                _engine.RemoveVariable("__options");
            }

            return result;
        }

        /// <summary>
        /// Generates code from given LHQ model file (*.lhq) and C# project file (*.csproj0 using handlebars templates.
        /// </summary>
        /// <param name="lhqModelFileName">Full path to LHQ model file (*.lhq).</param>
        /// <param name="csProjectFileName">
        /// Full path to C# project file (*.csproj). This value is optional and if is not specified, process will try to find it
        /// in the same directory as lhq model file.
        /// </param>
        /// <param name="outDir">Output directory for generated files.</param>
        /// <param name="hostData">Optional dictionary with host data that can be accessed in templates.</param>
        /// <returns>Result of generation process with list of generated files and used model group settings as an <see cref="GenerateResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="lhqModelFileName"/>.</exception>
        /// <exception cref="FileNotFoundException">Thrown when <paramref name="lhqModelFileName"/>.</exception>
        /// <exception cref="GeneratorException">Thrown when generation process fails on executing handlerbars templates.</exception>
        public GenerateResult Generate(string lhqModelFileName, string csProjectFileName,
            string outDir, Dictionary<string, object> hostData = null)
        {
            if (string.IsNullOrEmpty(lhqModelFileName))
            {
                throw new ArgumentNullException(lhqModelFileName);
            }

            if (!File.Exists(lhqModelFileName))
            {
                throw new FileNotFoundException($"LhQ model file '{lhqModelFileName}' was not found!");
            }

            _generatedFiles.Clear();

            if (hostData == null)
            {
                hostData = new Dictionary<string, object>();
            }

            string rootNamespace = string.Empty;

            if (hostData.TryGetValue(DataKeys.Namespace, out var @namespace))
            {
                rootNamespace = @namespace as string ?? string.Empty;
            }

            bool csProjFound = false;
            if (!string.IsNullOrEmpty(rootNamespace))
            {
                _logger.Info($"Using '{DataKeys.Namespace}' value '{rootNamespace}' from host data dictionary.");
            }

            var sb = new StringBuilder();
            sb.AppendLine("Starting code generating for:");
            sb.AppendLine($"- lhq model file: {lhqModelFileName}");
            sb.AppendLine(
                $"- c# project file: {(string.IsNullOrEmpty(csProjectFileName) ? "-" : csProjectFileName)} ({(csProjFound ? "auto found" : "cmd")})");
            sb.AppendLine($"- out dir: {outDir}");

            _logger.Info(sb.ToString());

            try
            {
                _engine.SetVariableValue("__modelFileName", lhqModelFileName);
                _engine.SetVariableValue("__model", File.ReadAllText(lhqModelFileName));
                var hostDataStr = JsonConvert.SerializeObject(hostData);
                _engine.SetVariableValue("__hostData", hostDataStr);

                var generateCall = "new LhqGenerators.Generator().generate(__modelFileName, __model, __hostData)";
                var resultJson = _engine.Evaluate<string>($"JSON.stringify({generateCall})");
                var result = JsonConvert.DeserializeObject<JSGenerateResult>(resultJson);
                result.GeneratedFiles?.ForEach(x => _generatedFiles.Add(new GeneratedFile(x.FileName, x.Content, x.Bom, x.LineEndings)));
            }
            catch (Exception e)
            {
                HandleJsRuntimeException(e);
                throw;
            }
            finally
            {
                _engine.RemoveVariable("__modelFileName");
                _engine.RemoveVariable("__model");
                _engine.RemoveVariable("__hostData");
            }

            _logger.Info($"Generating {_generatedFiles.Count} files from {lhqModelFileName} ended.");

            var distinctFileKeys = _generatedFiles.Select(x => x.FileName).Distinct().ToList();
            if (_generatedFiles.Count != distinctFileKeys.Count)
            {
                var duplicateFiles = _generatedFiles.Where(x => !distinctFileKeys.Contains(x.FileName))
                    .Select(x => x.FileName).ToArray();

                _logger.Info("Duplicated generated files: " + string.Join(", ", duplicateFiles));
            }

            return new GenerateResult(_generatedFiles);
        }
        
        public string LoadAndSerialize(string jsonModel)
        {
            try
            {
                _engine.SetVariableValue("__data", jsonModel);

                var generateCall = "LhqGenerators.ModelUtils.loadAndSerialize(__data)";
                var result = _engine.Evaluate<string>(generateCall);
                return result;
                //var result = JsonConvert.DeserializeObject<JSGenerateResult>(resultJson);
                //result.GeneratedFiles?.ForEach(x => _generatedFiles.Add(new GeneratedFile(x.FileName, x.Content, x.Bom, x.LineEndings)));
            }
            catch (Exception e)
            {
                HandleJsRuntimeException(e);
                throw;
            }
            finally
            {
                _engine.RemoveVariable("__data");
            }
        }

        private void HandleJsRuntimeException(Exception exception)
        {
            // var jsException = exception as JsException;
            // if (exception is JsRuntimeException jsRuntimeException)
            
            var errorKind = GeneratorErrorKind.GenericError;
            var errorCode = string.Empty;
            string title = "Error";
            string message = "";
            string callStack = "";
            string documentName = "";

            T GetDynamicValue<T>(dynamic dyn, string property, T defaultValue = default)
            {
                var val = dyn[property];
                if (val != null && val.GetType() == typeof(Microsoft.ClearScript.Undefined))
                {
                    return defaultValue;
                }

                return val;
            }

            if (exception.InnerException is Microsoft.ClearScript.ScriptEngineException see )
            {
                if (see.ScriptException != null && GetDynamicValue<string>(see.ScriptException, "name") == "AppError")
                {
                    // callStack = see.ScriptException.callStack ?? "";
                    callStack = GetDynamicValue<string>(see.ScriptException, "stack", "");
                    message = GetDynamicValue<string>(see.ScriptException, "message", "");
                    errorCode = GetDynamicValue<string>(see.ScriptException, "code", "");
                    string kind = GetDynamicValue<string>(see.ScriptException, "kind", "");
                    if (!string.IsNullOrEmpty(kind) && Enum.TryParse(kind, true, out GeneratorErrorKind parsedKind))
                    {
                        errorKind = parsedKind;
                    }
                }
            }
            
            if (exception is JsRuntimeException jsRuntimeException)
            {
                if (jsRuntimeException.Type == "AppError")
                {
                    callStack = jsRuntimeException.CallStack;
                    var description = jsRuntimeException.Description;
                    documentName = jsRuntimeException.DocumentName;

                    var items = description.Split('\n');
                    title = items.Length == 0 ? description : items[0];
                    message = items.Length > 1 ? string.Join("\n", items.Skip(1)) : string.Empty;
                }
            }
            
            _logger.Error($"{title}\n{message}\nFILE: {documentName} >> {callStack}");
            throw new GeneratorException(title, message, errorKind, errorCode, exception);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _engine.Dispose();
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Represents line endings type.
    /// </summary>
    public enum LineEndings
    {
        /// <summary>
        /// Unix like line endings (LF).
        /// </summary>
        LF,

        /// <summary>
        /// Windows like line endings (CRLF).
        /// </summary>
        CRLF
    }

    public class JSGenerateResult
    {
        [JsonProperty("generatedFiles")]
        public List<JSGeneratedFile> GeneratedFiles { get; set; }
    }

    public class JSGeneratedFile
    {
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("bom")]
        public bool Bom { get; set; }

        [JsonProperty("lineEndings")]
        public LineEndings LineEndings { get; set; }
    }

    public class JSFileInfo
    {
        [JsonProperty("exist")]
        public bool Exist { get; set; }

        [JsonProperty("full")]
        public string Full { get; set; }

        [JsonProperty("ext")]
        public string Ext { get; set; }

        [JsonProperty("extless")]
        public string Extless { get; set; }

        [JsonProperty("relative")]
        public string Relative { get; set; } // Optional

        [JsonProperty("dirname")]
        public string Dirname { get; set; }

        [JsonProperty("basename")]
        public string Basename { get; set; }

        [JsonProperty("content")]
        public object Content { get; set; } // Optional, can be string or byte[]
    }

    public class JSFindNamespaceOptions
    {
        [JsonProperty("lhqModelFile")]
        public JSFileInfo LhqModelFile { get; set; }

        [JsonProperty("csProjectFiles")]
        public JSFileInfo[] CsProjectFiles { get; set; }

        [JsonProperty("allowFileName")]
        public bool? AllowFileName { get; set; }
    }

    public class JSCSharpNamespaceInfo
    {
        [JsonProperty("csProjectFileName")]
        public JSFileInfo CsProjectFileName { get; set; }

        [JsonProperty("t4FileName")]
        public string T4FileName { get; set; }

        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        [JsonProperty("referencedLhqFile")]
        public bool ReferencedLhqFile { get; set; }

        [JsonProperty("referencedT4File")]
        public bool ReferencedT4File { get; set; }

        [JsonProperty("namespaceDynamicExpression")]
        public bool NamespaceDynamicExpression { get; set; }
    }

    [PublicAPI]
    public class JSHostEnvironment // inherits interface 'IHostEnvironment' from npm '@lhq/lhq-generators' package 
    {
        public void debugLog(string message)
        {
            Console.WriteLine(message);
        }

        public string pathCombine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        // public string webHtmlEncode(string input)
        // {
        //     return System.Net.WebUtility.HtmlEncode(input);
        // }

        // public double stopwatchStart()
        // {
        //     return DateTime.UtcNow.Ticks;
        // }
        //
        // public string stopwatchEnd(long start)
        // {
        //     var startTime = new DateTime(start, DateTimeKind.Utc);
        //     var duration = DateTime.UtcNow - startTime;
        //     return duration.ToString();
        // }
    }
}