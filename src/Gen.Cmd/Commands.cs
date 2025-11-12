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

using System.Drawing;
using System.Runtime.InteropServices;
using ConsoleAppFramework;
using LHQ.Gen.Lib;
using Pastel;

namespace LHQ.Gen.Cmd;

public class Commands
{
    internal static class GeneratorInstance
    {
        internal static Generator Instance;
    }
    
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

        var colorPath = ConsoleColor.White;
        
        var logger = DefaultLogger.Instance.Logger;
        var generator = GeneratorInstance.Instance;
        if (generator == null)
        {
            throw new Exception("Generator instance is not initialized.");
        }
        
        generator.UpdateLogger(logger);
        
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

        string? lhqFilePath = Path.GetDirectoryName(lhqFile);
        
        if (string.IsNullOrEmpty(outDir))
        {
            outDir = !string.IsNullOrEmpty(csProjFile)
                ? Path.GetDirectoryName(csProjFile)!
                : lhqFilePath!;
        }
        else if (outDir == ".")
        {
            outDir = Environment.CurrentDirectory;
        }
        else
        {
            TryGetLinuxHomeDir(ref outDir);
            var currentDir = lhqFilePath ?? Environment.CurrentDirectory;

            outDir = Path.IsPathRooted(outDir) 
                ? outDir 
                : Path.Combine(currentDir, outDir);
        }

        if (string.IsNullOrEmpty(outDir))
        {
            throw new Exception("Unable to determine output directory!");
        }

        var hasProjectFile = !string.IsNullOrEmpty(csProjFile);
        Console.WriteLine($"""
                           LHQ model file: 
                           {lhqFile.Pastel(colorPath)}
                           C# project file:
                           {(hasProjectFile ? csProjFile : "-").Pastel(colorPath)}

                           """);

        // Console.WriteLine("Generating files from LHQ model started ...\n".Pastel(ConsoleColor.White));

        try
        {
            var generateResult = generator.Generate(lhqFile, hostData);
            var generatedFiles = generateResult.GeneratedFiles;
        
            Console.WriteLine($"\nSaving files to output directory: \n{outDir.Pastel(colorPath)}\n");

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
                var shortName = Path.GetRelativePath(outDir, fileName);

                string str = overwritingFile 
                    ? "[overwritten]".Pastel(Color.DarkOrange)
                    : "[generated]".Pastel(ConsoleColor.Gray);
                Console.WriteLine($"{str} {shortName.Pastel(colorPath)}"); 

                var dir = Path.GetDirectoryName(fileName);
                if (dir != null && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                await file.WriteToDiskAsync(outDir);
            }
            
            Console.WriteLine($"\nSuccessfully saved {generatedFiles.Count} files.");
        }
        catch (GeneratorException e)
        {
            var reason = "";
            //var error = $"{e.Title ?? ""} \n {e.Message}";
            if (e.Kind == GeneratorErrorKind.InvalidModelSchema)
            {
                reason = "Invalid model schema.";
            }
            else if (e.Kind == GeneratorErrorKind.TemplateValidationError)
            {
                reason = $"Invalid template settings.";
            }

            if (e.Code == GeneratorErrorCodes.CsharpNamespaceMissing)
            {
                reason = "C# namespace is missing.\nProvide valid C# project file (*.csproj) using --project parameter or set 'namespace' value in template settings.";
            }

            var message = $"Error running code template for file '{lhqFile}'";
            logger.Error(e, message + $"\n{reason}");
            Console.WriteLine($"{message.Pastel(ConsoleColor.Red)}\n\nReason: {reason.Pastel(ConsoleColor.White)}\n");
        }
        catch (Exception e)
        {
            var message = $"Error running code template for file '{lhqFile}'";
            logger.Error(e, message);
            Console.WriteLine(message.Pastel(ConsoleColor.Red));
        }
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
