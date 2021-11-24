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
using LHQ.Utils;
using LHQ.Utils.Utilities;
using LHQ.Data.Interfaces.Metadata;
// ReSharper disable UnusedMember.Global

namespace LHQ.Data.Metadata
{
    /// <summary>
    ///     Attribute to mark custom type as model metadata descriptor implementation of interface
    ///     <see cref="IModelMetadataDescriptor" />
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ModelMetadataDescriptorAttribute : Attribute, ITypedAttribute
    {
        public ModelMetadataDescriptorAttribute(string typeName)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(typeName, "typeName");
            TypeName = typeName;
        }

        public ModelMetadataDescriptorAttribute(Type type)
        {
            ArgumentValidator.EnsureArgumentNotNull(type, "type");
            TypeName = type.AssemblyQualifiedName;
        }

        /// <summary>
        ///     Type name of custom model metadata descriptor class (implementing from <see cref="IModelMetadataDescriptor" />
        ///     interface).
        /// </summary>
        public string TypeName { get; set; }
    }
}
