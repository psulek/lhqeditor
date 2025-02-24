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

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Elements;
using LHQ.Core.Model.Translation;
using LHQ.Data.Interfaces.Key;
using LHQ.Utils;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation
{
    public class TranslationAsyncOperation : AsyncOperation
    {
        private readonly IShellViewContext _shellViewContext;
        private readonly IModelElementKey[] _resourceKeys;
        private readonly string _onlyLanguage;
        private readonly bool _onlyEmptyValues;
        private StopwatchUtil _stopwatchUtil;

        public TranslationAsyncOperation(IShellViewContext shellViewContext, IModelElementKey[] resourceKeys,
            SuggestTranslationMode translationMode, string onlyLanguage): base(shellViewContext.AppContext)
        {
            _shellViewContext = shellViewContext;
            _resourceKeys = resourceKeys;
            _onlyLanguage = onlyLanguage;
            _onlyEmptyValues = translationMode == SuggestTranslationMode.Empty;
        }

        public override string ProgressMessage { get; } = Strings.Services.Application.TranslationResourcesInProgress;

        public override bool ShowCancelButton { get; } = true;

        private ITreeViewService TreeViewService => _shellViewContext.TreeViewService;

        private ITranslationService TranslationService => AppContext.TranslationService;

        protected override async Task InternalStart()
        {
            StartMeasureTime();
            Result.SetError(Strings.Services.Translation.TranslationFailedOrNoResources);

            List<string> unsupportedLanguages = null;

            TranslationResult translationResult = new TranslationResult();
            int resourceCount = _resourceKeys.Length;
            if (resourceCount > 0)
            {
                var primaryResourceValues = new List<ResourceValueViewModel>();
                var targetLanguages = new List<string>();
                var cultureSpecificLanguages = new Dictionary<string, string>();

                foreach (IModelElementKey resourceKey in _resourceKeys)
                {
                    if (IsCancellationRequested)
                    {
                        break;
                    }

                    if (TreeViewService.FindElementByKey(resourceKey.Serialize()) is ResourceViewModel resourceViewModel)
                    {
                        foreach (ResourceValueViewModel resourceValue in resourceViewModel.Values)
                        {
                            if (resourceValue.IsReferenceLanguage)
                            {
                                primaryResourceValues.Add(resourceValue);
                            }
                            else
                            {
                                bool canTranslate = (!_onlyEmptyValues || resourceValue.Value.IsNullOrEmpty()) && !resourceValue.Locked;
                                string targetLanguage = resourceValue.LanguageName;

                                if (!_onlyLanguage.IsNullOrEmpty())
                                {
                                    canTranslate = targetLanguage == _onlyLanguage;
                                }

                                if (canTranslate && !targetLanguages.Contains(targetLanguage.ToLowerInvariant()))
                                {
                                    CultureInfo cultureInfo = CultureCache.Instance.GetCulture(targetLanguage);
                                    // if is culture specific get invariant culture instead
                                    if (!cultureInfo.IsNeutralCulture && cultureInfo.Parent.IsNeutralCulture)
                                    {
                                        // key - invariant culture, value - culture specific
                                        cultureSpecificLanguages.Add(cultureInfo.Parent.Name, targetLanguage);
                                        targetLanguage = cultureInfo.Parent.Name;
                                    }

                                    targetLanguages.Add(targetLanguage.ToLowerInvariant());
                                }
                            }
                        }
                    }
                }

                if (!IsCancellationRequested && primaryResourceValues.Count > 0 && targetLanguages.Count > 0)
                {
                    string fromLanguage = primaryResourceValues.First().LanguageName;

                    var resourceKeys = primaryResourceValues.Select(x => x.OwnerResource.ElementKey).ToList();
                    var texts = primaryResourceValues
                        .Where(x => !string.IsNullOrEmpty(x.Value))
                        .Select(x => x.Value)
                        .ToList();

                    string[] supportedLanguages = await TranslationService.GetSupportedLanguages(CancellationToken);

                    unsupportedLanguages = targetLanguages.Except(supportedLanguages).ToList();

                    translationResult = await TranslationService.GetTranslationsAsync(texts, fromLanguage, targetLanguages,
                        CancellationToken);

                    if (!IsCancellationRequested && translationResult.IsSuccess && translationResult.HasValidTranslation)
                    {
                        if (cultureSpecificLanguages.Count > 0)
                        {
                            foreach (var translation in translationResult.Translations)
                            {
                                if (cultureSpecificLanguages.ContainsKey(translation.LanguageName))
                                {
                                    translation.UpdateLanguageName(cultureSpecificLanguages[translation.LanguageName]);
                                }
                            }
                        }

                        ResourceViewModel resource = primaryResourceValues.First().OwnerResource;
                        if (_resourceKeys.Length == 1)
                        {
                            resource.Parent.AutoTranslateResource(translationResult, resource);
                        }
                        else
                        {
                            resource.GetRootModel().AutoTranslateResources(translationResult, resourceKeys, _onlyEmptyValues);
                        }
                    }
                }
            }

            if (translationResult.HasValidTranslation)
            {
                Result.SetSuccess(Strings.Services.Translation.TranslationCompletedSuccessfully(resourceCount));
            }

            if (unsupportedLanguages?.Count > 0)
            {
                var langNames = CultureCache.Instance.GetAll()
                    .Where(x => unsupportedLanguages.Contains(x.Name))
                    .Select(x => x.EnglishName)
                    .ToDelimitedString();

                var extraDetail = Strings.Services.Translation.SomeLanguagesAreNotSupported(TranslationService.ActiveProvider.DisplayName, langNames);
                if (Result.Detail.IsNullOrEmpty())
                {
                    Result.Detail = extraDetail;
                }
                else
                {
                    Result.Detail += " " + extraDetail;
                }
            }
        }

        [Conditional("DEBUG")]
        private void StartMeasureTime()
        {
            _stopwatchUtil = StopwatchUtil.Create();
        }

        [Conditional("DEBUG")]
        private void StopMeasureTime()
        {
            _stopwatchUtil.Stop($"{GetType().Name} completed translation of {_resourceKeys.Length} resources");
        }

        protected override Task InternalCompleted(bool userCancelled)
        {
            StopMeasureTime();
            IDialogService dialogService = AppContext.DialogService;

            if (userCancelled)
            {
                dialogService.ShowError(new DialogShowInfo(Strings.Services.Translation.TranslateResourcesCaption,
                    Strings.Services.Translation.TranslationWasCancelledByUser));
            }
            else
            {
                if (Result.IsSuccess)
                {
                    dialogService.ShowInfo(new DialogShowInfo(Strings.Services.Translation.TranslateResourcesCaption, Result.Message, Result.Detail));
                }
                else
                {
                    dialogService.ShowError(new DialogShowInfo(Strings.Services.Translation.TranslateResourcesCaption, Result.Message, Result.Detail));
                }
            }

            return Task.CompletedTask;
        }
    }
}