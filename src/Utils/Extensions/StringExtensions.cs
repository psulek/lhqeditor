#region License
// Copyright (c) 2021 Peter Šulek / ScaleHQ Solutions s.r.o.
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
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace LHQ.Utils.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex _regexMatchNonAlphaCodes = new Regex("[^a-zA-Z0-9]", RegexOptions.IgnoreCase);
        private static readonly Regex _regexPascalCasingToWords = new Regex(@"([A-Z]*[^A-Z]+)", RegexOptions.Compiled);

        public static string ToLowerInvariant(this object source)
        {
            return source?.ToString().ToLowerInvariant();
        }

        public static string ValueOrDefault(this string source, string defaultValue)
        {
            return string.IsNullOrEmpty(source) ? defaultValue : source;
        }


        /// <summary>
        ///     Compare two string values if they are equal.
        /// </summary>
        /// <param name="source">First string value to compare.</param>
        /// <param name="compareTo">Second string value to compare.</param>
        /// <param name="ignoreCase">True to ignore case.</param>
        /// <returns>Returns true if specified string values are equal, of false if not.</returns>
        public static bool EqualsTo(this string source, string compareTo, bool ignoreCase = true)
        {
            return EqualsTo(source, compareTo, ignoreCase, CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Compare two string values if they are equal.
        /// </summary>
        /// <param name="source">First string value to compare.</param>
        /// <param name="compareTo">Second string value to compare.</param>
        /// <param name="ignoreCase">True to ignore case.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>
        ///     Returns true if specified string values are equal, of false if not.
        /// </returns>
        public static bool EqualsTo(this string source, string compareTo, bool ignoreCase, CultureInfo cultureInfo)
        {
            if (source == null && compareTo != null || 
                compareTo == null && source != null)
            {
                return false;
            }

            if (source == null)
            {
                return true;
            }

            return string.Compare(source, compareTo, ignoreCase, cultureInfo) == 0;
        }

        /// <summary>
        ///     Tests if specified string ends with other specified string.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <param name="endWith">End with string.</param>
        /// <param name="ignoreCase">True to ignore case, false to not ignore case.</param>
        /// <returns>Returns true if <c>source</c> ends with <c>endWith</c>.</returns>
        public static bool EndsWith(this string source, string endWith, bool ignoreCase = true)
        {
            if (ignoreCase)
            {
                source = source.ToUpper();
                endWith = endWith.ToUpper();
            }

            return source.EndsWith(endWith);
        }

        /// <summary>
        ///     Tests if specified string contains other specified string.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <param name="containment">Contains string.</param>
        /// <param name="ignoreCase">True to ignore case, false to not ignore case.</param>
        /// <returns>
        ///     Returns true if <c>source</c> contains <c>contain</c>.
        /// </returns>
        public static bool Contains(this string source, string containment, bool ignoreCase)
        {
            return Contains(source, containment, ignoreCase, CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Tests if specified string contains other specified string.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <param name="containment">Contains string.</param>
        /// <param name="ignoreCase">True to ignore case, false to not ignore case.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>
        ///     Returns true if <c>source</c> contains <c>contain</c>.
        /// </returns>
        public static bool Contains(this string source, string containment, bool ignoreCase, CultureInfo cultureInfo)
        {
            if (ignoreCase)
            {
                return source.ToLower(cultureInfo).Contains(containment.ToLower(cultureInfo));
            }

            return source.Contains(containment);
        }

        /// <summary>
        ///     Tests if specified string start with other specified string.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <param name="startWith">Start with string.</param>
        /// <param name="ignoreCase">True to ignore case, false to not ignore case.</param>
        /// <returns>Returns true if <c>source</c> starts with <c>startWith</c>.</returns>
        public static bool StartsWith(this string source, string startWith, bool ignoreCase = true)
        {
            if (ignoreCase)
            {
                source = source.ToUpper();
                startWith = startWith.ToUpper();
            }

            return source.StartsWith(startWith);
        }

        /// <summary>
        ///     Compares source string if is occured in specified array of strings.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <param name="ignoreCase">True to ignore case, false to not ignore case.</param>
        /// <param name="mustMatchAll">
        ///     True if all strings in <c>toStrings</c> must be equal to <c>source</c>, false if only one
        ///     must be equal.
        /// </param>
        /// <param name="toStrings">Array of string to compare <c>source</c> string with.</param>
        /// <returns>
        ///     If <c>mustMatchAll</c> is true, then returns true only if all items in <c>toStrings</c> are equal to <c>source</c>,
        ///     otherwise returns false.
        ///     If <c>mustMatchAll</c> is false, then returns true if at least one item from <c>toStrings</c> is equal to
        ///     <c>source</c>,
        ///     otherwise returns false.
        /// </returns>
        public static bool IsInArray(string source, bool ignoreCase, bool mustMatchAll, params string[] toStrings)
        {
            var result = false;

            foreach (string s in toStrings)
            {
                result = EqualsTo(source, s, ignoreCase);
                if (mustMatchAll)
                {
                    if (!result)
                    {
                        return false;
                    }
                }
                else
                {
                    if (result)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Compares source string if is occured in specified array of strings.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <param name="mustMatchAll">
        ///     True if all strings in <c>toStrings</c> must be equal to <c>source</c>, false if only one
        ///     must be equal.
        /// </param>
        /// <param name="toStrings">Array of string to compare <c>source</c> string with.</param>
        /// <returns>
        ///     If <c>mustMatchAll</c> is true, then returns true only if all items in <c>toStrings</c> are equal to <c>source</c>,
        ///     otherwise returns false.
        ///     If <c>mustMatchAll</c> is false, then returns true if at least one item from <c>toStrings</c> is equal to
        ///     <c>source</c>,
        ///     otherwise returns false.
        /// </returns>
        /// <remarks>Parameter <c>ignoreCase</c> is set to true.</remarks>
        public static bool IsInArray(string source, bool mustMatchAll, params string[] toStrings)
        {
            return IsInArray(source, true, mustMatchAll, toStrings);
        }

        /// <summary>
        ///     Converts to base 64 string.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <returns>String encoded to UTF8 base 64 format.</returns>
        public static string ToBase64(this string source)
        {
            string base64String = null;
            if (!source.IsNullOrEmpty())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(source);
                base64String = Convert.ToBase64String(bytes);
            }

            return base64String;
        }

        /// <summary>
        ///     Converts from abse 64 string to decoded string.
        /// </summary>
        /// <param name="base64String">Source string.</param>
        /// <returns>Returns parsed string.</returns>
        public static string FromBase64(this string base64String)
        {
            string result = null;
            if (!base64String.IsNullOrEmpty())
            {
                byte[] bytes = Convert.FromBase64String(base64String);
                result = Encoding.UTF8.GetString(bytes);
            }

            return result;
        }

        public static bool IsBase64(this string base64String)
        {
            try
            {
                base64String.FromBase64();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string NormalizeCoding(this string source)
        {
            source = source.RemoveDiacritics();
            return _regexMatchNonAlphaCodes.Replace(source, string.Empty);
        }

        /// <summary>
        ///     Removes diacritics characters from text and replace it with its non-diacritics chars.
        /// </summary>
        /// <param name="source">The text.</param>
        /// <returns></returns>
        public static string RemoveDiacritics(this string source)
        {
            string normalized = source.Replace(' ', '-').Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < normalized.Length; i++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(normalized[i]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(normalized[i]);
                }
            }

            return sb.ToString();
        }

        public static string FromPascalCasingToWords(this string value)
        {
            int idx = -1;
            return _regexPascalCasingToWords.Replace(value, match =>
                {
                    idx++;
                    return (idx == 0 ? match.Value : match.Value.ToLowerInvariant()) + " ";
                }).TrimEnd();
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string IfNullOrEmpty(this string value, object defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue?.ToString() : value;
        }

        public static string FormatWith(this string format, params object[] args)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            return string.Format(format, args);
        }

        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            return string.Format(provider, format, args);
        }

        /// <summary>
        ///     Returns text shorted to selected length ending with continueSign string
        /// </summary>
        /// <example>
        ///     string shorted = GetShortText("Hallo world!!!", 4, "...", true);
        /// </example>
        /// <remarks>
        ///     length should be larger than continueSign length
        /// </remarks>
        /// <param name="source">Input text to be shorted</param>
        /// <param name="length">Selected length</param>
        /// <param name="continueSign">String to be added on the end as a sign explaining that text is shorted</param>
        /// <param name="endLength">
        ///     If true, output will be "length" characters long, else will be text shorted to "length" and
        ///     then will be added continuaSign at the end
        /// </param>
        /// <returns>Shorted string value</returns>
        public static string ShortenText(this string source, int length, string continueSign = "...", bool endLength = true)
        {
            string result = source;

            if (source.Length > length)
            {
                if (endLength && length > continueSign.Length)
                {
                    length -= continueSign.Length;
                }

                result = source.Substring(0, length) + continueSign;
            }

            return result;
        }

        /// <summary>
        ///     Converts enum value represented as string into its typed enum opposite.
        /// </summary>
        /// <typeparam name="TEnum">The target type of the enum.</typeparam>
        /// <param name="enumValue">The enum value as string.</param>
        /// <returns>
        ///     Typed enum parsed from its string representation in <paramref name="enumValue" />
        ///     or <c>null</c> if such string cant be converted to specified enum type.
        /// </returns>
        public static TEnum? ToEnum<TEnum>(this string enumValue)
            where TEnum : struct
        {
            if (!enumValue.IsNullOrEmpty() && Enum.TryParse(enumValue, true, out TEnum result))
            {
                return result;
            }

            return null;
        }

        public static bool IsGuid(this string stringValue)
        {
            return Guid.TryParse(stringValue, out Guid _);
        }

        public static Guid ToGuid(this string value)
        {
            return ToGuid(value, Guid.Empty);
        }

        public static Guid ToGuid(this string value, Guid nullValue)
        {
            return value.IsGuid() ? new Guid(value) : nullValue;
        }

        /// <summary>
        ///     Removes new line(\n\r) from specified string.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <returns>Returns string with removed new line(\n\r).</returns>
        /// <remarks>Parameter <c>replacewith</c> is set ot <c>string.Empty</c>.</remarks>
        public static string RemoveNewLines(this string source)
        {
            return RemoveNewLines(source, string.Empty);
        }

        /// <summary>
        ///     Removes new line(\n\r) from specified string.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <param name="replacewith">Replace with.</param>
        /// <returns>Returns string with removed new line(\n\r).</returns>
        public static string RemoveNewLines(this string source, string replacewith)
        {
            return source.Replace("\n", replacewith).Replace("\r", replacewith);
        }

        public static string Format(this CultureInfo cultureInfo)
        {
            return "{0} ({1})".FormatWith(cultureInfo.EnglishName, cultureInfo.Name);
        }

        public static bool IsGenericInvariantCulture(this CultureInfo cultureInfo)
        {
            return cultureInfo.LCID == 127;
        }

        public static string ToTitleCase(this string source, CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            return cultureInfo.TextInfo.ToTitleCase(source);
        }
    }
}
