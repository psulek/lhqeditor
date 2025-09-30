#region License
// Copyright (c) 2025 Peter Šulek / ScaleHQ Solutions s.r.o.
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
using System.Linq;
using System.Text.RegularExpressions;
using LHQ.App.Model;
using LHQ.Utils.Extensions;

namespace LHQ.App.Code
{
    public static class NameHelper
    {
        public static string GenerateUniqueResourceName(string baseName, ICategoryLikeViewModel parentElement)
        {
            List<string> existingNames = parentElement.Children.Where(x => x.ElementType == TreeElementType.Resource)
                .Select(x => x.Name)
                .Distinct()
                .ToList();

            return GenerateUniqueName(baseName, existingNames);
        }

        public static string GenerateUniqueCategoryName(string baseName, ICategoryLikeViewModel parentElement)
        {
            List<string> existingNames = parentElement.Children.Where(x => x.ElementType == TreeElementType.Category)
                .Select(x => x.Name)
                .Distinct()
                .ToList();

            return GenerateUniqueName(baseName, existingNames);
        }

        private static string GenerateUniqueName(string baseName, List<string> existingNames)
        {
            bool containSameName = existingNames.Any(x => x.EqualsTo(baseName));
            if (!containSameName)
            {
                return baseName;
            }

            string newPropertyName = baseName + @"[0-9]+";
            int len = baseName.Length;
            List<string> matchingEntities = existingNames.Where(x => Regex.Match(x, newPropertyName, RegexOptions.IgnoreCase).Success).ToList();
            int maxIndex = matchingEntities.Any()
                ? matchingEntities.Select(x => int.Parse(x.Substring(len))).Max()
                : 0;

            if (maxIndex == 0)
            {
                return GenerateUniqueName($"{baseName}{maxIndex + 1}", existingNames);
            }

            return $"{baseName}{maxIndex + 1}";
        }
    }
}