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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace LHQ.Gen.Lib
{
    public sealed class InMemoryLogger : IDisposable
    {
        private readonly Logger _logger;
        private readonly InMemoryTarget _memoryTarget;
        private readonly LogFactory _factory;
        private bool _disposed;

        #region InMemoryTarget

        internal sealed class InMemoryTarget : TargetWithLayout
        {
            public InMemoryTarget()
            {
                Logs = new List<Tuple<LogLevel, string>>();
                OptimizeBufferReuse = true;
            }

            public InMemoryTarget(string name)
                : this()
            {
                Name = name;
            }

            public List<Tuple<LogLevel, string>> Logs { get; }

            //public List<string> Errors { get; }

            [DefaultValue(0)]
            public int MaxLogsCount { get; set; }

            /// <summary>
            /// Renders the logging event message and adds it to the internal ArrayList of log messages.
            /// </summary>
            /// <param name="logEvent">The logging event.</param>
            protected override void Write(LogEventInfo logEvent)
            {
                if (MaxLogsCount > 0 && Logs.Count >= MaxLogsCount)
                {
                    Logs.RemoveAt(0);
                }

                string message = RenderLogEvent(Layout, logEvent);
                Logs.Add(Tuple.Create(logEvent.Level, message));
            }
        }

        #endregion

        public InMemoryLogger()
        {
            _memoryTarget = new InMemoryTarget
            {
                //Layout = "${longdate}: [${level}] ${message}"
                Layout = "${message}"
            };

            var config = new LoggingConfiguration();
            config.AddRuleForAllLevels(_memoryTarget);

            _factory = new LogFactory(config);
            _logger = _factory.GetCurrentClassLogger();
        }

        public Logger Logger => _logger;

        public override string ToString()
        {
            var logMessages = _memoryTarget.Logs.Select(x => $"{x.Item1} {x.Item2}");
            return string.Join(Environment.NewLine, logMessages);
        }

        public List<Tuple<string, byte>> AllLogs => _memoryTarget.Logs.Select(GetLogTuple).ToList();

        private Tuple<string, byte> GetLogTuple(Tuple<LogLevel, string> source)
        {
            // 0 - info, 1 - warn , 2 - error
            byte level = 0;
            LogLevel sourceLevel = source.Item1;
            if (sourceLevel == LogLevel.Warn)
            {
                level = 1;
            }
            else if (sourceLevel == LogLevel.Error || sourceLevel == LogLevel.Fatal)
            {
                level = 2;
            }

            return Tuple.Create(source.Item2, level);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _factory?.Dispose();
                _memoryTarget?.Dispose();
                _disposed = true;
            }
        }
    }
}
