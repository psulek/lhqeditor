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
using System.Linq;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.Data
{
    public partial class ModelContext
    {
        public ResourceValueElement AddResourceValue(CultureInfo cultureInfo, string value, ResourceElement parentResource)
        {
            return AddResourceValue(cultureInfo.Name, value, parentResource);
        }

        public ResourceValueElement AddResourceValue(string languageName, string value, ResourceElement parentResource)
        {
            var resourceValue = CreateResourceValue();
            resourceValue.LanguageName = languageName;
            resourceValue.Value = value;
            AddResourceValue(resourceValue, parentResource);
            return resourceValue;
        }

        internal void AddResourceValue(ResourceValueElement resourceValue, ResourceElement parentResource)
        {
            ArgumentValidator.EnsureArgumentNotNull(resourceValue, "resourceValue");
            ArgumentValidator.EnsureArgumentNotNull(parentResource, "parentResource");
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(resourceValue.LanguageName, "resourceValue.LanguageName");
            //ArgumentValidator.EnsureArgumentNotNullOrEmpty(resourceValue.Value, "resourceValue.Value");

            if (ContainsResourceValue(resourceValue.LanguageName, parentResource))
            {
                throw new ModelException(Model, resourceValue,
                    Errors.Model.ResourceAlreadyContainsValueForLanguage(parentResource.Name,
                        resourceValue.LanguageName));
            }

            parentResource.Values.Add(resourceValue);
            resourceValue.ParentElement = parentResource;
        }

        internal ResourceValueElement CreateResourceValue()
        {
            return new ResourceValueElement(GenerateKey()); //.SetRuntimeId(this.Model);
        }

        internal bool ContainsResourceValue(string resourceParameterLanguageName, ResourceElement parentResource)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(resourceParameterLanguageName, "resourceParameterName");
            ArgumentValidator.EnsureArgumentNotNull(parentResource, "parentResource");

            return parentResource.Values.Any(x => x.LanguageName.EqualsTo(resourceParameterLanguageName));
        }
    }
}