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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using LHQ.Utils.Extensions;
using LHQ.Data;
using Microsoft.VisualStudio.TextTemplating;
// ReSharper disable UnusedType.Global

namespace LHQ.VsExtension
{
    public class LHQDirectiveProcessor : IDirectiveProcessor
    {
        public const string DirectiveProcessorName = "LHQDirectiveProcessor";

        private const string DirectiveName = "LHQDirective";

        private StringBuilder _codeBuffer;

        private CompilerErrorCollection _errorsValue;

        public CompilerErrorCollection Errors => _errorsValue;

        protected ITextTemplatingEngineHost Host { get; private set; }

        public bool RequiresProcessingRunIsHostSpecific
        {
            get { return false; }
        }

        public void Initialize(ITextTemplatingEngineHost host)
        {
            Host = host;
        }

        public void StartProcessingRun(CodeDomProvider languageProvider, string templateContents, CompilerErrorCollection errors)
        {
            _errorsValue = errors;
            _codeBuffer = new StringBuilder();
        }

        public bool IsDirectiveSupported(string directiveName)
        {
            return directiveName == DirectiveName;
        }

        public void ProcessDirective(string directiveName, IDictionary<string, string> arguments)
        {
            if (directiveName == DirectiveName)
            {
                string fileName;
                string itemTemplate;

                if (!arguments.TryGetValue("FileName", out fileName))
                {
                    throw new Exception("Required argument 'FileName' not specified.");
                }

                if (string.IsNullOrEmpty(fileName))
                {
                    throw new Exception("Argument 'FileName' is null or empty.");
                }

                if (!arguments.TryGetValue("ItemTemplate", out itemTemplate))
                {
                    throw new Exception("Required argument 'ItemTemplate' not specified.");
                }

                if (string.IsNullOrEmpty(itemTemplate))
                {
                    throw new Exception("Argument 'ItemTemplate' is null or empty.");
                }

                fileName = Host.ResolvePath(fileName);

                _codeBuffer.AppendFormat(@"
                private LHQ.Data.ModelContext _modelContext;
                private LHQ.Data.Templating.Templates.CodeGeneratorTemplate _codeGeneratorTemplate;

                public LHQ.Data.ModelContext ModelContext
                {{
                    get
                    {{
                        if (this._modelContext == null) 
                        {{
                            this._modelContext = new LHQ.Data.ModelContext(LHQ.Data.ModelContextOptions.Default);" +
                            "var loadResult = LHQ.Data.ModelStorage.ModelSerializerManager.DeserializeFrom(@\"" + fileName + "\", this._modelContext);"
                            + @"if (loadResult.Status != LHQ.Data.ModelStorage.ModelLoadStatus.Success)
                            {{
                                throw new ApplicationException(""Error loading strings file, "" + loadResult.Status.ToString());
                            }}
                            
                            this._codeGeneratorTemplate = LHQ.Data.Extensions.ModelContextExtensions.GetCodeGeneratorTemplate(this._modelContext, """ + itemTemplate + @""");
                            if (this._codeGeneratorTemplate == null)
                            {{
                                throw new ApplicationException(""Could not find code generator template!"");
                            }}
                        }}

                        return this._modelContext;
                    }}
                }}

                public T CodeGeneratorTemplate<T>() where T: LHQ.Data.Templating.Templates.CodeGeneratorTemplate
                {{
                    if (this.ModelContext != null)
                    {{
                        return this._codeGeneratorTemplate as T;
                    }}
                    
                    return null;
                }}
", fileName);
            }
        }

        public void FinishProcessingRun()
        {
        }

        public string GetClassCodeForProcessingRun()
        {
            return _codeBuffer.ToString();
        }

        public string GetPreInitializationCodeForProcessingRun()
        {
            return string.Empty;
        }

        public string GetPostInitializationCodeForProcessingRun()
        {
            return string.Empty;
        }

        public string[] GetReferencesForProcessingRun()
        {
            List<string> references = new List<string>
            {
                typeof(ModelContext).Assembly.Location,
                typeof(TaskAsyncExtensions).Assembly.Location,
                GetType().Assembly.Location
            };
            return references.ToArray();
        }

        public string[] GetImportsForProcessingRun()
        {
            return new[]
            {
                "System.IO",
                "LHQ.Utils",
                "LHQ.Data",
                "LHQ.Data.Extensions"
            };
        }

        public CodeAttributeDeclarationCollection GetTemplateClassCustomAttributes()
        {
            return null;
        }

        public void SetProcessingRunIsHostSpecific(bool hostSpecific)
        {
        }
    }
}