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

using LHQ.Data.Interfaces;
using LHQ.Data.Interfaces.Key;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.Data
{
    public class ResourceParameterElement : ModelElement<ResourceElement>, IModelNamedElement, IModelDescriptionElement
    {
        public ResourceParameterElement()
        { }

        public ResourceParameterElement(IModelElementKey key) : base(key)
        { }

        public ResourceParameterElement(IModelElementKey key, ResourceElement parentResource)
            : base(key, parentResource)
        {
            Description = string.Empty;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Order { get; set; }

        public override ModelElementType ElementType => ModelElementType.ResourceParameter;

        public ResourceParameterElement Clone()
        {
            return new ResourceParameterElement
            {
                Key = Key?.Clone(),
                Description = Description,
                Name = Name,
                Order = Order,
                Model = Model,
                ParentElement = ParentElement,
                ParentKey = ParentKey
            };
        }

        public bool Assign(ResourceParameterElement other)
        {
            bool result = false;

            if (other.Name != Name)
            {
                Name = other.Name;
                result = true;
            }
            
            if (other.Description != Description)
            {
                Description = other.Description;
                result = true;
            }

            if (other.Order != Order)
            {
                Order = other.Order;
                result = true;
            }

            return result;
        }
    }
}
