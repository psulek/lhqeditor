#region License
// Copyright (c) 2021 Peter Šulek / ScaleHQ Solutions s.r.o.
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
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.Core.DependencyInjection;
using LHQ.Utils.Extensions;

namespace LHQ.App.Services.Implementation
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public sealed class TransactionManager : ShellViewContextServiceBase, ITransactionManager
    {
        private int _blockedUndoCount;

        public TransactionManager()
        {
            _blockedUndoCount = 0;
        }

        public bool IsBlockedUndoRedo => _blockedUndoCount > 0;

        private IUndoManager UndoManager => ShellViewContext.UndoManager;

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        public IDisposable TemporaryBlockUndoManager()
        {
            return new BlockedUndoScope(this);
        }

        private void IncBlockedCount()
        {
            _blockedUndoCount++;
        }

        private void DecBlockedCount()
        {
            _blockedUndoCount--;
            if (_blockedUndoCount < 0)
            {
                throw new InvalidOperationException("blockedUndoCount is lower than 0 (value: {0})".FormatWith(_blockedUndoCount));
            }
        }

        public void Execute(IUndoCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var commandReused = false;
            IUndoCommand lastUndoCommand = UndoManager.PeekUndo();
            if (lastUndoCommand != null && lastUndoCommand.UID == command.UID)
            {
                // if last undo command was same as incomming one (UID is compared) than update existing IUndoCommand's 'NewValue' property instead
                // and throw away incomming 'command'

                lastUndoCommand.Reuse(command);
                commandReused = true;
            }

            if (IsBlockedUndoRedo)
            {
                if (command.Manager == null)
                {
                    command.Manager = UndoManager;
                }
                command.Do();
            }
            else
            {
                if (commandReused)
                {
                    UndoManager.ReuseCommand(lastUndoCommand);
                }
                else
                {
                    UndoManager.AddCommand(command);
                }
            }
        }

        private class BlockedUndoScope : IDisposable
        {
            private readonly TransactionManager _owner;
            private bool _disposed;

            public BlockedUndoScope(TransactionManager owner)
            {
                _owner = owner;
                owner.IncBlockedCount();
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _owner.DecBlockedCount();
                    _disposed = true;
                }
            }
        }
    }
}
