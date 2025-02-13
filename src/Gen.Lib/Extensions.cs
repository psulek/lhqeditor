using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using Pastel;

namespace LHQ.Gen.Lib
{
    public static class Extensions
    {
        private static readonly IReadOnlyDictionary<ConsoleColor, Color> _consoleColorMapper = new ReadOnlyDictionary<ConsoleColor, Color>(new Dictionary<ConsoleColor, Color>
        {
            [ConsoleColor.Black] = Color.FromArgb(0),
            [ConsoleColor.DarkBlue] = Color.FromArgb(139),
            [ConsoleColor.DarkGreen] = Color.FromArgb(25600),
            [ConsoleColor.DarkCyan] = Color.FromArgb(35723),
            [ConsoleColor.DarkRed] = Color.FromArgb(9109504),
            [ConsoleColor.DarkMagenta] = Color.FromArgb(9109643),
            [ConsoleColor.DarkYellow] = Color.FromArgb(8421376),
            [ConsoleColor.Gray] = Color.FromArgb(8421504),
            [ConsoleColor.DarkGray] = Color.FromArgb(11119017),
            [ConsoleColor.Blue] = Color.FromArgb((int) byte.MaxValue),
            [ConsoleColor.Green] = Color.FromArgb(32768),
            [ConsoleColor.Cyan] = Color.FromArgb((int) ushort.MaxValue),
            [ConsoleColor.Red] = Color.FromArgb(16711680),
            [ConsoleColor.Magenta] = Color.FromArgb(16711935),
            [ConsoleColor.Yellow] = Color.FromArgb(16776960),
            [ConsoleColor.White] = Color.FromArgb(16777215)
        });

        public static string Pastel(this string input, ConsoleColor color)
        {
            return input.Pastel(_consoleColorMapper[color]);
        }   
    }
}