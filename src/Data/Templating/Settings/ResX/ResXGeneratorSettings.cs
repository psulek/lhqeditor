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

using LHQ.Data.Support;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.Data.Templating.Settings.ResX
{
    public class ResXGeneratorSettings : GeneratorSettingsBase
    {
        public ResXGeneratorSettings()
        {
            CultureCodeInFileNameForPrimaryLanguage = true;
        }

        public bool CultureCodeInFileNameForPrimaryLanguage { get; set; }

        /// <summary>
        /// CompatibleTextEncoding - when true - 'System.Web.HttpUtility.HtmlEncode()' is used on translated string to be written to resx <c>value</c> element,
        /// when false - new 'modern' encoding is used to only encode minimum chars which cannot be in XML element (using <see cref="XmlValueEncode"/>).
        /// </summary>
        public bool CompatibleTextEncoding { get; set; } = true;

        public override void AssignFrom(GeneratorSettingsBase other)
        {
            base.AssignFrom(other);

            if (other is ResXGeneratorSettings otherResx)
            {
                CultureCodeInFileNameForPrimaryLanguage = otherResx.CultureCodeInFileNameForPrimaryLanguage;
                CompatibleTextEncoding = otherResx.CompatibleTextEncoding;
            }
        }

        public override void Serialize(DataNode node, int modelVersion)
        {
            base.Serialize(node, modelVersion);

            node.AddAttribute(nameof(CultureCodeInFileNameForPrimaryLanguage), DataNodeValueHelper.ToString(CultureCodeInFileNameForPrimaryLanguage));
            // write only if CompatibleTextEncoding is false (for backward compatibility)
            if (!CompatibleTextEncoding)
            {
                node.AddAttribute(nameof(CompatibleTextEncoding), DataNodeValueHelper.ToString(CompatibleTextEncoding));
            }
        }

        public override bool Deserialize(DataNode node)
        {
            var result = base.Deserialize(node);
            if (result)
            {
                if (node.Attributes.Contains(nameof(CultureCodeInFileNameForPrimaryLanguage)))
                {
                    var attrCultureCodeInFileNameForPrimaryLanguage = node.Attributes[nameof(CultureCodeInFileNameForPrimaryLanguage)];

                    if (!attrCultureCodeInFileNameForPrimaryLanguage.Value.IsNullOrEmpty())
                    {
                        CultureCodeInFileNameForPrimaryLanguage = DataNodeValueHelper.FromString(attrCultureCodeInFileNameForPrimaryLanguage.Value, false);
                    }
                }

                CompatibleTextEncoding = true;
                if (node.Attributes.Contains(nameof(CompatibleTextEncoding)))
                {
                    var attrCompatibleTextEncoding = node.Attributes[nameof(CompatibleTextEncoding)];
                    if (!attrCompatibleTextEncoding.Value.IsNullOrEmpty())
                    {
                        CompatibleTextEncoding = DataNodeValueHelper.FromString(attrCompatibleTextEncoding.Value, true);
                    }
                }
            }

            return result; 
        }
    }
}
