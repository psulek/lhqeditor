#region License
// Copyright (c) 2025 Peter Šulek / ScaleHQ Solutions s.r.o.
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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using LHQ.Utils.Utilities;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.Data.Support
{
    internal static class DataNodeXmlSerializer
    {
        public static string Serialize(DataNode element, Encoding encoding, Action<XmlTextWriter> configureWriter)
        {
            ArgumentValidator.EnsureArgumentNotNull(element, "element");
            ArgumentValidator.EnsureArgumentNotNull(encoding, "encoding");

            using (var ms = new MemoryStream())
            {
                Serialize(element, encoding, ms, configureWriter);
                return encoding.GetString(ms.ToArray());
            }
        }

        public static void Serialize(DataNode element, Encoding encoding, Stream targetStream,
            Action<XmlTextWriter> configureWriter)
        {
            ArgumentValidator.EnsureArgumentNotNull(element, "element");
            ArgumentValidator.EnsureArgumentNotNull(encoding, "encoding");
            ArgumentValidator.EnsureArgumentNotNull(targetStream, "targetStream");

            using (var writer = new XmlTextWriter(targetStream, encoding))
            {
                configureWriter?.Invoke(writer);

                Serialize(element, writer);
                writer.Flush();
                writer.Close();
            }
        }

        public static void Serialize(DataNode element, TextWriter textWriter, Action<XmlTextWriter> configureWriter)
        {
            ArgumentValidator.EnsureArgumentNotNull(element, "element");
            ArgumentValidator.EnsureArgumentNotNull(textWriter, "textWriter");

            using (var writer = new XmlTextWriter(textWriter))
            {
                configureWriter?.Invoke(writer);

                Serialize(element, writer);
                writer.Flush();
                writer.Close();
            }
        }

        public static void Serialize(DataNode element, XmlWriter writer)
        {
            writer.WriteStartElement(element.NodeName);
            foreach (DataNodeAttribute attribute in element.Attributes)
            {
                writer.WriteAttributeString(attribute.Name, attribute.Value);
            }

            writer.WriteString(element.NodeValue);

            foreach (DataNode child in element.Children)
            {
                Serialize(child, writer);
            }
            writer.WriteEndElement();
        }

        public static DataNode Deserialize(string source, Encoding encoding)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(source, "source");
            ArgumentValidator.EnsureArgumentNotNull(encoding, "encoding");

            using (var memoryStream = new MemoryStream(encoding.GetBytes(source)))
            {
                using (var reader = new XmlTextReader(memoryStream))
                {
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    DataNode dataNode = Deserialize(reader);
                    reader.Close();
                    return dataNode;
                }
            }
        }

        public static DataNode Deserialize(Stream sourceStream, Encoding encoding)
        {
            return Deserialize(sourceStream, true, encoding);
        }

        public static DataNode Deserialize<T>(T sourceStream, bool leaveOpen, Encoding encoding)
            where T : Stream
        {
            ArgumentValidator.EnsureArgumentNotNull(sourceStream, "sourceStream");
            ArgumentValidator.EnsureArgumentNotNull(encoding, "encoding");

            using (var streamWrapper = new StreamWrapper<T>(sourceStream, leaveOpen))
            {
                using (var streamReader = new StreamReader(streamWrapper, encoding))
                {
                    using (var reader = new XmlTextReader(streamReader))
                    {
                        reader.WhitespaceHandling = WhitespaceHandling.None;
                        DataNode dataNode = Deserialize(reader);
                        reader.Close();
                        return dataNode;
                    }
                }
            }
        }

        public static DataNode Deserialize(TextReader textReader)
        {
            using (var reader = new XmlTextReader(textReader))
            {
                return Deserialize(reader);
            }
        }

        public static DataNode Deserialize(XmlReader reader)
        {
            DataNode result = null;
            reader.MoveToContent();

            int startDepth = reader.Depth;
            string startElementName = reader.Name;

            // key - depth, value - list of added xml nodes
            var mapping = new Dictionary<int, List<DataNode>>();
            DataNode lastElementNode = null;

            bool canRead = !reader.EOF;

            while (canRead && !reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        var newNode = new DataNode(reader.Name);
                        List<DataNode> elementNodes = mapping.SingleOrDefault(pair => pair.Key == reader.Depth).Value;
                        if (elementNodes == null)
                        {
                            elementNodes = new List<DataNode>();
                            mapping.Add(reader.Depth, elementNodes);
                        }
                        elementNodes.Add(newNode);

                        // parent
                        List<DataNode> parentNodes = mapping.SingleOrDefault(pair => pair.Key == reader.Depth - 1).Value;
                        if (parentNodes != null && parentNodes.Count > 0)
                        {
                            DataNode parentNode = parentNodes[parentNodes.Count - 1];
                            parentNode?.AddChildren(newNode);
                        }

                        // read attributes
                        if (reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                newNode.AddAttribute(reader.Name, reader.Value);
                            }
                        }

                        lastElementNode = newNode;
                        break;
                    }

                    case XmlNodeType.Text:
                    {
                        if (lastElementNode != null)
                        {
                            lastElementNode.NodeValue = reader.Value;
                        }
                        break;
                    }

                    case XmlNodeType.EndElement:
                    {
                        break;
                    }
                }

                canRead = reader.Read();
                reader.MoveToContent();

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == startElementName
                    && reader.Depth == startDepth)
                {
                    break;
                }
            }

            if (mapping.Count > 0)
            {
                List<DataNode> list = mapping.OrderBy(pair => pair.Key).Select(pair => pair.Value).First();
                if (list != null && list.Count > 0)
                {
                    result = list[0];
                }
            }

            return result;
        }
    }
}
