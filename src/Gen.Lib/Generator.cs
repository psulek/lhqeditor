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
    public class Generator : IDisposable
    {
        private ILogger _logger;
        private bool _disposed;
        private readonly JsEngine _engine;
        private readonly List<GeneratedFile> _generatedFiles = new List<GeneratedFile>();
        private readonly Dictionary<string, LhqModelGroupSettings> _modelGroupSettings = new Dictionary<string, LhqModelGroupSettings>();

        private static readonly string[] JsFiles =
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

                    //return (DateTime.Now.Ticks - start.ToString());
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

            foreach (var jsFile in JsFiles)
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

        public string FindOwnerCsProjectFile(string lhqModelFileName)
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
                //if (referencedLhqFile && (result == string.Empty || string.IsNullOrEmpty(resultNamespace)))
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

        public static string GetRootNamespace(string lhqModelFileName, string csProjectFileName,
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
                }
            }
            catch (Exception e)
            {
                logger?.Error(e, "Error getting root namespace.");
                rootNamespace = null;
            }

            return rootNamespace ?? string.Empty;
        }

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
                        //Utils.AddToLogFile($"C# project was not specified but one was auto find by app: {csProjectFileName}");
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

    public enum LineEndings
    {
        LF,
        CRLF
    }
}
