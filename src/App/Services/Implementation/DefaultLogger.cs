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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using LHQ.App.Extensions;
using LHQ.App.Services.Interfaces;
using LHQ.Core.Interfaces;
using LHQ.Utils.Extensions;
using NLog;
using NLog.Config;
using ILogger = LHQ.Core.Interfaces.ILogger;

namespace LHQ.App.Services.Implementation
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class DefaultLogger : AppContextServiceBase, ILogger, IHasInitialize
    {
        private readonly Logger _logger;
        private bool _isInitialized;

        public DefaultLogger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }
        
        public bool IsInitialized => _isInitialized;

        public void Initialize()
        {
            string appFolder = AppContext.AppFolder;
            var fileSystemService = Container.Get<IFileSystemService>();
            string logsFolder = AppContext.RunInVsPackage
                ? fileSystemService.GetDataFileName("logs")
                : Path.Combine(appFolder, "logs");
            
            Initialize(AppContext.AppFolder, logsFolder);
        }

        public void Initialize(string appFolder, string logsFolder)
        {
            if (_isInitialized)
            {
                return;
            }

            if (LogManager.Configuration == null)
            {
                string nlogConfigFile = Path.Combine(appFolder, "NLog.config");
                LogManager.Configuration = new XmlLoggingConfiguration(nlogConfigFile, true);
                LogManager.ReconfigExistingLoggers();
            }

            try
            {
                if (!Directory.Exists(logsFolder))
                {
                    Directory.CreateDirectory(logsFolder);
                }

                LogManager.Configuration.Variables["appLogsFolder"] = logsFolder;
            }
            catch (Exception e)
            {
                DebugUtils.Error($"{nameof(DefaultLogger)}.{nameof(Initialize)} failed", e);
            }

            _isInitialized = true;
        }

        public void Log(LogEventType eventType, string message, Exception exception = null)
        {
            _logger.Log(eventType.ToLogLevel(), exception, message);
        }

        public object GetSourceLogger()
        {
            return _logger;
        }

        public void Debug(string message, Exception exception = null)
        {
            Log(LogEventType.Debug, message, exception);
        }

        public void Error(string message, Exception exception = null)
        {
            Log(LogEventType.Error, message, exception);
        }

        public void Info(string message, Exception exception = null)
        {
            Log(LogEventType.Info, message, exception);
        }

        public void Warning(string message, Exception exception = null)
        {
            Log(LogEventType.Warn, message, exception);
        }

        public void Fatal(string message, Exception exception = null)
        {
            Log(LogEventType.Fatal, message, exception);
        }
    }
}
