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
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using LHQ.Data.Interfaces.Key;
using LHQ.Data.Interfaces.Metadata;
using LHQ.Data.Metadata;
using LHQ.Data.Support;

namespace LHQ.Data
{
    public partial class ModelContext
    {
        internal void SerializeMetadata()
        {
            ModelMetadataList metadata = Model.Metadata;
            if (metadata != null)
            {
                foreach (ModelMetadataDefinition definition in metadata)
                {
                    IModelMetadataDescriptor descriptor =
                        ModelMetadataDescriptorFactory.Instance.Get(definition.DescriptorUID);

                    if (descriptor != null)
                    {
                        IModelElementMetadata elementMetadata = definition.Metadata;
                        definition.Content = descriptor.Serialize(definition, elementMetadata, this);
                    }
                    else
                    {
                        DebugUtils.Error("ModelContext, serialize metadata for model '{0}', failed to retrieve metadata descriptor with UID '{1}'!"
                            .FormatWith(Model.Key.Serialize(), definition.DescriptorUID));
                    }
                }
            }
        }

        internal void DeserializeMetadata()
        {
            ModelMetadataList metadata = Model.Metadata;
            if (metadata != null)
            {
                var alreadyProcessed = new List<Guid>();
                var metadataToRemove = new List<ModelMetadataDefinition>();

                foreach (ModelMetadataDefinition definition in metadata)
                {
                    IModelMetadataDescriptor descriptor =
                        ModelMetadataDescriptorFactory.Instance.Get(definition.DescriptorUID);

                    if (descriptor != null)
                    {
                        bool deserializeResult = descriptor.Deserialize(definition, out var elementMetadata);
                        if (deserializeResult && elementMetadata != null)
                        {
                            if (!alreadyProcessed.Contains(definition.DescriptorUID))
                            {
                                alreadyProcessed.Add(definition.DescriptorUID);
                                definition.Metadata = elementMetadata;
                            }
                            else
                            {
                                metadataToRemove.Add(definition);
                            }

                            //definition.Metadata = elementMetadata;
                        }
                        else
                        {
                            metadataToRemove.Add(definition);

                            DebugUtils.Error(
                                "ModelContext, deserialize metadata for model '{0}' failed deserialize metadata using descriptor with UID '{1}'!"
                                    .FormatWith(Model.Key.Serialize(), definition.DescriptorUID));
                        }
                    }
                    else
                    {
                        DebugUtils.Error(
                            "ModelContext, deserialize metadata for model '{0}', failed to retrieve metadata descriptor with UID '{1}'!"
                                .FormatWith(Model.Key.Serialize(), definition.DescriptorUID));
                    }
                }

                foreach (var definition in metadataToRemove)
                {
                    if (Model.Metadata.Contains(definition))
                    {
                        Model.Metadata.Remove(definition);
                    }
                }
            }
        }

        public bool HasMetadata<TMetadata>()
            where TMetadata : IModelElementMetadata, new()
        {
            return Model.Metadata.HasMetadata<TMetadata>();
        }

        public void RemoveMetadata<TMetadata>()
            where TMetadata : IModelElementMetadata, new()
        {
            Model.Metadata.RemoveIfExist<TMetadata>();
        }

        public void RemoveAllMetadata<TMetadata>()
            where TMetadata : IModelElementMetadata, new()
        {
            Model.Metadata.RemoveAll(x => x.Metadata is TMetadata);
        }

        //public TMetadata GetMetadata<TMetadata>(ModelElementBase modelElement)
        public TMetadata GetMetadata<TMetadata>(Guid descriptorUID)
            where TMetadata : IModelElementMetadata, new()
        {
            return GetMetadataInternal(descriptorUID, default(TMetadata), false);
        }

        //internal void SetMetadata<TMetadata>(ModelElementBase modelElement, TMetadata metadata)
        internal void SetMetadata<TMetadata>(TMetadata metadata)
            where TMetadata : IModelElementMetadata, new()
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            //GetMetadataInternal(modelElement, metadata, true);
            GetMetadataInternal(metadata.DescriptorUID, metadata, true);
        }

        //private TMetadata GetMetadataInternal<TMetadata>(ModelElementBase modelElement,
        private TMetadata GetMetadataInternal<TMetadata>(Guid descriptorUID, TMetadata setMetadata, bool replaceExisting)
            where TMetadata : IModelElementMetadata, new()
        {
            ModelMetadataDefinition elementMetadataDefinition = null;
            ModelMetadataDefinition metadataDefinition = Model.Metadata.FindByUID(descriptorUID);
            if (metadataDefinition?.Metadata is TMetadata)
            {
                elementMetadataDefinition = metadataDefinition;
            } 
            else if (metadataDefinition != null && metadataDefinition.Content == null)
            {
                var toRemove = Model.Metadata.FindByUID(metadataDefinition.DescriptorUID);
                if (toRemove != null)
                {
                    Model.Metadata.Remove(toRemove);
                }
            }

            TMetadata elementMetadata = default(TMetadata);
            if (elementMetadataDefinition == null)
            {
                //descriptorUID = new TMetadata().DescriptorUID;

                IModelMetadataDescriptor metadataDescriptor = ModelMetadataDescriptorFactory.Instance.Get(descriptorUID);
                if (metadataDescriptor != null)
                {
                    IModelElementMetadata metadata = replaceExisting
                        ? setMetadata
                        : metadataDescriptor.Create();

                    if (metadata != null)
                    {
                        elementMetadata = (TMetadata)metadata;
                        elementMetadataDefinition = new ModelMetadataDefinition
                        {
                            DescriptorUID = descriptorUID,
                            Metadata = elementMetadata
                        };

                       
                        Model.Metadata.Add(elementMetadataDefinition);
                    }
                }
                else
                {
                    DebugUtils.Error("ModelContext.GetMetadata failed to retrieve metadata descriptor (uid: {0})".FormatWith(descriptorUID));
                }
            }
            else
            {
                if (replaceExisting) // update existing metadata
                {
                    elementMetadataDefinition.Metadata = setMetadata;
                }

                elementMetadata = (TMetadata)elementMetadataDefinition.Metadata;
            }

            return elementMetadata;
        }

        internal ModelMetadataDefinition CreateMetadataDefinition()
        {
            return new ModelMetadataDefinition();
        }

        internal void SetModelMetadataDefinition(ModelMetadataDefinition metadataDefinition,
            string descriptorUID, DataNode content)
        {
            ArgumentValidator.EnsureArgumentNotNull(metadataDefinition, "metadataDefinition");
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(descriptorUID, "descriptorUID");

            ModelMetadataDefinition definition = metadataDefinition;

            if (Guid.TryParse(descriptorUID, out var uid))
            {
                definition.DescriptorUID = uid;
            }

            definition.Content = content;
        }
    }
}
