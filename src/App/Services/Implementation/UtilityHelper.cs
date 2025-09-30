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
using LHQ.App.Code;
using LHQ.Core.DependencyInjection;
using LHQ.Core.Interfaces;
using LHQ.Utils.Extensions;
//using MoreLinq.Extensions;

namespace LHQ.App.Services.Implementation
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UtilityHelper : AppContextServiceBase, IUtilityHelper
    {
        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        {
        }

        public DateTime GetUtcDateTime()
        {
            return DateTimeHelper.UtcNow;
        }

        public IEnumerable<IEnumerable<TSource>> Batch<TSource>(IEnumerable<TSource> source, int size)
        {
            return MoreLinq.MoreEnumerable.Batch(source, size);
        }

        public IEnumerable<TSource> ExceptBy<TSource, TKey>(IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector)
        {
            return MoreLinq.MoreEnumerable.ExceptBy(first, second, keySelector);
        }

        public bool HasDuplicities<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, out Dictionary<TKey, int> duplicities)
            where TKey : IComparable<TKey>
        {
            return source.HasDuplicity(keySelector, out duplicities);
        }

        public bool HasDuplicities<TSource, TComparer, TKey>(IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, TComparer comparer, out Dictionary<TKey, int> duplicities)
            where TKey : IComparable<TKey>
            where TComparer : IEqualityComparer<TKey>
        {
            return source.HasDuplicity(keySelector, comparer, out duplicities);
        }

        public IEnumerable<TSource> RemoveDuplicities<TSource, TComparer, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, TComparer comparer)
            where TComparer : IEqualityComparer<TKey>
            where TKey : IComparable<TKey>
        {
            return EnumerableExtensions.RemoveDuplicities(source, keySelector, comparer);
        }


        public IEnumerable<IEnumerable<string>> PartitionTextOnLength(IList<string> texts, int maxCharsInTexts)
        {
            return EnumerableExtensions.PartitionTextOnLength(texts, maxCharsInTexts);
        }

        public List<List<string>> PartitionTextOnLength2(IList<string> texts, int maxCharsInTexts)
        {
            return EnumerableExtensions.PartitionTextOnLength2(texts, maxCharsInTexts);
        }
    }
}