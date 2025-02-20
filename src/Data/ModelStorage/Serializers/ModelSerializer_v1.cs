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
using System.Diagnostics;
using System.Linq;
using LHQ.Data.Extensions;
using LHQ.Data.Interfaces;
using LHQ.Data.Interfaces.Key;
using LHQ.Data.Metadata;
using LHQ.Data.Support;
using LHQ.Utils;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using Newtonsoft.Json.Linq;

namespace LHQ.Data.ModelStorage.Serializers
{
    // ReSharper disable once InconsistentNaming
    public class ModelSerializer_v1: IModelSerializer
    {
        private const string AttributeKey = "key";
        private const string AttributeUID = "uid";
        private const string AttributeName = "name";
        private const string AttributeValue = "value";
        private const string AttributeVersion = "version";
        private const string AttributeOptionsCategories = "categories";
        private const string AttributeOptionsResources = "resources";
        private const string AttributeLocked = "locked";
        private const string AttributeAuto = "auto";
        private const string AttributeDescription = "description";
        private const string AttributeOrder = "order";
        private const string AttributePrimaryLanguage = "primaryLanguage";
        private const string AttributeDescriptorUID = "descriptorUID";
        private const string AttributeNote = "note";
        private const string AttributeState = "state";
        private const string ElementCategories = "categories";
        private const string ElementModel = "model";
        private const string ElementModelOptions = "options";
        private const string ElementLanguages = "languages";
        private const string ElementResources = "resources";
        private const string ElementParameters = "parameters";
        private const string ElementValues = "values";
        private const string ElementMetadatas = "metadatas";
        private const string ElementMetadata = "metadata";
        private const string ElementContent = "content";

        private ModelContext _modelContext;
        private IModelElementKeyProvider _keyProvider;

        private Model _model;
        private ModelSaveOptions _saveOptions;

        public virtual int ModelVersion => 1;

        private void SetModelContext(ModelContext modelContext)
        {
            _modelContext = modelContext;
            _model = modelContext.Model;
            _keyProvider = modelContext.KeyProvider;
        }

        public JToken Serialize(ModelContext modelContext, ModelSaveOptions saveOptions)
        {
            SetModelContext(modelContext);
            _saveOptions = saveOptions;
            _modelContext.SerializeMetadata();
            return WriteRootModel();
        }

        public virtual bool Upgrade(ModelContext previousModelContext)
        {
            return true;
        }

        public virtual bool IsCompatibleWithCurrent()
        {
            return ModelConstants.CurrentModelVersion <= 2;
        }

        [Conditional("DEBUG")]
        private void LogDeserializeError(string elementName, string attrName)
        {
            DebugUtils.Log(
                $"[ModelJsonSerializer] Error deserializing model, validation of attribute '{attrName}' on element '{elementName}' failed.");
        }

        //public ModelLoadResult Deserialize(ModelContext modelContext, JObject jsonRoot, bool loadResourceKeys = false)
        public ModelLoadResult Deserialize(ModelContext modelContext, JObject jsonRoot, ModelLoadOptions loadOptions)
        {
            SetModelContext(modelContext);
            //var stopwatchUtil = StopwatchUtil.Create();
            var loadResourceKeys = loadOptions.LoadResourceKeys;
            bool upgrading = loadOptions.UpgradeFromModelVersion.HasValue;

            var result = new ModelLoadResult
            {
                Status = ModelLoadStatus.ModelSourceInvalid
            };

            _model = _modelContext.RecreateModel();

            if (jsonRoot == null || !jsonRoot.TryGetValue(ElementModel, out JToken jsonModelToken))
            {
                LogDeserializeError("model", ElementModel);
            }
            else
            {
                var jsonModel = (JObject)jsonModelToken;

                var jsonValid = false;
                if (jsonModel.TryGetValue(AttributeUID, out JToken attrUID))
                {
                    if (attrUID is JValue jValue && jValue.Type == JTokenType.String)
                    {
                        var uid = jValue.Value as string;
                        if (Guid.TryParse(uid, out Guid attrUIDGuid))
                        {
                            jsonValid = attrUIDGuid.Equals(ModelConstants.ModelUID);
                        }
                    }
                }

                if (!jsonValid)
                {
                    LogDeserializeError("model", AttributeUID);
                }

                // deserialize model element version
                if (jsonValid)
                {
                    jsonValid = TryGetJsonValue(jsonModel, AttributeVersion, out int modelVersion);
                    if (jsonValid)
                    {
                        _model.Version = modelVersion;

                        result.Status = modelVersion == ModelConstants.CurrentModelVersion || upgrading
                            ? ModelLoadStatus.Success
                            : ModelLoadStatus.ModelUpgraderRequired;

                        if (result.Status == ModelLoadStatus.ModelUpgraderRequired)
                        {
                            if (IsCompatibleWithCurrent())
                            {
                                result.Status = ModelLoadStatus.Success;
                            }
                            else
                            {
                                result.ModelVersion = modelVersion;
                            }
                        }
                    }
                    else
                    {
                        LogDeserializeError("model", AttributeVersion);
                    }
                }

                if (!jsonValid && result.Status == ModelLoadStatus.Success)
                {
                    result.Status = ModelLoadStatus.ModelSourceInvalid;
                }

                if (result.Status == ModelLoadStatus.Success)
                {
                    IModelElementKey key = _keyProvider.GenerateKey();
                    _model.UpdateKey(key);

                    var deserializationValid = false;
                    if (jsonModel.TryGetValue(AttributeName, out JToken attrName))
                    {
                        if (attrName is JValue jValue && jValue.Type == JTokenType.String)
                        {
                            _model.Name = jValue.Value as string;
                            deserializationValid = true;
                        }
                    }

                    // deserialize model description
                    if (!deserializationValid)
                    {
                        LogDeserializeError("model", AttributeName);
                    }
                    else
                    {
                        if (jsonModel.TryGetValue(AttributeDescription, out JToken attrDesc))
                        {
                            if (attrDesc is JValue jValue && jValue.Type == JTokenType.String)
                            {
                                _model.Description = jValue.Value as string;
                            }
                        }
                    }

                    // deserialize model options
                    if (deserializationValid)
                    {
                        deserializationValid = ReadModelOptions(jsonModel);

                        if (!deserializationValid)
                        {
                            LogDeserializeError("model", "base options");
                        }
                    }

                    // deserialize primary language
                    string primaryLanguage = null;
                    if (deserializationValid)
                    {
                        deserializationValid = TryGetJsonValue(jsonModel, AttributePrimaryLanguage,
                            out primaryLanguage);

                        if (deserializationValid)
                        {
                            deserializationValid = !primaryLanguage.IsNullOrEmpty() && CultureCache.Instance.ExistCulture(primaryLanguage);
                        }

                        if (!deserializationValid)
                        {
                            LogDeserializeError("model", "primary language");
                        }
                    }

                    // deserialize model languages
                    if (deserializationValid)
                    {
                        deserializationValid = ReadLanguages(jsonRoot, primaryLanguage);

                        if (!deserializationValid)
                        {
                            LogDeserializeError("model", "languages");
                        }
                    }

                    // deserialize model categories
                    if (deserializationValid)
                    {
                        deserializationValid = ReadCategories(jsonRoot, _model.Categories, null, loadResourceKeys);

                        if (!deserializationValid)
                        {
                            LogDeserializeError("model", "categories");
                        }
                    }

                    // deserialize model resources
                    if (deserializationValid)
                    {
                        deserializationValid = ReadResources(jsonRoot, _model.Resources, null, loadResourceKeys);

                        if (!deserializationValid)
                        {
                            LogDeserializeError("model", "resources");
                        }
                    }

                    // deserialize model metadatas
                    if (deserializationValid)
                    {
                        deserializationValid = ReadMetadatas(jsonRoot);

                        if (!deserializationValid)
                        {
                            LogDeserializeError("model", "metadata");
                        }
                    }

                    result.Status = deserializationValid
                        ? ModelLoadStatus.Success
                        : ModelLoadStatus.LoadError;
                }
            }

            if (result.Status == ModelLoadStatus.Success)
            {
                _modelContext.DeserializeMetadata();
            }

            //stopwatchUtil.Stop("Json Deserialize");

            return result;
        }

        private bool TryGetJsonValue<TResult>(JObject json, string attributeName, out TResult result)
        {
            result = default;
            bool valid = json.TryGetValue(attributeName, out JToken token);
            if (valid)
            {
                try
                {
                    result = token.Value<TResult>();
                }
                catch (Exception e)
                {
                    valid = false;
                    DebugUtils.Error($"TryGetJsonValue(attr: '{attributeName}') failed: " + e.GetFullExceptionDetails());
                }
            }

            return valid;
        }

        private bool ReadModelOptions(JObject jsonObject)
        {
            var modelOptions = new ModelOptions();
            var valid = true;

            if (!jsonObject.TryGetValue(ElementModelOptions, out JToken elementModelOptions))
            {
                LogDeserializeError("model", ElementModelOptions);
            }
            else
            {
                if (elementModelOptions is JObject jsonModelOptions)
                {
                    valid = TryGetJsonValue(jsonModelOptions, AttributeOptionsCategories, out bool optionsCategories);

                    if (!valid)
                    {
                        LogDeserializeError("model", AttributeOptionsCategories);
                    }
                    else
                    {
                        modelOptions.Categories = optionsCategories;

                        valid = TryGetJsonValue(jsonModelOptions, AttributeOptionsResources, out string optionsResourcesStr);
                        if (valid)
                        {
                            valid = ValueSerializer.TryDeserialize(optionsResourcesStr, true, out ModelOptionsResources optionsResources);
                            if (valid)
                            {
                                modelOptions.Resources = optionsResources;
                            }
                        }

                        if (!valid)
                        {
                            LogDeserializeError("model", AttributeOptionsResources);
                        }
                    }
                }
            }

            _model.Options = modelOptions;
            return valid;
        }

        private bool ReadMetadatas(JObject jsonObject)
        {
            var valid = true;

            if (jsonObject.TryGetValue(ElementMetadatas, out JToken elementMetadatas))
            {
                valid = DataNodeJsonSerializer.Deserialize(elementMetadatas as JObject, out DataNode metadatasNode);
                if (valid && metadatasNode != null)
                {
                    foreach (DataNode metadataElementNode in metadatasNode.Children)
                    {
                        valid = metadataElementNode.NodeName == ElementMetadata && metadataElementNode.Attributes != null
                            && metadataElementNode.Attributes.Count > 0;

                        if (valid)
                        {
                            //valid = false;
                            DataNodeAttributeCollection metadataAttributes = metadataElementNode.Attributes;
                            ModelMetadataDefinition metadata = _modelContext.CreateMetadataDefinition();

                            //IModelElementKey elementKey = null;
                            string descriptorUID = null;
                            DataNode content = null;

                            /*DataNodeAttribute attrKey = metadataAttributes[AttributeKey];
                            if (attrKey != null && !attrKey.Value.IsNullOrEmpty())
                            {
                                if (_keyProvider.TryParseKey(attrKey.Value, out elementKey))
                                {
                                    valid = true;
                                }
                            }*/

                            //if (valid)
                            {
                                valid = false;
                                DataNodeAttribute attrDescriptorUID = metadataAttributes[AttributeDescriptorUID];
                                if (attrDescriptorUID != null && !attrDescriptorUID.Value.IsNullOrEmpty())
                                {
                                    descriptorUID = attrDescriptorUID.Value;
                                    valid = true;
                                }
                            }

                            if (valid)
                            {
                                content = metadataElementNode[ElementContent];
                            }

                            if (valid)
                            {
                                _modelContext.SetModelMetadataDefinition(metadata, descriptorUID, content);
                                _model.Metadata.Add(metadata);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return valid;
        }

        private bool ReadResources(JObject jsonObject, ResourceElementList resources, ModelElement parentElement, bool loadResourceKeys)
        {
            var valid = true;

            if (jsonObject.TryGetValue(ElementResources, out JToken elementResources))
            {
                if (elementResources is JObject arrayResources)
                {
                    foreach (JObject jsonResource in arrayResources.Values().Cast<JObject>())
                    {
                        ResourceElement resource = _modelContext.CreateResource();
                        valid = ReadBaseModelElement(jsonResource, resource);
                        if (valid)
                        {
                            if (loadResourceKeys)
                            {
                                if (jsonResource.TryGetValue(AttributeKey, out JToken attrKey))
                                {
                                    var key = attrKey.Value<string>();
                                    if (key != null && !key.IsNullOrEmpty())
                                    {
                                        if (_keyProvider.TryParseKey(key, out IModelElementKey elementKey))
                                        {
                                            resource.UpdateKey(elementKey);
                                        }
                                    }
                                }
                            }

                            // attr 'state'
                            var translationState = ResourceElementTranslationState.New;
                            if (TryGetJsonValue(jsonResource, AttributeState, out string stateString))
                            {
                                ValueSerializer.TryDeserialize(stateString, true, out translationState);
                            }

                            resource.State = translationState;

                            // attr 'note'
                            if (jsonResource.TryGetValue(AttributeNote, out JToken jsonNote))
                            {
                                if (jsonNote is JValue jValue && jValue.Type == JTokenType.String)
                                {
                                    resource.TranslationNote = jValue.Value as string;
                                }
                            }

                            valid = ReadResourceParameters(jsonResource, resource) && ReadResourceValues(jsonResource, resource);
                            if (valid)
                            {
                                resource.SetParentElement(parentElement);
                                resources.Add(resource);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return valid;
        }

        private bool ReadResourceValues(JObject jsonResource, ResourceElement resource)
        {
            var valid = true;

            if (jsonResource.TryGetValue(ElementValues, out JToken elementValues))
            {
                if (elementValues is JObject arrayValues)
                {
                    foreach (JObject elementValue in arrayValues.Values().Cast<JObject>())
                    {
                        ResourceValueElement resourceValue = _modelContext.CreateResourceValue();
                        valid = ReadResourceValue(elementValue, resourceValue);
                        if (valid)
                        {
                            resourceValue.SetParentElement(resource);
                            resource.Values.Add(resourceValue);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return valid;
        }

        private bool ReadResourceValue(JObject jsonObject, ResourceValueElement resourceValue)
        {
            bool valid = ReadBaseModelElement(jsonObject, resourceValue);
            if (valid)
            {
                var jsonParent = jsonObject.Parent as JProperty;
                valid = jsonParent != null;
                if (valid)
                {
                    valid = !jsonParent.Name.IsNullOrEmpty();
                    if (valid)
                    {
                        resourceValue.LanguageName = jsonParent.Name;
                    }
                }

                if (valid)
                {
                    if (jsonObject.TryGetValue(AttributeValue, out JToken attrValue))
                    {
                        if (attrValue is JValue jValue && jValue.Type == JTokenType.String)
                        {
                            resourceValue.Value = jValue.Value as string;
                        }
                    }

                    TryGetJsonValue(jsonObject, AttributeLocked, out bool locked);
                    resourceValue.Locked = locked;

                    TryGetJsonValue(jsonObject, AttributeAuto, out bool auto);
                    resourceValue.Auto = auto;
                }
            }

            return valid;
        }

        private bool ReadResourceParameters(JObject jsonResource, ResourceElement resource)
        {
            var valid = true;

            if (jsonResource.TryGetValue(ElementParameters, out JToken elementParameters))
            {
                if (elementParameters is JObject jsonParameters)
                {
                    foreach (JObject elementParameter in jsonParameters.Values().Cast<JObject>())
                    {
                        ResourceParameterElement resourceParameter = _modelContext.CreateResourceParameter();
                        valid = ReadBaseModelElement(elementParameter, resourceParameter);
                        if (valid)
                        {
                            if (TryGetJsonValue(elementParameter, AttributeOrder, out int order))
                            {
                                resourceParameter.Order = order;
                            }

                            resourceParameter.SetParentElement(resource);
                            resource.Parameters.Add(resourceParameter);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return valid;
        }

        private bool ReadCategories(JObject jsonObject, CategoryElementList categories, ModelElement parentElement,
            bool loadResourceKeys)
        {
            var valid = true;

            if (jsonObject.TryGetValue(ElementCategories, out JToken elementCategories))
            {
                if (elementCategories is JObject jsonCategories)
                {
                    foreach (JObject jsonCategory in jsonCategories.Values().Cast<JObject>())
                    {
                        CategoryElement category = _modelContext.CreateCategory();
                        valid = ReadBaseModelElement(jsonCategory, category);
                        if (valid)
                        {
                            category.SetParentElement(parentElement);
                            categories.Add(category);

                            // read sub categories
                            ReadCategories(jsonCategory, category.Categories, category, loadResourceKeys);

                            // read sub resources
                            ReadResources(jsonCategory, category.Resources, category, loadResourceKeys);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return valid;
        }

        private bool ReadLanguages(JObject jsonObject, string primaryLanguage)
        {
            var valid = false;
            var modelLanguages = new List<LanguageElement>();

            if (!jsonObject.TryGetValue(ElementLanguages, out JToken elementLanguages))
            {
                LogDeserializeError("model", ElementLanguages);
            }
            else
            {
                if (elementLanguages is JArray jsonLanguages && jsonLanguages.Count > 0)
                {
                    foreach (JValue elementLanguage in jsonLanguages.Cast<JValue>())
                    {
                        LanguageElement language = _modelContext.CreateLanguage();
                        valid = elementLanguage.Value != null && elementLanguage.Value.GetType() == WellKnowTypes.System.String;

                        if (valid)
                        {
                            var languageName = elementLanguage.Value as string;
                            valid = CultureCache.Instance.ExistCulture(languageName);

                            if (valid)
                            {
                                language.Name = languageName;
                                language.IsPrimary = languageName == primaryLanguage;
                                modelLanguages.Add(language);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            if (valid)
            {
                _model.Languages = new LanguageElementList(modelLanguages);
            }

            return valid;
        }

        private bool ReadBaseModelElement(JObject jsonObject, ModelElementBase modelElement)
        {
            var modelNamedElement = modelElement as IModelNamedElement;
            var modelDescriptionElement = modelElement as IModelDescriptionElement;

            IModelElementKey elementKey = _keyProvider.GenerateKey();
            modelElement.UpdateKey(elementKey);

            var valid = true;
            if (modelNamedElement != null)
            {
                var jsonProperty = jsonObject.Parent as JProperty;
                valid = jsonProperty != null;
                if (valid)
                {
                    modelNamedElement.Name = jsonProperty.Name;
                }
            }

            if (modelDescriptionElement != null)
            {
                if (jsonObject.TryGetValue(AttributeDescription, out JToken attrDesc))
                {
                    modelDescriptionElement.Description = attrDesc.Value<string>();
                }
            }

            return valid;
        }

        private JToken WriteRootModel()
        {
            JToken result = new JObject();

            JToken jsonModel = new JObject();
            result[ElementModel] = jsonModel;

            jsonModel[AttributeUID] = ModelConstants.ModelUID.ToString("N");
            jsonModel[AttributeVersion] = _model.Version;

            if (!_model.Description.IsNullOrEmpty())
            {
                jsonModel[AttributeDescription] = _model.Description;
            }

            // write model options
            WriteModelOptions(jsonModel, _model.Options);

            if (!_model.Description.IsNullOrEmpty())
            {
                jsonModel[AttributeDescription] = _model.Description;
            }

            // write model name attribute
            jsonModel[AttributeName] = _model.Name;

            // write primary language name
            LanguageElement primaryLanguage = _model.Languages.GetPrimary() ?? _model.Languages.First();
            jsonModel[AttributePrimaryLanguage] = primaryLanguage.Name;

            // write languages list
            WriteLanguages(result);

            // write all, flatten list of categories
            WriteCategories(result, _model.Categories);

            // write all, flatten list of resources
            WriteResources(result, _model.Resources);

            // write model metadata
            WriteMetadatas(result);

            return result;
        }

        private void WriteModelOptions(JToken jsonModel, ModelOptions modelOptions)
        {
            JToken jsonModelOptions = new JObject();
            jsonModel[ElementModelOptions] = jsonModelOptions;

            jsonModelOptions[AttributeOptionsCategories] = modelOptions.Categories;
            jsonModelOptions[AttributeOptionsResources] = ValueSerializer.Serialize(modelOptions.Resources);
        }

        private void WriteMetadatas(JToken jsonModel)
        {
            ModelMetadataList metadata = _model.Metadata;
            if (metadata != null && metadata.Count > 0)
            {
                var metadataHolder = new DataNode();
                foreach (ModelMetadataDefinition definition in metadata)
                {
                    WriteMetadata(metadataHolder, definition);
                }

                if (metadataHolder.Children.Count > 0)
                {
                    string metadataRawJson = DataNodeJsonSerializer.Serialize(metadataHolder, _saveOptions);

                    jsonModel[ElementMetadatas] = new JRaw(metadataRawJson);
                }
            }
        }

        private void WriteMetadata(DataNode parentElementNode, ModelMetadataDefinition definition)
        {
            ArgumentValidator.EnsureArgumentNotNull(parentElementNode, "parentElementNode");
            ArgumentValidator.EnsureArgumentNotNull(definition, "definition");

            var descriptor = ModelMetadataDescriptorFactory.Instance.Get(definition.DescriptorUID);
            if (definition.Content != null || descriptor.AllowEmptyContent)
            {
                // <metadata>
                DataNode metadataElement = parentElementNode.AddChildren(ElementMetadata, null);

                // elementKey=""
                // string elementKey = definition.ElementKey.Serialize();
                // metadataElement.AddAttribute(AttributeKey, elementKey);

                // descriptorUID=""
                metadataElement.AddAttribute(AttributeDescriptorUID, DataNodeValueHelper.ToString(definition.DescriptorUID));

                if (definition.Content != null)
                {
                    // fixup node name in case custom metadata descriptor changed it (by mistake?!)
                    definition.Content.NodeName = ElementContent;
                    metadataElement.AddChildren(definition.Content);
                }
            }
        }

        private void WriteResources(JToken jsonParent, ResourceElementList resources)
        {
            if (resources.Count > 0)
            {
                var jsonResources = new JObject();
                foreach (ResourceElement resource in resources.OrderByName())
                {
                    var jsonResource = new JObject();
                    jsonResources[resource.Name] = jsonResource;

                    if (_saveOptions.SaveResourceKeys)
                    {
                        jsonResource[AttributeKey] = resource.Key.Serialize();
                    }

                    if (!resource.TranslationNote.IsNullOrEmpty())
                    {
                        jsonResource[AttributeNote] = resource.TranslationNote;
                    }

                    jsonResource[AttributeState] = ValueSerializer.Serialize(resource.State);

                    if (!resource.Description.IsNullOrEmpty())
                    {
                        jsonResource[AttributeDescription] = resource.Description;
                    }

                    WriteResourceParameters(jsonResource, resource);
                    WriteResourceValues(jsonResource, resource);
                }

                jsonParent[ElementResources] = jsonResources;
            }
        }

        private void WriteResourceValues(JObject jsonResource, ResourceElement resource)
        {
            if (resource.Values != null && resource.Values.Count > 0)
            {
                var jsonValues = new JObject();
                foreach (var resourceValue in resource.Values.OrderBy(x => x.LanguageName, StringComparer.InvariantCultureIgnoreCase))
                {
                    if (!resourceValue.Value.IsNullOrEmpty() || resourceValue.Auto || resourceValue.Locked)
                    {
                        var jsonValue = new JObject();
                        jsonValues[resourceValue.LanguageName] = jsonValue;

                        if (!resourceValue.Value.IsNullOrEmpty())
                        {
                            jsonValue[AttributeValue] = resourceValue.Value;
                        }

                        if (resourceValue.Locked)
                        {
                            jsonValue[AttributeLocked] = resourceValue.Locked;
                        }

                        if (resourceValue.Auto)
                        {
                            jsonValue[AttributeAuto] = resourceValue.Auto;
                        }
                    }
                }

                if (jsonValues.HasValues)
                {
                    jsonResource[ElementValues] = jsonValues;
                }
            }
        }

        private void WriteResourceParameters(JObject jsonResource, ResourceElement resource)
        {
            if (resource.Parameters != null && resource.Parameters.Count > 0)
            {
                var jsonParameters = new JObject();
                foreach (ResourceParameterElement resourceParameter in resource.Parameters.OrderBy(x => x.Order))
                {
                    var jsonParameter = new JObject();
                    jsonParameters[resourceParameter.Name] = jsonParameter;

                    if (!resourceParameter.Description.IsNullOrEmpty())
                    {
                        jsonParameter[AttributeDescription] = resourceParameter.Description;
                    }

                    jsonParameter[AttributeOrder] = resourceParameter.Order;
                }

                jsonResource[ElementParameters] = jsonParameters;
            }
        }

        private void WriteCategories(JToken jsonParent, CategoryElementList categories)
        {
            if (categories.Count > 0)
            {
                var jsonCategories = new JObject();
                foreach (CategoryElement category in categories.OrderByName())
                {
                    var jsonCategory = new JObject();
                    jsonCategories[category.Name] = jsonCategory;

                    if (!category.Description.IsNullOrEmpty())
                    {
                        jsonCategory[AttributeDescription] = category.Description;
                    }

                    if (!category.Description.IsNullOrEmpty())
                    {
                        jsonCategory[AttributeDescription] = category.Description;
                    }

                    if (category.Categories.Count > 0)
                    {
                        WriteCategories(jsonCategory, category.Categories);
                    }

                    if (category.Resources.Count > 0)
                    {
                        WriteResources(jsonCategory, category.Resources);
                    }
                }

                jsonParent[ElementCategories] = jsonCategories;
            }
        }

        private void WriteLanguages(JToken jsonModel)
        {
            if (_model.Languages.Count > 0)
            {
                string[] languages = _model.Languages
                    .Select(x => CultureCache.Instance.GetCulture(x.Name))
                    .OrderBy(x => x.LCID)
                    .Select(x => x.Name)
                    .ToArray();

                jsonModel[ElementLanguages] = new JArray(languages);
            }
        }
    }
}
