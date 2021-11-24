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

using System.ComponentModel;
using LHQ.Data.Templating.Settings.NetFw;
using Newtonsoft.Json;
// ReSharper disable UnusedMember.Global

namespace LHQ.Data.Templating.Templates.NetFw
{
    public class NetFwResxCsharp01Template: CSharpResXTemplateBase<CSharpGeneratorSettings>
    {
        [Browsable(false)]
        [JsonIgnore]
        public override string Id { get; } = "NetFwResxCsharp01";

        [Browsable(false)]
        [JsonIgnore]
        public override string Name { get; } = "Template which generates strongly typed C# and resource (*.resx) files.\n" +
            "Usable in classic desktop applications (Console, Windows Service, etc.)";

        [Browsable(false)]
        [JsonIgnore]
        public override ModelFeatures ModelFeatures { get; } = ModelFeatures.HideResourcesUnderRoot;

        [DisplayName("Return primary language text on missing foreign translation")]
        [Description("Return primary language text in case of missing translation in foreign language.")]
        [Category("C# Generator")]
        public bool MissingTranslationFallbackToPrimary
        {
            get => CSharp.MissingTranslationFallbackToPrimary;
            set => CSharp.MissingTranslationFallbackToPrimary = value;
        }

    }
}
