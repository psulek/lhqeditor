namespace LHQ.Cmd;

public static class CmdArgs
{
    public static string LhqFile { get; private set; }
    public static string? ProjectFile { get; private set; }
    public static string? OutDir { get; private set; }
    public static Dictionary<string, object> Data { get; private set; }
    
    public static void Parse(string[] args)
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
                Utils.AddToLogFile($"Error extracting data from cmd line. {Utils.GetFullException(e)}");
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