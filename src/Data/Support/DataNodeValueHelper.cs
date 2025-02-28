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
using System.Globalization;
using LHQ.Utils.Extensions;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.Data.Support
{
    internal class DataNodeValueHelper
    {
        internal static string ObjectToString(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            Type t = obj.GetType();
            if (t == typeof(bool))
            {
                return ToString((bool)obj);
            }
            if (t == typeof(int))
            {
                return ToString((int)obj);
            }
            if (t == typeof(long))
            {
                return ToString((long)obj);
            }
            return t.IsEnum ? ToString(obj as Enum) : obj.ToString();
        }

        internal static string ToString(bool value)
        {
            return value.ToLowerInvariant();
        }

        internal static string ToString(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        internal static string ToString(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        internal static string ToString(long value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        internal static string ToString(Enum value)
        {
            return value.ToString();
        }

        internal static string ToString(Guid value)
        {
            return value.ToString();
        }

        internal static string FromString(string value, string defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        internal static bool FromString(string value, bool defaultValue)
        {
            bool result = defaultValue;
            if (!value.IsNullOrEmpty())
            {
                bool.TryParse(value, out result);
            }
            return result;
        }

        internal static int FromString(string value, int defaultValue)
        {
            int result = defaultValue;
            if (!string.IsNullOrEmpty(value))
            {
                int.TryParse(value, out result);
            }
            return result;
        }

        internal static byte FromString(string value, byte defaultValue)
        {
            byte result = defaultValue;
            if (!string.IsNullOrEmpty(value))
            {
                byte.TryParse(value, out result);
            }
            return result;
        }

        internal static decimal FromString(string value, decimal defaultValue)
        {
            decimal result = defaultValue;
            if (!string.IsNullOrEmpty(value))
            {
                decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
            }
            return result;
        }

        internal static long FromString(string value, long defaultValue)
        {
            long result = defaultValue;
            if (!string.IsNullOrEmpty(value))
            {
                long.TryParse(value, out result);
            }
            return result;
        }

        internal static T EnumFromString<T>(string value, T defaultValue)
            where T : struct
        {
            T result = defaultValue;
            if (!string.IsNullOrEmpty(value))
            {
                if (Enum.TryParse(value, out T val))
                {
                    result = val;
                }
            }

            return result;
        }
    }
}
