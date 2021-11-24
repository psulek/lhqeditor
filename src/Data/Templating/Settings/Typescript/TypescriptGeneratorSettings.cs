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
using LHQ.Utils;

namespace LHQ.Data.Templating.Settings.Typescript
{
    public class TypescriptGeneratorSettings : GeneratorSettingsBase
    {
        public string AmbientNamespaceName { get; set; }

        public string InterfacePrefix { get; set; }

        public TypescriptGeneratorSettings()
        {
            OutputFolder = "typings";
            AmbientNamespaceName = null;
            InterfacePrefix = null;
        }

        public override void AssignFrom(GeneratorSettingsBase other)
        {
            base.AssignFrom(other);

            if (other is TypescriptGeneratorSettings otherTs)
            {
                AmbientNamespaceName = otherTs.AmbientNamespaceName;
                InterfacePrefix = otherTs.InterfacePrefix;
            }
        }

        public override void Serialize(DataNode node)
        {
            base.Serialize(node);

            node.AddAttribute(nameof(AmbientNamespaceName), AmbientNamespaceName);
            node.AddAttribute(nameof(InterfacePrefix), InterfacePrefix);
        }

        public override bool Deserialize(DataNode node)
        {
            var result = base.Deserialize(node);
            if (result)
            {
                if (node.Attributes.Contains(nameof(AmbientNamespaceName)))
                {
                    AmbientNamespaceName = node.Attributes[nameof(AmbientNamespaceName)].Value;
                }

                if (node.Attributes.Contains(nameof(InterfacePrefix)))
                {
                    InterfacePrefix = node.Attributes[nameof(InterfacePrefix)].Value;
                }
            }

            return result; 
        }

        public override SettingsValidationError Validate()
        {
            var error = base.Validate();
            if (error == null)
            {
                error = ValidateNameFor(AmbientNamespaceName, "Ambient namespace name", NameValidatorFlags.AllowEmpty) ?? 
                    ValidateNameFor(InterfacePrefix, "Interface prefix", NameValidatorFlags.AllowEmpty);
            }

            return error;
        }
    }
}
