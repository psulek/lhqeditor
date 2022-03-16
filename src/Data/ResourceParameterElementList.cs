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
using System.Globalization;
using LHQ.Data.Interfaces;
using LHQ.Utils;
// ReSharper disable UnusedMember.Global

namespace LHQ.Data
{
    public sealed class ResourceParameterElementList : ModelElementList<ResourceParameterElement>, IFindByNameList<ResourceParameterElement>, IClonable
    {
        public ResourceParameterElementList()
        { }

        public ResourceParameterElementList(List<ResourceParameterElement> list) : base(list)
        { }

        public ResourceParameterElementList(IList<ResourceParameterElement> collection) : base(collection)
        { }

        public ResourceParameterElement FindByName(string name, bool ignoreCase, CultureInfo cultureInfo)
        {
            return FindByNameInternal(name, ignoreCase, cultureInfo);
        }

        public bool ContainsByName(string name, bool ignoreCase, CultureInfo cultureInfo = null)
        {
            return ContainsByNameInternal(name, ignoreCase, cultureInfo);
        }

        object IClonable.Clone()
        {
            return Clone();
        }

        public ResourceParameterElementList Clone()
        {
            var cloned = new ResourceParameterElementList();
            foreach (ResourceParameterElement resourceParameter in this)
            {
                cloned.Add(resourceParameter.Clone());
            }

            return cloned;
        }

        public bool HasEqualBaseProps(ResourceParameterElementList other)
        {
            if (other == null || other.Count != Count)
            {
                return false;
            }

            if (Count == 0)
            {
                return true;
            }

            var equal = false;

            for (var i = 0; i < Count; i++)
            {
                ResourceParameterElement currentItem = this[i];
                ResourceParameterElement otherItem = other[i];

                equal = currentItem.Name == otherItem.Name &&
                    currentItem.Description == otherItem.Description &&
                    currentItem.Order == otherItem.Order;

                if (!equal)
                {
                    break;
                }
            }

            return equal;
        }
    }
}
