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
using LHQ.App.Services.Interfaces.Undo;
using LHQ.Utils.Extensions;
using Microsoft.VisualStudio.OLE.Interop;

namespace LHQ.VsExtension.Undo
{
    /// <summary>
    /// A OleUndoUnit can be used as a IOleUndoUnit or just a undo step in 
    /// a transaction  
    /// </summary>
    public sealed class OleUndoUnit : IOleUndoUnit
    {
        private UndoUnitState _unitState;
        private readonly IUndoCommand _command;
        private readonly IUndoManager _undoManager;

        internal OleUndoUnit(IUndoManager undoManager, IUndoCommand command)
        {
            _undoManager = undoManager;
            _command = command;
            _unitState = UndoUnitState.Undoing;
            Clsid = Guid.Empty;
            IsStillAtTop = true;
            UndoName = command.GetName();
        }

        public string UndoName { get; }

        public Guid Clsid { get; }

        /// <summary>
        /// This property indicates whether the undo unit is at the top (most recently added to)
        /// the undo stack. This is useful to know when deciding whether undo units for operations
        /// like typing can be coallesced together.
        /// </summary>
        public bool IsStillAtTop { get; private set; }

        void IOleUndoUnit.Do(IOleUndoManager oleUndoManager)
        {
            try
            {
                if (_unitState == UndoUnitState.Undoing)
                {
                    if (_undoManager.PeekUndo() == _command)
                    {
                        _undoManager.Undo();
                    }
                }
                else
                {
                    if (_undoManager.PeekRedo() == _command)
                    {
                        _undoManager.Redo();
                    }
                }

                _unitState = _unitState == UndoUnitState.Undoing ? UndoUnitState.Redoing : UndoUnitState.Undoing;
                oleUndoManager?.Add(this);
            }
            catch (Exception e)
            {
                DebugUtils.Error("OleUndoUnit.Do() failed", e);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        void IOleUndoUnit.GetDescription(out string pBstr)
        {
            pBstr = _command.GetName();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        void IOleUndoUnit.GetUnitType(out Guid pClsid, out int plID)
        {
            pClsid = Clsid;
            plID = 0;
        }

        /// <summary>
        /// </summary>
        void IOleUndoUnit.OnNextAdd()
        {
            // We are no longer the top most undo unit; another one was added.
            IsStillAtTop = false;
        }
    }
}