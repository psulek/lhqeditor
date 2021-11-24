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
using System.Collections.Concurrent;
using LHQ.App.Services.Interfaces;

namespace LHQ.App.Services.Implementation
{
    internal class DispatchActionQueue : IDispatchActionQueue
    {
        private readonly UIService _uiService;
        private readonly BlockingCollection<Tuple<Action, TimeSpan?>> _queue;

        public DispatchActionQueue(UIService uiService)
        {
            _uiService = uiService;
            _queue = new BlockingCollection<Tuple<Action, TimeSpan?>>();
        }

        public IDispatchActionQueue Add(Action action, TimeSpan? timeout = null)
        {
            _queue.Add(Tuple.Create(action, timeout));
            return this;
        }

        public void Run()
        {
            _queue.CompleteAdding();

            foreach (Tuple<Action, TimeSpan?> action in _queue.GetConsumingEnumerable())
            {
                _uiService.DispatchActionOnUI(action.Item1, action.Item2);
            }
        }
    }
}
