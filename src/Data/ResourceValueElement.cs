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
using System.Text;
using LHQ.Utils.Extensions;
using LHQ.Data.Interfaces.Key;
using Newtonsoft.Json;
// ReSharper disable UnusedMember.Global

namespace LHQ.Data
{
    public class ResourceValueElement : ModelElement<ResourceElement>
    {
        public ResourceValueElement()
        { }

        public ResourceValueElement(IModelElementKey key) : base(key)
        { }

        public ResourceValueElement(IModelElementKey key, ResourceElement parentResource)
            : base(key, parentResource)
        { }

        public string LanguageName { get; set; }

        public string Value { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whenever resource value is locked for new edit (<c>true</c>) or is translatable (<c>false</c>).
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whenever resource value was automatically translated by translation service.
        /// </summary>
        public bool Auto { get; set; }

        public override ModelElementType ElementType => ModelElementType.ResourceValue;

        protected override void InternalToString(StringBuilder sb)
        {
            base.InternalToString(sb);

            string locked = Locked ? "Locked, " : string.Empty;
            string auto = Auto ? "Auto, " : string.Empty;
            sb.Append($" Lng: {LanguageName}, {locked}, {auto}, Value: {Value.ShortenText(20)}");
        }

        public string GetEncodedValue()
        {
            if (Value.IsNullOrEmpty())
            {
                return Value;
            }

            string result = JsonConvert.SerializeObject(Value);
            return result.Substring(1, result.Length - 2);
        }

        public bool Assign(ResourceValueElement other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            bool changed = false;

            if (other.Auto != Auto)
            {
                Auto = other.Auto;
                changed = true;
            }

            if (other.Locked != Locked)
            {
                Locked = other.Locked;
                changed = true;
            }

            if (other.LanguageName != LanguageName)
            {
                LanguageName = other.LanguageName;
                changed = true;
            }

            if (other.Value != Value)
            {
                Value = other.Value;
                changed = true;
            }

            return changed;
        }
    }
}
