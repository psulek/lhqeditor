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

using System.Collections.Generic;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using Newtonsoft.Json;

namespace LHQ.App.Model
{
    public sealed class RecentFileInfo
    {
        private string _fileName;

        public RecentFileInfo(string fileName)
        {
            FileName = fileName;
        }

        [JsonIgnore]
        public string Name { get; private set; }

        public string FileName
        {
            get => _fileName;
            private set
            {
                _fileName = value;
                UpdateName();
            }
        }

        public bool IsLastOpened { get; set; }

        public List<string> ExpandedKeys { get; set; }

        public override string ToString()
        {
            return "{0} [{1}]".FormatWith(Name, FileName);
        }

        private void UpdateName()
        {
            Name = Ellipsis.Compact(FileName, 100, EllipsisFormat.Path | EllipsisFormat.Middle);
        }
    }
}
