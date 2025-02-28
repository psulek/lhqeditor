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

using System.Text;
using System.Xml.Serialization;
using LHQ.Data.Interfaces.Key;

namespace LHQ.Data
{
    public abstract class ModelElement<TParent> : ModelElement
        where TParent : ModelElement, new()
    {
        private TParent _parentElement;

        protected ModelElement()
        { }

        protected ModelElement(IModelElementKey key) : base(key)
        { }

        protected ModelElement(TParent parentElement)
            : this(null, parentElement)
        { }

        protected ModelElement(IModelElementKey key, TParent parentElement)
            : base(key)
        {
            ParentElement = parentElement;
        }

        [XmlIgnore]
        public override Model Model { get; set; }

        [XmlIgnore]
        public override IModelElementKey ParentKey { get; set; }

        [XmlIgnore]
        public TParent ParentElement
        {
            get => _parentElement;
            set
            {
                _parentElement = value;
                ParentKey = value?.Key;
            }
        }

        public override ModelElement GetParentElement()
        {
            return ParentElement;
        }

        internal override void SetParentElement(ModelElement modelElement)
        {
            ParentElement = (TParent)modelElement;
        }

        protected override void InternalToString(StringBuilder sb)
        {
            var parent = "-";
            if (_parentElement != null)
            {
                parent = ParentElement.ToShortIdentString();
            }
            sb.AppendFormat(" Parent: {0}", parent);
        }
    }
}
