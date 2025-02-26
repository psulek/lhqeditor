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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using ConsoleAppFramework;
using JavaScriptEngineSwitcher.Core;
using LHQ.Gen.Cmd;
using LHQ.Gen.Lib;
using NLog;
using Pastel;
using ConsoleColor = System.ConsoleColor;

string Grey(string msg) => msg.Pastel(ConsoleColor.Gray);
string White(string msg) => msg.Pastel(ConsoleColor.White);
string Error(string msg) => msg.Pastel(ConsoleColor.Red);

var logger = DefaultLogger.Instance.Logger;

bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

string exePath = Process.GetCurrentProcess().MainModule.FileName;
string exeName = Path.GetFileName(exePath);

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine(
    $"{White("LHQ Model Generator")}  {Grey($"Copyright (c) {DateTime.Today.Year} ScaleHQ Solutions\n")}");

#if DEBUG
var testDataMode = false;
//RunTestData();
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
        logger.Debug(new string('-', 50));
        logger.Info($"Program {exeName} started.");

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

                Console.Write($"{Error(title)}\n{message}");

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
        Console.WriteLine(Error("Unexpected error occurred. See log file for details."));
    }
}
finally
{
    logger.Info($"Program {exeName} finished.");
#if DEBUG
    if (testDataMode)
    {
        Console.ReadLine();
    }
#endif
}

void WriteHelp()
{
    var lhqcmd = exeName.Pastel(ConsoleColor.DarkCyan);
    var exam_lhq = White("Strings.lhq");
    var exam_csproj = "--project MyProject.csproj".Pastel(ConsoleColor.DarkYellow);

    var folderExam = isWindows ? "c:\\MyOutputFolder" : "~/MyOutputFolder";
    var exam_outDir = White($"--out {folderExam}");
    var param_lhq = "lhq model file".PastelBg(Color.DimGray).Pastel(ConsoleColor.White);
    var param_csproj = "--project MyProject.csproj".Pastel(ConsoleColor.DarkYellow);

    string usageHelp = $"Usage:\n{lhqcmd} <{param_lhq}> [{param_csproj}] [{Grey("--out DIR")}] {Grey("--data [data]")}";

    if (missingParams && !requestedHelp)
    {
        Console.WriteLine(
            $"""
             {Error("Missing required parameter(s).")}

             {usageHelp}

             Example:
             {lhqcmd} {exam_lhq} {exam_csproj} {exam_outDir}

             For more information, run {White(exeName + " --help")}
             """);
        return;
    }

    Console.WriteLine(
        $"""
         {usageHelp}

         Example 1: 
           {lhqcmd} {exam_lhq} {exam_csproj}
           
         Example 2:
           {lhqcmd} {exam_lhq} {exam_csproj} --out {folderExam} --data namespace=customNamespace key1=value1
           
         {White("NOTES:")}

         {White("--project <path to csproj file>")}
           - for C# related templates this parameter should be explicitly provided
           - if not specified app will search for C# project within same folder as lhq model 

         {White("--out DIR")}
           - optional output directory where to save generated files from lhq model
           - defaults to directory of {param_lhq} 

         {White("--data [data]")} 
           - optional data in format: --data key1=value1 key2=value2 , etc...

         Data with key {"namespace".Pastel(ConsoleColor.Yellow)} is required * 

         Value for {White("namespace")} must provided either from:
            1. command line argument: {White("--data namespace=customNamespace")}
             OR
            2. application will try to get value {White("namespace")} from {param_csproj} file automatically:
               - xml element {White("//Project/PropertyGroup/RootNamespace")} 
                OR
               - xml element {White($"/Project/ItemGroup/Content[@Include='{param_lhq}']/CustomToolNamespace")}
               
         * some templates does not require {"namespace".Pastel(ConsoleColor.Yellow)} value (eg: typescript one)
                    
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