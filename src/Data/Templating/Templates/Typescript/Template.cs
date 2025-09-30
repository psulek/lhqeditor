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

using System.ComponentModel;
using LHQ.Data.Support;
using LHQ.Data.Templating.Settings;
using LHQ.Data.Templating.Settings.Json;
using LHQ.Data.Templating.Settings.Typescript;
using LHQ.Utils.Extensions;
using Newtonsoft.Json;
// ReSharper disable UnusedMember.Global

namespace LHQ.Data.Templating.Templates.Typescript
{
    public class TypescriptJson01Template : CodeGeneratorTemplate
    {
        private const string CategoryTypescriptGenerator = "Typescript Generator";
        private const string CategoryJsonGenerator = "JSON Generator";

        // public TypescriptJson01Template(int modelVersion): base(modelVersion)
        public TypescriptJson01Template()
        {
            Typescript = new TypescriptGeneratorSettings();
            Json = new JsonGeneratorSettings();
        }

        [Browsable(false)]
        [JsonIgnore]
        public override string Id { get; } = "TypescriptJson01";

        [Browsable(false)]
        [JsonIgnore]
        public override string Name { get; } = "Template which generates JSON file to be consumed by javascript with typescript (*.d.ts) type definition.\n" +
            "Usable in web projects.";

        [Browsable(false)]
        [JsonIgnore]
        public override ModelFeatures ModelFeatures { get; } = ModelFeatures.Default;

        [Browsable(false)]
        [JsonIgnore]
        public TypescriptGeneratorSettings Typescript { get; }

        [Browsable(false)]
        [JsonIgnore]
        public JsonGeneratorSettings Json { get; }

        [DisplayName("Enabled")]
        [Description("Flag whenever generating typescript files are enabled or not.")]
        [Category(CategoryTypescriptGenerator)]
        public bool TypescriptEnabled
        {
            get => Typescript.Enabled;
            set => Typescript.Enabled = value;
        }

        [DisplayName("Output folder")]
        [Description("Folder name (within current project) where typescript files (*.d.ts) will be generated to.\n" +
            "Recommended value is same as 'Output Folder' for Json files.")]
        [Category(CategoryTypescriptGenerator)]
        public string OutputFolder
        {
            get => Typescript.OutputFolder;
            set => Typescript.OutputFolder = value;
        }

        [DisplayName("Ambient namespace name")]
        [Description("Ambient namespace name used in typescript definition.")]
        [Category(CategoryTypescriptGenerator)]
        public string TypescriptAmbientNamespaceName
        {
            get => Typescript.AmbientNamespaceName;
            set => Typescript.AmbientNamespaceName = value;
        }

        [DisplayName("Prefix for interface type")]
        [Description("Prefix for interface type used in typescript definition.")]
        [Category(CategoryTypescriptGenerator)]
        public string TypescriptInterfacePrefix
        {
            get => Typescript.InterfacePrefix;
            set => Typescript.InterfacePrefix = value;
        }

        [DisplayName("Encoding with BOM")]
        [Description("Whenever the encoding contains BOM (Byte Order Mark), where default is false (No BOM).")]
        [Category(CategoryTypescriptGenerator)]
        [ModelVersionDepend(2)]
        public bool TypescriptEncodingWithBOM
        {
            get => Typescript.EncodingWithBOM;
            set => Typescript.EncodingWithBOM = value;
        }
        
        [DisplayName("Line endings (LF or CRLF)")]
        [Description("Line endings used when generated files are saved on disk, where default is LF.")]
        [Category(CategoryTypescriptGenerator)]
        [ModelVersionDepend(2)]
        public LineEndings TypescriptLineEndings
        {
            get => Typescript.LineEndings;
            set => Typescript.LineEndings = value;
        }
        
        [DisplayName("File name for primary language includes language code")]
        [Description("Json file for primary language will also (like foreign language files) includes language code.")]
        [Category(CategoryJsonGenerator)]
        public bool JsonCultureCodeInFileNameForPrimaryLanguage
        {
            get => Json.CultureCodeInFileNameForPrimaryLanguage;
            set => Json.CultureCodeInFileNameForPrimaryLanguage = value;
        }

        [DisplayName("Enabled")]
        [Description("Flag whenever generating JSON files are enabled or not.")]
        [Category(CategoryJsonGenerator)]
        public bool JsonEnabled
        {
            get => Json.Enabled;
            set => Json.Enabled = value;
        }

        [DisplayName("Output folder")]
        [Description("Folder name (within current project) where *.json files will be generated to.\n" +
            "Recommended value is same as 'Output Folder' for Typescript files.")]
        [Category(CategoryJsonGenerator)]
        public string JsonOutputFolder
        {
            get => Json.OutputFolder;
            set => Json.OutputFolder = value;
        }

        [DisplayName("Metadata file name suffix")]
        [Description("Suffix for metadata file name.")]
        [Category(CategoryJsonGenerator)]
        public string JsonMetadataFileNameSuffix
        {
            get => Json.MetadataFileNameSuffix;
            set => Json.MetadataFileNameSuffix = value;
        }
        
        [DisplayName("Write empty translations")]
        [Description("Write key value pair even on empty translations.")]
        [Category(CategoryJsonGenerator)]
        public bool JsonWriteEmptyValues
        {
            get => Json.WriteEmptyValues;
            set => Json.WriteEmptyValues = value;
        }
        
        [DisplayName("Encoding with BOM")]
        [Description("Whenever the encoding contains BOM (Byte Order Mark), where default is false (No BOM).")]
        [Category(CategoryJsonGenerator)]
        [ModelVersionDepend(2)]
        public bool JsonEncodingWithBOM
        {
            get => Json.EncodingWithBOM;
            set => Json.EncodingWithBOM = value;
        }
        
        [DisplayName("Line endings (LF or CRLF)")]
        [Description("Line endings used when generated files are saved on disk, where default is LF.")]
        [Category(CategoryJsonGenerator)]
        [ModelVersionDepend(2)]
        public LineEndings JsonLineEndings
        {
            get => Json.LineEndings;
            set => Json.LineEndings = value;
        }


        public override void Serialize(DataNode node, int modelVersion)
        {
            var nodeTypescript = new DataNode(nameof(Typescript));
            Typescript.Serialize(nodeTypescript, modelVersion);

            var nodeJson = new DataNode(nameof(Json));
            Json.Serialize(nodeJson, modelVersion);

            node.Children.AddRange(new[] { nodeTypescript, nodeJson });
        }

        public override bool Deserialize(DataNode node)
        {
            bool result = node.Children.ContainsKey(nameof(Typescript)) && node.Children.ContainsKey(nameof(Json));
            if (result)
            {
                result = Typescript.Deserialize(node.Children[nameof(Typescript)]) && 
                    Json.Deserialize(node.Children[nameof(Json)]);
            }

            return result;
        }

        public override SettingsValidationError Validate()
        {
            var error = Typescript.Validate();

            if (error == null)
            {
                error = Json.Validate();
            }

            if (error == null)
            {
                if (!Typescript.OutputFolder.IsNullOrEmpty() && !Json.OutputFolder.IsNullOrEmpty() &&
                    Typescript.OutputFolder.EqualsTo(Json.OutputFolder))
                {
                    error = SettingsValidationError.Warning("'Output folder' for Typescript and Json generator are same.\n" +
                        "It is recommended to have output folder different for each generator.",
                        "Do you want still want to use same values for 'Output folder'?", true);
                }
            }

            return error;
        }
    }
}
