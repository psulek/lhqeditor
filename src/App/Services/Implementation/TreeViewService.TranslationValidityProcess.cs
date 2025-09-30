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
using System.Windows.Threading;
using LHQ.App.Code;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Elements;
using LHQ.Utils;

namespace LHQ.App.Services.Implementation
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public partial class TreeViewService
    {
        private class TranslationStateValidityProcess
        {
            private readonly IShellViewContext _shellViewContext;
            private readonly ITreeViewService _treeViewService;
            private readonly IPropertyChangeObserver _propertyChangeObserver;
            private readonly DispatcherTimer _timer;

            private readonly TimeSpan _timerInterval = TimeSpan.FromMilliseconds(250);
            private readonly TimeSpan _threesholdInterval = TimeSpan.FromMilliseconds(500);
            private DateTime? _lastNotifyDate;
            private Guid? _lastNotifyUID;
            private Guid? _lastProcessedUID;
            private bool _forceRecalculateNextIteration;
            private bool _recalculatingInProgress;

            public TranslationStateValidityProcess(IShellViewContext shellViewContext)
            {
                _shellViewContext = shellViewContext;
                _lastNotifyDate = null;
                _lastNotifyUID = null;
                _lastProcessedUID = null;
                _forceRecalculateNextIteration = false;
                _recalculatingInProgress = false;
                Dispatcher uiThreadDispatcher = shellViewContext.AppContext.UIService.UiThreadDispatcher;
                _treeViewService = shellViewContext.TreeViewService;
                _propertyChangeObserver = shellViewContext.PropertyChangeObserver;
                _timer = new DispatcherTimer(_timerInterval, DispatcherPriority.Normal, Process, uiThreadDispatcher);
                _timer.Start();
            }

            private void RecalculateValidity()
            {
                if (_treeViewService.IsTempraryBlocked(TemporaryBlockType.RefreshTranslationValidity))
                {
                    _forceRecalculateNextIteration = true;
                    return;
                }

                if (_recalculatingInProgress)
                {
                    return;
                }

                _forceRecalculateNextIteration = false;
                _recalculatingInProgress = true;
                Guid? processingUID = _lastNotifyUID;

                try
                {
                    ProcessCategory(_treeViewService.RootModel);

                    void ProcessCategory(ICategoryLikeViewModel category)
                    {
                        List<KeyValue<ResourceViewModel, bool?>> resources = null;
                        foreach (ITreeElementViewModel child in category.Children.OrderByElementType(true))
                        {
                            if (child.ElementType == TreeElementType.Resource)
                            {
                                if (resources == null)
                                {
                                    resources = new List<KeyValue<ResourceViewModel, bool?>>();
                                }
                                resources.Add(KeyValue.Create((ResourceViewModel)child, (bool?)null));
                            }
                            else // if category has another child category, stop processing, because we need to process lowest-child categories of tree!
                            {
                                ProcessCategory((ICategoryLikeViewModel)child);
                            }
                        }

                        if (resources != null)
                        {
                            var hasSomeChange = false;
                            foreach (KeyValue<ResourceViewModel, bool?> resourcePair in resources)
                            {
                                ResourceViewModel resource = resourcePair.Key;
                                bool translationValidForAllValues = resource.Values.Count == 0 ||
                                    resource.Values.All(x => x.IsTranslationValid);

                                // when property 'IsTranslationValid' on resource is same as new calculated 'translationValidForAllValues'
                                // do not change/notify about it
                                resourcePair.Value = resource.IsTranslationValid != translationValidForAllValues
                                    ? translationValidForAllValues
                                    : (bool?)null;

                                if (resourcePair.Value != null && !hasSomeChange)
                                {
                                    hasSomeChange = true;
                                }
                            }

                            if (hasSomeChange)
                            {
                                List<KeyValue<ResourceViewModel, bool?>> changedResources = resources.Where(x => x.Value.HasValue).ToList();
                                using (_propertyChangeObserver.BlockNotifications(changedResources.Select(x => x.Key), true))
                                {
                                    foreach (KeyValue<ResourceViewModel, bool?> pair in changedResources)
                                    {
                                        ResourceViewModel resource = pair.Key;
                                        resource.IsTranslationValid = pair.Value.GetValueOrDefault();
                                    }
                                }
                            }
                        }

                        bool categoryResourcesHasValidTranslation = category.Children.All(x => x.IsTranslationValid);
                        if (category.IsTranslationValid != categoryResourcesHasValidTranslation)
                        {
                            category.IsTranslationValid = categoryResourcesHasValidTranslation;
                        }
                    }
                }
                finally
                {
                    _lastProcessedUID = processingUID;
                    _recalculatingInProgress = false;
                }
            }

            private void Process(object sender, EventArgs e)
            {
                if (_lastNotifyDate != null)
                {
                    TimeSpan delta = DateTimeHelper.UtcNow - _lastNotifyDate.Value;
                    bool allowed = delta >= _threesholdInterval || _forceRecalculateNextIteration;
                    if (allowed && !_forceRecalculateNextIteration)
                    {
                        allowed = _lastNotifyUID == null && _lastProcessedUID == null || _lastNotifyUID != _lastProcessedUID;
                    }

                    if (allowed)
                    {
                        RecalculateValidity();
                    }
                }
            }

            public void Notify()
            {
                _lastNotifyUID = Guid.NewGuid();
                _lastNotifyDate = DateTimeHelper.UtcNow;
            }
        }
    }
}
