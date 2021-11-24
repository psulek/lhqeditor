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

using System.Globalization;
using System.Linq;
using System.Text;
using LHQ.Utils.Extensions;
using LHQ.Data.Interfaces;
using LHQ.Data.Interfaces.Key;

namespace LHQ.Data
{
    public class ResourceElement : ModelElement<CategoryElement>, ITreeLikeElement
    {
        public ResourceElement() : this(null)
        { }

        public ResourceElement(IModelElementKey key) : this(key, null)
        { }

        public ResourceElement(IModelElementKey key, CategoryElement parentCategory) : base(key, parentCategory)
        {
            Parameters = new ResourceParameterElementList();
            Values = new ModelElementList<ResourceValueElement>();
            Description = string.Empty;
        }

        /// <summary>
        ///     Gets or sets name of element
        /// </summary>
        public string Name { get; set; }

        public string Description { get; set; }

        public string TranslationNote { get; set; }

        public ResourceElementTranslationState State { get; set; }

        public bool IsRoot => ParentKey == null;

        public ResourceParameterElementList Parameters { get; set; }

        public ModelElementList<ResourceValueElement> Values { get; set; }

        public override ModelElementType ElementType => ModelElementType.Resource;

        public bool ContainsLanguage(string languageName)
        {
            return Values?.Any(x => x.LanguageName.EqualsTo(languageName, true, CultureInfo.InvariantCulture)) == true;
        }

        public ResourceValueElement FindValueByLanguage(string languageName)
        {
            return Values?.SingleOrDefault(x => x.LanguageName.EqualsTo(languageName, true, CultureInfo.InvariantCulture));
        }

        protected override void InternalToString(StringBuilder sb)
        {
            base.InternalToString(sb);

            sb.AppendFormat(" ParamCount: {0}, ValuesCount: {1}", Parameters.Count, Values.Count);
        }

        public ITreeLikeElement Parent => ParentElement;
    }
}
