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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Core.DependencyInjection;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public sealed class PropertyChangeObserver : ShellViewContextServiceBase, IPropertyChangeObserver
    {
        private static readonly IDisposable _noopObserver = new NoopObserverImpl();
        private Observer _currentObserver;
        private bool _blockForAll;

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        public bool IsBlockedPropertyChange(object obj)
        {
            ArgumentValidator.EnsureArgumentNotNull(obj, "obj");

            return _blockForAll || obj is ObservableModelObject observableObj && observableObj.IsBlockedPropertyChange();
        }

        public IDisposable BlockNotifications(ObservableModelObject obj, bool raiseNotifyChangeOnDispose)
        {
            ArgumentValidator.EnsureArgumentNotNull(obj, "obj");
            IDisposable obServer = BlockNotifications(new[]
            {
                obj
            }, raiseNotifyChangeOnDispose);

            return GetCurrentOrNew(obServer);
        }

        public IDisposable BlockNotifications(IEnumerable<ObservableModelObject> objects, bool raiseNotifyChangeOnDispose)
        {
            ArgumentValidator.EnsureArgumentNotNull(objects, "objects");
            ObservableModelObject[] modelObjects = objects.ToArray();
            ArgumentValidator.EnsureArgumentGreaterThanZero(modelObjects.Length, "objects.Length");

            return GetCurrentOrNew(new Observer(this, modelObjects, raiseNotifyChangeOnDispose));
        }

        public IDisposable BlockAllNotifications()
        {
            return GetCurrentOrNew(new Observer(this, false));
        }

        public void RaisePropertiesChanged(IEnumerable<ObservableModelObject> objects)
        {
            foreach (var obj in objects)
            {
                obj.RaiseObjectPropertiesChanged();
            }
        }

        public void RaisePropertiesChanged(ObservableModelObject obj)
        {
            obj.RaiseObjectPropertiesChanged();
        }

        public void RaisePropertyChanged(ObservableModelObject obj, string propertyName)
        {
            obj.RaisePropertyChanged(propertyName);
        }

        private IDisposable GetCurrentOrNew(IDisposable observer)
        {
            return _currentObserver != null && _blockForAll && !ReferenceEquals(_currentObserver, observer) ? _noopObserver : observer;
        }

        private class NoopObserverImpl : IDisposable
        {
            public void Dispose()
            { }
        }

        private class Observer : IDisposable
        {
            private readonly ObservableModelObject[] _objects;
            private readonly bool _blockAll;
            private PropertyChangeObserver _owner;
            private readonly bool _raiseNotifyChangeOnDispose;

            internal Observer(PropertyChangeObserver owner, bool raiseNotifyChangeOnDispose)
            {
                _raiseNotifyChangeOnDispose = raiseNotifyChangeOnDispose;
                _owner = owner;
                _owner._blockForAll = true;
                _blockAll = true;

                if (owner._currentObserver == null)
                {
                    owner._currentObserver = this;
                }
            }

            internal Observer(PropertyChangeObserver owner, ObservableModelObject[] objects, bool raiseNotifyChangeOnDispose)
            {
                _raiseNotifyChangeOnDispose = raiseNotifyChangeOnDispose;
                _owner = owner;
                _blockAll = false;

                if (objects != null && _owner._blockForAll == false)
                {
                    _objects = objects;
                    foreach (ObservableModelObject obj in objects)
                    {
                        obj.BlockPropertyChangeNotification(true);
                    }
                }
            }

            public void Dispose()
            {
                if (_owner != null)
                {
                    if (_owner._currentObserver == this)
                    {
                        _owner._currentObserver = null;
                    }

                    if (_blockAll)
                    {
                        if (_owner._blockForAll)
                        {
                            _owner._blockForAll = false;
                        }
                    }
                    else
                    {
                        foreach (ObservableModelObject obj in _objects)
                        {
                            obj.BlockPropertyChangeNotification(false);

                            // NOTE: Experimental, in case of FIX_RaisePropertyChanged
                            if (_raiseNotifyChangeOnDispose)
                            {
                                obj.RaiseObjectPropertiesChanged();
                            }
                        }
                    }

                    _owner = null;
                }
            }
        }
    }
}
