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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LHQ.App.Services.Implementation.PluginSystem;
using LHQ.App.Services.Interfaces;
using LHQ.Core.DependencyInjection;
using LHQ.Core.Interfaces;
using LHQ.Core.Model;
using LHQ.Core.Model.Translation;
using LHQ.Utils.Comparers;
using LHQ.Utils.Extensions;

namespace LHQ.App.Services.Implementation
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public sealed class TranslationService : AppContextServiceBase, ITranslationService
    {
        private readonly StringEqualityComparer _equalityComparer = new StringEqualityComparer();
        private static readonly TimeSpan _callTimeout = TimeSpan.FromMinutes(1);

        private PluginServiceContainer PluginServiceContainer => AppContext.PluginServiceContainer as PluginServiceContainer;

        public ITranslationProvider ActiveProvider
        {
            get
            {
                if (AppConfig.EnableTranslation && !AppConfig.TranslatorProviderKey.IsNullOrEmpty())
                {
                    return GetProvider(AppConfig.TranslatorProviderKey);
                }

                return null;
            }
        }

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        public List<ITranslationProvider> GetProviders()
        {
            return PluginServiceContainer.GetAll<ITranslationProvider>();
        }

        private ITranslationProvider GetProvider(string translatorProviderKey)
        {
            return GetProviders().SingleOrDefault(x => x.Key == translatorProviderKey);
        }

        public async Task<string[]> GetSupportedLanguages(CancellationToken cancellationToken)
        {
            ITranslationProvider translationProvider = ActiveProvider;
            string[] langs = new string[0];

            if (translationProvider != null)
            {
                OperationResult<string[]> result = await TaskAsyncExtensions.WaitUntil(cancellationToken, (int)_callTimeout.TotalMilliseconds, 
                    ct => translationProvider.GetSupportedLanguages(cancellationToken));

                if (result.IsSuccess)
                {
                    langs = result.Data;
                }
            }

            return langs;
        }

        public async Task<TranslationResult> GetTranslationsAsync(IList<string> texts, string fromLanguage, List<string> targetLanguages,
            CancellationToken cancellationToken)
        {
            var result = new TranslationResult();
            ITranslationProvider translationProvider = ActiveProvider;

            if (translationProvider != null)
            {
                string[] supportedLanguages = await GetSupportedLanguages(cancellationToken);

                targetLanguages = targetLanguages.Select(x => x.ToLowerInvariant()).ToList();

                // remove all langauges which are not supported by translator provider.
                targetLanguages.RemoveAll(x => !supportedLanguages.Contains(x));

                targetLanguages = targetLanguages.Count == 1 ? targetLanguages : targetLanguages.Distinct(_equalityComparer).ToList();

                if (targetLanguages.Count > 0)
                {
                    result = await TaskAsyncExtensions.WaitUntil(cancellationToken, (int)_callTimeout.TotalMilliseconds,
                        ct => translationProvider.GetTranslationsAsync(texts, fromLanguage, targetLanguages.ToArray(), cancellationToken));
                }
            }

            return result;
        }
    }
}
