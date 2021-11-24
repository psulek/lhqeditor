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
using LHQ.App.Services.Implementation.Undo;
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.App.Services.Interfaces.Undo
{
    public interface IUndoManager: IShellViewContextService
    {
        bool CanUndo { get; }

        bool CanRedo { get; }

        bool CanUndoOrRedo { get; }

        event EventHandler<UndoActionArgs> Changed;

        void AddCommand(IUndoCommand command);

        void Undo();

        void Redo();

        void Reset();

        void UndoAll(bool keepRedo);

        IUndoCommand[] GetUndoCommands();

        IUndoCommand[] GetRedoCommands();

        IUndoCommand PeekUndo();

        IUndoCommand PeekRedo();

        IUndoManager CreateEmptyManager();

        void Remove(IUndoCommand command);

        void ReuseCommand(IUndoCommand command);

        void RemoveAll(Func<IUndoCommand, bool> predicate);
    }
}
