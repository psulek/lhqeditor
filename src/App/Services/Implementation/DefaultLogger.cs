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
using LHQ.Core.DependencyInjection;
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
        private IFileSystemService _fileSystemService;

        public DefaultLogger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        {
            _fileSystemService = serviceContainer.Get<IFileSystemService>();
        }

        public void Initialize()
        {
            if (LogManager.Configuration == null)
            {
                string nlogConfigFile = Path.Combine(AppContext.AppFolder, "NLog.config");
                LogManager.Configuration = new XmlLoggingConfiguration(nlogConfigFile, true);
                LogManager.ReconfigExistingLoggers();
            }

            try
            {
                string logsFolder = GetLogsFolder();
                LogManager.Configuration.Variables["appLogsFolder"] = logsFolder;
            }
            catch (Exception e)
            {
                DebugUtils.Error($"{nameof(DefaultLogger)}.{nameof(Initialize)} failed", e);
            }

            string msg = AppContext.RunInVsPackage
                ? "Editor Instance Started"
                : "Application Started.";
            _logger.Info(msg);
        }

        private string GetLogsFolder()
        {
            bool useFileSystem = AppContext.RunInVsPackage;
#if !DEBUG
            useFileSystem = true;
#endif

            string appLogsFolder = useFileSystem
                ? _fileSystemService.GetDataFileName("logs")
                : Path.Combine(AppContext.AppFolder, "logs");

            if (!Directory.Exists(appLogsFolder))
            {
                Directory.CreateDirectory(appLogsFolder);
            }

            return appLogsFolder;
        }

        // private LogLevel GetLogLevel(LogEventType eventType)
        // {
        //     switch (eventType)
        //     {
        //         case LogEventType.Info:
        //         {
        //             return LogLevel.Info;
        //         }
        //         case LogEventType.Warn:
        //         {
        //             return LogLevel.Warn;
        //         }
        //         case LogEventType.Debug:
        //         {
        //             return LogLevel.Debug;
        //         }
        //         case LogEventType.Error:
        //         {
        //             return LogLevel.Error;
        //         }
        //         case LogEventType.Fatal:
        //         {
        //             return LogLevel.Fatal;
        //         }
        //         default:
        //         {
        //             throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
        //         }
        //     }
        // }

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
