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
using System.ComponentModel;
using LHQ.Data.Support;
using LHQ.Utils;
using LHQ.Utils.Extensions;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.Data.Templating.Settings
{
    public abstract class OutputSettings : IGeneratorSettings
    {
        [DefaultValue("Resources")]
        public string OutputFolder { get; set; }

        [ReadOnly(true)]
        public string OutputProjectName { get; set; }

        [DisplayName("Encoding with BOM")]
        [Description("Whenever the encoding contains BOM (Byte Order Mark), where default is false (No BOM).")]
        public bool EncodingWithBOM { get; set; } = false;

        [DisplayName("Line endings (LF or CRLF)")]
        [Description("Line endings used when generated files are saved on disk, where default is LF.")]
        public OutputSettingsLineEndings LineEndings { get; set; } = OutputSettingsLineEndings.LF;

        protected OutputSettings()
        {
            OutputFolder = "Resources";
        }

        protected SettingsValidationError ValidateNameFor(string propertyValue, string propertyName)
        {
            return ValidateNameFor(propertyValue, propertyName, out _);
        }

        protected SettingsValidationError ValidateNameFor(string propertyValue, string propertyName,
            NameValidatorFlags nameValidatorFlags)
        {
            return ValidateNameFor(propertyValue, propertyName, nameValidatorFlags, out _);
        }

        protected SettingsValidationError ValidateNameFor(string propertyValue, string propertyName, out NameValidatorResult validatorResult)
        {
            return ValidateNameFor(propertyValue, propertyName, NameValidatorFlags.None, out validatorResult);
        }

        protected SettingsValidationError ValidateNameFor(string propertyValue, string propertyName,
            NameValidatorFlags nameValidatorFlags, out NameValidatorResult validatorResult)
        {
            validatorResult = NameValidator.Validate(propertyValue, nameValidatorFlags);
            switch (validatorResult)
            {
                case NameValidatorResult.Valid:
                {
                    return null;
                }
                case NameValidatorResult.NameIsEmpty:
                {
                    return SettingsValidationError.Error($"'{propertyName}' could not be empty!");
                }
                case NameValidatorResult.NameCannotBeginWithNumber:
                {
                    return SettingsValidationError.Error($"'{propertyName}' cannot start with number!");
                }
                case NameValidatorResult.NameCanContainOnlyAlphaNumeric:
                {
                    return SettingsValidationError.Error($"'{propertyName}' can contain only alpha numeric characters!");
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public virtual SettingsValidationError Validate()
        {
            var validationError = ValidateNameFor(OutputFolder, "Output Folder", out var validatorResult);

            switch (validatorResult)
            {
                case NameValidatorResult.NameIsEmpty:
                {
                    return validationError;
                }
                case NameValidatorResult.Valid:
                case NameValidatorResult.NameCannotBeginWithNumber:
                case NameValidatorResult.NameCanContainOnlyAlphaNumeric:
                {
                    return null;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public virtual void Serialize(DataNode node, int modelVersion)
        {
            if (!OutputProjectName.IsNullOrEmpty())
            {
                node.AddAttribute(nameof(OutputProjectName), OutputProjectName);
            }

            if (!OutputFolder.IsNullOrEmpty())
            {
                node.AddAttribute(nameof(OutputFolder), OutputFolder);
            }

            if (modelVersion > 1)
            {
                node.AddAttribute(nameof(EncodingWithBOM), DataNodeValueHelper.ToString(EncodingWithBOM));
                node.AddAttribute(nameof(LineEndings), DataNodeValueHelper.ToString(LineEndings));
            }
        }

        public virtual bool Deserialize(DataNode node)
        {
            OutputProjectName = node.Attributes.Contains(nameof(OutputProjectName)) ? node.Attributes[nameof(OutputProjectName)].Value : null;
            OutputFolder = node.Attributes.Contains(nameof(OutputFolder)) ? node.Attributes[nameof(OutputFolder)].Value : null;

            EncodingWithBOM = false;
            if (node.Attributes.Contains(nameof(EncodingWithBOM)))
            {
                var attrEncodingWithBOM = node.Attributes[nameof(EncodingWithBOM)];
                EncodingWithBOM = attrEncodingWithBOM.Value.IsNullOrEmpty() || DataNodeValueHelper.FromString(attrEncodingWithBOM.Value, false);
            }

            LineEndings = OutputSettingsLineEndings.LF;
            if (node.Attributes.Contains(nameof(LineEndings)))
            {
                var attrLineEndings = node.Attributes[nameof(LineEndings)];
                if (!attrLineEndings.Value.IsNullOrEmpty())
                {
                    LineEndings = DataNodeValueHelper.EnumFromString(attrLineEndings.Value, OutputSettingsLineEndings.LF);
                }
            }

            return true;
        }
    }
    
    public enum OutputSettingsLineEndings
    {
        LF,
        CRLF
    }
}
