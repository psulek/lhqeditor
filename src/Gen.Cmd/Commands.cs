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
using ConsoleAppFramework;
using LHQ.Gen.Lib;
using Pastel;

namespace LHQ.Gen.Cmd;

public class Commands
{
    /// <summary>
    /// LHQ Model Generator
    /// </summary>
    /// <param name="modelFile">LHQ model file (*.lhq)</param>
    /// <param name="project">-p, File path to C# project file (*.csproj)</param>
    /// <param name="out">-o, Output directory where to save generated files from lhq model</param>
    /// <param name="data">-d, Extra data values, in format key1=value2</param>
    /// <returns></returns>
    [Command("")]
    public async Task GenerateCode([Argument] string modelFile, string project = "", string @out = "", params string[] data)
    {
        // Console.ReadLine();
        // Debugger.Break();
        
        var logger = DefaultLogger.Instance.Logger;
        
        var lhqFile = modelFile;
        var csProjFile = project;
        var outDir = @out;
        var hostData = new Dictionary<string, object>();

        if (data.Length > 0)
        {
            try
            {
                foreach (string item in data)
                {
                    string[] keyValue = item.Split("=", StringSplitOptions.RemoveEmptyEntries);
                    if (keyValue.Length > 0)
                    {
                        string key = keyValue[0];
                        string value = keyValue.Length > 1 ? keyValue[1] : string.Empty;

                        if (!string.IsNullOrEmpty(key))
                        {
                            hostData[key] = value;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error extracting data from cmd line.");
                throw;
            }
        }

        if (string.IsNullOrEmpty(lhqFile) || !File.Exists(lhqFile))
        {
            throw new Exception($"LhQ model file '{lhqFile}' was not found!");
        }

        if (!string.IsNullOrEmpty(csProjFile) && !File.Exists(csProjFile))
        {
            throw new Exception($"C# project file '{csProjFile}' was not found!");
        }

        if (string.IsNullOrEmpty(outDir))
        {
            outDir = !string.IsNullOrEmpty(csProjFile)
                ? Path.GetDirectoryName(csProjFile)!
                : Path.GetDirectoryName(lhqFile)!;
        }
        else if (outDir == ".")
        {
            outDir = Environment.CurrentDirectory;
        }
        else
        {
            TryGetLinuxHomeDir(ref outDir);
            outDir = Path.IsPathRooted(outDir) ? outDir : Path.Combine(Environment.CurrentDirectory, outDir);
        }

        if (string.IsNullOrEmpty(outDir))
        {
            throw new Exception("Unable to determine output directory!");
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

        using var generator = new Generator(logger);
        var start = Stopwatch.GetTimestamp();
        var generateResult = generator.Generate(lhqFile, csProjFile, outDir, hostData);
        var generatedFiles = generateResult.GeneratedFiles;
        //var modelGroupSettings = generateResult.ModelGroupSettings;
        var elapsedTime = Stopwatch.GetElapsedTime(start);
        
        var genMsg = $"Generated {generatedFiles.Count} files in {elapsedTime:g}\n";
        Console.WriteLine(genMsg);
        logger.Info(genMsg);
        Console.WriteLine($"Ouput directory: \n{outDir.Pastel(ConsoleColor.DarkCyan)}\n");

        // if (modelGroupSettings.Count > 0)
        // {
        //     Console.WriteLine("Settings:");
        //     foreach (var groupSetting in modelGroupSettings)
        //     {
        //         if (groupSetting.Settings != null)
        //         {
        //             Console.WriteLine($"  [{groupSetting.Group.Pastel(ConsoleColor.DarkYellow)}]");
        //             foreach (var settings in groupSetting.Settings)
        //             {
        //                 Console.WriteLine($"\t{settings.Key}: {settings.Value.ToString().Pastel(ConsoleColor.White)}");
        //             }
        //         }
        //     }
        //
        //     Console.WriteLine();
        // }

        var processedFiles = new List<string>();
        foreach (var file in generatedFiles)
        {
            var fileName = file.FileName;
            var overwritingFile = processedFiles.Contains(fileName);
            if (!overwritingFile)
            {
                processedFiles.Add(fileName);
            }

            fileName = Path.Combine(outDir, fileName);

            string str = overwritingFile ? "overwritten" : "generated";
            //string strBom = (file.Bom ? "with" : "without") + " BOM";
            Console.WriteLine($"[{str}] {fileName.Pastel(ConsoleColor.DarkCyan)}"); // ({file.LineEndings}) {strBom}");

            var dir = Path.GetDirectoryName(fileName);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            await file.WriteToDiskAsync(outDir);
        }

        Console.WriteLine($"\nSuccessfully saved {generatedFiles.Count} files.");
    }

    private static void TryGetLinuxHomeDir(ref string dir)
    {
        bool isLinuxLike = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        
        if (isLinuxLike && dir.StartsWith("~/"))
        {
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            dir = Path.Combine(homeDirectory, dir.Substring(2));
        }
    }
}
