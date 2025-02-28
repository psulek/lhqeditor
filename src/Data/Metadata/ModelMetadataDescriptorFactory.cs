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
using System.ComponentModel;
using System.Linq;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using LHQ.Data.Interfaces.Metadata;

namespace LHQ.Data.Metadata
{
    internal class ModelMetadataDescriptorFactory : DisposableObject
    {
        [Browsable(false)]
        private static readonly Lazy<ModelMetadataDescriptorFactory> _instance =
            new Lazy<ModelMetadataDescriptorFactory>(() => new ModelMetadataDescriptorFactory(), true);

        /// <summary>
        ///     Key - <see cref="IModelMetadataDescriptor.DescriptorUID" />, value - object implementing
        ///     <see cref="IModelMetadataDescriptor" />.
        /// </summary>
        private Dictionary<string, IModelMetadataDescriptor> _descriptors = new Dictionary<string, IModelMetadataDescriptor>();

        private ModelMetadataDescriptorFactory()
        {
            LoadAll();
        }

        public static ModelMetadataDescriptorFactory Instance => _instance.Value;

        private void LoadAll()
        {
            _descriptors.Clear();

            List<IModelMetadataDescriptor> descriptorInstances =
                TypeHelper.CreateInstancesFromTypedAttributes<ModelMetadataDescriptorAttribute, IModelMetadataDescriptor>(
                    "Model Metatada Descriptor");

            if (descriptorInstances != null && descriptorInstances.Count > 0)
            {
                foreach (IModelMetadataDescriptor descriptor in descriptorInstances)
                {
                    string descriptorUID = descriptor.DescriptorUID.ToString();

                    if (descriptorUID.IsNullOrEmpty())
                    {
                        DebugUtils.Error("Model metadata descriptor (type: {0}) have not specified its 'MetadataUID' which is required!"
                            .FormatWith(descriptor.GetType().AssemblyQualifiedName));
                    }
                    else
                    {
                        if (_descriptors.ContainsKey(descriptorUID))
                        {
                            DebugUtils.Warning(
                                "Model metadata descriptor (type: {0}, MetadataUID: {1}) is already registered, duplicity found!"
                                    .FormatWith(descriptor.GetType().AssemblyQualifiedName, descriptor.DescriptorUID));
                        }
                        else
                        {
                            _descriptors.Add(descriptorUID, descriptor);
                        }
                    }
                }
            }
        }

        protected override void DoDispose()
        {
            if (_descriptors != null)
            {
                _descriptors.Clear();
                _descriptors = null;
            }
        }

        internal IModelMetadataDescriptor Get(Guid descriptorUID)
        {
            string uid = descriptorUID.ToString();

            return _descriptors == null
                ? null
                : (_descriptors.Any(x => x.Key.EqualsTo(uid))
                    ? _descriptors.Single(x => x.Key.EqualsTo(uid)).Value
                    : null);
        }
    }
}
