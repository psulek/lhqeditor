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
    /// <summary>
    /// Represents information above single generated file as a result of <see cref="Generator.Generate"/> method.
    /// </summary>
    public sealed class GeneratedFile
    {
        private readonly LineEndings _lineEndings;
        private readonly string _content;
        private static readonly Regex _regexLF = new Regex(@"\r\n|\r", RegexOptions.Compiled);
        private static readonly Regex _regexCRLF = new Regex(@"(\r(?!\n))|(?<!\r)\n", RegexOptions.Compiled);

        /// <summary>
        /// Initializes new instance of <see cref="GeneratedFile"/>
        /// </summary>
        /// <param name="fileName">File name of generated file, usually relative path to source *.lhq model fie.</param>
        /// <param name="content">Generate content string that should be written to <paramref name="fileName"/>.</param>
        /// <param name="bom">BOM marker that should be used when writing <paramref name="content"/> to <paramref name="fileName"/> file.</param>
        /// <param name="lineEndings">Line endings that should be used when writing <paramref name="content"/> to <paramref name="fileName"/> file.</param>
        public GeneratedFile(string fileName, string content, bool bom, LineEndings lineEndings)
        {
            FileName = fileName;
            _content = content;
            Bom = bom;
            _lineEndings = lineEndings;
        }

        /// <summary>
        /// Gets file name of generated file, usually relative path to source *.lhq model fie.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets flag whenever to write BOM marker when saving content (from <see cref="GetContent"/>) to <see cref="FileName"/> file. 
        /// </summary>
        public bool Bom { get; }

        /// <summary>
        /// Gets generated content string that should be written to <see cref="FileName"/> file.
        /// </summary>
        /// <param name="applyLineEndings">Flag whenver to apply line endings to content or not.</param>
        /// <returns>
        /// Returns generated content string, without any modifications if <paramref name="applyLineEndings"/> is false or
        /// with line endings applied if <paramref name="applyLineEndings"/> is true.
        /// </returns>
        public string GetContent(bool applyLineEndings)
        {
            if (!applyLineEndings)
            {
                return _content;
            }

            if (_content.Length == 0)
            {
                return _content;
            }
            
            return _lineEndings == LineEndings.LF 
                ? _regexLF.Replace(_content, "\n") 
                : _regexCRLF.Replace(_content, "\r\n");
        }
    }
}
