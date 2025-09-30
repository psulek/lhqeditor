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
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using LHQ.Data.Interfaces.Key;

namespace LHQ.Data
{
    public sealed class GuidModelElementKeyProvider : DisposableObject, IModelElementKeyProvider
    {
        public IModelElementKey GenerateKey()
        {
            Guid key = Guid.NewGuid();
            return new ModelElementKey<GuidModelElementKeyProvider>(this, key.ToString("N"));
        }

        public string SerializeKey(IModelElementKey key)
        {
            ArgumentValidator.EnsureArgumentNotNull(key, "key");
            var modelElementKey = (ModelElementKey<GuidModelElementKeyProvider>)key;
            return modelElementKey.Value;
        }

        public bool TryParseKey(string value, out IModelElementKey key)
        {
            key = value.IsGuid() ? new ModelElementKey<GuidModelElementKeyProvider>(this, value) : (IModelElementKey)null;
            return key != null;
        }

        protected override void DoDispose()
        { }
    }
}
