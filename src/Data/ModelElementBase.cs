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

using System.Text;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using LHQ.Data.Extensions;
using LHQ.Data.Interfaces;
using LHQ.Data.Interfaces.Key;

namespace LHQ.Data
{
    public abstract class ModelElementBase
    {
        protected ModelElementBase()
        { }

        protected ModelElementBase(IModelElementKey key) : this()
        {
            Key = key;
        }

        public IModelElementKey Key { get; protected set; }

        public abstract ModelElementType ElementType { get; }

        protected internal string ToShortIdentString()
        {
            string key = Key == null ? "-" : Key.Serialize();
            return "[{0}|k:{1}]".FormatWith(ElementType.ToShortString(), key);
        }

        protected abstract void InternalToString(StringBuilder sb);

        internal void UpdateKey(IModelElementKey key)
        {
            ArgumentValidator.EnsureArgumentNotNull(key, "key");

            Key = key;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(ToShortIdentString());
            if (this is IModelNamedElement namedElement && !namedElement.Name.IsNullOrEmpty())
            {
                sb.AppendFormat(" '{0}'", namedElement.Name);
            }
            InternalToString(sb);
            return sb.ToString();
        }
    }
}
