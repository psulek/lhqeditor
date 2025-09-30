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
using LHQ.Data.Interfaces;
using LHQ.Data.Interfaces.Key;
using LHQ.Data.Metadata;

namespace LHQ.Data
{
    public class Model : ModelElementBase, ICategoryLikeElement
    {
        public Model() : this(null)
        { }

        public Model(IModelElementKey key) : base(key)
        {
            Languages = new LanguageElementList();
            Categories = new CategoryElementList();
            Resources = new ResourceElementList();
            Metadata = new ModelMetadataList();
            Version = ModelConstants.CurrentModelVersion;
            Options = new ModelOptions();
            Description = string.Empty;
        }

        public string Name { get; set; }

        public LanguageElementList Languages { get; internal set; }

        public CategoryElementList Categories { get; internal set; }

        public ResourceElementList Resources { get; internal set; }

        public ModelMetadataList Metadata { get; internal set; }

        public int ParentLevel => 0;

        public int Version { get; internal set; }

        public string Description { get; set; }

        public ModelOptions Options { get; internal set; }

        public override ModelElementType ElementType => ModelElementType.Model;

        protected override void InternalToString(StringBuilder sb)
        {
            sb.AppendFormat(" Version: {0}, LangCount: {1}, TopLevel(CategCount: {2}, ResCount: {3}, MtDt.Count: {4})",
                Version, Languages.Count, Categories.Count, Resources.Count, Metadata.Count);
        }

        public ITreeLikeElement Parent => null;
    }
}
