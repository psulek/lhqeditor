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
using System.Runtime.CompilerServices;
using LHQ.Utils.Extensions;

namespace LHQ.App.Model
{
    public abstract class ObservableObject : INotifyPropertyChanged, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;

            RaisePropertyChanged(propertyName);
            return true;
        }

        protected internal void RaiseObjectPropertiesChanged()
        {
            RaisePropertyChanged(null);
        }

        protected internal virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (IsDisposed)
            {
                DebugUtils.Log($"{GetType().Name}.NotifyPropertyChanged('{propertyName}') was already disposed!");
            }

            if (IsBlockedPropertyChange())
            {
                //DebugUtils.Log($"{GetType().Name}.NotifyPropertyChanged('{propertyName}') was blocked (by propertyChangeBlocker)");
            }
            else // FIX_RaisePropertyChanged
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            //else NOTE: this should be there !!
        }

        protected internal virtual bool IsBlockedPropertyChange()
        {
            return false;
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                OnDispose();
                IsDisposed = true;
            }
        }

        protected virtual void OnDispose()
        { }
    }
}
