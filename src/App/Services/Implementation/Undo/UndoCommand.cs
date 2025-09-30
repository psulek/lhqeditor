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
using LHQ.App.Services.Interfaces.Undo;
using LHQ.Utils.Utilities;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace LHQ.App.Services.Implementation.Undo
{
    public class UndoCommand : IUndoCommand
    {
        public UndoCommand(string uid)
        {
            UID = uid;
        }

        public UndoCommand(Func<string> getNameFunc, Action doAction, Action undoAction, string uid)
            : this(getNameFunc, doAction, undoAction, null, uid)
        { }

        public UndoCommand(Func<string> getNameFunc, Action doAction, Action undoAction, 
            Func<UndoCommandKind, UndoCommandCompleted> completed, string uid)
        {
            ArgumentValidator.EnsureArgumentNotNull(doAction, "doAction");
            ArgumentValidator.EnsureArgumentNotNull(undoAction, "undoAction");
            ArgumentValidator.EnsureArgumentNotNull(getNameFunc, "getNameFunc");

            Name = getNameFunc;
            Do = doAction;
            Undo = undoAction;
            Completed = completed;
            UID = uid;
        }

        public Func<string> Name { get; set; }

        public Action Do { get; set; }

        public Action Undo { get; set; }

        public Func<UndoCommandKind, UndoCommandCompleted> Completed { get; set; }

        public IUndoManager Manager { get; set; }

        public string UID { get; }

        public string GetName()
        {
            return Name();
        }

        public virtual void Reuse(IUndoCommand otherCommand)
        { }
    }
}
