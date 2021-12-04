using System.Diagnostics;

if (args.Length == 0)
{
    Console.WriteLine("Expect parameter with directory path.");
    return;
}

var sourceFolder = args[0];
if (Directory.Exists(sourceFolder))
{
    const string ildasm = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\ildasm.exe";
    var tempPath = Path.Combine(Path.GetTempPath(), $"ildasm-scan-{Guid.NewGuid():N}");
    Directory.CreateDirectory(tempPath);

    var dllFiles = Directory.GetFiles(sourceFolder, "*.dll", SearchOption.AllDirectories);
    Console.WriteLine($"Found {dllFiles.Length} files in folder: {sourceFolder}");

    int idx = 0;
    int failed = 0;
    foreach (var dllFile in dllFiles)
    {
        idx++;
        var fileInfo = new FileInfo(dllFile);
        string progress = $"({idx}/{dllFiles.Length})";
        Console.WriteLine($"Scanning file: {dllFile} {progress} ...");
        string outputFile = Path.Combine(tempPath, Path.GetFileNameWithoutExtension(dllFile) + "-result.lhqres");
        try
        {
            var process = Process.Start(new ProcessStartInfo(ildasm, $"/text \"{dllFile}\" /output:\"{outputFile}\""));
            if (process != null)
            {
                process.WaitForExit(30 * 1000);
                if (process.ExitCode == 0)
                {
                    Console.WriteLine($"\tScan of file: {fileInfo.Name} {progress} completed successfully.");
                }
                else
                {
                    failed++;
                    Console.WriteLine($"\tScan of file: {fileInfo.Name} {progress} failed with exit code: {process.ExitCode}.");
                }
            }
            else
            {
                Console.WriteLine($"Failed to scan file: {fileInfo.Name} {progress}, could not start scan ildasm on file!");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to scan file: {fileInfo.Name} {progress}, error: {e.Message}");
        }
    }

    var filesToRemove = from file in Directory.GetFiles(tempPath, "*.*", SearchOption.TopDirectoryOnly)
        let fileInfo = new FileInfo(file)
        where fileInfo.Extension != ".lhqres"
        select file;

    foreach (var file in filesToRemove)
    {
        File.Delete(file);
    }

    Console.WriteLine($"Scanning ended for {dllFiles.Length} files in folder: {sourceFolder}");
    Console.WriteLine($"\t{failed} files failed on scan");
    Console.WriteLine($"\t{dllFiles.Length-failed} files succeed on scan");
    Console.WriteLine($"Result files can be found at:\n{tempPath}");
}
else
{
    Console.WriteLine($"Directory {sourceFolder} does not exist!");
}