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
using System.Diagnostics;
using System.Text;

namespace LHQ.Utils.Extensions
{
    public static class ExceptionExtensions
    {
        private const string DetailInfo =
            @"
{0} {1}
{0} type: {2}
{0} message: {3}
{0} source: {4}
{0} stack trace: {5}
{0} has inner exception: {6}";

        /// <summary>
        ///     Returns exception string with stacktrace.
        /// </summary>
        /// <param name="e">Exception.</param>
        /// <param name="loopInnerExceptions">if set to <c>true</c> [loop inner exceptions].</param>
        /// <returns>Exception string.</returns>
        public static string GetFullExceptionDetails(this Exception e, bool loopInnerExceptions = true)
        {
            try
            {
                var sb = new StringBuilder();
                var deep = 1;
                Exception innerEx = AppendException(sb, "Exception was handled", deep, e);

                if (loopInnerExceptions)
                {
                    while (innerEx != null)
                    {
                        deep++;
                        innerEx = AppendException(sb, string.Format("Inner exception(deep: {0})", deep - 1), deep, innerEx);
                    }
                }

                return sb.ToString();
            }
            catch (Exception e1)
            {
                Debug.WriteLine("Exception in GetExceptionStr, {0}", e1.Message);
                return string.Empty;
            }
        }

        private static Exception AppendException(StringBuilder sb, string firstLine, int deep, Exception e)
        {
            sb.AppendFormat(DetailInfo,
                new string('-', deep),
                firstLine,
                e.GetType().FullName,
                e.Message,
                e.Source,
                e.StackTrace,
                e.InnerException != null ? "yes" : "no");

            return e.InnerException;
        }
    }
}
