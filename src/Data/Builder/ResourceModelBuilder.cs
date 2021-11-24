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

using System.Globalization;
using LHQ.Utils.Utilities;
using LHQ.Data.Interfaces.Builder;

namespace LHQ.Data.Builder
{
    internal sealed class ResourceModelBuilder : IResourceModelBuilder
    {
        private readonly ModelContext _modelContext;
        private ResourceElement _holderResource;

        public ResourceModelBuilder(ModelContext modelContext)
        {
            ArgumentValidator.EnsureArgumentNotNull(modelContext, "modelContext");
            _modelContext = modelContext;
        }

        internal void SetHolderResource(ResourceElement resource)
        {
            ArgumentValidator.EnsureArgumentNotNull(resource, "resource");
            _holderResource = resource;
        }

        public void AddParameter(string name)
        {
            ArgumentValidator.EnsureArgumentNotNull(_holderResource, "holderCategory");
            _modelContext.AddResourceParameter(name, _holderResource);
        }

        public void AddValue(string cultureName, string value)
        {
            ArgumentValidator.EnsureArgumentNotNull(_holderResource, "holderCategory");
            _modelContext.AddResourceValue(cultureName, value, _holderResource);
        }

        public void AddValue(CultureInfo cultureInfo, string value)
        {
            ArgumentValidator.EnsureArgumentNotNull(_holderResource, "holderCategory");
            _modelContext.AddResourceValue(cultureInfo, value, _holderResource);
        }
    }
}
