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
using System.Globalization;
using System.Linq;
using LHQ.Data.Interfaces;
using LHQ.Utils.Utilities;

namespace LHQ.Data
{
    public partial class ModelContext
    {
        public CategoryElement AddCategory(string categoryName)
        {
            return AddCategory(categoryName, null);
        }

        public CategoryElement AddCategory(string categoryName, ICategoryLikeElement parentCategory)
        {
            var category = CreateCategory(categoryName);
            AddCategory(category, parentCategory);
            return category;
        }

        public void RemoveCategory(string categoryName, ICategoryLikeElement parentCategory)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(categoryName, "categoryName");

            var category = FindCategoryByName(categoryName, parentCategory);
            if (category == null)
            {
                throw new ModelException(Model, Errors.Model.ModelDoesNotContainCategory(categoryName));
            }

            Model.Categories.Remove(category);
        }

        public void RemoveCategory(CategoryElement category)
        {
            ArgumentValidator.EnsureArgumentNotNull(category, "category");

            var categoryList = category.ParentElement == null ? Model.Categories : category.ParentElement.Categories;

            string categoryName = category.Name;
            category = categoryList.FindByName(categoryName, true, CultureInfo.InvariantCulture);
            if (category == null)
            {
                throw new ModelException(Model, Errors.Model.ModelDoesNotContainCategory(categoryName));
            }

            categoryList.Remove(category);
        }

        internal void AddCategories(CategoryElement parentCategory, params CategoryElement[] categories)
        {
            ArgumentValidator.EnsureArgumentNotNull(categories, "categories");

            foreach (CategoryElement category in categories)
            {
                AddCategory(category, parentCategory);
            }
        }

        internal void AddCategory(CategoryElement category, ICategoryLikeElement parentCategory)
        {
            ArgumentValidator.EnsureArgumentNotNull(category, "category");
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(category.Name, "category.Name");

            bool containsCategory = ContainsCategory(category.Name, parentCategory);

            if (parentCategory == null)
            {
                if (containsCategory)
                {
                    throw new ModelException(Model, category,
                        Errors.Model.ModelAlreadyContainsCategory(category.Name));
                }

                Model.Categories.Add(category);
            }
            else
            {
                if (containsCategory)
                {
                    throw new ModelException(Model, category,
                        Errors.Model.CategoryAlreadyContainsChildCategory(parentCategory.Name, category.Name));
                }

                if (parentCategory == category)
                {
                    throw new ModelException(Model, category,
                        Errors.Model.CategoryAndChildCategoryAreSame(category.Name));
                }

                parentCategory.Categories.Add(category);
                category.ParentElement = parentCategory as CategoryElement;
            }
        }

        internal CategoryElement CreateCategory(string categoryName)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(categoryName, "categoryName");

            CategoryElement category = CreateCategory();
            category.Name = categoryName;
            return category;
        }

        internal CategoryElement CreateCategory()
        {
            return new CategoryElement(GenerateKey());
        }

        public CategoryElement FindCategory(ModelPath modelPath, int deep)
        {
            if (modelPath == null)
            {
                throw new ArgumentNullException(nameof(modelPath));
            }

            CategoryElement result = null;

            int pathPartsCount = modelPath.PathPartsCount;
            if (pathPartsCount > 0 && pathPartsCount >= deep)
            {
                string[] pathParts = modelPath.PathParts.Take(deep).ToArray();
                if (pathParts.Length > 1)
                {
                    CategoryElement parentCategory = null;

                    foreach (string findByName in pathParts)
                    {
                        parentCategory = FindCategoryByName(findByName, parentCategory);
                        if (parentCategory == null)
                        {
                            break;
                        }
                    }

                    result = parentCategory;
                }
                else
                {
                    result = FindCategoryByName(pathParts[0], null);
                }
            }

            return result;
        }


        internal CategoryElement FindCategoryByName(string categoryName, ICategoryLikeElement parentCategory)
        {
            var categoryList = parentCategory == null ? Model.Categories : parentCategory.Categories;
            return categoryList.FindByName(categoryName, true, CultureInfo.InvariantCulture);
        }

        internal bool ContainsCategory(string categoryName, ICategoryLikeElement parentCategory)
        {
            return FindCategoryByName(categoryName, parentCategory) != null;
        }
    }
}