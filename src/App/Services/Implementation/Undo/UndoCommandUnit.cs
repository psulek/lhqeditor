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

namespace LHQ.App.Services.Implementation.Undo
{
    public abstract class UndoCommandUnit : IUndoCommand
    {
        private Func<string> _getNameFunc;

        protected UndoCommandUnit(string uid)
        {
            UID = uid;
            Do = InternalDo;
            Undo = InternalUndo;
            Completed = InternalCompleted;
        }
        
        public Action Do { get; set; }

        public Action Undo { get; set; }

        public Func<UndoCommandKind, UndoCommandCompleted> Completed { get; set; }

        public IUndoManager Manager { get; set; }

        public string UID { get; }

        public string GetName()
        {
            return _getNameFunc();
        }

        public void Reuse(IUndoCommand otherCommand)
        { }

        protected abstract void InternalDo();
        protected abstract void InternalUndo();
        protected abstract UndoCommandCompleted InternalCompleted(UndoCommandKind undoKind);

        protected void InternalUpdateGetName(Func<string> getNameFunc)
        {
            _getNameFunc = getNameFunc;
        }
    }
}
