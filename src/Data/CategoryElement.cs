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
using LHQ.Data.Interfaces;
using LHQ.Data.Interfaces.Key;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.Data
{
    public sealed class CategoryElement : ModelElement<CategoryElement>, ICategoryLikeElement
    {
        public CategoryElement() : this(null)
        { }

        public CategoryElement(IModelElementKey key) : this(key, null)
        { }

        public CategoryElement(IModelElementKey key, CategoryElement parentCategory)
            : base(key, parentCategory)
        {
            Categories = new CategoryElementList();
            Resources = new ResourceElementList();
            Description = string.Empty;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public CategoryElementList Categories { get; set; }

        public ResourceElementList Resources { get; set; }

        public override ModelElementType ElementType => ModelElementType.Category;

        public bool IsRoot => ParentKey == null;

        protected override void InternalToString(StringBuilder sb)
        {
            base.InternalToString(sb);
            sb.AppendFormat(" SubCategCount: {0}, ResCount: {1}", Categories.Count, Resources.Count);
        }

        public ITreeLikeElement Parent => ParentElement;
    }
}
