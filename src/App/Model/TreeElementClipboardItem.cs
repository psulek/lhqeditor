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
using LHQ.Data;
using LHQ.Utils.Extensions;

namespace LHQ.App.Model
{
    public sealed class TreeElementClipboardItem
    {
        public TreeElementClipboardItem()
        {
            Children = new List<TreeElementClipboardItem>();
        }

        public string ElementName { get; set; }

        public string ElementKey { get; set; }

        public string ElementParentKey { get; set; }

        public string LanguageName { get; set; }

        public string Value { get; set; }

        public bool Locked { get; set; }

        public int? Order { get; set; }

        public ModelElementType ModelElementType { get; set; }

        public string Description { get; set; }

        public string Parameters { get; set; }

        public List<TreeElementClipboardItem> Children { get; set; }

        public bool AutoTranslated { get; set; }

        public List<string> GetParameters()
        {
            return Parameters.IsNullOrEmpty()
                ? new List<string>()
                : Parameters.Split(new[]
                {
                    ";"
                }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
