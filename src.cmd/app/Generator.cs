using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using JavaScriptEngineSwitcher.Jint;

namespace LHQ.Cmd;

public class Generator : IDisposable
{
    private bool _disposed;
    private readonly JintJsEngine _engine;
    private readonly Dictionary<string,string> _generatedFiles = [];

    private static readonly string[] JsFiles =
    [
        "handlebars.min.js",
        "lhqgenerators.js"
    ];

    private const string XpathRootNamespace = "//ns:Project/ns:PropertyGroup/ns:RootNamespace";
    private const string XpathCustomToolNamespace1 = "//ns:Project/ns:ItemGroup/ns:Content[@Include='##MODELT4##']/ns:CustomToolNamespace";
    private const string XpathCustomToolNamespace2 = "//ns:Project/ns:ItemGroup/ns:None[@Update='##MODELT4##']/ns:CustomToolNamespace";

    private static class DataKeys
    {
        public const string Namespace = "namespace";
        //public const string OutDir = "outDir";
    }

    public Generator()
    {
        _engine = new JintJsEngine(new JintSettings
        {
            AllowReflection = false,
            EnableDebugging = true,
        });
        
        var hostDebugLog = new Action<string, string?>((msg, lvl) =>
        {   
            Console.WriteLine($"[{(lvl ?? "DEBUG")}] {msg}");
        });

        var hostAddResultFile = new Action<string, string>((file, content) =>
        {
            _generatedFiles.Add(file, content);
        });
        var hostPathCombine = new Func<string, string, string>(Path.Combine);

        _engine.EmbedHostObject("HostDebugLog", hostDebugLog);
        _engine.EmbedHostObject("HostAddResultFile", hostAddResultFile);
        _engine.EmbedHostObject("HostPathCombine", hostPathCombine);
        
        var type = typeof(Generator);
        var thisAssembly = type.Assembly;
        var rootNamespace = type.Namespace;

        foreach (var jsFile in JsFiles)
        {
            string resourceName = $"{rootNamespace}.js.{jsFile}";
            _engine.ExecuteResource(resourceName, thisAssembly);
        }

        var handlebarFiles = new Dictionary<string, string>();
        
        foreach (var file in Directory.GetFiles("hbs", "*.handlebars", SearchOption.TopDirectoryOnly))
        {
            var key = Path.GetFileNameWithoutExtension(file);
            handlebarFiles.Add(key, File.ReadAllText(file));
        } 

        _engine.SetVariableValue("__handlebarFiles", JsonSerializer.Serialize(handlebarFiles));
        _engine.Execute("LhqGenerators.TemplateManager.intialize(__handlebarFiles);");
    }

    private string GetRootNamespace(string lhqModelFileName, string csProjectFileName)
    {
        //var modelDir = Path.GetDirectoryName(lhqModelFileName);
        //var csProjFile = Directory.GetFiles(modelDir, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();

        var lhqFileT4 = Path.GetFileName(lhqModelFileName) + ".tt";

        string? rootNamespace = null;

        if (!string.IsNullOrEmpty(csProjectFileName))
        {
            try
            {
                var doc = XDocument.Parse(File.ReadAllText(csProjectFileName));
                var ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;
            
                // rootNamespace = doc
                //     .Descendants(ns + "PropertyGroup")
                //     .Select(e => e.Element(ns + "RootNamespace")?.Value)
                //     .FirstOrDefault(x => x != null);
            
                // var customToolNamespace = doc.Descendants(ns + "Content")
                //     .Where(e => e.Attribute("Include") != null && e.Attribute("Include")!.Value.Equals(lhqFile, StringComparison.InvariantCultureIgnoreCase))
                //     .Select(e => e.Element(ns + "CustomToolNamespace")?.Value)
                //     .FirstOrDefault();
                
                var manager = new XmlNamespaceManager(new NameTable());
                manager.AddNamespace("ns", ns.NamespaceName);

                var rootNamespaceElem = doc.XPathSelectElement(XpathRootNamespace, manager);
                rootNamespace = rootNamespaceElem?.Value;

                var customToolNamespaceElem = doc.XPathSelectElement(XpathCustomToolNamespace1.Replace("##MODELT4##", lhqFileT4), manager);
                if (customToolNamespaceElem == null)
                {
                    customToolNamespaceElem = doc.XPathSelectElement(XpathCustomToolNamespace2.Replace("##MODELT4##", lhqFileT4), manager);
                }

                var customToolNamespace = customToolNamespaceElem?.Value;
                if (!string.IsNullOrEmpty(customToolNamespace))
                {
                    rootNamespace = customToolNamespace;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                rootNamespace = null;
            }
        }

        return rootNamespace ?? string.Empty;
    }

    public Dictionary<string, string> Generate(string lhqModelFileName, string csProjectFileName, 
        Dictionary<string, object>? hostData = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(lhqModelFileName);
        ArgumentException.ThrowIfNullOrEmpty(csProjectFileName);
        
        if (!File.Exists(lhqModelFileName))
        {
            throw new FileNotFoundException($"LhQ model file '{lhqModelFileName}' was not found!");
        }

        if (!File.Exists(csProjectFileName))
        {
            throw new FileNotFoundException($"C# Project file '{csProjectFileName}' was not found!");
        }

        _generatedFiles.Clear();

        hostData ??= new Dictionary<string, object>();

        string rootNamespace = string.Empty;

        if (hostData.TryGetValue(DataKeys.Namespace, out var @namespace))
        {
            rootNamespace = @namespace as string ?? string.Empty;
        }
        
        if (string.IsNullOrEmpty(rootNamespace))
        {
            rootNamespace = GetRootNamespace(lhqModelFileName, csProjectFileName);
            hostData[DataKeys.Namespace] = rootNamespace;
        }

        if (string.IsNullOrEmpty(rootNamespace))
        {
            throw new Exception($"Missing parameter '{DataKeys.Namespace}' value!");
        }

        _engine.SetVariableValue("__model", File.ReadAllText(lhqModelFileName));
        var hostDataStr = JsonSerializer.Serialize(hostData);
        
        _engine.SetVariableValue("__hostData", hostDataStr);
        _engine.Execute("LhqGenerators.TemplateManager.runTemplate(__model, __hostData)");

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