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
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.Utils.Extensions
{
    public static class TaskAsyncExtensions
    {
        // code from:
        // https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/interop-with-other-asynchronous-patterns-and-types#WaitHandles
        public static Task WaitOneAsync(this WaitHandle waitHandle, int timeout = -1)
        {
            if (waitHandle == null)
            {
                throw new ArgumentNullException(nameof(waitHandle));
            }

            var tcs = new TaskCompletionSource<bool>();
            var rwh = ThreadPool.RegisterWaitForSingleObject(waitHandle,
                delegate { tcs.TrySetResult(true); }, null, timeout, true);
                //delegate { tcs.TrySetResult(true); }, null, -1, true);

            var t = tcs.Task;
            t.ContinueWith(antecedent => rwh.Unregister(null));
            return t;
        }

        public static Task<TResult> WaitUntil<TResult>(TimeSpan timeout, Func<CancellationToken, Task<TResult>> taskFunc)
        {
            var cts = new CancellationTokenSource(timeout);
            CancellationToken cancellationToken = cts.Token;
            return WaitUntil(cancellationToken, taskFunc);
        }

        public static Task<TResult> WaitUntil<TResult>(CancellationToken cancellationToken, Func<CancellationToken, Task<TResult>> taskFunc)
        {
            return WaitUntil(cancellationToken, -1, taskFunc);
        }

        public static async Task<TResult> WaitUntil<TResult>(CancellationToken cancellationToken,
            int timeout, Func<CancellationToken, Task<TResult>> taskFunc)
        {
            var delayTask = cancellationToken.WaitHandle.WaitOneAsync(timeout);
            var task = taskFunc(cancellationToken);
            var finalTask = await Task.WhenAny(delayTask, task);
            if (finalTask == task)
            {
                return task.Result;
            }

            return default;
        }

        public static async Task RetryOnError<TException>(string operationName,
            int retries, TimeSpan retriesDelay, Func<Task> task,
            Func<TException, bool> customRetryAllowance = null,
            CancellationToken cancellationToken = default)
            where TException : Exception
        {
            await RetryOnError<TException, object>(operationName, retries, retriesDelay, async () =>
                    {
                        await task();
                        return null;
                    },
                customRetryAllowance, cancellationToken);
        }

        public static async Task<TResult> RetryOnError<TException, TResult>(string operationName,
            int retries, TimeSpan retriesDelay, Func<Task<TResult>> task,
            Func<TException, bool> customRetryAllowance = null,
            CancellationToken cancellationToken = default)
            where TException : Exception
        {
            if (retries <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retries));
            }

            if (retriesDelay <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(retriesDelay));
            }

            var attempts = 0;
            do
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    attempts++;
                    return await task();
                }
                catch (TException exception)
                {
                    bool retryAllowed = attempts != retries;
                    if (retryAllowed && customRetryAllowance != null)
                    {
                        retryAllowed = customRetryAllowance(exception);
                    }

                    string opName = string.IsNullOrEmpty(operationName) ? string.Empty : $"{operationName} ";
                    string message = $"{opName}Exception on attempt {attempts}/{retries}." +
                    (retryAllowed
                        ? $" Task will be retried after delay for {retriesDelay.Milliseconds}ms."
                        : " No more retries allowed.");

                    DebugUtils.Warning($"{message}, {exception.GetFullExceptionDetails()}");

                    if (cancellationToken.IsCancellationRequested)
                    {
                        retryAllowed = false;
                    }

                    if (retryAllowed)
                    {
                        await Task.Delay(retriesDelay, cancellationToken);
                    }
                    else
                    {
                        throw;
                    }
                }
            } while (true);
        }
    }
}
