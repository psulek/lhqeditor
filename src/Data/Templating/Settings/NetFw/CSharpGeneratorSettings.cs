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

using LHQ.Data.Support;
using LHQ.Utils.Extensions;

namespace LHQ.Data.Templating.Settings.NetFw
{
    public class CSharpGeneratorSettings : CSharpGeneratorSettingsBase
    {
        public CSharpGeneratorSettings()
        {
            MissingTranslationFallbackToPrimary = false;
        }

        public bool MissingTranslationFallbackToPrimary { get; set; }

        public override void AssignFrom(GeneratorSettingsBase other)
        {
            base.AssignFrom(other);

            if (other is CSharpGeneratorSettings otherSettings)
            {
                MissingTranslationFallbackToPrimary = otherSettings.MissingTranslationFallbackToPrimary;
            }
        }

        public override void Serialize(DataNode node)
        {
            base.Serialize(node);

            node.AddAttribute(nameof(MissingTranslationFallbackToPrimary), DataNodeValueHelper.ToString(MissingTranslationFallbackToPrimary));
        }

        public override bool Deserialize(DataNode node)
        {
            var result = base.Deserialize(node);
            if (result)
            {
                if (node.Attributes.Contains(nameof(MissingTranslationFallbackToPrimary)))
                {
                    var attrMissingTranslationFallbackToPrimary = node.Attributes[nameof(MissingTranslationFallbackToPrimary)];

                    if (!attrMissingTranslationFallbackToPrimary.Value.IsNullOrEmpty())
                    {
                        MissingTranslationFallbackToPrimary = DataNodeValueHelper.FromString(attrMissingTranslationFallbackToPrimary.Value, false);
                    }
                }
            }

            return result; 
        }
    }
}
