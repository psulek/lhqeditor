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

// code from: https://github.com/bicycle-nerd/elevated-common/blob/master/projects/Elevated/Utils/ShortValue.cs
// original idea: http://kvz.io/blog/2009/06/10/create-short-ids-with-php-like-youtube-or-tinyurl/#toc_10

namespace LHQ.Utils.Utilities
{
    public static class ShortId
    {
        public static readonly string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static decimal BcPow(double a, double b)
        {
            return (decimal)Math.Floor(Math.Pow(a, b));
        }

        public static ulong Decode(string value, int pad = 0)
        {
            value = value.ReverseString();
            int len = value.Length - 1;
            ulong result = 0;

            for (int t = len; t >= 0; t--)
            {
                var bcp = (ulong)BcPow(Alphabet.Length, len - t);
                result += (ulong)Alphabet.IndexOf(value[t]) * bcp;
            }

            if (pad > 0)
            {
                result -= (ulong)BcPow(Alphabet.Length, pad);
            }

            return result;
        }

        public static string Encode(byte[] value, int startIndex = 0, int pad = 0)
        {
            return Encode(BitConverter.ToUInt64(value, startIndex), pad);
        }

        public static string Encode(Guid guid, int pad = 0)
        {
            byte[] bytes = guid.ToByteArray();

            string first = Encode(bytes, 0, pad);
            string second = Encode(bytes, 8, pad);

            return first + second;
        }

        public static string Encode(ulong value, int pad = 0)
        {
            string result = string.Empty;

            if (pad > 0)
            {
                value += (ulong)BcPow(Alphabet.Length, pad);
            }

            for (double t = value != 0 ? Math.Floor(Math.Log(value, Alphabet.Length)) : 0; t >= 0; t--)
            {
                var bcp = (ulong)BcPow(Alphabet.Length, t);
                ulong a = (ulong)Math.Floor(Convert.ToDouble(value / (decimal)bcp)) % (ulong)Alphabet.Length;
                result += Alphabet[(int)a];
                value = value - a * bcp;
            }

            return result.ReverseString();
        }

        private static string ReverseString(this string value)
        {
            char[] arr = value.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }
}
