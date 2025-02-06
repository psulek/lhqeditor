using System.Text;

namespace LHQ.Cmd;

public static class FileHelper
{
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
}
