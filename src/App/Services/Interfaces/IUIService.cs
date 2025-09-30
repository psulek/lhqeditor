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
using System.Globalization;
using System.Windows.Threading;
using LHQ.App.Model;
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.App.Services.Interfaces
{
    public interface IUIService : IAppContextService
    {
        UILanguage CurrentUiLanguage { get; }

        CultureInfo CurrentUiCulture { get; }

        IEnumerable<UILanguage> UiLanguages { get; }

        CultureInfo DefaultCulture { get; }

        int UiThreadId { get; }

        int CurrentThreadId { get; }

        Dispatcher UiThreadDispatcher { get; }

        /// <summary>
        /// Changes the UI language.
        /// </summary>
        /// <param name="cultureName">Name of the culture.</param>
        /// <returns>
        /// Returns <c>true</c> when app config save is required after language change.
        /// </returns>
        bool ChangeUiLanguage(string cultureName);
        UILanguage GetUiLanguage(string cultureName);

        void DelayedAction(Action action, TimeSpan? delayTimeout = null);
        void DispatchActionOnUIAsync(Action action, Action onComplete = null);
        void DispatchActionOnUI(Action action, TimeSpan? delayTimeout = null, DispatcherPriority priority = DispatcherPriority.Normal);
        void DelayAdvancedAction(Action beforeAction, Action action, Action afterAction);
        void DelayAdvancedAction(Action beforeAction, Action action, Action afterAction, TimeSpan? delay);
        IDispatchActionQueue CreateDispatchActionQueue();

        void RegisterSubscriber(IUiLanguageChangeSubscriber subscriber);
        void UnregisterSubscriber(IUiLanguageChangeSubscriber subscriber);
        void UnregisterSubscribers(IEnumerable<IUiLanguageChangeSubscriber> subscribers);
    }
}
