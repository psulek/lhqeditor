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
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LHQ.Core.Extensions;
using LHQ.Core.Interfaces;
using LHQ.Core.Interfaces.PluginSystem;
using LHQ.Core.Model;
using LHQ.Core.Model.Translation;
using LHQ.Utils.Comparers;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LHQ.Plugin.MicrosoftTranslator
{
    public class TranslationProvider : ITranslationProvider, IDisposable
    {
        private const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";
        private const string AzureHostUrl = "https://api.cognitive.microsofttranslator.com";

        private const string RouteTranslate = "/translate?api-version=3.0&from=";
        private const string RouteGetLanguages = "/languages?api-version=3.0&scope=translation";

        private const int TextElementsMaxCount = 100;
        //private const int MaxCharsInTexts = 5000;
        private const int MaxCharsInTexts = 1000;

        private bool _disposed;
        private string[] _supportedLanguages;

        public TranslationProvider()
        {
            _supportedLanguages = null;
        }

        private PluginConfig PluginConfig => this.GetPluginConfig<PluginConfig>();

        public IPluginSystemContext PluginSystemContext { get; set; }

        protected ILogger Logger => PluginSystemContext.Logger;

        protected IUtilityHelper UtilityHelper => PluginSystemContext.UtilityHelper;

        public string Key { get; } = "Microsoft.Azure.Translation";

        public string DisplayName { get; } = "Microsoft Translator (using Cognitive Services)";

        public string Version { get; } = "1.0";

        public string Help { get; } = "";

        public bool IsConfigured => !PluginConfig.MicrosoftTranslatorSubscriptionKey.IsNullOrEmpty();

        public void Initialize()
        {}

        public async Task<OperationResult> TestServiceAsync(CancellationToken cancellationToken)
        {
            ArgumentValidator.EnsureArgumentNotNull(cancellationToken, nameof(cancellationToken));

            var result = new OperationResult<string[]>(OperationResultStatus.Failed, "Unknown error");

            if (PluginConfig.MicrosoftTranslatorSubscriptionKey.IsNullOrEmpty())
            {
                result.SetError("Authentication key for Azure Translator Text API is required!");
            }
            else
            {
                try
                {
                    result = await GetSupportedLanguages(cancellationToken);
                    if (result.Data.Length == 0)
                    {
                        result.SetError("Azure Translator Text API does not support any translations right now, please try again later.");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"Testing Translation Service '{Key}' failed", e);
                }
            }

            return result;
        }

        public async Task<TranslationResult> GetTranslationsAsync(IList<string> texts, string fromLanguage,
            string[] targetLanguages, CancellationToken cancellationToken)
        {
            ArgumentValidator.EnsureArgumentNotEmptyCollection(texts, nameof(texts));
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(fromLanguage, nameof(fromLanguage));
            ArgumentValidator.EnsureArgumentNotEmptyCollection(targetLanguages, nameof(targetLanguages));
            ArgumentValidator.EnsureArgumentNotNull(cancellationToken, nameof(cancellationToken));

            var result = new TranslationResult();

            targetLanguages = PrepareTargetLanguages(targetLanguages);

            StringBuilder route = new StringBuilder(RouteTranslate + fromLanguage);
            foreach (string targetLanguage in targetLanguages)
            {
                route.Append($"&to={targetLanguage}");
            }

            CultureInfo sourceCulture = CultureCache.Instance.GetCulture(fromLanguage);
            var comparer = new StringWithCultureEqualityComparer(sourceCulture, false);

            if (texts.Count > 1)
            {
                texts = UtilityHelper.RemoveDuplicities(texts, x => x, comparer) as IList<string> ?? new List<string>();
            }

            // batch array into 100-count (TextElementsMaxCount) lists
            var batch = texts.Count <= TextElementsMaxCount
                ? new List<IList<string>> { texts } 
                : UtilityHelper.Batch(texts, TextElementsMaxCount);

            async Task CallApiRequest(IEnumerable<string> sourceList)
            {
                var batchList = sourceList.ToList();
                object[] body = batchList.Select(x => new { Text = x }).ToArray();

                (HttpStatusCode statusCode, var json) = await CallApi(route.ToString(), HttpMethod.Post, true, body, cancellationToken);
                if (HandleStatusCode(statusCode, out var error))
                {
                    if (json is JArray jsonArray)
                    {
                        int idx = 0;
                        foreach (JToken arrayItem in jsonArray)
                        {
                            if (arrayItem is JObject item && item.ContainsKey("translations"))
                            {
                                JToken tokenTranslations = item["translations"];
                                if (tokenTranslations is JArray arrayTranslations)
                                {
                                    foreach (JToken translationToken in arrayTranslations)
                                    {
                                        if (translationToken is JObject translation)
                                        {
                                            if (translation.ContainsKey("text") && translation.ContainsKey("to"))
                                            {
                                                string sourceText = batchList[idx];

                                                result.AddTranslation(sourceText, translation["to"].Value<string>(), translation["text"].Value<string>());
                                            }
                                        }
                                    }
                                }

                                idx++;
                            }
                        }
                    }

                    result.Status = OperationResultStatus.Success;
                }
                else
                {
                    result.SetError(error);
                }
            }

            foreach (IEnumerable<string> batchByCount in batch)
            {
                // partition batched list (batched by count, max 100) with total char count on list (max 5000 chars)
                var partitionedByTextLength = UtilityHelper.PartitionTextOnLength2(batchByCount.ToList(), MaxCharsInTexts);

                foreach (var batchedTexts in partitionedByTextLength)
                {
                    await CallApiRequest(batchedTexts.ToList());
                }
            }

            return result;
        }

        private Task<(HttpStatusCode statusCode, JToken json)> CallApi(string route, HttpMethod httpMethod, bool useSubscriptionKey,
            CancellationToken cancellationToken = default)
        {
            return CallApi<object>(route, httpMethod, useSubscriptionKey, null, cancellationToken);
        }

        private async Task<(HttpStatusCode statusCode, JToken json)> CallApi<TRequest>(string route, HttpMethod httpMethod, 
            bool useSubscriptionKey, TRequest request, CancellationToken cancellationToken = default)
        {
            HttpStatusCode statusCode;
            JToken json = default;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var requestMessage = new HttpRequestMessage())
                    {
                        requestMessage.Method = httpMethod;
                        requestMessage.RequestUri = new Uri(AzureHostUrl + route);

                        if (useSubscriptionKey)
                        {
                            requestMessage.Headers.Add(OcpApimSubscriptionKeyHeader, PluginConfig.MicrosoftTranslatorSubscriptionKey);
                        }

                        if (request != null)
                        {
                            var requestBody = JsonConvert.SerializeObject(request, Formatting.None);
                            requestMessage.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                        }

                        // Send request, get response
                        var response = await httpClient.SendAsync(requestMessage, cancellationToken);
                        statusCode = response.StatusCode;

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();
                            json = JToken.Parse(jsonResponse);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                statusCode = HttpStatusCode.InternalServerError;
                Logger.Error($"{nameof(TranslationProvider)}.{nameof(CallApi)}('{route}' / {httpMethod}, usekey:{useSubscriptionKey})", e);
            }

            return (statusCode, json);
        }

        public async Task<OperationResult<string[]>> GetSupportedLanguages(CancellationToken cancellationToken)
        {
            ArgumentValidator.EnsureArgumentNotNull(cancellationToken, nameof(cancellationToken));

            var result = new OperationResult<string[]>(OperationResultStatus.Failed);

            if (_supportedLanguages == null || _supportedLanguages.Length == 0)
            {
                (HttpStatusCode statusCode, var jtoken) = await CallApi(RouteGetLanguages, HttpMethod.Get, false, cancellationToken);
                if (HandleStatusCode(statusCode, out var error))
                {
                    var languages = new List<string>();

                    if (jtoken is JObject json && json.ContainsKey("translation"))
                    {
                        foreach (JToken child in json["translation"].Children())
                        {
                            if (child is JProperty property)
                            {
                                languages.Add(property.Name);
                            }
                        }
                    }

                    _supportedLanguages = languages.Select(x => x.ToLowerInvariant()).Distinct().ToArray();
                    result.Status = OperationResultStatus.Success;
                    result.Data = _supportedLanguages;
                }
                else
                {
                    result.SetError(error);
                    _supportedLanguages = new string[0];
                }
            }
            else if (_supportedLanguages?.Length > 0)
            {
                result.Status = OperationResultStatus.Success;
                result.Data = _supportedLanguages;
            }

            return result;
        }

        private bool HandleStatusCode(HttpStatusCode httpStatusCode, out string error)
        {
            bool result = false;
            error = null;

            switch (httpStatusCode)
            {
                case HttpStatusCode.Unauthorized:
                {
                    error = "Request to translation service is not authorized. Check that the Azure subscription key is valid.";
                    break;
                }
                case HttpStatusCode.Forbidden:
                {
                    error =
                        "Request to translation service is not authorized. For accounts in the free-tier, check that the account quota is not exceeded.";
                    break;
                }
                case HttpStatusCode.OK:
                {
                    result = true;
                    break;
                }
                default:
                {
                    error = "Error occured.";
                    break;
                }
            }

            return result;
        }

        private string[] PrepareTargetLanguages(params string[] targetLanguages)
        {
            if (_supportedLanguages != null && _supportedLanguages.Length > 0)
            {
                var newTarget = new List<string>();
                foreach (string targetLanguage in targetLanguages)
                {
                    var lang = targetLanguage.ToLowerInvariant();

                    bool support = _supportedLanguages.Contains(lang);
                    if (!support && lang.Contains("-"))
                    {
                        // try to find parent lang in supported language (eg., when sk-sk is not supported, try to find 'sk'
                        lang = lang.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0];
                        support = _supportedLanguages.Contains(lang);
                    }

                    if (support)
                    {
                        newTarget.Add(lang);
                    }
                }

                targetLanguages = newTarget.ToArray();
            }

            return targetLanguages;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                _disposed = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
