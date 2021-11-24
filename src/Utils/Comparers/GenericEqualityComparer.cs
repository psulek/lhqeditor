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
using LHQ.Utils.Utilities;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace LHQ.Utils.Comparers
{
    // Original code was copied from: http://codepaste.net/5x7x6w

    /// <summary>
    ///     Implements a projection strategy that select an "identity" property from an object
    ///     and uses the default comparer to implement IEqualityComparer<typeparamref name="T" />
    /// </summary>
    /// <typeparam name="T">Type of object to extract identity from</typeparam>
    /// <typeparam name="TRet">Type of the extracted identity value</typeparam>
    public class GenericEqualityComparer<T, TRet> : DisposableObject, IEqualityComparer<T>
        where TRet : IComparable<TRet>
    {
        private static readonly EqualityComparer<TRet> _defaultComparer = EqualityComparer<TRet>.Default;
        private readonly Func<T, TRet> _projector;
        private readonly IEqualityComparer<TRet> _comparer;

        private Dictionary<TRet, int> _equalList;
        private bool _storeEquals;

        public GenericEqualityComparer(Func<T, TRet> projector)
            : this(projector, _defaultComparer)
        { }

        public GenericEqualityComparer(Func<T, TRet> projector, IEqualityComparer<TRet> comparer)
        {
            ArgumentValidator.EnsureArgumentNotNull(projector, "projector");
            ArgumentValidator.EnsureArgumentNotNull(comparer, "comparer");

            _projector = projector;
            _comparer = comparer;
            StoreEquals = true;
        }

        public bool StoreEquals
        {
            get => _storeEquals;
            set
            {
                _storeEquals = value;
                _equalList = value ? new Dictionary<TRet, int>() : null;
            }
        }

        /// <summary>
        ///     Get dictionary of equal values (TRet), where in dictionary key = Tret, value = count of duplicates
        /// </summary>
        /// <returns></returns>
        public Dictionary<TRet, int> GetEquals()
        {
            return _equalList;
        }

        public bool Equals(T x, T y)
        {
            TRet valueX = _projector(x);
            TRet valueY = _projector(y);

            bool equals = _comparer.Equals(valueX, valueY);
            if (equals && _storeEquals && _equalList != null)
            {
                if (!_equalList.ContainsKey(valueX))
                {
                    _equalList.Add(valueX, 1);
                }
                else
                {
                    _equalList[valueX]++;
                }
            }
            return equals;
        }

        public int GetHashCode(T obj)
        {
            return _comparer.GetHashCode(_projector(obj));
        }

        protected override void DoDispose()
        {
            _equalList = null;
        }
    }

    public class GenericEqualityComparer<T, TRet, TComparer> : GenericEqualityComparer<T, TRet>
        where TRet : IComparable<TRet>
        where TComparer : IEqualityComparer<TRet>, new()
    {
        public GenericEqualityComparer(Func<T, TRet> projector)
            : base(projector, new TComparer())
        { }
    }
}
