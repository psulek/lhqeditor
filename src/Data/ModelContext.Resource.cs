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
using LHQ.Data.Interfaces;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.Data
{
    public partial class ModelContext
    {
        public ResourceElement AddResource(string resourceName, ICategoryLikeElement parentCategory)
        {
            ResourceElement resource = CreateResource(resourceName);
            AddResource(resource, parentCategory);
            return resource;
        }

        internal ResourceElement CreateResource(string resourceName)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(resourceName, "resourceName");

            ResourceElement resource = CreateResource();
            resource.Name = resourceName;
            return resource;
        }

        public void MoveResource(ResourceElement resource, CategoryElement newParentCategory)
        {
            ArgumentValidator.EnsureArgumentNotNull(resource, "resource");

            bool removalResult = RemoveResource(resource);
            if (!removalResult)
            {
                DebugUtils.Warning(
                    "Move resource with key '{0}' failed, cannot deatch resource from existing parent element association."
                        .FormatWith(resource.Key.Serialize()));
            }

            AddResource(resource, newParentCategory);
        }

        public void RemoveResource(string resourceName, CategoryElement parentCategory)
        {
            ResourceElement resource = FindResourceByName(resourceName, parentCategory);
            if (resource != null)
            {
                RemoveResource(resource);
            }
        }

        internal bool RemoveResource(ResourceElement resource)
        {
            ArgumentValidator.EnsureArgumentNotNull(resource, "resource");
            bool result;

            CategoryElement parentCategory = resource.ParentElement;
            if (parentCategory == null)
            {
                // remove from Model...
                ResourceElement founded = Model.Resources.FindByKey(resource.Key);
                result = founded != null;
                if (result)
                {
                    Model.Resources.Remove(founded);
                }
            }
            else
            {
                ResourceElement foundedResource = parentCategory.Resources.FindByKey(resource.Key);
                result = foundedResource != null;
                if (result)
                {
                    parentCategory.Resources.Remove(foundedResource);
                }
            }

            return result;
        }

        internal ResourceElement CreateResource()
        {
            return new ResourceElement(GenerateKey()); //.SetRuntimeId(this.Model);
        }

        internal void AddResource(ResourceElement resource, ICategoryLikeElement parentCategory)
        {
            ArgumentValidator.EnsureArgumentNotNull(resource, "resource");
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(resource.Name, "resource.Name");

            bool containsResource = ContainsResource(resource.Name, parentCategory);

            if (parentCategory == null)
            {
                if (containsResource)
                {
                    throw new ModelException(Model, resource, Errors.Model.ModelAlreadyContainsResource(resource.Name));
                }

                Model.Resources.Add(resource);
                resource.ParentElement = null;
            }
            else
            {
                if (containsResource)
                {
                    throw new ModelException(Model, resource,
                        Errors.Model.CategoryAlreadyContainsChildResource(parentCategory.Name, resource.Name));
                }

                parentCategory.Resources.Add(resource);
                resource.ParentElement = parentCategory as CategoryElement;
            }

            //resource.ParentElement = parentCategory ;
        }

        internal ResourceElement FindResourceByName(string resourceName, ICategoryLikeElement parentCategory)
        {
            ResourceElementList resources = parentCategory == null ? Model.Resources : parentCategory.Resources;
            return resources.FindByName(resourceName, true, CultureInfo.InvariantCulture);
        }

        internal bool ContainsResource(string resourceName, ICategoryLikeElement parentCategory)
        {
            return FindResourceByName(resourceName, parentCategory) != null;
        }
    }
}
