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

using System.Text.RegularExpressions;

namespace LHQ.Gen.Lib
{
    public sealed class GeneratedFile
    {
        private readonly string _content;
        private static readonly Regex _regexLF = new Regex(@"\r\n|\r", RegexOptions.Compiled);
        private static readonly Regex _regexCRLF = new Regex(@"(\r(?!\n))|(?<!\r)\n", RegexOptions.Compiled);
        
        public GeneratedFile(string fileName, string content, bool bom, LineEndings lineEndings)
        {
            FileName = fileName;
            _content = content;
            Bom = bom;
            LineEndings = lineEndings;
        }
        
        public string FileName { get; }

        public bool Bom { get; }

        public LineEndings LineEndings { get; }

        public string GetContent(bool applyLineEndings = true)
        {
            if (!applyLineEndings)
            {
                return _content;
            }

            if (_content.Length == 0)
            {
                return _content;
            }
            
            return LineEndings == LineEndings.LF 
                ? _regexLF.Replace(_content, "\n") 
                : _regexCRLF.Replace(_content, "\r\n");
        }
    }
}
