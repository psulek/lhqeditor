#region License
// Copyright (c) 2021 Peter �ulek / ScaleHQ Solutions s.r.o.
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
using System.Globalization;
using LHQ.Data.Interfaces;
// ReSharper disable UnusedMember.Global

namespace LHQ.Data
{
    public sealed class ResourceElementList : ModelElementList<ResourceElement>, IFindByNameList<ResourceElement>
    {
        public ResourceElementList()
        { }

        public ResourceElementList(List<ResourceElement> list) : base(list)
        { }

        public ResourceElementList(IList<ResourceElement> collection) : base(collection)
        { }

        public ResourceElement FindByName(string name, bool ignoreCase, CultureInfo cultureInfo)
        {
            return FindByNameInternal(name, ignoreCase, cultureInfo);
        }
    }
}
