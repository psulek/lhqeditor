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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using LHQ.Utils.Comparers;
using LHQ.Utils.Extensions;

namespace LHQ.Core.Model.Translation
{
    public class TranslationResult: OperationResult<List<TranslationResultItem>>
    {
        private readonly StringEqualityComparer _equalityComparer = new StringEqualityComparer();
        private readonly ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim();

        public TranslationResult() : base(OperationResultStatus.Failed)
        {
            Data = new List<TranslationResultItem>();
        }

        public List<TranslationResultItem> Translations => Data;

        public bool HasValidTranslation => Message.IsNullOrEmpty() && Translations.Count > 0;

        public List<string> TargetLanguages => Translations.Select(x => x.LanguageName).Distinct(_equalityComparer).ToList();

        public void AddTranslation(string sourceText, string targetLanguage, string translation)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                Translations.Add(new TranslationResultItem(sourceText, targetLanguage, translation));
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public Dictionary<string, string> GetTranslationsByLanguage()
        {
            _lockSlim.EnterReadLock();
            try
            {
                return Translations.ToDictionary(x => x.LanguageName, x => x.Translation);
            }
            finally
            {
                _lockSlim.ExitReadLock();
            }
        }
    }

    [DebuggerDisplay("[{LanguageName}] S[{SourceText}] T[{Translation}]")]
    public class TranslationResultItem
    {
        public TranslationResultItem(string sourceText, string languageName, string translation)
        {
            SourceText = sourceText;
            LanguageName = languageName;
            Translation = translation;
        }

        public string SourceText { get; set; }

        public string LanguageName { get; private set; }

        public string Translation { get; }

        public void UpdateLanguageName(string name)
        {
            LanguageName = name;
        }
    }
}
