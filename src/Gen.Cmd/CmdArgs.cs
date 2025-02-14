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

using NLog;

namespace LHQ.Gen.Cmd;

public static class CmdArgs
{
    public static string LhqFile { get; private set; }
    public static string? ProjectFile { get; private set; }
    public static string? OutDir { get; private set; }
    public static Dictionary<string, object> Data { get; private set; }
    
    public static void Parse(ILogger logger, string[] args)
    {
        LhqFile = args[0];

        Data = new();
        if (args.Length > 1)
        {
            try
            {
                for (int i = 1; i < args.Length; i++)
                {
                    var item = args[i];
                    if (item.StartsWith("-"))
                    {
                        var values = item.Split("=", StringSplitOptions.RemoveEmptyEntries);
                        if (values.Length != 2)
                        {
                            continue;
                        }
    
                        //var key = values[0].Length > 2 ? values[0].Substring(2) : string.Empty;
                        var key = values[0];
                        var isData = key.Length > 2 && key[0] == '-' && key[1] == '-';
                        key = isData ? key.Substring(2) : key.Substring(1);
                        var value = values[1];
    
                        if (!string.IsNullOrEmpty(key))
                        {
                            if (isData)
                            {
                                Data[key] = value;
                            }
                            else
                            {
                                key = key.ToLowerInvariant();
                                if (key == "project")
                                {
                                    ProjectFile = value;
                                }
                                else if (key == "out")
                                {
                                    OutDir = value;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error extracting data from cmd line.");
            }
        }
        
        if (string.IsNullOrEmpty(LhqFile) || !File.Exists(LhqFile))
        {
            throw new Exception($"LhQ model file '{LhqFile}' was not found!");
        }

        if (!string.IsNullOrEmpty(ProjectFile) && !File.Exists(ProjectFile))
        {
            throw new Exception($"C# project file '{ProjectFile}' was not found!");
        }
        
        // if (!string.IsNullOrEmpty(OutDir) && !Directory.Exists(OutDir))
        // {
        //     throw new DirectoryNotFoundException($"Output directory '{OutDir}' was not found!");
        // }

        if (string.IsNullOrEmpty(OutDir))
        {
            OutDir = !string.IsNullOrEmpty(ProjectFile)
                ? Path.GetDirectoryName(ProjectFile)!
                : Path.GetDirectoryName(LhqFile)!;
        }

        if (string.IsNullOrEmpty(OutDir))
        {
            throw new Exception("Unable to determine output directory!");
        }
    }
}