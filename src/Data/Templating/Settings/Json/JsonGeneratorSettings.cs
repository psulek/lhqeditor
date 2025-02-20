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

namespace LHQ.Data.Templating.Settings.Json
{
    public class JsonGeneratorSettings : GeneratorSettingsBase
    {
        public bool CultureCodeInFileNameForPrimaryLanguage { get; set; }

        public bool WriteEmptyValues { get; set; }

        public string MetadataFileNameSuffix { get; set; }

        public JsonGeneratorSettings()
        {
            CultureCodeInFileNameForPrimaryLanguage = true;
            MetadataFileNameSuffix = "metadata";
            WriteEmptyValues = true;
        }

        public override void AssignFrom(GeneratorSettingsBase other)
        {
            base.AssignFrom(other);

            if (other is JsonGeneratorSettings otherJson)
            {
                CultureCodeInFileNameForPrimaryLanguage = otherJson.CultureCodeInFileNameForPrimaryLanguage;
                MetadataFileNameSuffix = otherJson.MetadataFileNameSuffix;
                WriteEmptyValues = otherJson.WriteEmptyValues;
            }
        }

        public override void Serialize(DataNode node, int modelVersion)
        {
            base.Serialize(node, modelVersion);

            node.AddAttribute(nameof(CultureCodeInFileNameForPrimaryLanguage), DataNodeValueHelper.ToString(CultureCodeInFileNameForPrimaryLanguage));
            node.AddAttribute(nameof(MetadataFileNameSuffix), MetadataFileNameSuffix);
            node.AddAttribute(nameof(WriteEmptyValues), DataNodeValueHelper.ToString(WriteEmptyValues));
        }

        public override bool Deserialize(DataNode node)
        {
            var result = base.Deserialize(node);
            if (result)
            {
                // CultureCodeInFileNameForPrimaryLanguage
                if (node.Attributes.Contains(nameof(CultureCodeInFileNameForPrimaryLanguage)))
                {
                    var attrCultureCodeInFileNameForPrimaryLanguage = node.Attributes[nameof(CultureCodeInFileNameForPrimaryLanguage)];

                    if (!attrCultureCodeInFileNameForPrimaryLanguage.Value.IsNullOrEmpty())
                    {
                        CultureCodeInFileNameForPrimaryLanguage = DataNodeValueHelper.FromString(attrCultureCodeInFileNameForPrimaryLanguage.Value, false);
                    }
                }

                // MetadataFileNameSuffix
                if (node.Attributes.Contains(nameof(MetadataFileNameSuffix)))
                {
                    MetadataFileNameSuffix = node.Attributes[nameof(MetadataFileNameSuffix)].Value;
                }

                // WriteEmptyValues
                if (node.Attributes.Contains(nameof(WriteEmptyValues)))
                {
                    var attrWriteEmptyValues = node.Attributes[nameof(WriteEmptyValues)];

                    if (!attrWriteEmptyValues.Value.IsNullOrEmpty())
                    {
                        WriteEmptyValues = DataNodeValueHelper.FromString(attrWriteEmptyValues.Value, false);
                    }
                }
            }

            return result; 
        }

        public override SettingsValidationError Validate()
        {
            var error = base.Validate();
            if (error == null)
            {
                if (MetadataFileNameSuffix.IsNullOrEmpty())
                {
                    error = SettingsValidationError.Error("Metadata file name suffix");
                }
            }

            return error;
        }
    }
}