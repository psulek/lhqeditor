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
using System.Reactive.Linq;
using System.Threading;

namespace LHQ.App.Code.Observable
{
    /// <summary>
    /// Helper methods for using observable <see cref="IProgress{T}"/> implementations. These are hot observables.
    /// </summary>
    public static class ObservableProgressHelper
    {
        /// <summary>
        /// Creates an observable and an <see cref="IProgress{T}"/> implementation that are linked together, suitable for UI consumption.
        /// When progress reports are sent to the <see cref="IProgress{T}"/> implementation, they are sampled according to <paramref name="sampleInterval"/> and then forwarded to the UI thread.
        /// This method must be called from the UI thread.
        /// </summary>
        /// <typeparam name="T">The type of progress reports.</typeparam>
        /// <param name="sampleInterval">How frequently progress reports are sent to the UI thread.</param>
        public static ObservableProgress<T> CreateForUi<T>(TimeSpan? sampleInterval = null)
        {
            var result = Create<T>();
            result.Observable = result.Observable.Sample(sampleInterval ?? TimeSpan.FromMilliseconds(100))
                .ObserveOn(SynchronizationContext.Current);
            return result;
        }

        /// <summary>
        /// Creates an observable and an <see cref="IProgress{T}"/> implementation that are linked together.
        /// When progress reports are sent to the <see cref="IProgress{T}"/> implementation, they are immediately and synchronously sent to the observable.
        /// </summary>
        /// <typeparam name="T">The type of progress reports.</typeparam>
        private static ObservableProgress<T> Create<T>()
        {
            var progress = new EventProgress<T>();
            var observable = System.Reactive.Linq.Observable.FromEvent<T>(handler => progress.OnReport += handler, handler => progress.OnReport -= handler);
            return new ObservableProgress<T>(observable, progress);
        }

        private sealed class EventProgress<T> : IProgress<T>
        {
            public event Action<T> OnReport;
            void IProgress<T>.Report(T value) => OnReport?.Invoke(value);
        }
    }
}
