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
// ReSharper disable UnusedMember.Global

namespace LHQ.App.Code
{
    public static class DateTimeHelper
    {
        public static DateTime UtcNow => DateTime.UtcNow;

        public static void Initialize()
        { }

        public static void Stop()
        { }

        private const string IsoFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        private static DateTime Epoch { get; } = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static string IsoUtcDateTime()
        {
            return DateTimeToUtcIsoString(DateTime.UtcNow);
        }

        public static string DateTimeToUtcIsoString(DateTime value)
        {
            DateTime d = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
            DateTimeOffset dto = d;

            return dto.ToString(IsoFormat, CultureInfo.InvariantCulture);
        }

        public static DateTime GetUtcDateTimeFromIsoString(string iso8601String)
        {
            var dateTimeOffset = DateTimeOffset.ParseExact(
                iso8601String,
                new[] { IsoFormat },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None);

            return dateTimeOffset.UtcDateTime;
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            return Epoch.AddMilliseconds(unixTime);
        }

        public static DateTime? FromUnixTime(long? unixTime)
        {
            if (unixTime == null)
            {
                return null;
            }

            return Epoch.AddMilliseconds(unixTime.Value);
        }

        public static long ToUnixTime(DateTime? dateTime)
        {
            if (dateTime == null)
                return 0;

            return (long)dateTime.Value.Subtract(Epoch).TotalMilliseconds;
        }
    }
}
