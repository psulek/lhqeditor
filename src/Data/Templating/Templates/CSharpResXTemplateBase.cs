﻿#region License
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

using System.ComponentModel;
using LHQ.Data.Support;
using LHQ.Data.Templating.Settings;
using LHQ.Data.Templating.Settings.ResX;
using LHQ.Utils.Extensions;
using Newtonsoft.Json;
// ReSharper disable UnusedMember.Global

namespace LHQ.Data.Templating.Templates
{
    public abstract class CSharpResXTemplateBase<TCSharpSettings> : CodeGeneratorTemplate where TCSharpSettings : CSharpGeneratorSettingsBase, IGeneratorSettings, new()
    {
        private const string CategoryCSharpGenerator = "C# Generator";
        private const string CategoryResxGenerator = "ResX Generator";

        protected CSharpResXTemplateBase()
        {
            CSharp = new TCSharpSettings();
            ResX = new ResXGeneratorSettings();
        }

        [Browsable(false)]
        [JsonIgnore]
        public override ModelFeatures ModelFeatures { get; } = ModelFeatures.ValidateFullKeyNames;

        [Browsable(false)]
        [JsonIgnore]
        public TCSharpSettings CSharp { get; }

        [DisplayName("Enabled")]
        [Description("Flag whenever generating C# files are enabled or not.")]
        [Category(CategoryCSharpGenerator)]
        public bool CSharpEnabled
        {
            get => CSharp.Enabled;
            set => CSharp.Enabled = value;
        }

        [DisplayName("Output folder")]
        [Description("Folder name (within current project) where C# classes will be generated to.\n" +
            "Recommended value is same as 'Output Folder' for *.resx files.")]
        [Category(CategoryCSharpGenerator)]
        public string CSharpOutputFolder
        {
            get => CSharp.OutputFolder;
            set => CSharp.OutputFolder = value;
        }

        [DisplayName("Use expression body syntax")]
        [Description("Generated code will use 'Expression body syntax' when available.\n" +
            "Required C# language level 7.0 or higher.")]
        [Category(CategoryCSharpGenerator)]
        public bool UseExpressionBodySyntax
        {
            get => CSharp.UseExpressionBodySyntax;
            set => CSharp.UseExpressionBodySyntax = value;
        }

        [Browsable(false)]
        [JsonIgnore]
        public ResXGeneratorSettings ResX { get; }

        [DisplayName("Enabled")]
        [Description("Flag whenever generating resx files are enabled or not.")]
        [Category(CategoryResxGenerator)]
        public bool ResXEnabled
        {
            get => ResX.Enabled;
            set => ResX.Enabled = value;
        }

        [DisplayName("Output folder")]
        [Description("Folder name (within current project) where *.resx files will be generated to.\n" +
            "Recommended value is same as 'Output Folder' for C# classes.")]
        [Category(CategoryResxGenerator)]
        public string ResXOutputFolder
        {
            get => ResX.OutputFolder;
            set => ResX.OutputFolder = value;
        }

        [DisplayName("File name for primary language includes language code")]
        [Description("Resx file for primary language will also (like foreign language files) includes language code.")]
        [Category(CategoryResxGenerator)]
        public bool CultureCodeInFileNameForPrimaryLanguage
        {
            get => ResX.CultureCodeInFileNameForPrimaryLanguage;
            set => ResX.CultureCodeInFileNameForPrimaryLanguage = value;
        }

        public override void Serialize(DataNode node)
        {
            var nodeCSharp = new DataNode(nameof(CSharp));
            CSharp.Serialize(nodeCSharp);

            var nodeResX = new DataNode(nameof(ResX));
            ResX.Serialize(nodeResX);

            node.Children.AddRange(new[] { nodeCSharp, nodeResX });
        }

        public override bool Deserialize(DataNode node)
        {
            bool result = node.Children.ContainsKey(nameof(CSharp)) && node.Children.ContainsKey(nameof(ResX));
            if (result)
            {
                result = CSharp.Deserialize(node.Children[nameof(CSharp)]) &&
                    ResX.Deserialize(node.Children[nameof(ResX)]);
            }

            return result;
        }

        public override SettingsValidationError Validate()
        {
            var error = CSharp.Validate();
            if (error == null)
            {
                error = ResX.Validate();
            }

            if (error == null)
            {
                if (!CSharp.OutputFolder.IsNullOrEmpty() && !ResX.OutputFolder.IsNullOrEmpty() &&
                    !CSharp.OutputFolder.EqualsTo(ResX.OutputFolder))
                {
                    error = SettingsValidationError.Warning("'Output folder' for C# and ResX generator are different.\n" +
                        "It is recommended to have output folder same for both generators.",
                        "Do you want still want to use different values for 'Output folder'?", true);
                }
            }

            return error;
        }
    }
}