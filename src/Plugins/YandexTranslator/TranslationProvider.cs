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
using Newtonsoft.Json.Linq;

namespace LHQ.Plugin.YandexTranslator
{
    public class TranslationProvider : ITranslationProvider, IDisposable
    {
        private const string YandexHostUrl = "https://translate.yandex.net/api/v1.5/tr.json";

        private static class Routes
        {
            public const string Translate = "/translate?key={0}";
            public const string GetLanguages = "/getLangs?ui=en&key={0}";
        }

        private bool _disposed;
        private string[] _supportedLanguages;
        private const int MaxCharsInTexts = 10000;

        public TranslationProvider()
        {
            _supportedLanguages = null;
        }

        private PluginConfig PluginConfig => this.GetPluginConfig<PluginConfig>();

        public IPluginSystemContext PluginSystemContext { get; set; }

        protected ILogger Logger => PluginSystemContext.Logger;

        protected IUtilityHelper UtilityHelper => PluginSystemContext.UtilityHelper;

        public string Key { get; } = "Yandex.Translation";

        public string DisplayName { get; } = "Yandex Translator";

        public string Version { get; } = "1.0";

        public string Help { get; } = "";

        public bool IsConfigured => !ApiKey.IsNullOrEmpty();

        private string ApiKey => PluginConfig?.YandexTranslatorApiKey;

        public void Initialize()
        {}

        public async Task<OperationResult> TestServiceAsync(CancellationToken cancellationToken)
        {
            var result = new OperationResult<string[]>(OperationResultStatus.Failed, "Unknown error");

            if (ApiKey.IsNullOrEmpty())
            {
                result.SetError("API key for Yandex Translator is required!");
            }
            else
            {
                try
                {
                    result = await GetSupportedLanguages(cancellationToken);
                    if (result.Data.Length == 0)
                    {
                        result.SetError("Yandex Translator does not support any translations right now, please try again later.");
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

            CultureInfo sourceCulture = CultureCache.Instance.GetCulture(fromLanguage);
            var comparer = new StringWithCultureEqualityComparer(sourceCulture, false);

            if (texts.Count > 1)
            {
                texts = UtilityHelper.RemoveDuplicities(texts, x => x, comparer) as IList<string> ?? new List<string>();
            }

            async Task<List<string>> GetTranslation(string targetLang, IEnumerable<string> sourceTexts)
            {
                var body = sourceTexts.Select(x => new KeyValuePair<string, string>("text", x)).ToArray();
                var translations = new List<string>();
                string route = Routes.Translate + $"&lang={fromLanguage}-{targetLang}";
                (HttpStatusCode statusCode, var json) = await CallApi(route, body, cancellationToken);
                if (HandleStatusCode(statusCode, out var error))
                {
                    if (json is JObject jObject)
                    {
                        if (jObject.ContainsKey("text"))
                        {
                            JToken jToken = jObject["text"];
                            if (jToken is JArray jArray)
                            {
                                foreach (JToken item in jArray)
                                {
                                    if (item is JValue itemValue)
                                    {
                                        var translation = itemValue.Value<string>();
                                        if (!translation.IsNullOrEmpty())
                                        {
                                            translation = WebUtility.UrlDecode(translation);
                                            translations.Add(translation);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    result.Status = OperationResultStatus.Success;
                }
                else
                {
                    result.SetError(error);
                }

                return translations;
            }

            //string[] encodedTexts = texts.Select(WebUtility.UrlEncode).ToArray();
            string[] encodedTexts = texts.ToArray();
            var batch = UtilityHelper.PartitionTextOnLength(encodedTexts, MaxCharsInTexts);

            foreach (IEnumerable<string> batchedTexts in batch)
            {
                var batchList = batchedTexts.ToList();

                foreach (string targetLanguage in targetLanguages)
                {
                    var translations = await GetTranslation(targetLanguage, batchList);

                    for (int i = 0; i < translations.Count; i++)
                    {
                        string sourceText = batchList[i];
                        var translation = translations[i];
                        result.AddTranslation(sourceText, targetLanguage, translation);
                    }
                }
            }

            return result;
        }

        private Task<(HttpStatusCode statusCode, JToken json)> CallApi(string route,  
            CancellationToken cancellationToken = default)
        {
            return CallApi(route, null, cancellationToken);
        }

        private async Task<(HttpStatusCode statusCode, JToken json)> CallApi(string route,
            IEnumerable<KeyValuePair<string, string>> formKeyValues, CancellationToken cancellationToken = default)
        {
            HttpStatusCode statusCode;
            JToken json = default;
            HttpMethod httpMethod = formKeyValues == null ? HttpMethod.Get : HttpMethod.Post;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var requestMessage = new HttpRequestMessage())
                    {
                        requestMessage.Method = httpMethod;
                        requestMessage.RequestUri = new Uri(YandexHostUrl + string.Format(route, ApiKey));

                        if (formKeyValues != null)
                        {
                            requestMessage.Content = new FormUrlEncodedContent(formKeyValues);
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
                Logger.Error($"{nameof(TranslationProvider)}.{nameof(CallApi)}('{route}' / {httpMethod}, ApiKey:{ApiKey})", e);
            }

            return (statusCode, json);
        }

        public async Task<OperationResult<string[]>> GetSupportedLanguages(CancellationToken cancellationToken)
        {
            var result = new OperationResult<string[]>(OperationResultStatus.Failed);

            if (_supportedLanguages == null || _supportedLanguages.Length == 0)
            {
                string route = Routes.GetLanguages;// + "?ui=en";
                (HttpStatusCode statusCode, var jtoken) = await CallApi(route, cancellationToken);
                if (HandleStatusCode(statusCode, out var error))
                {
                    var languages = new List<string>();

                    if (jtoken is JObject json && json.ContainsKey("langs"))
                    {
                        foreach (JToken child in json["langs"].Children())
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