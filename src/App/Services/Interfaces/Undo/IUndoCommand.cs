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

// ReSharper disable UnusedMemberInSuper.Global

namespace LHQ.App.Services.Interfaces.Undo
{
    public interface IUndoCommand
    {
        Action Do { get; set; }

        Action Undo { get; set; }

        Func<UndoCommandKind, UndoCommandCompleted> Completed { get; set; }

        IUndoManager Manager { get; set; }

        string UID { get; }

        string GetName();

        void Reuse(IUndoCommand otherCommand);
    }

    public enum UndoCommandKind
    {
        Do,
        Undo
    }

    public class UndoCommandCompleted
    {
        public static readonly UndoCommandCompleted Default = new UndoCommandCompleted(true);

        public UndoCommandCompleted(bool allowedReverseAction)
        {
            AllowedReverseAction = allowedReverseAction;
        }

        /// <summary>
        /// With value <c>true</c>, after executing 'Undo' action, this action is added to 'redo' actions.
        /// With value <c>false</c>, after executing 'Undo' action, this action is NOT added to 'redo' actions.
        /// </summary>
        public bool AllowedReverseAction { get; }
    }
}
