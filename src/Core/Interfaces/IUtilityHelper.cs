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
using LHQ.Core.DependencyInjection;
// ReSharper disable UnusedMember.Global

namespace LHQ.Core.Interfaces
{
    public interface IUtilityHelper: IService
    {
        /// <summary>
        /// Gets current UTC date time. This date time is secured and is not able to be changed back and front in system date time settings.
        /// It counts time from started app instance.
        /// </summary>
        /// <returns></returns>
        DateTime GetUtcDateTime();

        /// <summary>
        /// Batches the source sequence into sized buckets.
        /// </summary>
        /// <param name="source">Source list of items</param>
        /// <param name="size">Size of buckets.</param>
        /// <returns>
        /// A sequence of equally sized buckets containing elements of the source collection.
        /// </returns>
        IEnumerable<IEnumerable<TSource>> Batch<TSource>(IEnumerable<TSource> source, int size);

        /// <summary>
        /// Returns the set of elements in the first sequence which aren't in the second sequence, according to a given key selector.
        /// </summary>
        /// <param name="first">The sequence of potentially included elements.</param>
        /// <param name="second">The sequence of elements whose keys may prevent elements in first from being returned.</param>
        /// <param name="keySelector">The mapping from source element to key.</param>
        /// <returns>
        /// A sequence of elements from first whose key was not also a key for any element in second.
        /// </returns>
        IEnumerable<TSource> ExceptBy<TSource, TKey>(IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector);

        bool HasDuplicities<TSource, TKey>(IEnumerable<TSource> source, 
            Func<TSource, TKey> keySelector, 
            out Dictionary<TKey, int> duplicities)
            where TKey : IComparable<TKey>;

        bool HasDuplicities<TSource, TComparer, TKey>(IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, TComparer comparer, out Dictionary<TKey, int> duplicities)
            where TKey : IComparable<TKey>
            where TComparer : IEqualityComparer<TKey>;

        IEnumerable<TSource> RemoveDuplicities<TSource, TComparer, TKey>(IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, TComparer comparer)
            where TKey : IComparable<TKey>
            where TComparer : IEqualityComparer<TKey>;

        IEnumerable<IEnumerable<string>> PartitionTextOnLength(IList<string> texts, int maxCharsInTexts);
        List<List<string>> PartitionTextOnLength2(IList<string> texts, int maxCharsInTexts);
    }
}
