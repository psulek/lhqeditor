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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using LHQ.Utils.Extensions;
using LHQ.Data.Interfaces;
using LHQ.Data.Interfaces.Key;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.Data
{
    public class ModelElementList<T> : Collection<T>
        where T : ModelElementBase
    {
        private bool _namedElements;
        private string _thisType;

        public ModelElementList()
        {
            Initialize();
        }

        public ModelElementList(List<T> list) : base(list)
        {
            Initialize();
        }

        public ModelElementList(IList<T> list) : base(list)
        {
            Initialize();
        }

        private void Initialize()
        {
            Type type = typeof(T);
            _thisType = type.Name;
            _namedElements = ModelWellKnowTypes.ModelNamedElement.IsAssignableFrom(type);
        }

        public void AddRange(IList<T> collection)
        {
            foreach (T item in collection)
            {
                Add(item);
            }
        }

        public void RemoveAll(Predicate<T> predicate)
        {
            foreach (T item in this.Where(x => predicate(x)).ToArray())
            {
                Remove(item);
            }
        }

        public bool ContainsByKey(IModelElementKey elementKey)
        {
            return FindByKey(elementKey) != null;
        }

        public T FindByKey(IModelElementKey elementKey)
        {
            return this.SingleOrDefault(x => x.Key.Equals(elementKey));
        }

        protected T FindByNameInternal(string name, bool ignoreCase, CultureInfo cultureInfo)
        {
            if (!_namedElements)
            {
                throw new InvalidOperationException(
                    "Type '{0}' does not implement model named type '{1}'!".FormatWith(_thisType,
                        ModelWellKnowTypes.ModelNamedElement.Name));
            }

            return this.Cast<IModelNamedElement>().SingleOrDefault(x => x.Name.EqualsTo(name, ignoreCase, cultureInfo)) as T;
        }

        protected bool ContainsByNameInternal(string name, bool ignoreCase, CultureInfo cultureInfo = null)
        {
            cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
            return this.Cast<IModelNamedElement>().Any(x => x.Name.EqualsTo(name, ignoreCase, cultureInfo));
        }
    }
}
