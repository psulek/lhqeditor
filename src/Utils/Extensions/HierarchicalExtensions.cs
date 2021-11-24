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
using System.Linq;
using LHQ.Utils.GenericTree;
using LHQ.Utils.Utilities;
// ReSharper disable UnusedMember.Global

namespace LHQ.Utils.Extensions
{
    public static class HierarchicalExtensions
    {
        public static List<T> FlattenToList<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getChildItems, IEnumerable<T> getRootItems = null)
            where T : class
        {
            ArgumentValidator.EnsureArgumentNotNull(source, "source");
            var result = new List<T>();

            if (getRootItems == null)
            {
                getRootItems = source.ToList();
            }

            IList<T> rooItems = getRootItems as IList<T> ?? getRootItems.ToList();

            var tree = new GenericTreeIterator<T>(getChildItems, () => rooItems);
            tree.IterateTree(false, null, x => result.Add(x.Current));
            return result;
        }

        public static IEnumerable<LevelAndData<T>> FlattenHierarchyWithLevel<T>(this T node, Func<T, IEnumerable<T>> getChildEnumerator)
        {
            yield return new LevelAndData<T> { Level = 1, Data = node };
            if (getChildEnumerator(node) != null)
            {
                foreach (T child in getChildEnumerator(node))
                {
                    foreach (LevelAndData<T> childOrDescendant in child.FlattenHierarchyWithLevel(getChildEnumerator))
                    {
                        yield return new LevelAndData<T> { Level = childOrDescendant.Level + 1, Data = childOrDescendant.Data };
                    }
                }
            }
        }

        public class LevelAndData<T>
        {
            public int Level { get; set; }

            public T Data { get; set; }
        }
    }
}
