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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LHQ.Utils.Utilities
{
    public static class ProcessUtils
    {
        // these overloads match the ones in Process.Start to make it a simpler transition for callers
        // see http://msdn.microsoft.com/en-us/library/system.diagnostics.process.start.aspx

        /// <summary>
        /// Runs asynchronous process.
        /// </summary>
        /// <param name="fileName">An application or document which starts the process.</param>
        public static Task<ProcessResults> RunAsync(string fileName)
            => RunAsync(new ProcessStartInfo(fileName));

        /// <summary>
        /// Runs asynchronous process.
        /// </summary>
        /// <param name="fileName">An application or document which starts the process.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public static Task<ProcessResults> RunAsync(string fileName, CancellationToken cancellationToken)
            => RunAsync(new ProcessStartInfo(fileName), cancellationToken);

        /// <summary>
        /// Runs asynchronous process.
        /// </summary>
        /// <param name="fileName">An application or document which starts the process.</param>
        /// <param name="arguments">Command-line arguments to pass to the application when the process starts.</param>
        public static Task<ProcessResults> RunAsync(string fileName, string arguments)
            => RunAsync(new ProcessStartInfo(fileName, arguments));

        /// <summary>
        /// Runs asynchronous process.
        /// </summary>
        /// <param name="fileName">An application or document which starts the process.</param>
        /// <param name="arguments">Command-line arguments to pass to the application when the process starts.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public static Task<ProcessResults> RunAsync(string fileName, string arguments, CancellationToken cancellationToken)
            => RunAsync(new ProcessStartInfo(fileName, arguments), cancellationToken);

        /// <summary>
        /// Runs asynchronous process.
        /// </summary>
        /// <param name="processStartInfo">The <see cref="T:System.Diagnostics.ProcessStartInfo" /> that contains the information that is used to start the process, including the file name and any command-line arguments.</param>
        public static Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo)
            => RunAsync(processStartInfo, CancellationToken.None);

        /// <summary>
        /// Runs asynchronous process.
        /// </summary>
        /// <param name="processStartInfo">The <see cref="T:System.Diagnostics.ProcessStartInfo" /> that contains the information that is used to start the process, including the file name and any command-line arguments.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public static Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo, CancellationToken cancellationToken)
            => RunAsync(processStartInfo, new List<string>(), new List<string>(), cancellationToken);

        public static async Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo, List<string> standardOutput,
            List<string> standardError, CancellationToken cancellationToken)
        {
            // force some settings in the start info so we can capture the output
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            var tcs = new TaskCompletionSource<ProcessResults>();

            var process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            var standardOutputResults = new TaskCompletionSource<string[]>();
            process.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                        standardOutput.Add(args.Data);
                    else
                        standardOutputResults.SetResult(standardOutput.ToArray());
                };

            var standardErrorResults = new TaskCompletionSource<string[]>();
            process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                        standardError.Add(args.Data);
                    else
                        standardErrorResults.SetResult(standardError.ToArray());
                };

            var processStartTime = new TaskCompletionSource<DateTime>();

            process.Exited += async (sender, args) =>
                {
                    // Since the Exited event can happen asynchronously to the output and error events, 
                    // we await the task results for stdout/stderr to ensure they both closed.  We must await
                    // the stdout/stderr tasks instead of just accessing the Result property due to behavior on MacOS.  
                    // For more details, see the PR at https://github.com/jamesmanning/RunProcessAsTask/pull/16/
                    tcs.TrySetResult(
                        new ProcessResults(
                            process,
                            await processStartTime.Task.ConfigureAwait(false),
                            await standardOutputResults.Task.ConfigureAwait(false),
                            await standardErrorResults.Task.ConfigureAwait(false)
                        )
                    );
                };

            using (cancellationToken.Register(
                       () =>
                           {
                               tcs.TrySetCanceled();
                               try
                               {
                                   if (!process.HasExited)
                                       process.Kill();
                               }
                               catch (InvalidOperationException)
                               { }
                           }))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var startTime = DateTime.Now;
                if (process.Start() == false)
                {
                    tcs.TrySetException(new InvalidOperationException("Failed to start process"));
                }
                else
                {
                    try
                    {
                        startTime = process.StartTime;
                    }
                    catch (Exception)
                    {
                        // best effort to try and get a more accurate start time, but if we fail to access StartTime
                        // (for instance, process has already existed), we still have a valid value to use.
                    }

                    processStartTime.SetResult(startTime);

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }

                return await tcs.Task.ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Contains information about process after it has exited.
    /// </summary>
    public sealed class ProcessResults : IDisposable
    {
        public ProcessResults(Process process, DateTime processStartTime, string[] standardOutput, string[] standardError)
        {
            Process = process;
            ExitCode = process.ExitCode;
            RunTime = process.ExitTime - processStartTime;
            StandardOutput = standardOutput;
            StandardError = standardError;
        }

        public Process Process { get; }

        public int ExitCode { get; }

        public TimeSpan RunTime { get; }

        public string[] StandardOutput { get; }

        public string[] StandardError { get; }

        public void Dispose()
        {
            Process.Dispose();
        }
    }
}
