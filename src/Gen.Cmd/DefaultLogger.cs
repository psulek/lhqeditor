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

using System.Text;
using NLog;
using NLog.Config;

namespace LHQ.Gen.Cmd;

public class DefaultLogger
{
    private const string DetailInfo =
        @"
{0} {1}
{0} type: {2}
{0} message: {3}
{0} source: {4}
{0} stack trace: {5}
{0} has inner exception: {6}";
    
    private static readonly Lazy<DefaultLogger> _instance = new Lazy<DefaultLogger>(() => new DefaultLogger());

    public static DefaultLogger Instance => _instance.Value;

    private DefaultLogger()
    {
        try
        {
            // var appFolder = Path.GetDirectoryName(Environment.ProcessPath);
            var appFolder = Directory.GetCurrentDirectory();
            string nlogConfigFile = Path.Combine(appFolder, "NLog.config");
            string logsFolder = Path.Combine(appFolder, "logs");
            if (!Directory.Exists(logsFolder))
            {
                Directory.CreateDirectory(logsFolder);
            }

            Logger = LogManager.GetCurrentClassLogger();
            if (LogManager.Configuration == null)
            {
                //Console.WriteLine("Loading nlog.config from: " + nlogConfigFile);
                LogManager.Configuration = new XmlLoggingConfiguration(nlogConfigFile, true);
                LogManager.ReconfigExistingLoggers();
            }

            //Console.WriteLine("Current 'appLogsFolder': " + logsFolder);
            LogManager.Configuration.Variables["appLogsFolder"] = logsFolder;
        }
        catch (Exception e)
        {
            Console.WriteLine(GetFullExceptionDetails(e));
        }
    }
    
    public static string GetFullExceptionDetails(Exception e, bool loopInnerExceptions = true)
    {
        try
        {
            var sb = new StringBuilder();
            var deep = 1;
            Exception innerEx = AppendException(sb, "Exception was handled", deep, e);

            if (loopInnerExceptions)
            {
                while (innerEx != null)
                {
                    deep++;
                    innerEx = AppendException(sb, string.Format("Inner exception(deep: {0})", deep - 1), deep, innerEx);
                }
            }

            return sb.ToString();
        }
        catch (Exception e1)
        {
            Console.WriteLine("Exception in GetExceptionStr, {0}", e1.Message);
            return string.Empty;
        }
    }

    private static Exception AppendException(StringBuilder sb, string firstLine, int deep, Exception e)
    {
        sb.AppendFormat(DetailInfo,
            new string('-', deep),
            firstLine,
            e.GetType().FullName,
            e.Message,
            e.Source,
            e.StackTrace,
            e.InnerException != null ? "yes" : "no");

        return e.InnerException;
    }

    public ILogger Logger { get; }
}
