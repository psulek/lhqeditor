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
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Core.DependencyInjection;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public sealed class UIService : AppContextServiceBase, IUIService
    {
        private static readonly TimeSpan _defaultDelayActionTimeout = TimeSpan.FromMilliseconds(100);

        private readonly List<UILanguage> _uiLanguages;

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        public UIService()
        {
            UiThreadDispatcher = Application.Current.Dispatcher;

            _uiLanguages = (from cultureName in StringsContext.Instance.AvailableCultures
                let culture = CultureCache.Instance.GetCulture(cultureName)
                select new UILanguage(culture.EnglishName, culture)).ToList();

            DefaultCulture =
                _uiLanguages.Single(x => x.Culture.Name == CultureCache.EnglishUS.Name || x.Culture.Name == CultureCache.English.Name)
                    .Culture;
        }

        public CultureInfo DefaultCulture { get; }

        public UILanguage CurrentUiLanguage { get; private set; }

        public CultureInfo CurrentUiCulture => CurrentUiLanguage.Culture;

        public IEnumerable<UILanguage> UiLanguages => _uiLanguages;

        public Dispatcher UiThreadDispatcher { get; }

        public int UiThreadId => UiThreadDispatcher.Thread.ManagedThreadId;

        public int CurrentThreadId => Thread.CurrentThread.ManagedThreadId;

        public void DelayAdvancedAction(Action beforeAction, Action action, Action afterAction)
        {
            DelayAdvancedAction(beforeAction, action, afterAction, TimeSpan.FromMilliseconds(100));
        }

        public void DelayAdvancedAction(Action beforeAction, Action action, Action afterAction, TimeSpan? delay)
        {
            UiThreadDispatcher.Invoke(beforeAction, DispatcherPriority.Send);

            DelayedAction(() =>
                {
                    UiThreadDispatcher.Invoke(() =>
                        {
                            try
                            {
                                action();
                            }
                            finally
                            {
                                UiThreadDispatcher.Invoke(afterAction, DispatcherPriority.Normal);
                            }
                        }, DispatcherPriority.Send);
                }, delay);
        }

        public bool ChangeUiLanguage(string cultureName)
        {
            var saveAppConfig = false;

            if (cultureName.IsNullOrEmpty())
            {
                cultureName = DefaultCulture.Name;
                saveAppConfig = true;
            }

            UILanguage uiLanguage = _uiLanguages.SingleOrDefault(x => x.Culture.Name == cultureName);

            CurrentUiLanguage = uiLanguage ??
                throw new InvalidOperationException($"Could not find culture '{cultureName}' in available localization languages!");
            StringsContext.Instance.Culture = uiLanguage.Culture;

            OnCurrentUiLanguageChanged(EventArgs.Empty);

            if (saveAppConfig)
            {
                AppConfig.UILanguage = uiLanguage.Culture.Name;
                //ApplicationService.SaveAppConfig();
            }

            return saveAppConfig;
        }

        public UILanguage GetUiLanguage(string cultureName)
        {
            return UiLanguages.SingleOrDefault(x => x.Culture.Name == cultureName);
        }

        public void RegisterSubscriber(IUiLanguageChangeSubscriber subscriber)
        {
            // TODO: Add later after initial launch!
            //            if (!uiSubscribers.Contains(subscriber))
            //            {
            //                uiSubscribers.Add(subscriber);
            //
            //                subscriber.OnRegistrationChanged(true);
            //                LoggerProvider.Instance.Debug("[UIService] Registered new subscriber, total count: " + uiSubscribers.Count);
            //            }
        }

        public void UnregisterSubscriber(IUiLanguageChangeSubscriber subscriber)
        {
            // TODO: Add later after initial launch!
            //            if (uiSubscribers.Contains(subscriber))
            //            {
            //                uiSubscribers.Remove(subscriber);
            //                subscriber.OnRegistrationChanged(false);
            //
            //                LoggerProvider.Instance.Debug("[UIService] Unregistered subscriber, total count: " + uiSubscribers.Count);
            //            }
        }

        public void UnregisterSubscribers(IEnumerable<IUiLanguageChangeSubscriber> subscribers)
        {
            // TODO: Add later after initial launch!
            //            foreach (var subscriber in subscribers)
            //            {
            //                uiSubscribers.Remove(subscriber);
            //            }
            //
            //            LoggerProvider.Instance.Debug("[UIService.UnregisterSubscribers] Unregistered subscribers, total count: " + uiSubscribers.Count);
        }

        public IDispatchActionQueue CreateDispatchActionQueue()
        {
            return new DispatchActionQueue(this);
        }

        public void DispatchActionOnUIAsync(Action action, Action onComplete = null)
        {
            UiThreadDispatcher.InvokeAsync(action, DispatcherPriority.Normal).Completed += (_, __) => onComplete?.Invoke();
        }

        public void DispatchActionOnUI(Action action, TimeSpan? delayTimeout = null, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (delayTimeout.HasValue)
            {
                DelayedAction(() => { DispatchActionOnUI(action); }, delayTimeout.Value);
            }
            else
            {
                UiThreadDispatcher.InvokeAsync(action, priority);
            }
        }

        public void DelayedAction(Action action, TimeSpan? delayTimeout = null)
        {
            string timerId = Guid.NewGuid().ToString("N");
            TimeSpan timeout = delayTimeout.GetValueOrDefault(_defaultDelayActionTimeout);

            var dispatcherTimer = new DispatcherTimer(timeout, DispatcherPriority.Normal,
                (sender, args) =>
                    {
                        (sender as DispatcherTimer)?.Stop();

                        try
                        {
                            Logger.Debug("Executing action of dispatcher timer (id: {0})".FormatWith(timerId));
                            action();
                        }
                        catch (Exception e)
                        {
                            DebugUtils.Error($"DelayedAction({timerId}) error: " + e.GetFullExceptionDetails());
                        }
                    }, UiThreadDispatcher);

            dispatcherTimer.Start();
        }

        private void OnCurrentUiLanguageChanged(EventArgs e)
        {
            // TODO: Add later after initial launch!
            /*if (CurrentUiLanguageChanged != null)
            {
                CurrentUiLanguageChanged(this, e);
            }*/

            //            foreach (IUiLanguageChangeSubscriber subscriber in _uiSubscribers)
            //            {
            //                try
            //                {
            //                    subscriber.LanguageChanged();
            //                }
            //                catch (Exception exception)
            //                {
            //                    Logger.Debug("OnCurrentUiLanguageChanged, exc: " + exception.GetFullExceptionDetails());
            //                }
            //            }
        }
    }
}
