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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using LHQ.Utils.Comparers;
using LHQ.Utils.Utilities;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.Utils.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool In<T>(this T item, params T[] collection)
        {
            IEnumerable<T> enumerable = collection;
            return In(item, enumerable);
        }

        public static bool In<T>(this T item, IEnumerable<T> collection)
        {
            foreach (T i in collection)
            {
                if (i.Equals(item))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasDuplicity<TSource, TComparer, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, TComparer comparer)
            where TKey : IComparable<TKey>
            where TComparer : IEqualityComparer<TKey>
        {
            return HasDuplicity(source, keySelector, comparer, out Dictionary<TKey, int> _);
        }

        public static bool HasDuplicity<TSource, TComparer, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, TComparer comparer, out Dictionary<TKey, int> duplicities)
            where TKey : IComparable<TKey>
            where TComparer : IEqualityComparer<TKey>
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            var equalityComparer = new GenericEqualityComparer<TSource, TKey>(keySelector, comparer);
            return InternalHasDuplicity(source, equalityComparer, out duplicities);
        }

        public static bool HasDuplicity<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            return HasDuplicity(source, keySelector, out Dictionary<TKey, int> _);
        }

        public static bool HasDuplicity<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            out Dictionary<TKey, int> duplicities)
            where TKey : IComparable<TKey>
        {
            using (var equalityComparer = new GenericEqualityComparer<TSource, TKey>(keySelector))
            {
                return InternalHasDuplicity(source, equalityComparer, out duplicities);
            }
        }

        private static bool InternalHasDuplicity<TSource, TKey>(IEnumerable<TSource> source,
            GenericEqualityComparer<TSource, TKey> equalityComparer, out Dictionary<TKey, int> duplicities)
            where TKey : IComparable<TKey>
        {
            ArgumentValidator.EnsureArgumentNotNull(source, "source");

            List<TSource> list;
            duplicities = null;

            if (source is List<TSource>)
            {
                list = (List<TSource>)source;
            }
            else
            {
                list = new List<TSource>(source);
            }

            equalityComparer.StoreEquals = true;

            int allCount = list.Count;
            List<TSource> distinctList = list.Distinct(equalityComparer).ToList();
            int distinctCount = distinctList.Count;
            bool hasDuplicity = allCount != distinctCount;
            if (hasDuplicity)
            {
                Dictionary<TKey, int> equalsDict = equalityComparer.GetEquals();
                duplicities = equalsDict.Where(x => x.Value > 0).ToDictionary(x => x.Key, x => x.Value);
            }

            return hasDuplicity;
        }

        public static void RemoveDuplicates<TSource, TKey>(this IList<TSource> source, Func<TSource, TKey> getKeyFunc,
            Dictionary<TKey, int> duplicities)
        {
            ArgumentValidator.EnsureArgumentNotNull(source, "source");
            ArgumentValidator.EnsureArgumentNotNull(duplicities, "duplicities");

            if (duplicities.Count > 0)
            {
                foreach (KeyValuePair<TKey, int> duplicityItem in duplicities)
                {
                    TKey key = duplicityItem.Key;
                    int duplicityCount = duplicityItem.Value;

                    if (duplicityCount > 0)
                    {
                        var projected = source
                            .Select((item, index) => new { item, index })
                            .Where(x => getKeyFunc(x.item).Equals(key))
                            .OrderByDescending(item => item.index)
                            .Take(duplicityCount)
                            .ToList();
                        if (projected.Count > 0)
                        {
                            foreach (var item in projected)
                            {
                                source.RemoveAt(item.index);
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<TItem> One<TItem>(TItem value)
        {
            return new List<TItem> { value };
        }

        // public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T value)
        // {
        //     if (source != null)
        //     {
        //         foreach (T item in source)
        //         {
        //             yield return item;
        //         }
        //     }
        //
        //     yield return value;
        // }

        /// <summary>
        ///     Joins the collection to string with separator.
        /// </summary>
        /// <param name="source">collection</param>
        /// <param name="separator">separator</param>
        /// <param name="decorateItemWith">The decorate item with.</param>
        /// <returns>joined string with separator</returns>
        public static string ToDelimitedString(this IEnumerable source, string separator = ",", string decorateItemWith = "")
        {
            ArgumentValidator.EnsureArgumentNotNull(source, "source");

            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(separator))
            {
                foreach (object item in source)
                {
                    string itemStr = string.Format("{0}{1}{2}", decorateItemWith ?? string.Empty, item,
                        decorateItemWith ?? string.Empty);

                    builder.Append(itemStr);
                    builder.Append(separator);
                }

                int separatorLength = separator.Length;
                if (builder.Length > 0)
                {
                    builder.Remove(builder.Length - separatorLength, separatorLength);
                }
            }
            else
            {
                foreach (object item in source)
                {
                    builder.Append(item);
                }
            }

            return builder.ToString();
        }

        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ReadOnlyCollection<T>(enumerable.ToList());
        }

        public static void Run<T>(this IEnumerable<T> enumerable, Action<T> predicate)
        {
            ArgumentValidator.EnsureArgumentNotNull(enumerable, "enumerable");

            foreach (T item in enumerable)
            {
                predicate(item);
            }
        }

        /// <summary>
        ///     Code from http://codereview.stackexchange.com/a/37211
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <param name="comparer"></param>
        public static int AddSorted<T>(this IList<T> list, T item, IComparer<T> comparer = null)
        {
            int index = GetIndexForAddSorted(list, item, comparer);
            list.Insert(index, item);
            return index;
        }

        public static int GetIndexForAddSorted<T>(this IList<T> list, T item, IComparer<T> comparer = null)
        {
            if (comparer == null)
            {
                comparer = Comparer<T>.Default;
            }

            var index = 0;
            while (index < list.Count && comparer.Compare(list[index], item) < 0)
            {
                index++;
            }

            return index;
        }

        public static int FindItemIndex<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            int result = -1;
            var list = enumerable as IList;

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (predicate((T)list[i]))
                    {
                        result = i;
                        break;
                    }
                }
            }
            else
            {
                int i = 0;
                foreach (T item in enumerable)
                {
                    if (predicate(item))
                    {
                        result = i;
                        break;
                    }
                    i++;
                }
            }

            return result;
        }

        public static IEnumerable<TSource> RemoveDuplicities<TSource, TKey>(IEnumerable<TSource> source, 
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
            where TKey : IComparable<TKey>
        {
            IList<TSource> list;

            if (source is IList<TSource> listSource)
            {
                list = listSource;
            }
            else
            {
                list = new List<TSource>(source);
            }

            if (HasDuplicity(list, keySelector, comparer, out var duplicities))
            {
                RemoveDuplicates(list, keySelector, duplicities);
            }

            return list;
        }

        public static IEnumerable<IEnumerable<string>> PartitionTextOnLength(IList<string> texts, int maxCharsInTexts)
        {
            bool hasData = texts.Count > 0;
            int charCount = 0;
            bool allowMoveNext = true;

            var enumerator = texts.GetEnumerator();

            IEnumerable<string> GetBatch()
            {
                do
                {
                    hasData = !allowMoveNext || enumerator.MoveNext();
                    if (hasData)
                    {
                        string current = enumerator.Current;
                        int newCharCount = charCount + (current ?? string.Empty).Length;

                        allowMoveNext = true;

                        if (newCharCount <= maxCharsInTexts)
                        {
                            charCount = newCharCount;
                            yield return current;
                        }
                        else
                        {
                            allowMoveNext = false;
                            charCount = 0;
                            break;
                        }
                    }
                } while (hasData && charCount <= maxCharsInTexts);
            }

            do
            {
                yield return GetBatch();
            } while (hasData);
        }
        
        public static List<List<string>> PartitionTextOnLength2(IList<string> texts, int maxCharsInTexts)
        {
            var resultList = new List<List<string>>();
            List<string> currentList = null;
            int charCount = 0;
            foreach (string current in texts)
            {
                int newCharCount = charCount + (current ?? string.Empty).Length;

                if (newCharCount <= maxCharsInTexts)
                {
                    charCount = newCharCount;
                }
                else
                {
                    charCount = 0;
                    currentList = null;
                }

                if (currentList == null)
                {
                    currentList = new List<string>();
                    resultList.Add(currentList);
                }

                currentList.Add(current);
            }

            return resultList;
        }
    }
}
