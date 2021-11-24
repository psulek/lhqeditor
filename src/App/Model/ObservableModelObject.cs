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
using LHQ.App.Services.Interfaces;
using LHQ.Core.Interfaces;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.Model
{
    public abstract class ObservableModelObject : ObservableObject, IUiLanguageChangeSubscriber
    {
        private readonly ThreadSafeSingleShotGuard _propertyChangeBlocker = ThreadSafeSingleShotGuard.Create(false);
        private readonly bool _subscribeToLocalizationChange;

        protected ObservableModelObject(IAppContext appContext, bool subscribeToLocalizationChange = false)
        {
            AppContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _subscribeToLocalizationChange = subscribeToLocalizationChange;
            Initialize();
        }

        public IAppContext AppContext { get; }

        protected ILogger Logger => AppContext.Logger;

        protected IUIService UIService => AppContext.UIService;

        protected IDialogService DialogService => AppContext.DialogService;

        protected override void OnDispose()
        {
            if (UIService != null)
            {
                //DebugUtils.Log($"{GetType().Name}.Dispose(), UiThreadId: {UIService.UiThreadId}, CurrentThreadId: {UIService.CurrentThreadId}");
                UnsubscribeFromLocalizationChange();
            }
        }

        void IUiLanguageChangeSubscriber.LanguageChanged()
        {
            RaiseObjectPropertiesChanged();
            LocalizeStrings();
        }

        void IUiLanguageChangeSubscriber.OnRegistrationChanged(bool registered)
        {
            OnUISubscriberRegistrationChanged(registered);
        }

        private void Initialize()
        {
            if (_subscribeToLocalizationChange)
            {
                UIService.RegisterSubscriber(this);
            }
        }

        protected void UnsubscribeFromLocalizationChange()
        {
            UIService.UnregisterSubscriber(this);
        }

        protected virtual void OnUISubscriberRegistrationChanged(bool registered)
        { }

        protected void LocalizeStrings()
        {
            DoLocalizeStrings();
        }

        protected virtual void DoLocalizeStrings()
        { }

        protected internal override bool IsBlockedPropertyChange()
        {
            return _propertyChangeBlocker.IsSignalSet();
        }

        protected internal void BlockPropertyChangeNotification(bool block)
        {
            if (block)
            {
                _propertyChangeBlocker.SetSignal();
            }
            else
            {
                _propertyChangeBlocker.ResetSignal();
            }
        }
    }
}
