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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Implementation;
using LHQ.App.Services.Implementation.Undo;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.App.ViewModels.Elements;
using LHQ.Data;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.UndoUnits
{
    public class UpdateLanguagesUndoUnit : UndoCommandUnit, IModelElementUndoCommand
    {
        private readonly List<string> _allLanguages;
        private readonly Dictionary<string, string> _replacedLanguages;
        private readonly string _primaryLanguageName;
        private Dictionary<string, List<ResourceValueElement>> _savedResourcesValues;
        private bool _isInitialized;
        private List<string> _languagesToAdd;
        private string _savedPrimaryLanguage;

        public UpdateLanguagesUndoUnit(List<string> allLanguages, Dictionary<string, string> replacedLanguages, string primaryLanguageName)
            : base(Guid.NewGuid().ToString())
        {
            _allLanguages = allLanguages;
            _replacedLanguages = replacedLanguages;
            _primaryLanguageName = primaryLanguageName;

            InternalUpdateGetName(() => Strings.Operations.UpdateLanguagesUndoUnitCommandName);

            _isInitialized = false;
        }

        private TransactionUndoManager UndoManager => (TransactionUndoManager)Manager;

        private ITreeViewService TreeViewService => UndoManager.ShellViewContext.TreeViewService;


        private List<string> FindLanguagesToBeRemoved()
        {
            return TreeViewService.RootModel.Languages
                .Where(x => !_allLanguages.Contains(x.Name) && !_replacedLanguages.ContainsKey(x.Name))
                .Select(x => x.Name)
                .ToList();
        }

        private List<ResourceViewModel> FindAllResources()
        {
            return TreeViewService.FindAllElements(x => x.ElementType == TreeElementType.Resource)
                .Cast<ResourceViewModel>()
                .ToList();
        }

        private void Initialize()
        {
            // find all languages which to be removed from project
            List<string> languagesToBeRemoved = FindLanguagesToBeRemoved();

            // backup all resource values with languages which will be removed..
            if (languagesToBeRemoved.Count > 0)
            {
                List<ResourceViewModel> allResources = FindAllResources();
                _savedResourcesValues = allResources.ToDictionary(x => x.ElementKey,
                    x => x.Values
                        .Where(y => languagesToBeRemoved.Contains(y.LanguageName))
                        .Select(z => z.SaveToModelElement())
                        .ToList());
            }

            _savedPrimaryLanguage = TreeViewService.RootModel.GetPrimaryLanguageName();

            _isInitialized = true;
        }

        protected override void InternalDo()
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            RootModelViewModel rootModel = TreeViewService.RootModel;

            _languagesToAdd = _allLanguages.Where(x => !_replacedLanguages.ContainsKey(x)).ToList();

            if (_replacedLanguages.Count > 0)
            {
                List<ResourceViewModel> allResources = FindAllResources();

                // change languages in resources values
                allResources.SelectMany(x => x.Values)
                    .Where(x => _replacedLanguages.ContainsKey(x.LanguageName))
                    .Run(x => x.ChangeLanguage(_replacedLanguages[x.LanguageName]));

                // replace languages in root languages
                foreach (LanguageViewModel modelLanguage in rootModel.Languages.Where(x => _replacedLanguages.ContainsKey(x.Name)))
                {
                    string newLanguageName = _replacedLanguages[modelLanguage.Name];
                    CultureInfo newCulture = CultureCache.Instance.GetCulture(newLanguageName);

                    modelLanguage.UpdateFrom(newCulture);
                }
            }

            // find all languages which to be removed from project
            List<string> languagesToBeRemoved = FindLanguagesToBeRemoved();
            if (languagesToBeRemoved.Count > 0)
            {
                rootModel.AddOrRemoveLanguages(languagesToBeRemoved, false);

                _languagesToAdd.RemoveAll(x => languagesToBeRemoved.Contains(x));
            }

            // add new languages
            if (_languagesToAdd.Count > 0)
            {
                _languagesToAdd = rootModel.GetMissingLanguages(_languagesToAdd);
                if (_languagesToAdd.Count > 0)
                {
                    rootModel.AddOrRemoveLanguages(_languagesToAdd, true);
                }
            }

            if (!_savedPrimaryLanguage.EqualsTo(_primaryLanguageName))
            {
                rootModel.SetPrimaryLanguage(_primaryLanguageName);
            }

            TreeViewService.NotifyTranslationValidChanged();
        }

        protected override void InternalUndo()
        {
            RootModelViewModel rootModel = TreeViewService.RootModel;

            if (_replacedLanguages.Count > 0)
            {
                Dictionary<string, string> replacedLanguagesReversed = _replacedLanguages.ToDictionary(x => x.Value, x => x.Key);

                List<ResourceViewModel> allResources = FindAllResources();

                // change languages in resources values
                allResources.SelectMany(x => x.Values)
                    .Where(x => replacedLanguagesReversed.ContainsKey(x.LanguageName))
                    .Run(x => x.ChangeLanguage(replacedLanguagesReversed[x.LanguageName]));

                // replace languages in root languages
                foreach (LanguageViewModel modelLanguage in rootModel.Languages.Where(x => replacedLanguagesReversed.ContainsKey(x.Name)))
                {
                    string newLanguageName = replacedLanguagesReversed[modelLanguage.Name];
                    CultureInfo newCulture = CultureCache.Instance.GetCulture(newLanguageName);

                    modelLanguage.UpdateFrom(newCulture);
                }
            }

            // find all languages which to be removed from project
            List<string> languagesToBeRemoved = FindLanguagesToBeRemoved();
            if (languagesToBeRemoved.Count > 0)
            {
                //languagesToAdd.RemoveAll(x => languagesToBeRemoved.Contains(x));
            }

            var languagesToRestore = new List<string>();

            // restore previously deleted resources values
            if (_savedResourcesValues != null)
            {
                var resources = from resource in FindAllResources()
                    let savedValue = _savedResourcesValues.SingleOrDefault(x => x.Key.EqualsTo(resource.ElementKey))
                    where savedValue.Key != null
                    select new
                    {
                        resource,
                        value = savedValue.Value
                    };

                foreach (var resourcePair in resources)
                {
                    ResourceViewModel resource = resourcePair.resource;
                    List<ResourceValueElement> savedResourceValues = resourcePair.value;

                    foreach (ResourceValueElement savedResourceValue in savedResourceValues)
                    {
                        ResourceValueViewModel existingValue =
                            resource.Values.SingleOrDefault(x => x.LanguageName.EqualsTo(savedResourceValue.LanguageName));
                        if (existingValue == null)
                        {
                            resource.Values.Add(new ResourceValueViewModel(resource, savedResourceValue));

                            if (!languagesToRestore.Contains(savedResourceValue.LanguageName))
                            {
                                languagesToRestore.Add(savedResourceValue.LanguageName);
                            }
                        }
                    }
                }
            }

            // restore previously removed languages to root model
            languagesToRestore = languagesToRestore.Except(_languagesToAdd).ToList();
            if (languagesToRestore.Count > 0)
            {
                languagesToRestore =
                    languagesToRestore.Except(rootModel.Languages.Select(x => x.Name)).ToList();

                foreach (string languageName in languagesToRestore)
                {
                    rootModel.Languages.Add(new LanguageViewModel(rootModel.ShellViewContext, CultureCache.Instance.GetCulture(languageName), false));
                }
            }

            // remove previously added languages
            if (_languagesToAdd.Count > 0)
            {
                rootModel.AddOrRemoveLanguages(_languagesToAdd, false);
            }

            // update primary language
            rootModel.SetPrimaryLanguage(_savedPrimaryLanguage);

            TreeViewService.NotifyTranslationValidChanged();
        }

        protected override UndoCommandCompleted InternalCompleted(UndoCommandKind undoKind)
        {
            TreeViewService.Select(TreeViewService.RootModel);
            TreeViewService.RootModel.LanguagesView.Refresh();
            return UndoCommandCompleted.Default;
        }

        public bool CanRemove(string keyToRemove)
        {
            return false;
        }
    }
}
