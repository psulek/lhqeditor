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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using LHQ.App.Model.Messaging;
using LHQ.App.Services.Interfaces;
using LHQ.Core.DependencyInjection;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using Syncfusion.Data.Extensions;

namespace LHQ.App.Services.Implementation.Messaging
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class Messenger : ShellViewContextServiceBase, IMessenger
    {
        private readonly object _syncRoot = new object();
        private readonly List<IMessageHandler> _subscribers = new List<IMessageHandler>();

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        public void Register(IMessageHandler handler)
        {
            ArgumentValidator.EnsureArgumentNotNull(handler, "action");
            lock (_syncRoot)
            {
                if (_subscribers.TrueForAll(x => x.HandlerName != handler.HandlerName))
                {
                    _subscribers.Add(handler);
                }
            }
        }

        public void Unregister(IMessageHandler handler)
        {
            ArgumentValidator.EnsureArgumentNotNull(handler, "action");

            lock (_syncRoot)
            {
                if (_subscribers.Contains(handler))
                {
                    _subscribers.Remove(handler);
                }
            }
        }

        public void SingleDispatch(params IMessengerData[] data)
        {
            Send(MessengerSendType.SingleDispatch, data);
        }

        public void MultiDispatch(params IMessengerData[] data)
        {
            Send(MessengerSendType.MultiDispatch, data);
        }

        private void Send(MessengerSendType sendType = MessengerSendType.SingleDispatch, params IMessengerData[] dataItems)
        {
            if (dataItems == null)
            {
                throw new ArgumentNullException(nameof(dataItems));
            }

            if (dataItems.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dataItems));
            }

            IMessageHandler[] messageHandlers;
            lock (_syncRoot)
            {
                messageHandlers = _subscribers.ToArray();
            }

#if DEBUG 
            string stackTrace = new StackTrace(true).ToString();
            dataItems.ForEach(x => x.StackTrace = stackTrace);
#endif
            foreach (IMessageHandler subscriber in messageHandlers)
            {
                if (sendType == MessengerSendType.SingleDispatch)
                {
                    AppContext.UIService.DispatchActionOnUI(() =>
                        {
                            foreach (IMessengerData data in dataItems)
                            {
                                subscriber.HandleMessage(data);
                            }
                        });
                }
                else
                {
                    var queue = AppContext.UIService.CreateDispatchActionQueue();
                    dataItems.Run(data => queue.Add(() => { subscriber.HandleMessage(data); }));
                    queue.Run();
                }
            }
        }
    }
}
