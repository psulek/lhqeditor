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

using System.Collections.Generic;
using System.Text;

namespace LHQ.Utils.Utilities
{
    public static class XmlValueEncode
    {
        private static readonly Dictionary<char, string> _xml = new Dictionary<char, string>
        {
            { '>', "&gt;" },
            { '<', "&lt;" },
            { '&', "&amp;" }
        };

        private static readonly Dictionary<char, string> _xmlQuotes = new Dictionary<char, string>
        {
            {'>', "&gt;"},
            {'<', "&lt;"},
            {'&', "&amp;"},
            {'"', "&quot;"},
            {'\'', "&apos;"}
        };

        public static string EncodeText(string input, bool quotes)
        {
            var map = quotes ? _xmlQuotes : _xml;

            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (map.TryGetValue(c, out var val))
                {
                    sb.Append(val);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
