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
using System.Linq;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.Core.DependencyInjection;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation.Undo
{
    public class UndoManager : ShellViewContextServiceBase, IUndoManager
    {
        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        private readonly StackExt<IUndoCommand> _undoStack = new StackExt<IUndoCommand>();
        private readonly StackExt<IUndoCommand> _redoStack = new StackExt<IUndoCommand>();

        public bool CanUndo => _undoStack.Count > 0;

        public bool CanRedo => _redoStack.Count > 0;

        public bool CanUndoOrRedo => CanUndo || CanRedo;

        public event EventHandler<UndoActionArgs> Changed;

        public virtual void AddCommand(IUndoCommand command)
        {
            ArgumentValidator.EnsureArgumentNotNull(command, "command");

            ArgumentValidator.EnsureArgumentNotNull(command.Do, "command.Do");
            ArgumentValidator.EnsureArgumentNotNull(command.Undo, "command.Undo");

            if (command.Manager != null)
            {
                throw new InvalidOperationException(
                    "Undo command '{0}' already belongs to another undo manager!".FormatWith(command.GetName()));
            }

            command.Manager = this;

            // perform action for first time
            if (InvokeDo(command))
            {
                //save command on undo stack
                _undoStack.Push(command);
            }

            // notify about removing commands from redo stack before clearing redo stack
            OnRemovedCommands(_redoStack);
            // clear current redo stack
            _redoStack.Clear();

            OnChanged(UndoAction.AddCommand, command);
        }

        public void ReuseCommand(IUndoCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.Manager == null)
            {
                throw new InvalidOperationException(
                    "Undo command '{0}' does not belongs to any undo manager!".FormatWith(command.GetName()));
            }

            InvokeDo(command);

            OnChanged(UndoAction.ReusedCommand, command);
        }

        private bool InvokeDo(IUndoCommand command)
        {
            command.Do();

            return InvokeCompleted(command, UndoCommandKind.Do);
        }

        private bool InvokeUndo(IUndoCommand command)
        {
            command.Undo();

            return InvokeCompleted(command, UndoCommandKind.Undo);
        }

        private bool InvokeCompleted(IUndoCommand command, UndoCommandKind kind)
        {
            UndoCommandCompleted undoCommandCompleted = command.Completed?.Invoke(kind);
            return undoCommandCompleted == null || undoCommandCompleted.AllowedReverseAction;
        }

        // ReSharper disable once UnusedParameter.Local
        private void OnRemovedCommands(IEnumerable<IUndoCommand> commands)
        { }

        public void Reset()
        {
            OnRemovedCommands(_undoStack.Concat(_redoStack));

            _undoStack.Clear();
            _redoStack.Clear();

            OnChanged(UndoAction.Reset, (IUndoCommand[])null);
        }

        public IUndoManager CreateEmptyManager()
        {
            return new UndoManager();
        }

        public void Undo()
        {
            IUndoCommand command = _undoStack.Pop();
            if (InvokeUndo(command))
            {
                _redoStack.Push(command);
            }

            OnChanged(UndoAction.Undo, command);
        }

        public void Redo()
        {
            IUndoCommand command = _redoStack.Pop();
            if (InvokeDo(command))
            {
                _undoStack.Push(command);
            }

            OnChanged(UndoAction.Redo, command);
        }

        public void UndoAll(bool keepRedo)
        {
            IUndoCommand[] commands = _undoStack.ToArray();
            _undoStack.Clear();
            if (commands.Length > 0)
            {
                if (!keepRedo)
                {
                    OnRemovedCommands(commands);
                }

                var commandsAllowToReverse = new Dictionary<IUndoCommand, bool>();

                foreach (IUndoCommand command in commands)
                {
                    bool allowToReverse = InvokeUndo(command);
                    commandsAllowToReverse.Add(command, allowToReverse);
                }

                if (keepRedo)
                {
                    OnRemovedCommands(_redoStack.Except(commands));

                    _redoStack.Clear();
                    foreach (IUndoCommand command in commands)
                    {
                        var allowToReverse = commandsAllowToReverse[command];
                        if (allowToReverse)
                        {
                            _redoStack.Push(command);
                        }
                    }
                }
            }

            OnChanged(UndoAction.UndoAll, commands);
        }

        public IUndoCommand[] GetUndoCommands()
        {
            return _undoStack.ToArray();
        }

        public IUndoCommand[] GetRedoCommands()
        {
            return _redoStack.ToArray();
        }

        public IUndoCommand PeekUndo()
        {
            return _undoStack.Count > 0 ? _undoStack.First.Value : null;
        }

        public IUndoCommand PeekRedo()
        {
            return _redoStack.Count > 0 ? _redoStack.First.Value : null;
        }

        public void Remove(IUndoCommand command)
        {
            bool isInUndo = _undoStack.Contains(command);
            bool isInRedo = _redoStack.Contains(command);

            if (isInUndo || isInRedo)
            {
                OnRemovedCommands(new[] { command });
            }

            if (isInUndo)
            {
                _undoStack.Remove(command);
            }

            if (isInRedo)
            {
                _redoStack.Remove(command);
            }
        }

        public void RemoveAll(Func<IUndoCommand, bool> predicate)
        {
            IEnumerable<IUndoCommand> undoCommandsRemoved = _undoStack.RemoveAll(predicate);
            IEnumerable<IUndoCommand> redoCommandsRemoved = _redoStack.RemoveAll(predicate);

            OnRemovedCommands(undoCommandsRemoved.Concat(undoCommandsRemoved.Except(redoCommandsRemoved)));
        }

        public override string ToString()
        {
            return "UndoStack:{0}, RedoStack:{1}".FormatWith(_undoStack.Count, _redoStack.Count);
        }

        private void OnChanged(UndoAction undoAction, IUndoCommand command)
        {
            ArgumentValidator.EnsureArgumentNotNull(command, "command");

            OnChanged(undoAction, new[]
            {
                command
            });
        }

        private void OnChanged(UndoAction undoAction, IUndoCommand[] commands)
        {
            EventHandler<UndoActionArgs> handler = Changed;
            if (handler != null)
            {
                try
                {
                    handler(this, new UndoActionArgs(undoAction, commands));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetFullExceptionDetails());
                }
            }
        }
    }
}
