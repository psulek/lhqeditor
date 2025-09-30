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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Core.DependencyInjection;
using LHQ.Utils;
using LHQ.Utils.Extensions;

namespace LHQ.App.Services.Implementation
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public sealed class ValidatorContext : ShellViewContextServiceBase, IValidatorContext, IHasInitialize
    {
        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(10);
        private readonly object _syncErrors = new object();
        private int _iterationCount;
        private bool _hasErrors;

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        public bool IsValidationDisabled => GetIsValidationDisabled();

        public ObservableCollectionExt<IValidationError> Errors { get; private set; }

        public bool HasErrors
        {
            get => _hasErrors;
            set
            {
                _hasErrors = value;
                RaisePropertyChanged();
            }
        }

        public void Initialize()
        {
            Errors = new ObservableCollectionExt<IValidationError>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IDisposable DisableValidators()
        {
            return new ValidatorDisabler(this);
        }

        private void UpdateHasErrors()
        {
            HasErrors = Errors.Count > 0;
        }

        public bool SetValidationError(object owner, string propertyName, string error,
            ValidationErrorLocalizationRefresh errorRefreshCallback, object data)
        {
            var changed = false;

            lock (_syncErrors)
            {
                IValidationError validationError = Errors.SingleOrDefault(x => x.Owner == owner && x.PropertyName.EqualsTo(propertyName));
                if (validationError == null)
                {
                    validationError = new ValidationError(owner, propertyName, error, errorRefreshCallback, data);
                    Errors.Add(validationError);
                    changed = true;
                }
                else
                {
                    if (data != validationError.Data || error != validationError.Error)
                    {
                        validationError.Data = data;
                        validationError.Error = error;
                        changed = true;
                    }
                }

                UpdateHasErrors();
            }

            return changed;
        }

        public bool RemoveValidationError(object owner, string propertyName)
        {
            var removed = false;

            lock (_syncErrors)
            {
                Errors.RemoveAll(x =>
                    {
                        bool match = x.Owner == owner && x.PropertyName.EqualsTo(propertyName);
                        if (!removed && match)
                        {
                            removed = true;
                        }

                        return match;
                    });

                UpdateHasErrors();
            }

            return removed;
        }

        public void RemoveValidationErrors(object owner)
        {
            lock (_syncErrors)
            {
                Errors.RemoveAll(x => x.Owner == owner);
                UpdateHasErrors();
            }
        }

        public void Clear()
        {
            lock (_syncErrors)
            {
                Errors.Clear();
                _iterationCount = 0;
                UpdateHasErrors();
            }
        }

        private bool GetIsValidationDisabled()
        {
            bool hasReadLock = _locker.TryEnterReadLock(_lockTimeout);
            if (hasReadLock)
            {
                try
                {
                    return _iterationCount > 0;
                }
                finally
                {
                    _locker.ExitReadLock();
                }
            }

            return _iterationCount > 0;
        }

        private void AddSubscriber()
        {
            bool hasWriteLock = _locker.TryEnterWriteLock(_lockTimeout);

            if (hasWriteLock)
            {
                try
                {
                    _iterationCount++;
                }
                finally
                {
                    _locker.ExitWriteLock();
                }
            }
        }

        private void RemoveSubscriber()
        {
            bool hasWriteLock = _locker.TryEnterWriteLock(_lockTimeout);

            if (hasWriteLock)
            {
                try
                {
                    _iterationCount--;

                    if (_iterationCount < 0)
                    {
                        _iterationCount = 0;
                    }
                }
                finally
                {
                    _locker.ExitWriteLock();
                }
            }
        }

        private sealed class ValidatorDisabler : IDisposable
        {
            private ValidatorContext _owner;

            public ValidatorDisabler(ValidatorContext owner)
            {
                owner.AddSubscriber();
            }

            public void Dispose()
            {
                if (_owner != null)
                {
                    _owner.RemoveSubscriber();
                    _owner = null;
                }
            }
        }
    }
}
