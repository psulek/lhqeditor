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
using System.Text;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.Utils.Extensions
{
    public static class GuidExtensions
    {
        /// <summary>
        ///     Returns null guid <c>00000000-0000-0000-0000-000000000000</c> as string
        /// </summary>
        public static readonly string GuidNullString = Guid.Empty.ToString();

        /// <summary>
        ///     Test if specified guid value is equal to null guid <c>00000000-0000-0000-0000-000000000000</c>.
        /// </summary>
        /// <param name="source"><see cref="Guid" /> value to test for null guid value.</param>
        /// <returns>Returns true if specified guid is equal to null guid, or false if not.</returns>
        public static bool IsEmpty(this Guid source)
        {
            return source.ToString() == GuidNullString;
        }

        /// <summary>
        ///     Compare two guid values if they are equal.
        /// </summary>
        /// <param name="source">First guid value to compare.</param>
        /// <param name="compareTo">Second guid value to compare.</param>
        /// <returns>Returns true if specified guid values are equal, or false if not.</returns>
        public static bool EqualsTo(this Guid source, Guid compareTo)
        {
            return source.ToString() == compareTo.ToString();
        }

        /// <summary>
        ///     Compare two nullable guid values if they are equal.
        /// </summary>
        /// <param name="source">The g1.</param>
        /// <param name="compareTo">The g2.</param>
        /// <returns></returns>
        public static bool EqualsTo(this Guid? source, Guid? compareTo)
        {
            Guid guidFrom = source ?? Guid.Empty;
            Guid guidTo = compareTo ?? Guid.Empty;

            return EqualsTo(guidFrom, guidTo);
        }

        /// <summary>
        ///     Clean up charactors - and { from guid string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string CleanupToString(this Guid source)
        {
            return source.ToString().Replace("-", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty);
        }

        public static string ToHexString(this Guid source, bool shortVersion = false)
        {
            var builder = new StringBuilder(source.ToString());
            builder = builder.Replace("-", "").Replace("{", "").Replace("}", "");
            if (shortVersion)
            {
                int start = builder.Length / 2;
                builder = builder.Remove(start, builder.Length - start);
            }

            return builder.ToString();
        }
    }
}
