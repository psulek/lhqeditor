using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using JavaScriptEngineSwitcher.Core;
using LHQ.Core.Interfaces;
using LHQ.Utils.Extensions;
using Newtonsoft.Json;
// using JsEngine = JavaScriptEngineSwitcher.Jint.JintJsEngine;
// using JsEngineSettings = JavaScriptEngineSwitcher.Jint.JintSettings;
using JsEngine = JavaScriptEngineSwitcher.ChakraCore.ChakraCoreJsEngine;
using JsEngineSettings = JavaScriptEngineSwitcher.ChakraCore.ChakraCoreSettings;

namespace LHQ.Gen.Lib
{
    public class Generator : IDisposable
    {
        private readonly ILogger _logger;
        private bool _disposed;
        private readonly JsEngine _engine;
        private readonly Dictionary<string, string> _generatedFiles = new Dictionary<string, string>();

        private static readonly string[] JsFiles =
        {
            "handlebars.min.js",
            "lhqgenerators.js"
        };

        private const string XpathRootNamespace = "//ns:Project/ns:PropertyGroup/ns:RootNamespace";

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

            var hostAddResultFile = new Action<string, string>((file, content) => { _generatedFiles.Add(file, content); });
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

        public static string FindOwnerCsProjectFile(string lhqModelFileName)
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
                var rootNamespace = GetRootNamespace(lhqModelFileName, csProj, out var referencedLhqFile);
                if (referencedLhqFile && (result == string.Empty || string.IsNullOrEmpty(resultNamespace)))
                {
                    result = csProj;
                    resultNamespace = rootNamespace;
                }
            }

            return result;
        }

        public static string GetRootNamespace(string lhqModelFileName, string csProjectFileName)
        {
            return GetRootNamespace(lhqModelFileName, csProjectFileName, out _);
        }

        public static string GetRootNamespace(string lhqModelFileName, string csProjectFileName,
            out bool referencedLhqFile)
        {
            referencedLhqFile = false;

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
                //if (referencedLhqFile)
                {
                    var t4FileElement = FindFileElement(lhqFileT4);
                    //CustomToolNamespace
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
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error getting root namespace. {e.GetFullExceptionDetails()}");
                rootNamespace = null;
            }

            return rootNamespace ?? string.Empty;
        }

        public Dictionary<string, string> Generate(string lhqModelFileName, string csProjectFileName,
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

            _logger.Info($"Generating files from {lhqModelFileName} ended.");

            var distinctFileKeys = _generatedFiles.Select(x => x.Key).Distinct().ToList();
            if (_generatedFiles.Count != distinctFileKeys.Count)
            {
                var duplicateFiles = _generatedFiles.Where(x => !distinctFileKeys.Contains(x.Key)).ToArray();
                _logger.Info("Duplicated generated files: " + string.Join(", ", duplicateFiles));
            }

            return _generatedFiles;
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
}
