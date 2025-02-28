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

using System.Linq;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using LHQ.Data.ModelStorage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LHQ.Data.Support
{
    internal static class DataNodeJsonSerializer
    {
        private const string AttributeValue = "value";
        private const string AttributeAttrs = "attrs";
        private const string AttributeChilds = "childs";
        private const string AttributeName = "name";

        public static string Serialize(DataNode dataNode, ModelSaveOptions saveOptions)
        {
            var rootJson = new JObject();
            WriteDataNode(dataNode, rootJson);
            return JsonUtils.TokenToString(rootJson, GetFormatting(saveOptions));
        }

        private static Formatting GetFormatting(ModelSaveOptions saveOptions)
        {
            return saveOptions.IndentOutput ? Formatting.Indented : Formatting.None;
        }

        private static void WriteDataNode(DataNode element, JObject json)
        {
            ArgumentValidator.EnsureArgumentNotNull(element, "element");
            ArgumentValidator.EnsureArgumentNotNull(json, "json");

            if (!element.NodeName.IsNullOrEmpty())
            {
                json[AttributeName] = element.NodeName;
            }

            if (!element.NodeValue.IsNullOrEmpty())
            {
                json[AttributeValue] = element.NodeValue;
            }

            if (element.Attributes.Count > 0)
            {
                var jsonAttributes = new JObject();
                foreach (DataNodeAttribute attribute in element.Attributes)
                {
                    jsonAttributes[attribute.Name] = attribute.Value;
                }

                json[AttributeAttrs] = jsonAttributes;
            }

            if (element.Children.Count > 0)
            {
                var childsArray = new JArray();
                foreach (DataNode child in element.Children)
                {
                    var childJson = new JObject();
                    WriteDataNode(child, childJson);
                    childsArray.Add(childJson);
                }

                json[AttributeChilds] = childsArray;
            }
        }

        public static bool Deserialize(string source, out DataNode dataNode)
        {
            dataNode = null;
            bool result = !source.IsNullOrEmpty();
            if (result)
            {
                JObject jsonObject = JObject.Parse(source);
                result = jsonObject != null;
                if (result)
                {
                    result = Deserialize(jsonObject, out dataNode);
                }
            }

            return result;
        }

        public static bool Deserialize(JObject jsonObject, out DataNode dataNode)
        {
            dataNode = new DataNode();

            if (jsonObject.TryGetValue(AttributeName, out JToken attrNodeName))
            {
                var nodeName = attrNodeName.Value<string>();
                if (!nodeName.IsNullOrEmpty())
                {
                    dataNode.NodeName = nodeName;
                }
            }

            if (jsonObject.TryGetValue(AttributeValue, out JToken attrNodeValue))
            {
                var nodeValue = attrNodeValue.Value<string>();
                dataNode.NodeValue = nodeValue;
            }

            if (jsonObject.TryGetValue(AttributeAttrs, out JToken attrAttributes))
            {
                var attrs = (JObject)attrAttributes;
                foreach (JProperty property in attrs.Properties())
                {
                    if (property.Value is JValue propertyValue)
                    {
                        dataNode.AddAttribute(property.Name, propertyValue.Value<string>());
                    }
                }
            }

            if (jsonObject.TryGetValue(AttributeChilds, out JToken attrChilds))
            {
                if (attrChilds is JArray childArray)
                {
                    foreach (JObject jsonChild in childArray.Cast<JObject>())
                    {
                        if (Deserialize(jsonChild, out DataNode childNode))
                        {
                            dataNode.AddChildren(childNode);
                        }
                    }
                }
            }

            return dataNode != null;
        }
    }
}
