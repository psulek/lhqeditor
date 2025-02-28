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
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using JavaScriptEngineSwitcher.Core;
using Newtonsoft.Json;
using NLog;
// using JsEngine = JavaScriptEngineSwitcher.Jint.JintJsEngine;
// using JsEngineSettings = JavaScriptEngineSwitcher.Jint.JintSettings;
using JsEngine = JavaScriptEngineSwitcher.ChakraCore.ChakraCoreJsEngine;
using JsEngineSettings = JavaScriptEngineSwitcher.ChakraCore.ChakraCoreSettings;

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
        private readonly Dictionary<string, LhqModelGroupSettings> _modelGroupSettings = new Dictionary<string, LhqModelGroupSettings>();

        private static readonly string[] _jsFiles =
        {
            "handlebars.min.js",
            "lhqgenerators.js"
        };

        private const string XpathRootNamespace = "//ns:Project/ns:PropertyGroup/ns:RootNamespace";
        private const string XpathAssemblyName = "//ns:Project/ns:PropertyGroup/ns:AssemblyName";

        private static readonly string[] ItemGroupTypes = { "Content", "None" };
        private static readonly string[] ItemGroupTypesAttrs = { "Include", "Update" };

        private const string CsProjectXPath =
            "//ns:Project/ns:ItemGroup/ns:##TYPE##[@##ATTR##='##FILE##']";

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

            var hostDebugLog = new Action<string, string>((msg, lvl) => { Console.WriteLine($"[{(lvl ?? "DEBUG")}] {msg}"); });

            Action<string, string, bool, string> hostAddResultFile = HostAddResultFile;
            Action<string, string> hostAddModelGroupSettings = HostAddModelGroupSettings;
            var hostPathCombine = new Func<string, string, string>(Path.Combine);
            var hostWebHtmlEncode = new Func<string, string>(System.Net.WebUtility.HtmlEncode);
            var hostStopwatchStart = new Func<long>(() => DateTime.UtcNow.Ticks);
            var hostStopwatchEnd = new Func<long, string>(start =>
                {
                    var startTime = new DateTime(start, DateTimeKind.Utc);
                    var duration = DateTime.UtcNow - startTime;
                    return duration.ToString();
                });

            _engine.EmbedHostObject("HostDebugLog", hostDebugLog);
            _engine.EmbedHostObject("HostAddResultFile", hostAddResultFile);
            _engine.EmbedHostObject("HostAddModelGroupSettings", hostAddModelGroupSettings);
            _engine.EmbedHostObject("HostPathCombine", hostPathCombine);
            _engine.EmbedHostObject("HostWebHtmlEncode", hostWebHtmlEncode);
            _engine.EmbedHostObject("HostStopwatchStart", hostStopwatchStart);
            _engine.EmbedHostObject("HostStopwatchEnd", hostStopwatchEnd);

            var type = typeof(Generator);
            var thisAssembly = type.Assembly;
            var rootNamespace = type.Namespace;

            foreach (var jsFile in _jsFiles)
            {
                string resourceName = $"{rootNamespace}.js.{jsFile}";
                _engine.ExecuteResource(resourceName, thisAssembly);
            }

            var handlebarFiles = new Dictionary<string, string>();
            var hbsFiles = thisAssembly.GetManifestResourceNames().Where(x => x.Contains(".hbs.")).ToArray();
            foreach (var hbsFile in hbsFiles)
            {
                var content = JavaScriptEngineSwitcher.Core.Utilities.Utils.GetResourceAsString(hbsFile, thisAssembly);
                var key = hbsFile.Replace($"{rootNamespace}.hbs.", "").Replace(".handlebars", "");
                handlebarFiles.Add(key, content);
            }

            var json = JsonConvert.SerializeObject(handlebarFiles);
            _engine.SetVariableValue("__handlebarFiles", json);
            _engine.Execute("LhqGenerators.TemplateManager.intialize(__handlebarFiles);");
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

        private void HostAddResultFile(string file, string content, bool bom, string le)
        {
            var lineEndings = (LineEndings)Enum.Parse(typeof(LineEndings), le, true);
            _generatedFiles.Add(new GeneratedFile(file, content, bom, lineEndings));
        }

        private void HostAddModelGroupSettings(string group, string settings)
        {
            var jsonSettings = JsonConvert.DeserializeObject<Dictionary<string, object>>(settings);

            LhqModelGroupSettings groupSettings;
            if (!_modelGroupSettings.TryGetValue(group, out groupSettings))
            {
                groupSettings = new LhqModelGroupSettings(group);
                _modelGroupSettings.Add(group, groupSettings);
            }

            groupSettings.Settings = jsonSettings;
        }

        private string FindOwnerCsProjectFile(string lhqModelFileName)
        {
            if (!File.Exists(lhqModelFileName))
            {
                return string.Empty;
            }

            var dir = Path.GetDirectoryName(lhqModelFileName);
            var csProjectFiles = Directory.GetFiles(dir, "*.csproj", SearchOption.TopDirectoryOnly);
            var result = string.Empty;
            var resultNamespace = string.Empty;
            foreach (var csProj in csProjectFiles)
            {
                var rootNamespace = GetRootNamespace(lhqModelFileName, csProj);
                if (result == string.Empty || string.IsNullOrEmpty(resultNamespace))
                {
                    result = csProj;
                    resultNamespace = rootNamespace;
                }
            }

            return result;
        }

        private string GetRootNamespace(string lhqModelFileName, string csProjectFileName)
        {
            return GetRootNamespace(lhqModelFileName, csProjectFileName, _logger);
        }

        internal static string GetRootNamespace(string lhqModelFileName, string csProjectFileName,
            ILogger logger)
        {
            var referencedLhqFile = false;

            if (string.IsNullOrEmpty(csProjectFileName) || !File.Exists(csProjectFileName))
            {
                return string.Empty;
            }

            lhqModelFileName = Path.GetFileName(lhqModelFileName);
            var lhqFileT4 = lhqModelFileName + ".tt";

            string rootNamespace;

            try
            {
                var doc = XDocument.Parse(File.ReadAllText(csProjectFileName));
                var ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;

                var manager = new XmlNamespaceManager(new NameTable());
                manager.AddNamespace("ns", ns.NamespaceName);

                XElement FindFileElement(string fileName)
                {
                    foreach (var itemGroupType in ItemGroupTypes)
                    {
                        foreach (var attr in ItemGroupTypesAttrs)
                        {
                            var xpath = CsProjectXPath.Replace("##TYPE##", itemGroupType)
                                .Replace("##ATTR##", attr).Replace("##FILE##", fileName);

                            var element = doc.XPathSelectElement(xpath, manager);
                            if (element != null)
                            {
                                return element;
                            }
                        }
                    }

                    return null;
                }

                var rootNamespaceElem = doc.XPathSelectElement(XpathRootNamespace, manager);
                rootNamespace = rootNamespaceElem?.Value;

                // if csproj has imported lhq file (lhqModelFileName) explicitly eg: /ItemGroup/None[@Update="Strings.lhq"] (and other variants)
                // we can look at CustomToolNamespace element under '{lhqModelFileName}.tt' element (if it exists)
                referencedLhqFile = FindFileElement(lhqModelFileName) != null;
                var t4FileElement = FindFileElement(lhqFileT4);
                if (t4FileElement != null)
                {
                    var dependentUpon = t4FileElement.Descendants(ns + "DependentUpon").FirstOrDefault()?.Value;
                    if (!string.IsNullOrEmpty(dependentUpon) && dependentUpon == lhqModelFileName)
                    {
                        referencedLhqFile = true;
                    }

                    var customToolNamespace =
                        t4FileElement.Descendants(ns + "CustomToolNamespace").FirstOrDefault()?.Value;
                    if (!string.IsNullOrEmpty(customToolNamespace))
                    {
                        rootNamespace = customToolNamespace;
                    }
                }

                if (string.IsNullOrEmpty(rootNamespace))
                {
                    var assemblyNameElem = doc.XPathSelectElement(XpathAssemblyName, manager);
                    rootNamespace = assemblyNameElem?.Value;

                    if (string.IsNullOrEmpty(rootNamespace) && !string.IsNullOrEmpty(csProjectFileName))
                    {
                        rootNamespace = Path.GetFileNameWithoutExtension(csProjectFileName).Replace(" ", "_");
                    }
                }
            }
            catch (Exception e)
            {
                logger?.Error(e, "Error getting root namespace.");
                rootNamespace = null;
            }

            return rootNamespace ?? string.Empty;
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
            if (string.IsNullOrEmpty(rootNamespace))
            {
                if (string.IsNullOrEmpty(csProjectFileName))
                {
                    csProjectFileName = FindOwnerCsProjectFile(lhqModelFileName);
                    if (!string.IsNullOrEmpty(csProjectFileName))
                    {
                        _logger.Info($"Found '{csProjectFileName}' associated with '{lhqModelFileName}'.");
                        csProjFound = true;
                    }
                }

                rootNamespace = GetRootNamespace(lhqModelFileName, csProjectFileName);
                hostData[DataKeys.Namespace] = rootNamespace;
            }
            else
            {
                _logger.Info($"Using '{DataKeys.Namespace}' value '{rootNamespace}' from cmd args.");
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Starting code generating for:");
            sb.AppendLine($"- lhq model file: {lhqModelFileName}");
            sb.AppendLine(
                $"- c# project file: {(string.IsNullOrEmpty(csProjectFileName) ? "-" : csProjectFileName)} ({(csProjFound ? "auto found" : "cmd")})");
            sb.AppendLine($"- out dir: {outDir}");

            _logger.Info(sb.ToString());

            try
            {
                _engine.SetVariableValue("__model", File.ReadAllText(lhqModelFileName));
                var hostDataStr = JsonConvert.SerializeObject(hostData);

                _engine.SetVariableValue("__hostData", hostDataStr);
                _engine.Execute("LhqGenerators.TemplateManager.runTemplate(__model, __hostData)");
            }
            catch (Exception e)
            {
                if (e is JsRuntimeException jsRuntimeException)
                {
                    if (jsRuntimeException.Type == "AppError")
                    {
                        var callStack = jsRuntimeException.CallStack;
                        var description = jsRuntimeException.Description;
                        var documentName = jsRuntimeException.DocumentName;

                        var items = description.Split('\n');
                        var title = items.Length == 0 ? description : items[0];
                        var message = items.Length > 1 ? string.Join("\n", items.Skip(1)) : string.Empty;

                        _logger.Error($"{title}\n{message}\nFILE: {documentName} >> {callStack}");
                        throw new GeneratorException(title, message, e);
                    }
                }

                throw;
            }

            _logger.Info($"Generating {_generatedFiles.Count} files from {lhqModelFileName} ended.");

            var distinctFileKeys = _generatedFiles.Select(x => x.FileName).Distinct().ToList();
            if (_generatedFiles.Count != distinctFileKeys.Count)
            {
                var duplicateFiles = _generatedFiles.Where(x => !distinctFileKeys.Contains(x.FileName))
                    .Select(x => x.FileName).ToArray();

                _logger.Info("Duplicated generated files: " + string.Join(", ", duplicateFiles));
            }

            return new GenerateResult(_generatedFiles, _modelGroupSettings.Values.ToList());
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
}
