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

using System.Collections.Generic;
using System.Linq;
using LHQ.Utils.Extensions;
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.Data.Support
{
    public class DataNodeCollection : List<DataNode>
    {
        public DataNodeCollection()
        { }

        public DataNodeCollection(IEnumerable<DataNode> source) : base(source)
        { }

        public DataNode this[string nodeName]
        {
            get { return this.SingleOrDefault(x => x.NodeName.EqualsTo(nodeName)); }
            set
            {
                int index = IndexOf(nodeName);
                if (index == -1)
                {
                    Add(value);
                }
                else
                {
                    base[index] = value;
                }
            }
        }

        public bool ContainsKey(string nodeName)
        {
            return IndexOf(nodeName) > -1;
        }

        public int IndexOf(string nodeName)
        {
            DataNode item = this[nodeName];
            return item == null ? -1 : IndexOf(item);
        }

        public void Clear(bool recursive)
        {
            if (recursive)
            {
                ForEach(child => child.Children.Clear(true));
            }

            Clear();
        }

        public DataNodeCollection Clone()
        {
            return new DataNodeCollection(this);
        }

        public DataNode Add(string nodeName, string elementValue)
        {
            var item = new DataNode(nodeName, elementValue);
            Add(item);
            return item;
        }

        public DataNode Add(string nodeName)
        {
            return Add(nodeName, null);
        }

        public void Remove(string nodeName)
        {
            int idx = IndexOf(nodeName);
            if (idx > -1)
            {
                RemoveAt(idx);
            }
        }
    }
}
