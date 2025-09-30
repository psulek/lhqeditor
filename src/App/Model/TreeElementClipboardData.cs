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
using System.Linq;
using LHQ.Data;
using LHQ.Data.Extensions;
using LHQ.Data.Interfaces;
using LHQ.Data.ModelStorage;
using LHQ.Data.Support;
using LHQ.Utils.Extensions;
using LHQ.Utils.GenericTree;
using LHQ.Utils.Utilities;

namespace LHQ.App.Model
{
    public sealed class TreeElementClipboardData
    {
        private const string AttributeIsMarkedForCut = "isMarkedForCut";
        private const string AttributeShellInstanceId = "shellInstanceId";
        private const string AttributeRootModelElementKey = "rootModelElementKey";
        private const string AttributeName = "name";
        private const string AttributeDescription = "Description";
        private const string AttributeOrder = "order";
        private const string AttributeKey = "key";
        private const string AttributeParentKey = "parentKey";
        private const string AttributeElementType = "elementType";
        private const string AttributeParameters = "Parameters";
        private const string AttributeValue = "value";
        private const string AttributeLocked = "locked";
        private const string AttributeAuto = "auto";
        private const string AttributeLanguageName = "languageName";
        private const string AttributeShellVersion = "version";

        private const string ElementRoot = "Root";
        private const string ElementElements = "Elements";
        private const string ElementElement = "Element";

        public bool IsMarkedForCut { get; private set; }

        public Guid ShellInstanceId { get; private set; }

        public Version ShellVersion { get; private set; }

        public List<TreeElementClipboardItem> Items { get; private set; }

        public string RootModelElementKey { get; private set; }

        // ReSharper disable once UnusedParameter.Global
        public static TreeElementClipboardData Deserialize(Version currentShellVersion, string content)
        {
            TreeElementClipboardData result = null;
            try
            {
                bool deserializeSuccess = DataNodeJsonSerializer.Deserialize(content, out DataNode dataNode);

                if (deserializeSuccess && dataNode != null && dataNode.NodeName == ElementRoot)
                {
                    DataNodeAttribute attrShellInstanceId = dataNode.Attributes[AttributeShellInstanceId];
                    DataNodeAttribute attrShellVersion = dataNode.Attributes[AttributeShellVersion];
                    DataNodeAttribute attrRootModelElementKey = dataNode.Attributes[AttributeRootModelElementKey];
                    DataNodeAttribute attrIsMarkedForCut = dataNode.Attributes[AttributeIsMarkedForCut];

                    if (attrShellInstanceId != null && attrShellVersion != null
                        && attrRootModelElementKey != null && attrIsMarkedForCut != null)
                    {
                        if (ValueSerializer.TryDeserialize(attrShellInstanceId.Value, out Guid? shellInstanceId) &&
                            ValueSerializer.TryDeserialize(attrShellVersion.Value, out Version shellVersion) &&
                            ValueSerializer.TryDeserialize(attrIsMarkedForCut.Value, out bool? isMarkedForCut) &&
                            !attrRootModelElementKey.Value.IsNullOrEmpty())
                        {
                            /*if (currentShellVersion != shellVersion)
                            {
                                result = HandleDifferentVersionOfClipboardData(currentShellVersion, shellVersion, dataNode);
                            }*/

                            if (shellInstanceId.HasValue)
                            {
                                DataNode nodeElements = dataNode.Children[ElementElements];
                                if (nodeElements?.Children != null)
                                {
                                    result = new TreeElementClipboardData
                                    {
                                        ShellInstanceId = shellInstanceId.Value,
                                        ShellVersion = shellVersion,
                                        RootModelElementKey = attrRootModelElementKey.Value,
                                        IsMarkedForCut = isMarkedForCut.GetValueOrDefault(false),
                                        Items = new List<TreeElementClipboardItem>()
                                    };

                                    Deserialize(nodeElements, result.Items);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = null;
            }

            return result;
        }

        private static void Deserialize(DataNode nodeElements, List<TreeElementClipboardItem> targetItemsHolder)
        {
            List<DataNode> elementNodes = nodeElements.Children.Where(x => x.NodeName == ElementElement).ToList();
            if (elementNodes.Count > 0)
            {
                foreach (DataNode elementNode in elementNodes)
                {
                    DataNodeAttribute attrElementType = elementNode.Attributes[AttributeElementType];
                    DataNodeAttribute attrKey = elementNode.Attributes[AttributeKey];
                    DataNodeAttribute attrParentKey = elementNode.Attributes[AttributeParentKey];
                    DataNodeAttribute attrName = elementNode.Attributes[AttributeName];
                    DataNodeAttribute attrOrder = elementNode.Attributes[AttributeOrder];
                    DataNodeAttribute attrDescription = elementNode.Attributes[AttributeDescription];
                    DataNodeAttribute attrParameters = elementNode.Attributes[AttributeParameters];
                    DataNodeAttribute attrLanguageName = elementNode.Attributes[AttributeLanguageName];
                    DataNodeAttribute attrValue = elementNode.Attributes[AttributeValue];
                    DataNodeAttribute attrLocked = elementNode.Attributes[AttributeLocked];
                    DataNodeAttribute attrAuto = elementNode.Attributes[AttributeAuto];
                    DataNode childElements = elementNode.Children[ElementElements];

                    if (attrElementType != null)
                    {
                        if (ModelExtensions.TryParseModelElemenType(attrElementType.Value,
                            out ModelElementType modelElementType))
                        {
                            string elementKey = attrKey?.Value;
                            string parentElementKey = attrParentKey?.Value;
                            string elementName = attrName?.Value;

                            int? order = null;
                            if (attrOrder != null)
                            {
                                ValueSerializer.TryDeserialize(attrOrder.Value, out order);
                            }

                            string description = attrDescription?.Value;
                            string parameters = attrParameters?.Value;
                            string languageName = attrLanguageName?.Value;
                            string value = attrValue?.Value;

                            var locked = false;
                            if (attrLocked != null)
                            {
                                if (ValueSerializer.TryDeserialize(attrLocked.Value, out bool? b))
                                {
                                    locked = b.Value;
                                }
                            }

                            var autoTranslated = false;
                            if (attrAuto != null)
                            {
                                if (ValueSerializer.TryDeserialize(attrAuto.Value, out bool? b))
                                {
                                    autoTranslated = b.Value;
                                }
                            }

                            var item = new TreeElementClipboardItem
                            {
                                ModelElementType = modelElementType,
                                ElementKey = elementKey,
                                ElementParentKey = parentElementKey,
                                Order = order,
                                ElementName = elementName,
                                Description = description,
                                Parameters = parameters,
                                LanguageName = languageName,
                                Value = value,
                                Locked = locked,
                                AutoTranslated = autoTranslated
                            };

                            targetItemsHolder.Add(item);

                            if (childElements?.Children != null)
                            {
                                Deserialize(childElements, item.Children);
                            }
                        }
                    }
                }
            }
        }

        public static string Serialize(Guid shellInstanceId, Version shellVersion,
            bool isMarkedForCut, string rootModelElementKey, List<ModelElementBase> elements)
        {
            ArgumentValidator.EnsureArgumentNotEmptyGuid(shellInstanceId, "shellInstanceId");
            ArgumentValidator.EnsureArgumentNotNull(shellVersion, "shellVersion");
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(rootModelElementKey, "rootModelElementKey");
            ArgumentValidator.EnsureArgumentNotNull(elements, "elements");
            ArgumentValidator.EnsureArgumentGreaterThanZero(elements.Count, "elements.Count");

            var rootNode = new DataNode(ElementRoot);
            rootNode.AddAttribute(AttributeShellInstanceId, ValueSerializer.Serialize(shellInstanceId));
            rootNode.AddAttribute(AttributeShellVersion, ValueSerializer.Serialize(shellVersion));
            rootNode.AddAttribute(AttributeRootModelElementKey, rootModelElementKey);
            rootNode.AddAttribute(AttributeIsMarkedForCut, ValueSerializer.Serialize(isMarkedForCut));

            SerializeElements(rootNode, ElementElements, elements);

            var saveOptions = new ModelSaveOptions(false);
            return DataNodeJsonSerializer.Serialize(rootNode, saveOptions);
        }

        private static void SerializeElements(DataNode parentNode, string newHolderNodeName, List<ModelElementBase> elements)
        {
            if (elements.Count > 0)
            {
                DataNode elementsHolder = parentNode.AddChildren(newHolderNodeName, null);
                foreach (ModelElementBase element in elements)
                {
                    DataNode elementNode = elementsHolder.AddChildren(ElementElement, null);
                    SerializeElement(elementNode, element);
                }
            }
        }

        private static void SerializeElement(DataNode elementNode, ModelElementBase element)
        {
            elementNode.AddAttribute(AttributeElementType, element.ElementType.ToShortString());

            elementNode.AddAttribute(AttributeKey, element.Key.Serialize());

            var modelElement = element as ModelElement;
            if (modelElement?.ParentKey != null)
            {
                elementNode.AddAttribute(AttributeParentKey, modelElement.ParentKey.Serialize());
            }

            if (element is IModelNamedElement modelNamedElement)
            {
                elementNode.AddAttribute(AttributeName, modelNamedElement.Name);
            }

            if (element is IModelDescriptionElement modelDescriptionElement)
            {
                elementNode.AddAttribute(AttributeDescription, modelDescriptionElement.Description);
            }

            switch (element.ElementType)
            {
                case ModelElementType.Model:
                case ModelElementType.Language:
                {
                    throw new NotSupportedException();
                }
                case ModelElementType.Category:
                {
                    var category = (CategoryElement)element;

                    List<ModelElementBase> categoryChildren = GetChildren(category).ToList();
                    SerializeElements(elementNode, ElementElements, categoryChildren);

                    break;
                }
                case ModelElementType.Resource:
                {
                    var resource = (ResourceElement)element;

                    string parameters = resource.Parameters
                        .OrderBy(x => x.Order)
                        .Select(x => x.Name)
                        .ToDelimitedString(";");
                    elementNode.AddAttribute(AttributeParameters, parameters);

                    List<ModelElementBase> values = resource.Values.Cast<ModelElementBase>().ToList();
                    if (values.Count > 0)
                    {
                        SerializeElements(elementNode, ElementElements, values);
                    }
                    break;
                }
                case ModelElementType.ResourceParameter:
                {
                    var parameter = (ResourceParameterElement)element;
                    elementNode.AddAttribute(AttributeOrder, ValueSerializer.Serialize(parameter.Order));
                    break;
                }
                case ModelElementType.ResourceValue:
                {
                    var resourceValue = (ResourceValueElement)element;

                    elementNode.AddAttribute(AttributeLanguageName, resourceValue.LanguageName);
                    elementNode.AddAttribute(AttributeValue, resourceValue.Value);
                    elementNode.AddAttribute(AttributeLocked, ValueSerializer.Serialize(resourceValue.Locked));
                    elementNode.AddAttribute(AttributeAuto, ValueSerializer.Serialize(resourceValue.Auto));

                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static IEnumerable<ModelElementBase> GetChildren(CategoryElement category)
        {
            foreach (CategoryElement childCategory in category.Categories)
            {
                yield return childCategory;
            }

            foreach (ResourceElement childResource in category.Resources)
            {
                yield return childResource;
            }
        }

        public void IterateTreeItems(Action<CancellableArgs<TreeElementClipboardItem>> iterationAction)
        {
            ArgumentValidator.EnsureArgumentNotNull(iterationAction, "iterationAction");

            var treeIterator = new GenericTreeIterator<TreeElementClipboardItem>(x => x.Children, () => Items);
            var cancellableArgs = new CancellableArgs<TreeElementClipboardItem>();
            foreach (TreeElementClipboardItem treeElementClipboardItem in treeIterator.IterateTree(null))
            {
                cancellableArgs.Data = treeElementClipboardItem;
                iterationAction(cancellableArgs);
                if (cancellableArgs.Cancel)
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     Recursive scan all items and child items and return distinct list of used languages in resources values.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllUniqueLanguages()
        {
            var uniqueLanguages = new List<string>();
            InternalGetAllUniqueLanguages(uniqueLanguages, Items);

            return uniqueLanguages;
        }

        private void InternalGetAllUniqueLanguages(List<string> uniqueLanguages, List<TreeElementClipboardItem> items)
        {
            foreach (TreeElementClipboardItem item in items)
            {
                if (item.ModelElementType == ModelElementType.ResourceValue)
                {
                    if (uniqueLanguages.All(x => !item.LanguageName.EqualsTo(x)))
                    {
                        uniqueLanguages.Add(item.LanguageName);
                    }
                }
                else if (item.Children != null && item.Children.Count > 0)
                {
                    InternalGetAllUniqueLanguages(uniqueLanguages, item.Children);
                }
            }
        }
    }
}
