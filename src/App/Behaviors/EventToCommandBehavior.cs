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
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace LHQ.App.Behaviors
{
    /// <summary>
    ///     Behavior that will connect an UI event to a viewmodel Command,
    ///     allowing the event arguments to be passed as the CommandParameter.
    /// </summary>
    /// <remarks>
    /// Code from http://stackoverflow.com/a/16317999/316886.
    /// </remarks>
    public class EventToCommandBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty EventProperty =
            DependencyProperty.Register(nameof(Event), typeof(string), typeof(EventToCommandBehavior), new PropertyMetadata(null, OnEventChanged));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(EventToCommandBehavior), new PropertyMetadata(null));

        public static readonly DependencyProperty PassArgumentsProperty =
            DependencyProperty.Register(nameof(PassArguments), typeof(bool), typeof(EventToCommandBehavior), new PropertyMetadata(false));

        public static readonly DependencyProperty PropagateEventFromChildsProperty =
            DependencyProperty.Register(nameof(PropagateEventFromChilds), typeof(bool), typeof(EventToCommandBehavior), new PropertyMetadata(false));

        private Delegate _handler;
        private EventInfo _oldEvent;

        // TODO: update comments to xml
        // Event
        public string Event
        {
            get => (string)GetValue(EventProperty);
            set => SetValue(EventProperty, value);
        }

        // Command
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        // PassArguments (default: false)
        public bool PassArguments
        {
            get => (bool)GetValue(PassArgumentsProperty);
            set => SetValue(PassArgumentsProperty, value);
        }

        public bool PropagateEventFromChilds
        {
            get => (bool)GetValue(PropagateEventFromChildsProperty);
            set => SetValue(PropagateEventFromChildsProperty, value);
        }

        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var beh = (EventToCommandBehavior)d;

            if (beh.AssociatedObject != null) // is not yet attached at initial load
            {
                beh.AttachHandler((string)e.NewValue);
            }
        }

        protected override void OnAttached()
        {
            AttachHandler(Event); // initial set
        }

        /// <summary>
        ///     Attaches the handler to the event
        /// </summary>
        private void AttachHandler(string eventName)
        {
            // detach old event
            if (_oldEvent != null)
            {
                _oldEvent.RemoveEventHandler(AssociatedObject, _handler);
            }

            // attach new event
            if (!string.IsNullOrEmpty(eventName))
            {
                EventInfo ei = AssociatedObject.GetType().GetEvent(eventName);
                if (ei != null)
                {
                    MethodInfo mi = GetType().GetMethod("ExecuteCommand", BindingFlags.Instance | BindingFlags.NonPublic);
                    _handler = Delegate.CreateDelegate(ei.EventHandlerType, this, mi);
                    ei.AddEventHandler(AssociatedObject, _handler);
                    _oldEvent = ei; // store to detach in case the Event property changes
                }
                else
                {
                    throw new ArgumentException($"The event '{eventName}' was not found on type '{AssociatedObject.GetType().Name}'");
                }
            }
        }

        /// <summary>
        ///     Executes the Command
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        private void ExecuteCommand(object sender, EventArgs e)
        {
            object parameter = PassArguments ? e : null;
            var routedEventArgs = e as RoutedEventArgs;
            var validSender = PropagateEventFromChilds || routedEventArgs == null || routedEventArgs.OriginalSource == sender;

            if (Command != null && validSender)
            {
                if (Command.CanExecute(parameter))
                {
                    Command.Execute(parameter);
                }
            }
        }
    }
}
