using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using JavaScriptEngineSwitcher.Core;
using Jint;
using Jint.Runtime;
using LHQ.Cmd;
using Pastel;

string Grey(string msg) => msg.Pastel(ConsoleColor.Gray);
string White(string msg) => msg.Pastel(ConsoleColor.White);
string Error(string msg) => msg.Pastel(ConsoleColor.Red);

//args = ["--help"];

var logFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "lhqcmd.log");

AddToLogFile("Program lhqcmd.exe started.");

RunTestData();

var missingParams = args.Length < 2;

try
{
    Console.OutputEncoding = Encoding.UTF8;

    Console.WriteLine($"{White("LHQ Model Generator")}  " +
                      Grey($"Copyright (c) {DateTime.Today.Year} ScaleHQ Solutions\n"));

    if (missingParams)
    {
        WriteHelp();
    }
    else
    {
        var lhqFile = args[0];
        var csProjFile = args[1];

        if (string.IsNullOrEmpty(lhqFile) || !File.Exists(lhqFile))
        {
            throw new Exception($"LhQ model file '{lhqFile}' was not found!");
        }

        if (!string.IsNullOrEmpty(csProjFile) && !File.Exists(csProjFile))
        {
            throw new Exception($"C# project file '{csProjFile}' was not found!");
        }

        string outDir = args.Length > 2 ? args[2] : string.Empty;
        if (!string.IsNullOrEmpty(outDir))
        {
            if (!Directory.Exists(outDir))
            {
                throw new DirectoryNotFoundException($"Output directory '{outDir}' was not found!");
            }
        }

        if (string.IsNullOrEmpty(outDir))
        {
            outDir = !string.IsNullOrEmpty(csProjFile)
                ? Path.GetDirectoryName(csProjFile)!
                : Path.GetDirectoryName(lhqFile)!;
        }

        if (string.IsNullOrEmpty(outDir))
        {
            throw new Exception("Unable to determine output directory!");
        }

        Dictionary<string, object> hostData = new();
        if (args.Length > 2)
        {
            int startIdx = args[2].StartsWith("--") ? 2 : 3;
            if (startIdx == 2 || args.Length > 3)
            {
                for (int i = startIdx; i < args.Length; i++)
                {
                    var item = args[i];
                    if (item.StartsWith("--"))
                    {
                        var values = item.Split("=");
                        var key = values[0].Length > 2 ? values[0].Substring(2) : string.Empty;
                        var value = values[1];

                        if (!string.IsNullOrEmpty(key))
                        {
                            hostData[key] = value;
                        }
                    }
                }
            }
        }

        Console.WriteLine($"LHQ model file: {lhqFile.Pastel(ConsoleColor.DarkCyan)}");
        Console.WriteLine($"C# project file: {csProjFile.Pastel(ConsoleColor.DarkCyan)}");
        Console.WriteLine($"Output directory: {outDir.Pastel(ConsoleColor.DarkCyan)}");
        Console.WriteLine("Generating files from LHQ model started ...");

        using var generator = new Generator();
        var start = Stopwatch.GetTimestamp();
        var generatedFiles = generator.Generate(lhqFile, csProjFile, hostData);
        var elapsedTime = Stopwatch.GetElapsedTime(start);

        Console.WriteLine($"Generated {generatedFiles.Count} files in {elapsedTime:g}\n");

        foreach (var file in generatedFiles)
        {
            string fileName = Path.Combine(outDir, file.Key);
            Console.WriteLine($"Generated file: {fileName.Pastel(ConsoleColor.DarkCyan)}");

            var dir = Path.GetDirectoryName(fileName);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            FileHelper.WriteAllText(fileName, file.Value);
        }

        Console.WriteLine($"Successfully saved {generatedFiles.Count} files.");
    }
}
catch (Exception e)
{
    bool genericLog = true;

    StringBuilder sb = new();
    FillException(e, sb);

    AddToLogFile(sb.ToString());

    try
    {
        if (e is JsRuntimeException jsRuntimeException &&
            e.InnerException is JavaScriptException jsException)
        {
            if (jsRuntimeException.Type == "AppError")
            {
                var message = jsException.Error.Get("message");

                if (!string.IsNullOrEmpty(message.ToString()))
                {
                    Console.Write(Error(message.ToString()));
                    genericLog = false;
                }
            }
        }
    }
    catch
    {
    }

    if (genericLog)
    {
        Console.WriteLine(Error("Error: \n") + e.Message);
    }
}
finally
{
    AddToLogFile("Program lhqcmd.exe finished.");
}


//sb.AppendLine($"Root: {e.GetType().Name}: {e.Message}");
void FillException(Exception e, StringBuilder sb, int level = 0)
{
    var prefix = level == 0 ? "Root" : $"Inner_{level}";
    sb.AppendLine($"[{prefix}] {e.GetType().Name} -> {e.Message} , StackTrace: {e.StackTrace}");
    if (e.InnerException != null)
    {
        FillException(e.InnerException, sb, level + 1);
    }
}


void WriteHelp()
{
    var lhqcmd = "lhqcmd.exe".Pastel(ConsoleColor.DarkCyan);
    var exam_lhq = White("Strings.lhq");
    var exam_csproj = White("MyProject.csproj");
    var param_lhq = "lhq model file".PastelBg(Color.DimGray).Pastel(ConsoleColor.White);
    var param_csproj = "C# project file".PastelBg(Color.DimGray).Pastel(ConsoleColor.White);

    string usageHelp = $"Usage:\n{lhqcmd} <{param_lhq}> <{param_csproj}> {Grey("[output dir]")} {Grey("[data]")}";

    bool requestedHelp = args is ["--help"];

    if (missingParams && !requestedHelp)
    {
        Console.WriteLine(
            $"""
             {Error("Missing required parameter(s).")}

             {usageHelp}

             Example:
             {lhqcmd} {exam_lhq} {exam_csproj}

             For more information, run {White("lhqcmd.exe --help")}
             """);
        return;
    }

    Console.WriteLine(
        $"""
         {usageHelp}

         Example 1: 
           {lhqcmd} {exam_lhq} {exam_csproj}
           
         Example 2:
           {lhqcmd}{exam_lhq}{exam_csproj} C:\MyCustomOutputDir\ --namespace=customNamespace
           
         Example 3:
           {lhqcmd}{exam_lhq}{exam_csproj} --namespace=customNamespace

         {White("NOTES:")}

         {White("[output dir]")}
           - optional output directory where to generated files from lhq model
           - defaults to directory of {param_lhq} 

         {White("[data]")} 
           - optional data in format: --key1=value1 --key2=value2 , etc...

         Data with key {"namespace".Pastel(ConsoleColor.Yellow)} is required * 

         Value for {White("namespace")} must provided either from:
            1. command line argument: {White("--namespace=customNamespace")}
             OR
            2. application will try to get value {White("namespace")} from {param_csproj} file automatically:
               - xml element {White("//Project/PropertyGroup/RootNamespace")} 
                OR
               - xml element {White($"/Project/ItemGroup/Content[@Include='{param_lhq}']/CustomToolNamespace")}
                    
         """);
}

void AddToLogFile(params string[] lines)
{
    File.AppendAllLines(logFile, lines.Select(x => $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: {x}"));
}

void RunTestData()
{
    string? customNamespace = null;

    var lhqFullPath = "C:\\dev\\github\\psulek\\lhqeditor\\src.cmd\\App.Tests\\TestData\\WinFormsResxCsharp01\\Strings.lhq";
    var csProjName = "WinFormsResxCsharp01.csproj";

// var lhqFullPath = Path.Combine(Path.GetFullPath("..\\..\\..\\App.Tests\\TestData\\NetCoreResxCsharp01\\"), "Strings.lhq");
// var csProjName = "NetCoreResxCsharp01.csproj";

// var lhqFullPath = "c:\\Temp\\neo_online\\Neo.Localization\\Strings.lhq";
// var csProjName = "Neo.Localization.csproj";
// customNamespace = "Neo.Localization";

// var lhqFullPath = Path.Combine(Path.GetFullPath("..\\..\\..\\Test.Localization"), "Strings.lhq");
// var csProjName = "Test.Localization.csproj";

// var lhqFullPath = "c:\\Terminal\\Localization.Common\\StringsCommon.lhq";
// var csProjName = "Localization.Common.csproj";

// var lhqFullPath = "C:\\Users\\peter.sulek\\source\\repos\\ScaleHQ.Windows.WPF1\\ScaleHQ.Windows.WPF1\\Strings.lhq";
// var csProjName = "ScaleHQ.Windows.WPF1.csproj";

// var lhqFullPath = "C:\\dev\\github\\psulek\\lhqeditor\\src.cmd\\App.Tests\\TestData\\WpfResxCsharp01v2\\Strings.lhq";
// var csProjName = "WpfResxCsharp01v2.csproj";

    var outputDir = Path.Combine(Path.GetFullPath("..\\..\\..\\App.Tests"), "GenOutput");
    if (!Directory.Exists(outputDir))
    {
        Directory.CreateDirectory(outputDir);
    }
    else
    {
        Directory.Delete(outputDir, true);
        Directory.CreateDirectory(outputDir);
    }

    var testDataFolder = Path.GetDirectoryName(lhqFullPath)!;

    args =
    [
        lhqFullPath,
        string.IsNullOrEmpty(csProjName) ? csProjName : Path.Combine(testDataFolder, csProjName),
        outputDir
    ];

    if (!string.IsNullOrEmpty(customNamespace))
    {
        args = args.Append("--namespace=" + customNamespace).ToArray();
    }
}
