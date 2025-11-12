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

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using ConsoleAppFramework;
using JavaScriptEngineSwitcher.Core;
using LHQ.Gen.Cmd;
using LHQ.Gen.Lib;
using LHQ.Utils.Extensions;
using Pastel;
using ConsoleColor = System.ConsoleColor;

string ColorGrey(string msg) => msg.Pastel(ConsoleColor.Gray);
string ColorWhite(string msg) => msg.Pastel(ConsoleColor.White);
string ColorError(string msg) => msg.Pastel(ConsoleColor.Red);
string ColorPath(string msg) => msg.Pastel(ConsoleColor.Yellow);

var logger = DefaultLogger.Instance.Logger;

bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

var mainModule = Process.GetCurrentProcess().MainModule;
string exeName = isWindows ? "lhqcmd.exe" : "lhqcmd";
string exePath = mainModule?.FileName ?? string.Empty;
if (!exePath.IsNullOrEmpty())
{
    exeName = Path.GetFileName(exePath);
}

using var generator = new Generator(logger);
Commands.GeneratorInstance.Instance = generator;
string libraryVersion = generator.GetLibraryVersion();
(string model, string codeGenerator) = generator.GetModelVersions();

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine($"{ColorWhite("LHQ Code Template Generator")}  {ColorGrey($"Copyright (c) {DateTime.Today.Year} ScaleHQ Solutions")}");
Console.WriteLine(ColorGrey($"  engine library version: {libraryVersion}"));
Console.WriteLine(ColorGrey($"  supported model version: <= {model}, code generator version: <= {codeGenerator}\n"));

#if DEBUG
var testDataMode = false;
//RunTestData();
#endif

var missingParams = args.Length == 0;
bool requestedHelp = args is ["--help"] or ["-h"] or ["/?"];

try
{
    if (missingParams || requestedHelp)
    {
        WriteHelp();
    }
    else
    {
        logger.Debug(new string('-', 50));
        logger.Debug($"Program {exeName} started.");

        var app = ConsoleApp.Create();
        app.Add<Commands>();
        await app.RunAsync(args);
    }
}
catch (Exception e)
{
    bool genericLog = true;

    try
    {
        bool handled = false;

        if (e is JsRuntimeException jsRuntimeException)
        {
            if (jsRuntimeException.Type == "AppError")
            {
                var callStack = jsRuntimeException.CallStack;
                var description = jsRuntimeException.Description;
                var documentName = jsRuntimeException.DocumentName;

                var items = description.Split("\n");
                var title = items.Length == 0 ? description : items[0];
                var message = items.Length > 1 ? string.Join("\n", items.Skip(1)) : string.Empty;

                Console.Write($"{ColorError(title)}\n{message}");

                logger.Info($"{title}\n{message}\nFILE: {documentName} >> {callStack}");

                genericLog = false;
                handled = true;
            }
        }

        if (!handled)
        {
            logger.Info(e);
        }
    }
    catch
    { }

    if (genericLog)
    {
        Console.WriteLine(ColorError("Unexpected error occurred. See log file for details."));
    }
}
finally
{
    logger.Debug($"Program {exeName} finished.");
#if DEBUG
    if (testDataMode)
    {
        Console.ReadLine();
    }
#endif
}

void WriteHelp()
{
    var lhqcmd = exeName.Pastel(ConsoleColor.Cyan);

    var folderExam = isWindows ? "c:\\MyOutputFolder" : "~/MyOutputFolder";
    var param_lhq = ColorPath("lhq model file");
    var param_csproj = "--project MyProject.csproj";
    var exam_lhq = ColorPath("Strings.lhq");

    string usageHelp = $"Usage:\n{lhqcmd} <{param_lhq}> [{param_csproj}] [--out DIR] [--data data]";

    if (missingParams && !requestedHelp)
    {
        Console.WriteLine(
            $"""
             {ColorError("Missing required parameter(s).")}

             {usageHelp}

             Example:
             {lhqcmd} {exam_lhq} {param_csproj} --out {folderExam}

             For more information, run {ColorWhite(exeName + " --help")}
             """);
        return;
    }

    string param_namespace = "namespace".Pastel(ConsoleColor.White);
    
    Console.WriteLine(
        $"""
         {usageHelp}

         Example 1: 
           {lhqcmd} {exam_lhq} {param_csproj}
           
         Example 2:
           {lhqcmd} {exam_lhq} {param_csproj} --out {folderExam} --data namespace=customNamespace key1=value1
           
         {ColorWhite("NOTES:")}

         {ColorWhite("--project <path to csproj file>")}
           - only these templates require C# project file to be specified: 
             {ColorWhite("NetCoreResxCsharp01")}, {ColorWhite("NetFwResxCsharp01")}, {ColorWhite("WinFormsResxCsharp01")}, {ColorWhite("WpfResxCsharp01")}
           - and only if {param_lhq} does not have set {param_namespace} value in template settings
           - if not provided but required by template, application will try to locate 
             appropriate C# project file (*.csproj) in the same folder as {param_lhq} 
             where *.csproj file is linked to the {param_lhq} file (*)

         {ColorWhite("--out DIR")}
           - optional output directory where to save generated files from lhq model
           - defaults to directory of {param_lhq} 

         {ColorWhite("--data [data]")} 
           - optional data in format: --data key1=value1 key2=value2 , etc...

         Data with key {param_namespace} is required * 

         * Value for {ColorWhite("namespace")} must provided either from:
            1. command line argument: {ColorWhite("--data namespace=customNamespace")}
             OR
            2. application will try to get value {ColorWhite("namespace")} from {param_csproj} file automatically:
               - xml element {ColorWhite("//Project/PropertyGroup/RootNamespace")} 
                OR
               - xml element {ColorWhite($"/Project/ItemGroup/Content[@Include='{param_lhq}']/CustomToolNamespace")}

         """);
}

#if DEBUG
void RunTestData()
{
    string? customNamespace = null;
    var csProjName = "";
    testDataMode = true;

    // var lhqFullPath = @"c:\dev\github\psulek\lhqeditor\src.cmd\App.Tests\TestData\WpfResxCsharp01\Strings.lhq";
    // var lhqFullPath = @"c:\dev\github\psulek\lhqeditor\src.cmd\App.Tests\TestData\WinFormsResxCsharp01\Strings.lhq";
    // var lhqFullPath = "C:\\dev\\github\\psulek\\lhqeditor\\src\\Gen.Lib.Tests\\TestData\\NetCoreResxCsharp01\\Strings.lhq";
    // var lhqFullPath = "C:\\dev\\github\\psulek\\lhqeditor\\src\\Gen.Lib.Tests\\TestData\\WpfResxCsharp01v4\\Strings.lhq";
    // var lhqFullPath = "c:\\Terminal\\Base\\Localization\\Strings.lhq";
    // var lhqFullPath = "c:\\Temp\\3\\2\\Strings.lhq";

    var lhqFullPath = "C:\\dev\\github\\psulek\\lhqeditor\\src\\Gen.Lib.Tests\\TestData\\TypescriptJson01\\Strings.lhq";

    var outputDir = Path.Combine(Path.GetFullPath("..\\..\\..\\App.Tests"), "GenOutput");
    //var outputDir = Path.GetDirectoryName(lhqFullPath);
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
        "--project " + (string.IsNullOrEmpty(csProjName) ? csProjName : Path.Combine(testDataFolder, csProjName)),
        "--out " + outputDir
    ];

    if (!string.IsNullOrEmpty(customNamespace))
    {
        args = Enumerable.Append(args, "--data namespace=" + customNamespace).ToArray();
    }
}
#endif