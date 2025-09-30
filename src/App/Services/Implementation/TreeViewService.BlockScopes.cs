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

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using LHQ.App.Services.Interfaces;

namespace LHQ.App.Services.Implementation
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public partial class TreeViewService
    {
        private abstract class BlockedScopeBase : IBlockedScope
        {
            private bool _disposed;

            public void Dispose()
            {
                if (!_disposed)
                {
                    InternalDispose();
                    _disposed = true;
                }
            }

            protected abstract void InternalDispose();
        }

        private class BlockedSelectionScope : BlockedScopeBase
        {
            private static int _counter;

            public BlockedSelectionScope()
            {
                Inc();
            }

            public static bool IsBlocked => _counter > 0;

            private static void Inc()
            {
                Interlocked.Increment(ref _counter);
            }

            private static void Dec()
            {
                Interlocked.Decrement(ref _counter);
            }

            protected override void InternalDispose()
            {
                Dec();
            }
        }

        private class BlockedRefreshTranslationValidityScope : BlockedScopeBase
        {
            private static int _counter;

            public BlockedRefreshTranslationValidityScope()
            {
                Inc();
            }

            public static bool IsBlocked => _counter > 0;

            private static void Inc()
            {
                Interlocked.Increment(ref _counter);
            }

            private static void Dec()
            {
                Interlocked.Decrement(ref _counter);
            }

            protected override void InternalDispose()
            {
                Dec();
            }
        }
    }
}
