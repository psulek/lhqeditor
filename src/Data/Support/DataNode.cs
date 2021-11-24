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

using LHQ.Utils.Extensions;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace LHQ.Data.Support
{
    public class DataNode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DataNode" /> class.
        /// </summary>
        public DataNode()
        {
            Attributes = new DataNodeAttributeCollection();
            Children = new DataNodeCollection();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataNode" /> class.
        /// </summary>
        /// <param name="nodeName">Node name.</param>
        public DataNode(string nodeName) : this()
        {
            NodeName = nodeName;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataNode" /> class.
        /// </summary>
        /// <param name="nodeName">Node name.</param>
        /// <param name="nodeValue">Node value.</param>
        public DataNode(string nodeName, string nodeValue)
            : this(nodeName)
        {
            NodeValue = nodeValue;
        }

        /// <summary>
        ///     Gets or sets the name of xml element.
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        ///     Gets or sets the value of xml element.
        /// </summary>
        public string NodeValue { get; set; }

        /// <summary>
        ///     Gets the list attributes collection.
        /// </summary>
        public DataNodeAttributeCollection Attributes { get; private set; }

        /// <summary>
        ///     Gets the the list of child nodes.
        /// </summary>
        public DataNodeCollection Children { get; }

        public DataNode this[string childElementName] => Children[childElementName];

        public DataNode this[int index] => Children[index];

        /// <summary>
        ///     Add attribute.
        /// </summary>
        /// <param name="attributeName">Attribute attributeName.</param>
        /// <param name="value">Attribute value.</param>
        public DataNodeAttribute AddAttribute(string attributeName, string value)
        {
            return Attributes.Add(attributeName, value);
        }

        /// <summary>
        ///     Add child.
        /// </summary>
        /// <param name="childNodeName">Node name.</param>
        /// <param name="childNodeValue">Node string value.</param>
        /// <returns>
        ///     instance of <see cref="DataNode" /> if succesfully, otherwise <c>null</c>.
        /// </returns>
        public DataNode AddChildren(string childNodeName, string childNodeValue)
        {
            return Children.Add(childNodeName, childNodeValue);
        }

        /// <summary>
        ///     Add child.
        /// </summary>
        /// <param name="childNode"><see cref="DataNode" /> object as a child.</param>
        public void AddChildren(DataNode childNode)
        {
            Children.Add(childNode);
        }

        /// <summary>
        ///     Clones <see cref="DataNode" /> object
        /// </summary>
        /// <returns>Cloned <see cref="DataNode" /> object.</returns>
        public DataNode Clone()
        {
            var cloned = new DataNode(NodeName, NodeValue)
            {
                Attributes = Attributes.Clone()
            };

            foreach (DataNode child in Children)
            {
                cloned.AddChildren(child.Clone());
            }

            return cloned;
        }

        public override string ToString()
        {
            return "Node name: {0}, value: {1}, attrs: {2}, childs: {3}".FormatWith(NodeName,
                NodeValue.IsNullOrEmpty() ? string.Empty : NodeValue.ShortenText(40),
                Attributes.Count, Children.Count);
        }
    }
}
