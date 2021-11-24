﻿#region License
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

using System.Collections.Generic;
using LHQ.App.Model;
using LHQ.App.Model.Messaging;

// ReSharper disable UnusedMember.Global

namespace LHQ.App.Services.Implementation.Messaging
{
    public static class MessengerDefinitions
    {
        public static readonly IMessengerData ResetTree = new MessengerData("ResetTree");
        public static readonly IMessengerData FocusTree = new MessengerData("FocusTree");
        public static readonly IMessengerData FocusSearchPanel = new MessengerData("FocusSearchPanel");
        public static readonly IMessengerData FocusPrimaryResourceValue = new MessengerData("FocusPrimaryResourceValue");
        
        public static IMessengerData FocusAutoSelectName(ITreeElementViewModel element)
        {
            return new MessengerDataFocusAutoSelectName(element);
        }

        public static IMessengerData TreeNodeScrollIntoView(ITreeElementViewModel element)
        {
            return new MessengerDataTreeNodeScrollIntoView(element);
        }

        public static IMessengerData SelectNodeInTree(ITreeElementViewModel element)
        {
            return new MessengerDataTreeSelection(MessengerDataTreeSelection.Mode.Select, new[] { element });
        }

        public static IMessengerData SelectNodesInTree(IEnumerable<ITreeElementViewModel> elements)
        {
            return new MessengerDataTreeSelection(MessengerDataTreeSelection.Mode.Select, elements);
        }

        public static IMessengerData DeSelectNodeInTree(ITreeElementViewModel element)
        {
            return new MessengerDataTreeSelection(MessengerDataTreeSelection.Mode.DeSelect, new[] { element });
        }

        public static IMessengerData DeSelectNodesInTree(IEnumerable<ITreeElementViewModel> elements)
        {
            return new MessengerDataTreeSelection(MessengerDataTreeSelection.Mode.DeSelect, elements);
        }

        public static IMessengerData ReSelectNodeInTree(ITreeElementViewModel element)
        {
            return new MessengerDataTreeSelection(MessengerDataTreeSelection.Mode.ReSelect, new[] { element });
        }

        public static IMessengerData ReSelectNodesInTree(IEnumerable<ITreeElementViewModel> elements)
        {
            return new MessengerDataTreeSelection(MessengerDataTreeSelection.Mode.ReSelect, elements);
        }
    }
}
