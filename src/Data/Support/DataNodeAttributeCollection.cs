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
    public class DataNodeAttributeCollection : List<DataNodeAttribute>
    {
        public DataNodeAttributeCollection()
        { }

        public DataNodeAttributeCollection(IEnumerable<DataNodeAttribute> source) : base(source)
        { }

        public DataNodeAttribute this[string name]
        {
            get { return this.SingleOrDefault(x => x.Name.EqualsTo(name)); }
            set
            {
                int index = IndexOf(name);
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

        public DataNodeAttribute Add(string name, string value)
        {
            var attribute = new DataNodeAttribute(name, value);
            Add(attribute);
            return attribute;
        }

        public bool Contains(string name)
        {
            return IndexOf(name) > -1;
        }

        public int IndexOf(string name)
        {
            DataNodeAttribute item = this[name];
            return item == null ? -1 : IndexOf(item);
        }

        public DataNodeAttributeCollection Clone()
        {
            return new DataNodeAttributeCollection(this);
        }

        public string GetValue(string name)
        {
            DataNodeAttribute attribute = this[name];
            return attribute?.Value ?? string.Empty;
        }

        public DataNodeAttribute SetValue(string name, string value)
        {
            DataNodeAttribute attribute = this[name];
            if (attribute == null)
            {
                attribute = Add(name, value);
            }
            else
            {
                attribute.Value = value;
            }

            return attribute;
        }
    }
}
