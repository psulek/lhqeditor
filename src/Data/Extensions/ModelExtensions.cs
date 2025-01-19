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
using System.Globalization;
using System.Linq;
using LHQ.Utils.Extensions;
using LHQ.Utils.GenericTree;
using LHQ.Utils.Utilities;
using LHQ.Data.Interfaces;
using LHQ.Data.Interfaces.Key;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.Data.Extensions
{
    public static class ModelExtensions
    {
        public static List<CategoryElement> GetRootCategories(this Model model)
        {
            return model.Categories.Where(x => x.ParentKey == null).ToList();
        }

        public static IEnumerable<ModelElementBase> FlattenAllElements(this Model model, bool includeLanguages)
        {
            if (includeLanguages)
            {
                foreach (LanguageElement language in model.Languages)
                {
                    yield return language;
                }
            }

            foreach (CategoryElement category in model.FlattenAllCategories())
            {
                yield return category;
            }

            List<ResourceElement> allResources = model.FlattenAllResources();
            foreach (ResourceElement resource in allResources)
            {
                yield return resource;
            }

            foreach (ResourceParameterElement resourceParameter in allResources.SelectMany(x => x.Parameters))
            {
                yield return resourceParameter;
            }

            foreach (ResourceValueElement resourceValue in allResources.SelectMany(x => x.Values))
            {
                yield return resourceValue;
            }
        }

        public static List<CategoryElement> FlattenAllCategories(this Model model)
        {
            List<CategoryElement> rootCategories = model.GetRootCategories();
            return model.Categories.FlattenToList(category => category.Categories, rootCategories);
        }

        public static List<ResourceElement> FlattenAllResources(this Model model)
        {
            List<ResourceElement> resources = model.Resources.Where(x => x.ParentKey == null).ToList();

            var tree = new GenericTreeIterator<CategoryElement>(category => category.Categories, () => model.Categories);
            tree.IterateTree(false, null,
                x =>
                    {
                        if (x.Current.Resources != null)
                        {
                            resources.AddRange(x.Current.Resources);
                        }
                    });

            return resources;
        }

        public static string GetDisplayName<T>(this T element)
            where T : ModelElementBase
        {
            string result = string.Empty;

            if (element is IModelNamedElement namedElement)
            {
                result = namedElement.Name;
            }
            else if (element is ResourceValueElement resourceValue)
            {
                result = resourceValue.Value;
            }

            return result;
        }

        public static ModelElementType ToModelElementType(this string value)
        {
            if (!TryParseModelElemenType(value, out var modelElementType))
            {
                throw new ArgumentNullException(nameof(value));
            }

            return modelElementType;
        }

        public static bool TryParseModelElemenType(string value, out ModelElementType modelElementType)
        {
            bool result = !value.IsNullOrEmpty();
            modelElementType = default;

            if (result)
            {
                switch (value.ToUpperInvariant())
                {
                    case "M":
                    {
                        modelElementType = ModelElementType.Model;
                        break;
                    }
                    case "L":
                    {
                        modelElementType = ModelElementType.Language;
                        break;
                    }
                    case "C":
                    {
                        modelElementType = ModelElementType.Category;
                        break;
                    }
                    case "R":
                    {
                        modelElementType = ModelElementType.Resource;
                        break;
                    }
                    case "P":
                    {
                        modelElementType = ModelElementType.ResourceParameter;
                        break;
                    }
                    case "V":
                    {
                        modelElementType = ModelElementType.ResourceValue;
                        break;
                    }
                    default:
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        public static string ToShortString(this ModelElementType modelElementType)
        {
            switch (modelElementType)
            {
                case ModelElementType.Model:
                {
                    return "M";
                }
                case ModelElementType.Language:
                {
                    return "L";
                }
                case ModelElementType.Category:
                {
                    return "C";
                }
                case ModelElementType.Resource:
                {
                    return "R";
                }
                case ModelElementType.ResourceParameter:
                {
                    return "P";
                }
                case ModelElementType.ResourceValue:
                {
                    return "V";
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(modelElementType));
                }
            }
        }

        public static IOrderedEnumerable<T> OrderByName<T>(this IEnumerable<T> source)
            where T : IModelNamedElement
        {
            return OrderByName(source, StringComparer.InvariantCultureIgnoreCase);
        }

        public static IOrderedEnumerable<T> OrderByName<T>(this IEnumerable<T> source, IComparer<string> comparer)
            where T : IModelNamedElement
        {
            ArgumentValidator.EnsureArgumentNotNull(source, "source");

            return source.OrderBy(x => x.Name, comparer);
        }

        public static IOrderedEnumerable<T> OrderByKey<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
            where T : ModelElement
            where TKey : IModelElementKey
        {
            ArgumentValidator.EnsureArgumentNotNull(source, "source");

            var keyComparer = new ModelElementKeyComparer();
            return source.OrderBy(x => keySelector(x), keyComparer);
        }

        public static LanguageElement GetPrimary(this IEnumerable<LanguageElement> languages)
        {
            ArgumentValidator.EnsureArgumentNotNull(languages, "languages");
            return languages.SingleOrDefault(x => x.IsPrimary);
        }

        public static bool ContainsByName<T>(this IFindByNameList<T> list, string name, bool ignoreCase)
            where T : ModelElementBase, IModelNamedElement
        {
            return ContainsByName(list, name, ignoreCase, CultureInfo.InvariantCulture);
        }

        public static bool ContainsByName<T>(this IFindByNameList<T> list, string name, bool ignoreCase,
            CultureInfo cultureInfo)
            where T : ModelElementBase, IModelNamedElement
        {
            return list.FindByName(name, ignoreCase, cultureInfo) != null;
        }

        public static List<string> GetParentNames(this ITreeLikeElement treeElement, bool includeCurrent,
            bool fromRootToCurrent, bool includeRoot = true)
        {
            List<ITreeLikeElement> parents = treeElement.GetParents(includeCurrent, fromRootToCurrent, includeRoot);
            return parents.Select(x => x.Name).ToList();
        }

        private static List<ITreeLikeElement> GetParents(this ITreeLikeElement treeElement, bool includeCurrent,
            bool fromRootToCurrent, bool includeRoot = true)
        {
            ArgumentValidator.EnsureArgumentNotNull(treeElement, "treeElement");

            var result = new List<ITreeLikeElement>();
            if (includeCurrent)
            {
                result.Add(treeElement);
            }

            ITreeLikeElement parentCategory = treeElement.Parent;
            while (parentCategory != null)
            {
                bool isRoot = parentCategory.Parent == null;

                if (!isRoot || includeRoot)
                {
                    result.Add(parentCategory);
                }

                parentCategory = parentCategory.Parent;
            }

            if (fromRootToCurrent)
            {
                result.Reverse();
            }

            return result;
        }
        
        public static int GetParentLevel(this ITreeLikeElement element)
        {
            int result = 0;
            var current = element.Parent;
            while (current != null)
            {
                result++;
                current = current.Parent;
            }

            return result;
        }
    }
}
