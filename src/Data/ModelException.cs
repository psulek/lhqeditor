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
using System.Runtime.Serialization;
using LHQ.Utils;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.Data
{
    public class ModelException : Exception
    {
        public ModelException(Model model, ErrorDefinition errorDefinition)
            : this(model, new ModelElementBase[0], errorDefinition)
        { }

        public ModelException(Model model, ModelElementBase element, ErrorDefinition errorDefinition)
            : this(model, new[] { element }, errorDefinition, null)
        { }

        public ModelException(Model model, IEnumerable<ModelElementBase> elements, ErrorDefinition errorDefinition)
            : this(model, elements, errorDefinition, null)
        { }

        public ModelException(Model model, IEnumerable<ModelElementBase> elements, ErrorDefinition errorDefinition,
            Exception innerException)
            : this(model, elements, errorDefinition.GetMessage(), innerException)
        { }

        public ModelException(Model model, string message)
            : this(model, new ModelElementBase[0], message)
        { }

        public ModelException(Model model, ModelElementBase element, string message)
            : this(model, new[] { element }, message)
        { }

        public ModelException(Model model, IEnumerable<ModelElementBase> elements, string message)
            : this(model, elements, message, null)
        { }

        public ModelException(Model model, IEnumerable<ModelElementBase> elements, string message, Exception innerException)
            : base(message, innerException)
        {
            Model = model;
            Elements = elements?.ToArray() ?? new ModelElementBase[0];
        }

        protected ModelException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public Model Model { get; }

        public ModelElementBase[] Elements { get; }
    }
}
