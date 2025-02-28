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
using System.Linq;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Implementation;
using LHQ.App.Services.Implementation.Undo;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.App.ViewModels.Elements;
using LHQ.Core.Model.Translation;
using LHQ.Utils.Extensions;

namespace LHQ.App.UndoUnits
{
    internal class AutoTranslateMultiResourcesUndoUnit : UndoCommandUnit, IModelElementUndoCommand
    {
        private readonly RootModelViewModel _parent;
        private readonly TranslationResult _translationResult;
        private readonly List<string> _resourceKeys;
        private readonly bool _onlyEmptyValues;

        private bool _isInitialized;
        private readonly List<IGrouping<string, TranslationResultItem>> _translationsGroupedBySourceText;
        private Dictionary<string, List<(string language, bool autoTranslated, string value)>> _savedResourceStates;

        public AutoTranslateMultiResourcesUndoUnit(RootModelViewModel parent,
            TranslationResult translationResult, List<string> resourceKeys, bool onlyEmptyValues)
            : base(Guid.NewGuid().ToString())
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _translationResult = translationResult ?? throw new ArgumentNullException(nameof(translationResult));
            _resourceKeys = resourceKeys;
            _onlyEmptyValues = onlyEmptyValues;
            _translationsGroupedBySourceText = _translationResult.Translations.GroupBy(x => x.SourceText).ToList();

            InternalUpdateGetName(() => Strings.Services.UndoUnits.AutoTranslateMultiResources.ActionName);
            _isInitialized = false;
        }

        private ITreeViewService TreeViewService => UndoManager.ShellViewContext.TreeViewService;

        private TransactionUndoManager UndoManager => (TransactionUndoManager)Manager;

        private List<ResourceViewModel> FindResourcesToUpdate()
        {
            return TreeViewService.FindAllElements(x => x.ElementType == TreeElementType.Resource && _resourceKeys.Contains(x.ElementKey))
                .Cast<ResourceViewModel>()
                .Where(x => !x.Locked)
                .ToList();
        }

        private List<ResourceViewModel> Initialize()
        {
            List<ResourceViewModel> resourcesToUpdate = FindResourcesToUpdate();

            if (!_isInitialized)
            {
                _isInitialized = true;

                _savedResourceStates = new Dictionary<string, List<(string language, bool autoTranslated, string value)>>();
                var targetLanguages = _translationResult.TargetLanguages;

                foreach (ResourceViewModel resource in resourcesToUpdate)
                {
                    var valuesToTranslate = resource.Values.Where(x => !x.IsReferenceLanguage && !x.Locked && (!_onlyEmptyValues || x.Value.IsNullOrEmpty()));
                    foreach (ResourceValueViewModel resourceValue in valuesToTranslate)
                    {
                        if (targetLanguages.Contains(resourceValue.LanguageName))
                        {
                            List<(string language, bool autoTranslated, string value)> savedResourceValues;
                            if (_savedResourceStates.ContainsKey(resource.ElementKey))
                            {
                                savedResourceValues = _savedResourceStates[resource.ElementKey];
                            }
                            else
                            {
                                savedResourceValues = new List<(string language, bool autoTranslated, string value)>();
                                _savedResourceStates.Add(resource.ElementKey, savedResourceValues);
                            }

                            savedResourceValues.Add((resourceValue.LanguageName, resourceValue.AutoTranslated, resourceValue.Value));
                        }
                    }
                }
            }

            return resourcesToUpdate;
        }

        protected override void InternalDo()
        {
            List<ResourceViewModel> resourcesToUpdate = Initialize();

            using (TreeViewService.TemporaryBlock(TemporaryBlockType.RefreshTranslationValidity))
            {
                foreach (ResourceViewModel resource in resourcesToUpdate)
                {
                    var referenceLanguageValue = resource.GetReferenceLanguageValue();

                    if (!referenceLanguageValue.Value.IsNullOrEmpty() && !referenceLanguageValue.OwnerResource.Locked)
                    {
                        bool wasChanged = false;
                        var groupValues = _translationsGroupedBySourceText.SingleOrDefault(x => x.Key == referenceLanguageValue.Value);
                        if (groupValues != null)
                        {
                            foreach (TranslationResultItem translation in groupValues)
                            {
                                var resourceValue = resource.Values.SingleOrDefault(x => x.LanguageName == translation.LanguageName 
                                    && !x.IsReferenceLanguage && !x.Locked && (!_onlyEmptyValues || x.Value.IsNullOrEmpty()));

                                if (resourceValue != null)
                                {
                                    bool autoTranslatedChanged = resourceValue.SetAutoTranslated(translation.Translation, true);

                                    wasChanged = wasChanged || autoTranslatedChanged;
                                }
                            }
                        }

                        if (wasChanged)
                        {
                            resource.Parent.UpdateVersion(true);
                        }
                    }
                }
            }
        }

        protected override void InternalUndo()
        {
            using (TreeViewService.TemporaryBlock(TemporaryBlockType.RefreshTranslationValidity))
            {
                foreach (ResourceViewModel resource in FindResourcesToUpdate())
                {
                    bool wasChanged = false;

                    if (_savedResourceStates.TryGetValue(resource.ElementKey, out var savedResourceValues))
                    {
                        foreach (var savedResourceValue in savedResourceValues)
                        {
                            var resourceValue = resource.Values.SingleOrDefault(x => x.LanguageName == savedResourceValue.language
                                && !x.IsReferenceLanguage && !x.Locked);

                            if (resourceValue != null)
                            {
                                bool autoTranslatedChanged = resourceValue.SetAutoTranslated(savedResourceValue.value, savedResourceValue.autoTranslated);
                                wasChanged = wasChanged || autoTranslatedChanged;
                            }
                        }
                    }

                    if (wasChanged)
                    {
                        resource.Parent.UpdateVersion(false);
                    }
                }
            }
        }

        public bool CanRemove(string keyToRemove)
        {
            return false;
        }

        protected override UndoCommandCompleted InternalCompleted(UndoCommandKind undoKind)
        {
            _parent.RefreshAllResourcesCount();
            _parent.InvalidateTranslationCountInfo();

            return UndoCommandCompleted.Default;
        }
    }
}