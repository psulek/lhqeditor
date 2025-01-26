using JavaScriptEngineSwitcher.Jint;
using Newtonsoft.Json;

namespace CmdGen.Code;

public class Generator : IDisposable
{
    private bool _disposed;
    private readonly JintJsEngine _engine;
    private readonly Dictionary<string,string> generatedFiles = [];

    private static readonly string[] JsFiles =
    [
        "handlebars.min.js",
        "lhqgenerators.js"
    ];

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
            generatedFiles.Add(file, content);
        });
        var hostPathCombine = new Func<string, string, string>(Path.Combine);

        _engine.EmbedHostObject("HostDebugLog", hostDebugLog);
        _engine.EmbedHostObject("HostAddResultFile", hostAddResultFile);
        _engine.EmbedHostObject("HostPathCombine", hostPathCombine);
        
        //Path.IsPathRooted()


        var type = typeof(Generator);
        var thisAssembly = type.Assembly;
        var assemblyName = thisAssembly.GetName().Name;

        foreach (var jsFile in JsFiles)
        {
            string resourceName = $"{assemblyName}.js.{jsFile}";
            _engine.ExecuteResource(resourceName, thisAssembly);
        }

        var handlebarFiles = new Dictionary<string, string>();
        
        foreach (var file in Directory.GetFiles("hbs", "*.handlebars", SearchOption.TopDirectoryOnly))
        {
            var key = Path.GetFileNameWithoutExtension(file);
            handlebarFiles.Add(key, File.ReadAllText(file));
        } 

        _engine.SetVariableValue("__handlebarFiles", JsonConvert.SerializeObject(handlebarFiles));
        _engine.Execute("LhqGenerators.TemplateManager.intialize(__handlebarFiles);");
    }

    public string Generate(string? lhqModelFileName, 
        Dictionary<string, object>? extraData = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(lhqModelFileName);
        if (!File.Exists(lhqModelFileName))
        {
            throw new FileNotFoundException($"File not found: {lhqModelFileName}");
        }
        
        generatedFiles.Clear();
        
        _engine.SetVariableValue("__model", File.ReadAllText(lhqModelFileName));
        var hostData = extraData?.Count > 0 ? JsonConvert.SerializeObject(extraData) : "{}";
        
        _engine.SetVariableValue("__hostData", hostData);
        _engine.Execute("LhqGenerators.TemplateManager.runTemplate(__model, __hostData)");
        
        Console.WriteLine($"Generated {generatedFiles.Count} files:\n");

        foreach (var file in generatedFiles)
        {
            Console.WriteLine($"[{file.Key}]");
            //Console.WriteLine(file.Value);
            
            var dir = Path.GetDirectoryName(file.Key);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            File.WriteAllText(file.Key, file.Value);
        }
        
        return string.Empty;
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