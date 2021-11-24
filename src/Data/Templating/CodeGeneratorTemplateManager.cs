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
using System.Collections.Generic;
using LHQ.Data.Templating.Templates;
using LHQ.Data.Templating.Templates.NetCore;
using LHQ.Data.Templating.Templates.NetFw;
using LHQ.Data.Templating.Templates.Typescript;
using LHQ.Data.Templating.Templates.WinForms;
using LHQ.Data.Templating.Templates.Wpf;

namespace LHQ.Data.Templating
{
    public class CodeGeneratorTemplateManager
    {
        private static readonly Lazy<CodeGeneratorTemplateManager> _instance = new Lazy<CodeGeneratorTemplateManager>(() => new CodeGeneratorTemplateManager());

        public static CodeGeneratorTemplateManager Instance => _instance.Value;

        private readonly Dictionary<string, Type> _templates = new Dictionary<string, Type>();

        public CodeGeneratorTemplateManager()
        {
            Register<WinFormsResxCsharp01Template>();
            Register<NetFwResxCsharp01Template>();
            Register<WpfResxCsharp01Template>();
            Register<NetCoreResxCsharp01Template>();
            Register<TypescriptJson01Template>();
        }

        private void Register<T>()
            where T : CodeGeneratorTemplate, new()
        {
            T template = new T();
            if (_templates.ContainsKey(template.Id))
            {
                throw new InvalidOperationException($"Template '{template.Name}' already registered in Templates Manager.");
            }

            _templates.Add(template.Id, template.GetType());
        }

        public CodeGeneratorTemplate CreateTemplate(string templateId)
        {
            if (_templates.TryGetValue(templateId, out var type))
            {
                return Activator.CreateInstance(type) as CodeGeneratorTemplate;
            }

            return null;
        }
    }
}
