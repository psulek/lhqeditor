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
using LHQ.Data.Support;
using LHQ.Utils;
using LHQ.Utils.Extensions;

namespace LHQ.Data.Templating.Settings.WinForms
{
    public class CSharpWinFormsGeneratorSettings : CSharpGeneratorSettingsBase
    {
        public CSharpWinFormsGeneratorSettings()
        {
            GenerateParamsMethods = true;
            ParamsMethodsSuffix = "WithParams";
            MissingTranslationFallbackToPrimary = false;
        }

        public bool GenerateParamsMethods { get; set; }

        public string ParamsMethodsSuffix { get; set; }

        public bool MissingTranslationFallbackToPrimary { get; set; }

        public override void AssignFrom(GeneratorSettingsBase other)
        {
            base.AssignFrom(other);

            if (other is CSharpWinFormsGeneratorSettings other2)
            {
                GenerateParamsMethods = other2.GenerateParamsMethods;
                ParamsMethodsSuffix = other2.ParamsMethodsSuffix;
                MissingTranslationFallbackToPrimary = other2.MissingTranslationFallbackToPrimary;
            }
        }

        public override SettingsValidationError Validate()
        {
            var error = base.Validate();
            if (error == null)
            {
                switch (NameValidator.Validate(ParamsMethodsSuffix))
                {
                    case NameValidatorResult.Valid:
                    {
                        return null;
                    }
                    case NameValidatorResult.NameIsEmpty:
                    {
                        return SettingsValidationError.Error("'Suffix for methods with parameters' could not be empty!");
                    }
                    case NameValidatorResult.NameCannotBeginWithNumber:
                    {
                        return SettingsValidationError.Error("'Suffix for methods with parameters' cannot start with number!");
                    }
                    case NameValidatorResult.NameCanContainOnlyAlphaNumeric:
                    {
                        return SettingsValidationError.Error("'Suffix for methods with parameters' can contain only alpha numeric characters!");
                    }
                    default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return error;
        }

        public override void Serialize(DataNode node)
        {
            base.Serialize(node);

            node.AddAttribute(nameof(GenerateParamsMethods), DataNodeValueHelper.ToString(GenerateParamsMethods));
            node.AddAttribute(nameof(MissingTranslationFallbackToPrimary), DataNodeValueHelper.ToString(MissingTranslationFallbackToPrimary));

            if (!ParamsMethodsSuffix.IsNullOrEmpty())
            {
                node.AddAttribute(nameof(ParamsMethodsSuffix), ParamsMethodsSuffix);
            }
        }

        public override bool Deserialize(DataNode node)
        {
            var result = base.Deserialize(node);
            if (result)
            {
                if (node.Attributes.Contains(nameof(GenerateParamsMethods)))
                {
                    var attrGenerateParamsMethods = node.Attributes[nameof(GenerateParamsMethods)];

                    if (!attrGenerateParamsMethods.Value.IsNullOrEmpty())
                    {
                        GenerateParamsMethods = DataNodeValueHelper.FromString(attrGenerateParamsMethods.Value, true);
                    }
                }

                if (node.Attributes.Contains(nameof(MissingTranslationFallbackToPrimary)))
                {
                    var attrMissingTranslationFallbackToPrimary = node.Attributes[nameof(MissingTranslationFallbackToPrimary)];

                    if (!attrMissingTranslationFallbackToPrimary.Value.IsNullOrEmpty())
                    {
                        MissingTranslationFallbackToPrimary = DataNodeValueHelper.FromString(attrMissingTranslationFallbackToPrimary.Value, false);
                    }
                }

                ParamsMethodsSuffix = node.Attributes.Contains(nameof(ParamsMethodsSuffix)) ? node.Attributes[nameof(ParamsMethodsSuffix)].Value : null;
            }

            return result; 
        }
    }
}
