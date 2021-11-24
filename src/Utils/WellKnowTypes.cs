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
// ReSharper disable UnusedMember.Global

namespace LHQ.Utils
{
    public static class WellKnowTypes
    {
        public static class System
        {
            public static readonly Type Byte = typeof(byte);
            public static readonly Type ByteNullable = typeof(byte?);
            public static readonly Type Boolean = typeof(bool);
            public static readonly Type BooleanNullable = typeof(bool?);
            public static readonly Type Decimal = typeof(decimal);
            public static readonly Type DecimalNullable = typeof(decimal?);
            public static readonly Type Int16 = typeof(short);
            public static readonly Type Int16Nullable = typeof(short?);
            public static readonly Type Int32 = typeof(int);
            public static readonly Type Int32Nullable = typeof(int?);
            public static readonly Type Int64 = typeof(long);
            public static readonly Type Int64Nullable = typeof(long?);
            public static readonly Type String = typeof(string);
            public static readonly Type DateTime = typeof(DateTime);
            public static readonly Type DateTimeNullable = typeof(DateTime?);
            public static readonly Type TimeSpan = typeof(TimeSpan);
            public static readonly Type TimeSpanNullable = typeof(TimeSpan?);
            public static readonly Type Double = typeof(double);
            public static readonly Type DoubleNullable = typeof(double?);
        }
    }
}
