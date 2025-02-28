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
using LHQ.App.Services.Interfaces;
using LHQ.Core.Interfaces;
using LHQ.Core.Model;
using LHQ.Utils.Extensions;

namespace LHQ.App.Code
{
    public abstract class AsyncOperation : IAsyncOperation
    {
        private bool _internalCompletedCalled;

        protected AsyncOperation(IAppContext appContext)
        {
            AppContext = appContext;
            Result = new OperationResult(OperationResultStatus.Failed);
            State = AsyncOperationState.NotStarted;
            CancellationTokenSource = new CancellationTokenSource(OperationTimeout);
            _internalCompletedCalled = false;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        protected CancellationTokenSource CancellationTokenSource { get; }

        protected IAppContext AppContext { get; }

        protected ILogger Logger => AppContext.Logger;

        // ReSharper disable once MemberCanBePrivate.Global
        protected bool IsRunning => State == AsyncOperationState.Running;

        public CancellationToken CancellationToken => CancellationTokenSource.Token;

        public OperationResult Result { get; }

        public bool UserCancelled { get; private set; }

        public AsyncOperationState State { get; private set; }

        public bool CanBeCancelled => IsRunning &&
            !UserCancelled &&
            CancellationToken.CanBeCanceled &&
            !CancellationToken.IsCancellationRequested;

        protected bool IsCancellationRequested => UserCancelled || CancellationToken.IsCancellationRequested;

        public TimeSpan OperationTimeout => GetOperationTimeout();

        protected virtual TimeSpan GetOperationTimeout() => TimeSpan.FromMinutes(1);

        public virtual Task BeforeStart()
        {
            return Task.CompletedTask;
        }

        public virtual async Task Start()
        {
            if (State != AsyncOperationState.NotStarted)
            {
                throw new InvalidOperationException($"Async operation '{GetType().Name}' already started or completed!");
            }

            State = AsyncOperationState.Running;
            try
            {
                await InternalStart();
            }
            catch (Exception e)
            {
                DebugUtils.Error($"{GetType().Namespace}.InternalStart() failed", e);
            }
            finally
            {
                State = AsyncOperationState.Completed;
            }
        }

        protected abstract Task InternalStart();

        protected abstract Task InternalCompleted(bool userCancelled);

        public abstract string ProgressMessage { get; }

        public abstract bool ShowCancelButton { get; }

        public Task Completed(bool userCancelled)
        {
            if (State != AsyncOperationState.Completed)
            {
                State = AsyncOperationState.Completed;
            }

            if (!_internalCompletedCalled)
            {
                UserCancelled = userCancelled;
                _internalCompletedCalled = true;
                return InternalCompleted(userCancelled);
            }

            return Task.CompletedTask;
        }

        public void Cancel(bool userCancelled)
        {
            if (CanBeCancelled)
            {
                UserCancelled = userCancelled;
                CancellationTokenSource.Cancel();
            }
        }
    }
}
