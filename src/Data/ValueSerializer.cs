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
using LHQ.Utils.Utilities;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.Data
{
    public static class ValueSerializer
    {
        public static string Serialize<TEnum>(TEnum value) where TEnum: struct 
        {
            return value.ToString();
        }

        public static string Serialize(short value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string Serialize(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string Serialize(long value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string Serialize(bool value)
        {
            return Convert.ToInt32(value).ToString(CultureInfo.InvariantCulture);
        }

        public static string Serialize(DateTime value)
        {
            return JsonUtils.DateToJsonString(value);
        }

        public static string Serialize(Guid value)
        {
            return value.ToString();
        }

        public static string Serialize(Version value)
        {
            return value.ToString();
        }

        public static bool TryDeserialize<TEnum>(string value, bool ignoreCase, out TEnum @enum) where TEnum : struct
        {
            return Enum.TryParse(value, ignoreCase, out @enum);
        }

        public static bool TryDeserialize(string value, out DateTime? output)
        {
            output = null;
            bool convertResult = !value.IsNullOrEmpty();
            if (convertResult)
            {
                try
                {
                    output = JsonUtils.DateFromJsonString(value);
                }
                catch (Exception e)
                {
                    convertResult = false;
                    DebugUtils.Warning(
                        "Could not deserialize datetime value from xml format value '{0}', error: {1}".FormatWith(value, e.Message));
                }
            }
            return convertResult;
        }

        public static bool TryDeserialize(string value, out short? output)
        {
            output = null;
            short i = 0;
            bool convertResult = !value.IsNullOrEmpty() && short.TryParse(value, out i);
            if (convertResult)
            {
                output = i;
            }
            return convertResult;
        }

        public static bool TryDeserialize(string value, out int? output)
        {
            output = null;
            var i = 0;
            bool convertResult = !value.IsNullOrEmpty() && int.TryParse(value, out i);
            if (convertResult)
            {
                output = i;
            }
            return convertResult;
        }

        public static bool TryDeserialize(string value, out long? output)
        {
            output = null;
            long i = 0;
            bool convertResult = !value.IsNullOrEmpty() && long.TryParse(value, out i);
            if (convertResult)
            {
                output = i;
            }
            return convertResult;
        }

        public static bool TryDeserialize(string value, out bool? output)
        {
            output = null;
            var i = 0;
            bool convertResult = !value.IsNullOrEmpty() && int.TryParse(value, out i);
            if (convertResult)
            {
                if (i == 0 || i == 1)
                {
                    output = Convert.ToBoolean(i);
                }
            }
            else
            {
                convertResult = bool.TryParse(value, out bool b);
                if (convertResult)
                {
                    output = b;
                }
            }

            return convertResult;
        }

        public static bool TryDeserialize(string value, out Guid? output)
        {
            output = null;
            Guid g = Guid.Empty;
            bool convertResult = !value.IsNullOrEmpty() && Guid.TryParse(value, out g);
            if (convertResult)
            {
                output = g;
            }
            return convertResult;
        }

        public static bool TryDeserialize(string value, out Version output)
        {
            output = null;
            Version v = null;
            bool convertResult = !value.IsNullOrEmpty() && Version.TryParse(value, out v);
            if (convertResult)
            {
                output = v;
            }
            return convertResult;
        }

        public static bool CompareEqualityAsBool(string value1, bool value2)
        {
            var equals = false;

            if (TryDeserialize(value1, out bool? b1))
            {
                equals = b1 == value2;
            }

            return equals;
        }

        public static bool CompareEqualityAsBool(string value1, string value2)
        {
            return CompareEqualityAsBool(value1, value2, out bool? _, out bool? _);
        }

        public static bool CompareEqualityAsBool(string value1, string value2, out bool? bool1, out bool? bool2)
        {
            var equals = false;
            bool2 = null;

            if (TryDeserialize(value1, out bool1) && TryDeserialize(value2, out bool2))
            {
                equals = bool1 == bool2;
            }

            return equals;
        }
    }
}
