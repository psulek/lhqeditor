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
using LHQ.Core.Interfaces;
using NLog;

namespace LHQ.App.Extensions
{
    public static class LoggerExtensions
    {
        public static LogEventType ToLogEventType(this LogLevel logLevel)
        {
            if (logLevel == LogLevel.Info)
            {
                return LogEventType.Info;
            }

            if (logLevel == LogLevel.Warn)
            {
                return LogEventType.Warn;
            }

            if (logLevel == LogLevel.Debug)
            {
                return LogEventType.Debug;
            }

            if (logLevel == LogLevel.Error)
            {
                return LogEventType.Error;
            }

            if (logLevel == LogLevel.Fatal)
            {
                return LogEventType.Fatal;
            }

            throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
        }

        public static LogLevel ToLogLevel(this LogEventType eventType)
        {
            switch (eventType)
            {
                case LogEventType.Info:
                {
                    return LogLevel.Info;
                }
                case LogEventType.Warn:
                {
                    return LogLevel.Warn;
                }
                case LogEventType.Debug:
                {
                    return LogLevel.Debug;
                }
                case LogEventType.Error:
                {
                    return LogLevel.Error;
                }
                case LogEventType.Fatal:
                {
                    return LogLevel.Fatal;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
                }
            }
        }
    }
}
