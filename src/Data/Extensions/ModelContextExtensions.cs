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

using LHQ.Data.CodeGenerator;
using LHQ.Data.Templating;
using LHQ.Data.Templating.Templates;
using LHQ.Utils.Extensions;
// ReSharper disable UnusedMember.Global

namespace LHQ.Data.Extensions
{
    public static class ModelContextExtensions
    {
        public static string GetCodeGeneratorTemplateId(this ModelContext modelContext)
        {
            CodeGeneratorMetadata metadata = modelContext.GetMetadata<CodeGeneratorMetadata>(CodeGeneratorMetadataDescriptor.UID);
            return metadata?.TemplateId;
        }

        public static bool HasCodeGeneratorTemplate(this ModelContext modelContext)
        {
            CodeGeneratorMetadata metadata = modelContext.GetMetadata<CodeGeneratorMetadata>(CodeGeneratorMetadataDescriptor.UID);
            return metadata?.Template != null;
        }

        public static CodeGeneratorTemplate GetCodeGeneratorTemplate(this ModelContext modelContext, string fallbackTemplateId)
        {
            var metadata = modelContext.GetMetadata<CodeGeneratorMetadata>(CodeGeneratorMetadataDescriptor.UID);
            CodeGeneratorTemplate template = metadata.Template;
            if (template == null)
            {
                var templateId = metadata.TemplateId.ValueOrDefault(fallbackTemplateId);
                if (!templateId.IsNullOrEmpty())
                {
                    template = CodeGeneratorTemplateManager.Instance.CreateTemplate(templateId);
                }
            }

            return template;
        }
    }
}
