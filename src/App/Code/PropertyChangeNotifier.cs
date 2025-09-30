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
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.App.Code
{
    /// <summary>
    ///     PropertyChangeNotifier class implementation.
    /// </summary>
    public sealed class PropertyChangeNotifier : DependencyObject, IDisposable
    {
        /// <summary>
        ///     Identifies the <see cref="SourceValue" /> dependency property
        /// </summary>
        public static readonly DependencyProperty SourceValueProperty = DependencyProperty.Register(
            "SourceValue",
            typeof(object),
            typeof(PropertyChangeNotifier),
            new FrameworkPropertyMetadata(null, OnPropertyChanged));

        private readonly WeakReference _propertySource;

        /// <summary>
        ///     Initializes a new instance of the PropertyChangeNotifier class.
        /// </summary>
        /// <param name="propertySource">The source dependency property.</param>
        /// <param name="path">The name of the event of the property.</param>
        public PropertyChangeNotifier(DependencyObject propertySource, string path)
            : this(propertySource, new PropertyPath(path))
        { }

        /// <summary>
        ///     Initializes a new instance of the PropertyChangeNotifier class.
        /// </summary>
        /// <param name="propertySource">The source dependency property.</param>
        /// <param name="property">The event of the property.</param>
        public PropertyChangeNotifier(DependencyObject propertySource, DependencyProperty property)
            : this(propertySource, new PropertyPath(property))
        { }

        /// <summary>
        ///     Initializes a new instance of the PropertyChangeNotifier class.
        /// </summary>
        /// <param name="propertySource">The source dependency property.</param>
        /// <param name="property">The path to the event of the property.</param>
        public PropertyChangeNotifier(DependencyObject propertySource, PropertyPath property)
        {
            if (null == propertySource)
            {
                throw new ArgumentNullException(nameof(propertySource));
            }

            if (null == property)
            {
                throw new ArgumentNullException(nameof(property));
            }

            _propertySource = new WeakReference(propertySource);
            var binding = new Binding
            {
                Path = property,
                Mode = BindingMode.OneWay,
                Source = propertySource
            };

            BindingOperations.SetBinding(this, SourceValueProperty, binding);
        }

        /// <summary>
        ///     Gets the source object of the event.
        /// </summary>
        public DependencyObject PropertySource
        {
            get
            {
                try
                {
                    // note, it is possible that accessing the target property
                    // will result in an exception so i’ve wrapped this check
                    // in a try catch
                    return _propertySource.IsAlive ? _propertySource.Target as DependencyObject : null;
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the value of the property
        /// </summary>
        /// <seealso cref="SourceValueProperty" />
        [Description("Returns/sets the value of the property")]
        [Category("Behavior")]
        [Bindable(true)]
        public object SourceValue
        {
            get => GetValue(SourceValueProperty);

            set => SetValue(SourceValueProperty, value);
        }

        /// <summary>
        ///     Finalizes an instance of the PropertyChangeNotifier class.
        /// </summary>
        ~PropertyChangeNotifier()
        {
            Dispose(false);
        }

        /// <summary>
        ///     An event that fires when the value changes.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        ///     Disposes of the managed and unmanaged resources used in this class and calls the garbage collector.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var notifier = (PropertyChangeNotifier)d;
            notifier.ValueChanged?.Invoke(notifier, EventArgs.Empty);
        }

        /// <summary>
        ///     The bulk of the clean-up code is done in this method.
        /// </summary>
        /// <param name="disposing">A parameter indicating whether we are disposing of the managed resources.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                BindingOperations.ClearBinding(this, SourceValueProperty);
            }
        }
    }
}
