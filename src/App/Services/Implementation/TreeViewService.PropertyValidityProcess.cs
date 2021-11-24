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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Threading;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.Validators;
using LHQ.App.ViewModels.Elements;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public partial class TreeViewService
    {
        private class PropertyValidityProcess : IDisposable
        {
            private readonly IShellViewContext _shellViewContext;
            //private const string CategoryWithSameNameAlreadyExistUnder = "Category with name '{0}' already exist under same parent";
            //private const string ResourceWithSameNameAlreadyExistUnder = "Resource with name '{0}' already exist under same parent";

            private readonly ITreeViewService _treeViewService;
            private readonly IPropertyChangeObserver _propertyChangeObserver;
            private readonly IValidatorContext _validatorContext;
            private readonly Hashtable _elementVersions = Hashtable.Synchronized(new Hashtable());

            private readonly TimeSpan _timerInterval = TimeSpan.FromMilliseconds(250);
            private readonly TimeSpan _threesholdInterval = TimeSpan.FromMilliseconds(500);
            private readonly DispatcherTimer _timer;
            private readonly ThreadSafeSingleShotGuard _recalculatingInProgress;

            private DateTime? _lastNotifyDate;
            private Guid? _lastNotifyUID;
            private Guid? _lastProcessedUID;
            private bool _forceRecalculateNextIteration;
            private bool _disposed;

            private int _elementKeySize;

            public PropertyValidityProcess(IShellViewContext shellViewContext)
            {
                _shellViewContext = shellViewContext;
                _lastNotifyDate = null;
                _lastNotifyUID = null;
                _lastProcessedUID = null;
                _forceRecalculateNextIteration = false;
                _recalculatingInProgress = ThreadSafeSingleShotGuard.Create(false);
                _elementKeySize = -1;
                Dispatcher uiThreadDispatcher = shellViewContext.AppContext.UIService.UiThreadDispatcher;
                _treeViewService = shellViewContext.TreeViewService;
                _propertyChangeObserver = shellViewContext.PropertyChangeObserver;
                _validatorContext = shellViewContext.ValidatorContext;
                _timer = new DispatcherTimer(_timerInterval, DispatcherPriority.Normal, Process, uiThreadDispatcher);
                _timer.Start();
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
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
                        Recalculate();
                    }
                }
            }

            private bool Recalculate(bool respectDisabled = true)
            {
                if (_validatorContext.IsValidationDisabled && respectDisabled)
                {
                    _forceRecalculateNextIteration = true;
                    return false;
                }

                if (!_recalculatingInProgress.SetSignal())
                {
                    return false;
                }

                _forceRecalculateNextIteration = false;

                Guid? processingUID = _lastNotifyUID;

                try
                {
                    var changedElementValidations = new List<ITreeElementViewModel>();

                    ICategoryLikeViewModel lastSiblingsCategoryParent = null;
                    HashSet<string> lastSiblingCategories = null;

                    HashSet<string> GetSiblingCategories(CategoryViewModel currentCategory)
                    {
                        var currentCategoryParent = (ICategoryLikeViewModel)currentCategory.Parent;

                        if (lastSiblingsCategoryParent != currentCategoryParent)
                        {
                            IEnumerable<string> collection = currentCategoryParent.GetCategories().Select(x =>
                                {
                                    if (_elementKeySize == -1)
                                    {
                                        _elementKeySize = x.ElementKey.Length;
                                    }

                                    return x.ElementKey + x.Name.ToLowerInvariant();
                                });
                            lastSiblingCategories = new HashSet<string>(collection);
                            lastSiblingsCategoryParent = currentCategoryParent;
                        }

                        return lastSiblingCategories;
                    }

                    ICategoryLikeViewModel lastSiblingsResourceParent = null;
                    ILookup<string, ResourceViewModel> lastSiblingsResources = null;

                    ILookup<string, ResourceViewModel> GetSiblingResources(ICategoryLikeViewModel parent)
                    {
                        ICategoryLikeViewModel currentResourceParent = parent;
                        if (lastSiblingsResourceParent != currentResourceParent)
                        {
                            lastSiblingsResources = currentResourceParent.GetResources().ToLookup(x => x.Name.ToLowerInvariant());
                            lastSiblingsResourceParent = currentResourceParent;
                        }

                        return lastSiblingsResources;
                    }

                    using (_propertyChangeObserver.BlockAllNotifications())
                    {
                        _validatorContext.Clear();

                        ProcessCategory(_treeViewService.RootModel);
                    }

                    if (changedElementValidations.Count > 0)
                    {
                        changedElementValidations.Run(x =>
                            _propertyChangeObserver.RaisePropertiesChanged(x as ObservableModelObject));
                    }

                    if (_treeViewService.RootModel.SelectedTreeElements.Count == 1)
                    {
                        _propertyChangeObserver.RaisePropertiesChanged(_treeViewService.RootModel.SelectedTreeElement as ObservableModelObject);
                    }

                    void ProcessCategory(ICategoryLikeViewModel category)
                    {
                        if (HasChangedVersion(category))
                        {
                            bool changedValidation;
                            if (category.ElementType == TreeElementType.Model)
                            {
                                changedValidation = ValidateRootModel((RootModelViewModel)category);
                            }
                            else
                            {
                                changedValidation =
                                    ValidateCategory((CategoryViewModel)category, GetSiblingCategories);
                            }

                            if (changedValidation)
                            {
                                changedElementValidations.Add(category);
                            }
                        }

                        var childResources = new List<ResourceViewModel>();
                        var childCategories = new List<ICategoryLikeViewModel>();
                        foreach (ITreeElementViewModel child in category.Children)
                        {
                            if (child.ElementType == TreeElementType.Resource)
                            {
                                childResources.Add((ResourceViewModel)child);
                            }
                            else // if category has another child category, stop processing, because we need to process lowest-child categories of tree!
                            {
                                childCategories.Add((ICategoryLikeViewModel)child);
                            }
                        }

                        foreach (ICategoryLikeViewModel childCategory in childCategories)
                        {
                            ProcessCategory(childCategory);
                        }

                        ILookup<string, ResourceViewModel> siblingResources = GetSiblingResources(category);
                        foreach (ResourceViewModel childResource in childResources)
                        {
                            if (HasChangedVersion(childResource) &&
                                ValidateResource(childResource, siblingResources))
                            {
                                changedElementValidations.Add(childResource);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugUtils.Log("[PropertyValidityProcess] Recalculate, error: \n" + e.GetFullExceptionDetails());
                }
                finally
                {
                    _lastProcessedUID = processingUID;
                    _recalculatingInProgress.ResetSignal();
                }

                return true;
            }

            private bool HasChangedVersion(ITreeElementViewModel element)
            {
                object elementVersionUID;
                lock (_elementVersions.SyncRoot)
                {
                    elementVersionUID = _elementVersions[element.ElementKey];
                }

                return elementVersionUID == null || (string)elementVersionUID != element.VersionUID;
            }

            private void UpdateElementVersion(ITreeElementViewModel element)
            {
                lock (_elementVersions.SyncRoot)
                {
                    _elementVersions[element.ElementKey] = element.VersionUID;
                }
            }

            private void ResetElementVersion(ITreeElementViewModel element)
            {
                lock (_elementVersions.SyncRoot)
                {
                    _elementVersions.Remove(element.ElementKey);
                }
            }

            private bool ValidateRootModel(RootModelViewModel rootModel)
            {
                UpdateElementVersion(rootModel);

                string error = ModelValidator.ValidateName(_validatorContext, rootModel.Name);

                bool validationChanged;

                if (error.IsNullOrEmpty())
                {
                    validationChanged = _validatorContext.RemoveValidationError(rootModel, nameof(rootModel.Name));
                }
                else
                {
                    validationChanged = _validatorContext.SetValidationError(rootModel, nameof(rootModel.Name), error,
                        () => ModelValidator.ValidateName(_validatorContext, rootModel.Name), null);
                }

                if (validationChanged)
                {
                    ResetElementVersion(rootModel);
                }

                return validationChanged;
            }

            private bool ValidateCategory(CategoryViewModel category,
                Func<CategoryViewModel, HashSet<string>> getSiblingsCategories)
            {
                UpdateElementVersion(category);
                string error = CategoryValidator.ValidateName(_validatorContext, category.Name);
                bool validationChanged;

                if (error.IsNullOrEmpty())
                {
                    HashSet<string> siblingsCategories = getSiblingsCategories(category);
                    string categoryName = category.Name.ToLowerInvariant();

                    bool ContainsKey()
                    {
                        using (HashSet<string>.Enumerator enumerator = siblingsCategories.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                string elementKey = enumerator.Current.Substring(0, _elementKeySize);
                                string elementName = enumerator.Current.Substring(_elementKeySize,
                                    enumerator.Current.Length - _elementKeySize);

                                if (elementName == categoryName && elementKey != category.ElementKey)
                                {
                                    return true;
                                }
                            }
                        }

                        return false;
                    }

                    if (ContainsKey())
                    {
                        siblingsCategories.Remove(category.ElementKey + categoryName);

                        error = Strings.Validations.CategoryWithSameNameAlreadyExist(category.Name);
                        //error = CategoryWithSameNameAlreadyExistUnder.FormatWith(category.Name);

                        validationChanged = _validatorContext.SetValidationError(category, nameof(category.Name),
                            error, () => Strings.Validations.CategoryWithSameNameAlreadyExist(category.Name), null);
                    }
                    else
                    {
                        validationChanged = _validatorContext.RemoveValidationError(category, nameof(category.Name));
                    }
                }
                else
                {
                    validationChanged = _validatorContext.SetValidationError(category, nameof(category.Name), error,
                        () => CategoryValidator.ValidateName(_validatorContext, category.Name), null);
                }

                if (validationChanged)
                {
                    ResetElementVersion(category);
                }

                return validationChanged;
            }

            private bool ValidateResource(ResourceViewModel resource,
                ILookup<string, ResourceViewModel> siblingsResources)
            {
                UpdateElementVersion(resource);
                string error = ResourceValidator.ValidateName(_validatorContext, resource.Name);
                bool validationChanged;

                if (error.IsNullOrEmpty())
                {
                    string resourceName = resource.Name.ToLowerInvariant();

                    if (siblingsResources[resourceName].Count() > 1)
                    {
                        //error = ResourceWithSameNameAlreadyExistUnder.FormatWith(resource.Name);
                        error = Strings.Validations.ResourceWithSameNameAlreadyExist(resource.Name);

                        validationChanged = _validatorContext.SetValidationError(resource, nameof(resource.Name),
                            error, () => Strings.Validations.ResourceWithSameNameAlreadyExist(resource.Name), null);
                    }
                    else
                    {
                        validationChanged = _validatorContext.RemoveValidationError(resource, nameof(resource.Name));
                    }
                }
                else
                {
                    validationChanged = _validatorContext.SetValidationError(resource, nameof(resource.Name), error,
                        () => ResourceValidator.ValidateName(_validatorContext, resource.Name), null);
                }

                if (validationChanged)
                {
                    ResetElementVersion(resource);
                }

                return validationChanged;
            }

            public void Reset()
            {
                lock (_elementVersions.SyncRoot)
                {
                    _elementVersions?.Clear();
                }
            }

            public void Notify()
            {
                _lastNotifyUID = Guid.NewGuid();
                _lastNotifyDate = DateTimeHelper.UtcNow;
            }

            public void ForceRefresh()
            {
                _forceRecalculateNextIteration = true;
                try
                {
                    Recalculate(false);
                }
                finally
                {
                    _forceRecalculateNextIteration = false;
                }
            }
        }
    }
}
