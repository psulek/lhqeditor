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
using System.Linq;
using System.Text;
using LHQ.Utils.Extensions;
using LHQ.Utils.GenericTree;
using LHQ.Data;
using LHQ.Data.Extensions;
using LHQ.Data.Interfaces;

namespace LHQ.UnitTests.Visualizer
{
    public class ModelVisualizer
    {
        private const int IndentSize = 3;

        private const string PrefixCharMultipleChilds = "├─ ";
        private const string PrefixCharSingleChild = "└─ ";
        private const char PrefixCharVertLine = '│';
        private int _indent;
        private StringBuilder _stringBuilder;

        private ModelTreeWalker _modelTreeWalker;

        public ModelVisualizer(ModelVisualizerOptions options)
        {
            Options = options;
        }

        private ModelVisualizerOptions Options { get; }

        private void Reset()
        {
            _stringBuilder = new StringBuilder();
            _indent = 0;
        }

        public string VisualizeModel(Model model)
        {
            Reset(); // indent == 0

            _modelTreeWalker = new ModelTreeWalker(model);

            string key = GetKey(model);
            string version = GetVersion(model);
            _stringBuilder.AppendFormat("M{0}:{1}{2}", version, model.Name, key);
            WriteLine();

            var treeIterator = new GenericTreeIterator<ModelElementBase>(GetChilds, () => GetRootChilds(model));
            treeIterator.IterateTree(false, null, IterateElement);

            // metadata
            if (model.Metadata != null && model.Metadata.Count > 0)
            {
                _stringBuilder.AppendLine("Has Branch Metadata: Yes");
            }

            return _stringBuilder.ToString();
        }

        private void IterateElement(GenericTreeIterationArgs<ModelElementBase> args)
        {
            int level = args.Level;
            _indent = level + 1;

            var localBuilder = new StringBuilder();

            ModelElementBase element = args.Current;
            var modelNamedElement = element as IModelNamedElement;
            var order = "";

            string name = modelNamedElement == null ? element.Key.Serialize() : modelNamedElement.Name;
            string key = GetKey(element);
            string version = GetVersion(element);
            if (element is ResourceValueElement resourceValue)
            {
                name = resourceValue.Value;
            }

            var parentElement = (ModelElement)args.Parent;

            void WriteIndent()
            {
                localBuilder.Append(new string(' ', IndentSize));
            }

            var parentElementIsLeaf = false;
            if (level > 0 && parentElement != null)
            {
                parentElementIsLeaf = _modelTreeWalker.HasNext(parentElement) == false;
            }

            for (var i = 0; i < _indent; i++)
            {
                WriteIndent();
                if (_indent > 2 && i > 0 && i < level)
                {
                    if (i + IndentSize > _indent && parentElementIsLeaf)
                    {
                        localBuilder.Append(' ');
                    }
                    else
                    {
                        localBuilder.Append(PrefixCharVertLine);
                    }
                }
            }

            string icon = string.Empty;
            if (level > 0 && parentElement != null)
            {
                icon = _modelTreeWalker.HasNext(element) ? PrefixCharMultipleChilds : PrefixCharSingleChild;
            }

            localBuilder.AppendFormat("{0}{1}{2}{3}:{4}{5}", icon, element.ElementType.ToShortString(), order, version, name, key);
            localBuilder.Append(Environment.NewLine);
            _stringBuilder.Append(localBuilder);
        }

        private IEnumerable<ModelElementBase> GetRootChilds(Model model)
        {
            foreach (LanguageElement language in model.Languages.OrderBy(x => x.Name))
            {
                yield return language;
            }

            foreach (CategoryElement category in model.Categories.OrderBy(x => x.Name))
            {
                yield return category;
            }

            foreach (ResourceElement resource in model.Resources.OrderBy(x => x.Name))
            {
                yield return resource;
            }
        }

        private IEnumerable<ModelElementBase> GetChilds(ModelElementBase modelElement)
        {
            if (modelElement is CategoryElement category)
            {
                foreach (CategoryElement child in category.Categories.OrderBy(x => x.Name))
                {
                    yield return child;
                }

                foreach (ResourceElement childResource in category.Resources.OrderBy(x => x.Name))
                {
                    yield return childResource;
                }
            }

            if (modelElement is ResourceElement resource)
            {
                foreach (ResourceParameterElement resourceParameter in resource.Parameters.OrderBy(x => x.Order))
                {
                    yield return resourceParameter;
                }

                foreach (ResourceValueElement resourceValue in resource.Values)
                {
                    yield return resourceValue;
                }
            }
        }

        private void WriteLine()
        {
            _stringBuilder.Append(Environment.NewLine);
        }

        private string GetKey(ModelElementBase element)
        {
            return Options.ShowKey ? "[{0}]".FormatWith(element.Key.Serialize()) : string.Empty;
        }

        private string GetVersion(ModelElementBase element)
        {
            return Options.ShowVersion && element is Model model ? "~V" + model.Version : string.Empty;
        }
    }
}
