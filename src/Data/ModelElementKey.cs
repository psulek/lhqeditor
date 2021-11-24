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
using LHQ.Utils.Utilities;
using LHQ.Data.Interfaces.Key;

namespace LHQ.Data
{
    public struct ModelElementKey<T> : IModelElementKey
        where T : IModelElementKeyProvider
    {
        private readonly T _provider;

        public string Value { get; }

        public ModelElementKey(T provider, string value)
            : this()
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(value, "value");
            ArgumentValidator.EnsureArgumentNotNull(provider, "provider");

            Value = value;
            _provider = provider;
        }

        public bool Equals(IModelElementKey other)
        {
            if (other is ModelElementKey<T> otherKey)
            {
                return string.Equals(Value, otherKey.Value);
            }

            return false;
        }

        public IModelElementKey Clone()
        {
            string serialized = _provider.SerializeKey(this);
            _provider.TryParseKey(serialized, out IModelElementKey clonedKey);
            return clonedKey;
        }

        public override string ToString()
        {
            return Serialize();
        }

        public string Serialize()
        {
            return _provider.SerializeKey(this);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is ModelElementKey<T> key && Equals(key);
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }

        int IComparer<IModelElementKey>.Compare(IModelElementKey x, IModelElementKey y)
        {
            return new ModelElementKeyComparer().Compare(x, y);
        }
    }
}
