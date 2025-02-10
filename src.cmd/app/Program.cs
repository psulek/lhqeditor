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
//     "c:\\Temp\\3\\Strings.lhq",
//     "-out=C:\\Temp\\3\\1\\",
//     "--namespace=abc"
// ];

// args = [
//     @"C:\dev\github\psulek\lhqeditor\src.cmd\App.Tests\TestData\WpfResxCsharp01v4\Strings.lhq"
// ];

//var csProjectFile = Generator.FindOwnerCsProjectFile("c:\\Temp\\3\\Strings.lhq");

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine(
    $"{White("LHQ Model Generator")}  {Grey($"Copyright (c) {DateTime.Today.Year} ScaleHQ Solutions\n")}");

// Utils.StartLogger();
// Utils.AddToLogFile(new string('-', 50));
// Utils.AddToLogFile("Program lhqcmd.exe started.");

#if DEBUG
RunTestData();
#endif

var missingParams = args.Length == 0;
bool requestedHelp = args is ["--help"];

try
{
    if (missingParams || requestedHelp)
    {
        WriteHelp();
    }
    else
    {
        Utils.StartLogger();
        Utils.AddToLogFile(new string('-', 50));
        Utils.AddToLogFile("Program lhqcmd.exe started.");

        CmdArgs.Parse(args);
        var lhqFile = CmdArgs.LhqFile;
        var csProjFile = CmdArgs.ProjectFile;
        var outDir = CmdArgs.OutDir!;
        var hostData = CmdArgs.Data;
        
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
        var generatedFiles = generator.Generate(lhqFile, csProjFile, outDir, hostData);
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
    //Console.ReadLine();
}

void WriteHelp()
{
    var lhqcmd = "lhqcmd.exe".Pastel(ConsoleColor.DarkCyan);
    var exam_lhq = White("Strings.lhq");
    var exam_csproj = "-project=MyProject.csproj".Pastel(ConsoleColor.DarkYellow);
    var exam_outDir = White("-out=c:\\MyOutputFolder");
    var param_lhq = "lhq model file".PastelBg(Color.DimGray).Pastel(ConsoleColor.White);
    //var param_csproj = "C# project file".PastelBg(Color.DimGray).Pastel(ConsoleColor.White);
    var param_csproj = "-project=MyProject.csproj".Pastel(ConsoleColor.DarkYellow);

    string usageHelp = $"Usage:\n{lhqcmd} <{param_lhq}> [{param_csproj}] [{Grey("-out=DIR")}] {Grey("[data]")}";

    if (missingParams && !requestedHelp)
    {
        Console.WriteLine(
            $"""
             {Error("Missing required parameter(s).")}

             {usageHelp}

             Example:
             {lhqcmd} {exam_lhq} {exam_csproj} {exam_outDir}

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
           {lhqcmd} {exam_lhq} {exam_csproj} -out=C:\MyCustomOutputDir\ --namespace=customNamespace
           
         {White("NOTES:")}

         {White("-project=<path to csproj file>")}
           - for C# related templates this parameter should be explicitly provided
           - if not specified app will search for C# project within same folder as lhq model 

         {White("-out=DIR")}
           - optional output directory where to save generated files from lhq model
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
               
         * some templates does not require {"namespace".Pastel(ConsoleColor.Yellow)} value (eg: typescript one)
                    
         """);
}


void RunTestData()
{
    string? customNamespace = null;
    var csProjName = "";

    // var lhqFullPath = @"c:\dev\github\psulek\lhqeditor\src.cmd\App.Tests\TestData\WpfResxCsharp01\Strings.lhq";
    // var lhqFullPath = @"c:\dev\github\psulek\lhqeditor\src.cmd\App.Tests\TestData\WinFormsResxCsharp01\Strings.lhq";
    // var lhqFullPath = "c:\\dev\\github\\psulek\\lhqeditor\\src.cmd\\App.Tests\\TestData\\NetCoreResxCsharp01\\Strings.lhq";
    // var lhqFullPath = "C:\\dev\\github\\psulek\\lhqeditor\\src.cmd\\App.Tests\\TestData\\WpfResxCsharp01v4\\Strings.lhq";

    var lhqFullPath = "c:\\dev\\github\\psulek\\lhqeditor\\src.cmd\\App.Tests\\TestData\\TypescriptJson01\\Strings.lhq";
    
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
        "-project=" + (string.IsNullOrEmpty(csProjName) ? csProjName : Path.Combine(testDataFolder, csProjName)),
        "-out=" + outputDir
    ];

    if (!string.IsNullOrEmpty(customNamespace))
    {
        args = args.Append("--namespace=" + customNamespace).ToArray();
    }
}