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

using System.Collections.Generic;
using LHQ.Utils;
using LHQ.Utils.Extensions;
using LHQ.Data.Extensions;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming

namespace LHQ.Data
{
    public static class Errors
    {
        public static class Model
        {
            private const int ErrorCodeStart = 100;

            public static ErrorDefinition ModelDoesNotContainLanguage(string languageName)
            {
                return ErrorDefinition.Create(ErrorCodeStart + 1,
                    () => "Model does not contain language '{0}'.".FormatWith(languageName));
            }

            public static ErrorDefinition ModelAlreadyContainsLanguage(string languageName)
            {
                return ErrorDefinition.Create(ErrorCodeStart + 2,
                    () => "Model already contains language '{0}'.".FormatWith(languageName));
            }

            public static ErrorDefinition ModelAlreadyContainOtherLanguageAsPrimary(string languageName)
            {
                return ErrorDefinition.Create(ErrorCodeStart + 3,
                    () => "Model already contain other language('{0}') as primary.".FormatWith(languageName));
            }

            public static ErrorDefinition ModelDoesNotContainCategory(string categoryName)
            {
                return ErrorDefinition.Create(ErrorCodeStart + 4,
                    () => "Model does not contain category '{0}'.".FormatWith(categoryName));
            }

            public static ErrorDefinition ModelAlreadyContainsCategory(string categoryName)
            {
                return ErrorDefinition.Create(ErrorCodeStart + 5,
                    () => "Model already contains category '{0}'.".FormatWith(categoryName));
            }

            public static ErrorDefinition CategoryAlreadyContainsChildCategory(string parentCategoryName, string categoryName)
            {
                return ErrorDefinition.Create(ErrorCodeStart + 6,
                    () => "Category '{0}' already contains child category '{1}'.".FormatWith(parentCategoryName, categoryName));
            }

            public static ErrorDefinition ModelAlreadyContainsResource(string resourceName)
            {
                return ErrorDefinition.Create(ErrorCodeStart + 7,
                    () => "Model already contains resource '{0}'.".FormatWith(resourceName));
            }

            public static ErrorDefinition CategoryAlreadyContainsChildResource(string parentCategoryName, string resourceName)
            {
                return ErrorDefinition.Create(ErrorCodeStart + 8,
                    () => "Category '{0}' already contains child resource '{1}'.".FormatWith(parentCategoryName, resourceName));
            }

            public static ErrorDefinition CategoryAndChildCategoryAreSame(string categoryName)
            {
                return ErrorDefinition.Create(ErrorCodeStart + 9,
                    () => "Circular reference detected: category and parent category '{0}' are the same.".FormatWith(categoryName));
            }

            public static ErrorDefinition ResourceAlreadyContainsParameter(string parentResourceName, string resourceParameterName)
            {
                return ErrorDefinition.Create(ErrorCodeStart + 10,
                    () => "Resource '{0}' already contains parameter '{1}'.".FormatWith(parentResourceName, resourceParameterName));
            }

            public static ErrorDefinition ResourceAlreadyContainsValueForLanguage(string parentResourceName, string resourceValueLanguageName)
            {
                return ErrorDefinition.Create(ErrorCodeStart + 11,
                    () => "Resource '{0}' already contains value for language '{1}'.".FormatWith(parentResourceName, resourceValueLanguageName));
            }

            public static ErrorDefinition CategoryDoesNotContainCategory(string parentCategoryName, string categoryName)
            {
                return ErrorDefinition.Create(ErrorCodeStart + 12,
                    () => "Category '{0}' does not contain category '{1}'.".FormatWith(parentCategoryName, categoryName));
            }
        }

        public static class Validation
        {
            private const int ErrorCodeStart = 1000;

            public static ErrorDefinition NoPrimaryLanguageFound = ErrorDefinition.Create(ErrorCodeStart + 2,
                () => "No primary language was found.");

            public static ErrorDefinition DuplicitLanguageFound(string languageName)
            {
                return ErrorDefinition.Create(ErrorCodeStart + 1,
                    () => "Duplicit language '{0}' found.".FormatWith(languageName));
            }

            public static ErrorDefinition MultiplePrimaryLanguagesFound(IEnumerable<string> languageNames)
            {
                string merged = languageNames.ToDelimitedString();
                return ErrorDefinition.Create(ErrorCodeStart + 3,
                    () => "Multiple primary languages ('{0}') found.".FormatWith(merged));
            }
        }

        public static class Traversal
        {
            private const int ErrorCodeStart = 2000;

            public static ErrorDefinition ElementOrderMustNotBeZero<T>(T element)
                where T : ModelElement
            {
                string elementTypeName = element.ElementType.ToString();
                string name = element.GetDisplayName();

                return ErrorDefinition.Create(ErrorCodeStart + 1,
                    () => "{0} '{1}' order must not be zero.".FormatWith(elementTypeName, name));
            }

            public static ErrorDefinition ParentElementMustNotBeNull<T>(T element)
                where T : ModelElement
            {
                string elementTypeName = element.ElementType.ToString();
                string name = element.GetDisplayName();

                return ErrorDefinition.Create(ErrorCodeStart + 2,
                    () => "{0} '{1}' parent must not be null.".FormatWith(elementTypeName, name));
            }
        }
    }
}
