using System;
using System.Linq;
using System.Text;

namespace LHQ.Generators;

public static class StringIndentExtensions
{
    public static string ToStringIndent(this StringBuilder sb, int identLevel = 0) => ToStringIndent(sb.ToString(), identLevel);

    public static string ToStringIndent(this string str, int identLevel = 0)
    {
        var lines = str.Split(Environment.NewLine);
        if (lines.Length == 0)
        {
            return str;
        }
        
        var currentIndent = new string('\t', identLevel);
        if (lines.Length == 1)
        {
            return currentIndent + lines[0];
        }
        
        //var res = lines[0] + Environment.NewLine + string.Join(Environment.NewLine, lines.Skip(1).Select(line => currentIndent + line));
        StringBuilder res = new StringBuilder();
        res.AppendLine(lines[0]);
        for (var i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.Length == 0)
            {
                res.AppendLine();
            }
            else
            {
                res.AppendLine(currentIndent + line);
            }
        }

        return res.ToString();
    }
}