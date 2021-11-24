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

namespace LHQ.Data
{
    public partial class ModelContext
    {
        public ResourceParameterElement AddResourceParameter(string resourceParameterName, ResourceElement parentResource)
        {
            ResourceParameterElement resourceParameter = CreateResourceParameter(resourceParameterName);
            AddResourceParameter(resourceParameter, parentResource);
            return resourceParameter;
        }

        internal void AddResourceParameter(ResourceParameterElement resourceParameter, ResourceElement parentResource)
        {
            ArgumentValidator.EnsureArgumentNotNull(resourceParameter, "resourceParameter");
            ArgumentValidator.EnsureArgumentNotNull(parentResource, "parentResource");
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(resourceParameter.Name, "resourceParameter.Name");

            if (ContainsResourceParameter(resourceParameter.Name, parentResource))
            {
                throw new ModelException(Model, resourceParameter,
                    Errors.Model.ResourceAlreadyContainsParameter(parentResource.Name, resourceParameter.Name));
            }

            parentResource.Parameters.Add(resourceParameter);
            resourceParameter.ParentElement = parentResource;
        }

        internal ResourceParameterElement CreateResourceParameter(string resourceParameterName)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(resourceParameterName, "resourceParameterName");

            ResourceParameterElement resourceParameter = CreateResourceParameter();
            resourceParameter.Name = resourceParameterName;
            return resourceParameter;
        }

        internal ResourceParameterElement CreateResourceParameter()
        {
            return new ResourceParameterElement(GenerateKey()); //.SetRuntimeId(this.Model);
        }

        internal bool ContainsResourceParameter(string resourceParameterName, ResourceElement parentResource)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(resourceParameterName, "resourceParameterName");
            ArgumentValidator.EnsureArgumentNotNull(parentResource, "parentResource");

            return parentResource.Parameters.FindByName(resourceParameterName, true, CultureInfo.InvariantCulture)
                != null;
        }
    }
}
