using System.Text;
using Pastel;

namespace LHQ.Cmd;

public static class Utils
{
    private static string logFile = "";
    
    private static readonly Encoding NoBomEncoding = new UTF8Encoding(false);

    public static string ToLinuxLineEndings(string str)
    {
        return str.Replace("\r\n", "\n").Replace('\r', '\n');
    }

    public static void WriteAllText(string fileName, string content, bool useBOM = false, bool linuxLineEndings = false)
    {
        if (linuxLineEndings)
        {
            content = ToLinuxLineEndings(content);
        }

        File.WriteAllText(fileName, content, useBOM ? Encoding.UTF8 : NoBomEncoding);
    }

    public static void StartLogger()
    {
        var logDir = Path.GetDirectoryName(Environment.ProcessPath);
        logFile = Path.Combine(logDir, "lhqcmd.log");
        Console.WriteLine($"Logging to file: {logFile}\n".Pastel(ConsoleColor.Gray));
        if (File.Exists(logFile))
        {
            try
            {
                // split log file when size is more than 500kb
                var split = new FileInfo(logFile).Length > 1024 * 500;
                if (split)
                {
                    File.Move(logFile, Path.Combine(logDir, $"lhqcmd_{DateTime.Now.Ticks}.log"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Starting logger failed, {e.Message}, {e.StackTrace}");
            }
        }
    }

    public static void AddToLogFile(params string[] lines)
    {
        if (!string.IsNullOrEmpty(logFile))
        {
            File.AppendAllLines(logFile, lines.Select(x => $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: {x}"));
        }
    }
    
    public static string GetFullException(Exception e, int level = 0)
    {
        StringBuilder sb = new();
        FillException(e, sb, level);
        return sb.ToString();
    }

    private static void FillException(Exception e, StringBuilder sb, int level = 0)
    {
        var prefix = level == 0 ? "Root" : $"Inner_{level}";
        sb.AppendLine($"[{prefix}] {e.GetType().Name} -> {e.Message} , StackTrace: {e.StackTrace}");
        if (e.InnerException != null)
        {
            FillException(e.InnerException, sb, level + 1);
        }
    }
}
