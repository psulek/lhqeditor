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

using System;
using System.IO;
using System.Text.RegularExpressions;
using LHQ.Utils.Extensions;

// inspired by - http://www.codeproject.com/Articles/37503/Auto-Ellipsis
// updated by psulek to remove usage of 'TextRenderer'

namespace LHQ.Utils.Utilities
{
    public class Ellipsis
    {
        /// <summary>
        ///     String used as a place holder for trimmed text.
        /// </summary>
        public static readonly string EllipsisChars = "...";

        private static readonly Regex _prevWord = new Regex(@"\W*\w*$", RegexOptions.Compiled);
        private static readonly Regex _nextWord = new Regex(@"\w*\W*", RegexOptions.Compiled);

        private static string GetPathRootEx(string path)
        {
            if (path.IsNullOrEmpty())
            {
                return path;
            }

            string[] strings = path.Split(new[] { Path.DirectorySeparatorChar.ToString() },
                StringSplitOptions.RemoveEmptyEntries);

            if (strings.Length > 0)
            {
                return strings[0] + Path.DirectorySeparatorChar;
            }

            return path;
        }

        /// <summary>
        ///     Truncates a text string to fit within a given control width by replacing trimmed text with ellipses.
        /// </summary>
        /// <param name="text">String to be trimmed.</param>
        /// <param name="maxLength">max length</param>
        /// <param name="options">Format and alignment of ellipsis.</param>
        /// <returns>This function returns text trimmed to the specified witdh.</returns>
        public static string Compact(string text, int maxLength, EllipsisFormat options)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            // no aligment information
            if ((EllipsisFormat.Middle & options) == 0)
            {
                return text;
            }

            int textLength = text.Length;

            /*Size s = TextRenderer.MeasureText(dc, text, ctrl.Font);

                // control is large enough to display the whole text
                if (s.Width <= ctrl.Width)
                    return text;*/

            if (textLength <= maxLength)
            {
                return text;
            }

            var pre = "";
            string mid = text;
            var post = "";

            bool isPath = (EllipsisFormat.Path & options) != 0;

            // split path string into <drive><directory><filename>
            if (isPath)
            {
                //pre = Path.GetPathRoot(text);
                pre = GetPathRootEx(text);
                mid = Path.GetDirectoryName(text).Substring(pre.Length);
                post = Path.GetFileName(text);
            }

            var len = 0;
            int seg = mid.Length;
            var fit = "";

            // find the longest string that fits into 
            // the control boundaries using bisection method
            while (seg > 1)
            {
                seg -= seg / 2;

                int left = len + seg;
                int right = mid.Length;

                if (left > right)
                {
                    continue;
                }

                if ((EllipsisFormat.Middle & options) == EllipsisFormat.Middle)
                {
                    right -= left / 2;
                    left -= left / 2;
                }
                else if ((EllipsisFormat.Start & options) != 0)
                {
                    right -= left;
                    left = 0;
                }

                // trim at a word boundary using regular expressions
                if ((EllipsisFormat.Word & options) != 0)
                {
                    if ((EllipsisFormat.End & options) != 0)
                    {
                        left -= _prevWord.Match(mid, 0, left).Length;
                    }
                    if ((EllipsisFormat.Start & options) != 0)
                    {
                        right += _nextWord.Match(mid, right).Length;
                    }
                }

                // build and measure a candidate string with ellipsis
                string tst = mid.Substring(0, left) + EllipsisChars + mid.Substring(right);

                // restore path with <drive> and <filename>
                if (isPath)
                {
                    tst = Path.Combine(Path.Combine(pre, tst), post);
                }

                //s = TextRenderer.MeasureText(dc, tst, ctrl.Font);
                textLength = tst.Length;

                // candidate string fits into control boundaries, try a longer string
                // stop when seg <= 1
                if (textLength <= maxLength)
                {
                    len += seg;
                    fit = tst;
                }
            }

            if (len == 0) // string can't fit into control
            {
                // "path" mode is off, just return ellipsis characters
                if (!isPath)
                {
                    return EllipsisChars;
                }

                // <drive> and <directory> are empty, return <filename>
                if (pre.Length == 0 && mid.Length == 0)
                {
                    return post;
                }

                // measure "C:\...\filename.ext"
                fit = Path.Combine(Path.Combine(pre, EllipsisChars), post);

                //s = TextRenderer.MeasureText(dc, fit, ctrl.Font);
                textLength = fit.Length;

                // if still not fit then return "...\filename.ext"
                //if (s.Width > ctrl.Width)
                if (textLength > maxLength)
                {
                    fit = Path.Combine(EllipsisChars, post);
                }
            }

            return fit;
        }
    }
}
