using System.Diagnostics;
using System.Drawing;
using System.Text;
using JavaScriptEngineSwitcher.Core;
using Jint.Runtime;
using LHQ.Cmd;
using Pastel;

string Grey(string msg) => msg.Pastel(ConsoleColor.Gray);
string White(string msg) => msg.Pastel(ConsoleColor.White);
string Error(string msg) => msg.Pastel(ConsoleColor.Red);

//args = ["--help"];
// args = [
//     @"C:\\dev\\github\\psulek\\lhqeditor\\src.cmd\\App.Tests\\TestData\\WinFormsResxCsharp01\\Strings.lhq",
//     "--namespace=ScaleHQ.Windows.WinForms1"
// ];

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine(
    $"{White("LHQ Model Generator")}  {Grey($"Copyright (c) {DateTime.Today.Year} ScaleHQ Solutions\n")}");

Utils.StartLogger();
Utils.AddToLogFile(new string('-', 50));
Utils.AddToLogFile("Program lhqcmd.exe started.");

#if DEBUG
//RunTestData();
#endif

var missingParams = args.Length == 0;

try
{
    if (missingParams)
    {
        WriteHelp();
    }
    else
    {
        var lhqFile = args[0];
        var csProjFile = args.Length > 1 ? args[1] : null;
        if (!string.IsNullOrEmpty(csProjFile) && csProjFile.StartsWith("--"))
        {
            csProjFile = null;
        }

        if (string.IsNullOrEmpty(lhqFile) || !File.Exists(lhqFile))
        {
            throw new Exception($"LhQ model file '{lhqFile}' was not found!");
        }
        
        if (!string.IsNullOrEmpty(csProjFile) && !File.Exists(csProjFile))
        {
            throw new Exception($"C# project file '{csProjFile}' was not found!");
        }

        var outDir = args.Length > 2 ? args[2] : null;
        if (!string.IsNullOrEmpty(outDir) && outDir.StartsWith("--"))
        {
            outDir = null;
        }

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
        if (args.Length > 1)
        {
            try
            {
                for (int i = 1; i < args.Length; i++)
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
            catch (Exception e)
            {
                Utils.AddToLogFile($"Error extracting data from cmd line. {Utils.GetFullException(e)}");
            }
        }

        Console.WriteLine($"""
                           LHQ model file: 
                           {lhqFile.Pastel(ConsoleColor.DarkCyan)}
                           
                           C# project file:
                           {(csProjFile ?? "-").Pastel(ConsoleColor.DarkCyan)}
                           
                           Output directory:
                           {outDir.Pastel(ConsoleColor.DarkCyan)}
                           
                           """);
        
        Console.WriteLine("Generating files from LHQ model started ...\n".Pastel(ConsoleColor.White));
        
        using var generator = new Generator();
        var start = Stopwatch.GetTimestamp();
        var generatedFiles = generator.Generate(lhqFile, csProjFile, hostData);
        var elapsedTime = Stopwatch.GetElapsedTime(start);

        Console.WriteLine($"Generated {generatedFiles.Count} files in {elapsedTime:g}\n");
        Console.WriteLine($"Ouput directory: \n{outDir.Pastel(ConsoleColor.DarkCyan)}\n");

        var processedFiles = new List<string>();
        foreach (var file in generatedFiles)
        {
            var overwritingFile = processedFiles.Contains(file.Key);
            if (!overwritingFile)
            {
                processedFiles.Add(file.Key);
            }

            string fileName = Path.Combine(outDir, file.Key);

            string str = overwritingFile ? "overwritten" : "generated";
            Console.WriteLine($"[{str}] {file.Key.Pastel(ConsoleColor.DarkCyan)}");

            var dir = Path.GetDirectoryName(fileName);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            Utils.WriteAllText(fileName, file.Value);
        }

        Console.WriteLine($"\nSuccessfully saved {generatedFiles.Count} files.");
    }
}
catch (Exception e)
{
    bool genericLog = true;
    Utils.AddToLogFile(Utils.GetFullException(e));

    try
    {
        if (e is JsRuntimeException jsRuntimeException &&
            e.InnerException is JavaScriptException jsException)
        {
            if (jsRuntimeException.Type == "AppError")
            {
                var title = jsException.Error.Get("title");
                var message = jsException.Error.Get("message");
                
                Console.Write($"{Error(title.ToString())}\n{message}");
                genericLog = false;
            }
        }
    }
    catch
    {
    }

    if (genericLog)
    {
        Console.WriteLine(Error("Unexpected error occurred. See log file for details."));
    }
}
finally
{
    Utils.AddToLogFile("Program lhqcmd.exe finished.");
    Console.ReadLine();
}

void WriteHelp()
{
    var lhqcmd = "lhqcmd.exe".Pastel(ConsoleColor.DarkCyan);
    var exam_lhq = White("Strings.lhq");
    var exam_csproj = White("MyProject.csproj");
    var param_lhq = "lhq model file".PastelBg(Color.DimGray).Pastel(ConsoleColor.White);
    var param_csproj = "C# project file".PastelBg(Color.DimGray).Pastel(ConsoleColor.White);

    string usageHelp = $"Usage:\n{lhqcmd} <{param_lhq}> [{param_csproj}] {Grey("[output dir]")} {Grey("[data]")}";

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

         {param_csproj}
           - for most template(s) (C# related this parameter is required
           - for some template(s) (eg: Typescript) this parameter is not required

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


void RunTestData()
{
    string? customNamespace = null;

    var lhqFullPath =
        "C:\\dev\\github\\psulek\\lhqeditor\\src.cmd\\App.Tests\\TestData\\WinFormsResxCsharp01\\Strings.lhq";
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