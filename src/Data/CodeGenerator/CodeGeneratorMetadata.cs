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
using LHQ.Data.Interfaces.Metadata;
using LHQ.Data.Support;
using LHQ.Data.Templating;
using LHQ.Data.Templating.Templates;
using LHQ.Utils.Extensions;
using Newtonsoft.Json;

namespace LHQ.Data.CodeGenerator
{
    public sealed class CodeGeneratorMetadata : IModelElementMetadata
    {
        private const string ElementSettings = "Settings";
        private const string AttributeTemplateId = "templateId";

        [JsonIgnore]
        public Guid DescriptorUID { get; } = CodeGeneratorMetadataDescriptor.UID;

        [JsonIgnore]
        public CodeGeneratorTemplate Template { get; set; }

        [JsonProperty("templateId")]
        public string TemplateId { get; set; }

        public DataNode Serialize()
        {
            var result = new DataNode();
            result.AddAttribute(AttributeTemplateId, TemplateId);
            if (Template != null)
            {
                var templateNode = new DataNode(ElementSettings);
                Template.Serialize(templateNode);
                result.AddChildren(templateNode);
            }

            return result;
        }

        public bool Deserialize(DataNode sourceNode)
        {
            bool result = sourceNode != null;
            if (result)
            {
                result = sourceNode.Attributes.Contains(AttributeTemplateId) &&
                    sourceNode.Children.ContainsKey(ElementSettings);

                if (result)
                {
                    TemplateId = sourceNode.Attributes[AttributeTemplateId].Value;

                    if (!TemplateId.IsNullOrEmpty())
                    {
                        var template = CodeGeneratorTemplateManager.Instance.CreateTemplate(TemplateId);

                        if (template != null)
                        {
                            DataNode nodeSettings = sourceNode.Children[ElementSettings];
                            result = template.Deserialize(nodeSettings);
                            if (result)
                            {
                                Template = template;
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
