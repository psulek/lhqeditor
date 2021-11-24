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
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using LHQ.Data.Builder;
using LHQ.Data.Interfaces.Builder;
using LHQ.Data.Interfaces.Key;
using LHQ.Data.Interfaces.Metadata;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.Data
{
    public sealed partial class ModelContext : DisposableObject, IModelMetadataContainer
    {
        private HashSet<string> _allElementKeys = new HashSet<string>();

        public ModelContext(ModelContextOptions options)
        {
            Options = options;
            Builder = new ModelBuilder(this);
        }

        public Model Model { get; private set; }

        public ModelContextOptions Options { get; }

        public int AllElementCount => _allElementKeys.Count;

        public IModelBuilder Builder { get; }

        public IModelElementKeyProvider KeyProvider => Options.KeyProvider;

        /// <summary>
        ///     Creates new model with specified name.
        /// </summary>
        /// <param name="modelName">Name of model.</param>
        /// <returns>
        ///     Returns newly created model as <see cref="Data.Model" /> type, same as accessing property
        ///     <see cref="ModelContext.Model" /> on type <see cref="ModelContext" />.
        /// </returns>
        public Model CreateModel(string modelName)
        {
            if (Model != null)
            {
                throw new InvalidOperationException(
                    "ModelContext has already associated its Model, call ModelContext.Close() prior creating new model.");
            }

            Model = new Model(GenerateKey())
            {
                Name = modelName
            };
            return Model;
        }

        public void Close()
        {
            _allElementKeys?.Clear();

            if (Model != null)
            {
                Model = null;
            }
        }

        internal Model RecreateModel()
        {
            return RecreateModel(string.Empty);
        }

        internal Model RecreateModel(string modelName)
        {
            Close();
            return CreateModel(modelName);
        }

        public IModelMetadataContainer GetMetadata()
        {
            return this;
        }

        private void AddKeyToCache(IModelElementKey key)
        {
            ArgumentValidator.EnsureArgumentNotNull(key, "key");

            string serializedKey = key.Serialize();

            if (_allElementKeys.Contains(serializedKey))
            {
                throw new InvalidOperationException(
                    "Key provider type '{0}' generate non-unique key '{1}' within model!".FormatWith(KeyProvider.GetType().AssemblyQualifiedName,
                        serializedKey));
            }

            _allElementKeys.Add(serializedKey);
        }

        private void RemoveKeyFromCache(IModelElementKey key)
        {
            ArgumentValidator.EnsureArgumentNotNull(key, "key");

            string serializedKey = key.Serialize();

            if (_allElementKeys.Contains(serializedKey))
            {
                _allElementKeys.Remove(serializedKey);
            }
        }

        private IModelElementKey GenerateKey()
        {
            IModelElementKey key = KeyProvider.GenerateKey();
            AddKeyToCache(key);
            return key;
        }

        internal void UpdateKey(ModelElementBase element, IModelElementKey key)
        {
            ArgumentValidator.EnsureArgumentNotNull(element, "element");
            ArgumentValidator.EnsureArgumentNotNull(key, "key");

            if (element.Key == null || !element.Key.Equals(key))
            {
                // remove old key
                if (element.Key != null)
                {
                    RemoveKeyFromCache(element.Key);
                }

                RemoveKeyFromCache(key);

                element.UpdateKey(key);
                AddKeyToCache(key);
            }
        }

        protected override void DoDispose()
        {
            Close();
            _allElementKeys = null;
        }

        internal void AssignModelFrom(Model otherModel)
        {
            Model.Metadata = otherModel.Metadata;
            Model.Languages = otherModel.Languages;
            Model.Categories = otherModel.Categories;
            Model.Resources = otherModel.Resources;
            Model.Name = otherModel.Name;
            Model.Description = otherModel.Description;
            Model.Version = otherModel.Version;
            Model.Options = otherModel.Options;
        }
    }
}
