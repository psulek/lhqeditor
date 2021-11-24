#region License
// Copyright (c) 2021 Peter �ulek / ScaleHQ Solutions s.r.o.
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
using LHQ.App.Model;
using LHQ.App.Model.Messaging;

namespace LHQ.App.Services.Implementation.Messaging
{
    public sealed class MessengerDataTreeSelection : MessengerData<IEnumerable<ITreeElementViewModel>>
    {
        public enum Mode
        {
            Select,
            DeSelect,
            ReSelect,
        }

        private const string MessageKeySelectNodes = "TreeSelectNodes";
        private const string MessageKeyDeSelectNodes = "TreeDeSelectNodes";
        private const string MessageKeyReSelectNode = "TreeReSelectNode";

        public MessengerDataTreeSelection(Mode mode, IEnumerable<ITreeElementViewModel> elements)
            : base(GetMessageKey(mode), elements)
        {
            ActionMode = mode;
        }

        public Mode ActionMode { get; }

        private static string GetMessageKey(Mode mode)
        {
            switch (mode)
            {
                case Mode.Select:
                {
                    return MessageKeySelectNodes;
                }
                case Mode.DeSelect:
                {
                    return MessageKeyDeSelectNodes;
                }
                case Mode.ReSelect:
                {
                    return MessageKeyReSelectNode;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
            }
        }
    }
}
