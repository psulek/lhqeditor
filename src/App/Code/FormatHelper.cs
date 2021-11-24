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
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.Utils.Extensions;

namespace LHQ.App.Code
{
    public static class TextHelper
    {
        public static string GetElementCountText(TreeElementType elementType, int count, bool includeNumber, bool emptyOnZero = true)
        {
            if (!elementType.In(TreeElementType.Category, TreeElementType.Resource))
            {
                throw new ArgumentOutOfRangeException($"Element type '{elementType}' is not supported!");
            }

            return elementType == TreeElementType.Category
                ? GetCategoryCountText(count, includeNumber, emptyOnZero)
                : GetResourceCountText(count, includeNumber, emptyOnZero);
        }

        public static string GetResourceCountText(int count, bool includeNumber, bool emptyOnZero = true)
        {
            string result = string.Empty;
            string prefix = includeNumber ? $"{count} " : string.Empty;

            if (count == 0 && !emptyOnZero)
            {
                result = $"{prefix}{Strings.Common.CountTexts.Count0Resource}";
            }
            else if (count == 1)
            {
                result = $"{prefix}{Strings.Common.CountTexts.Count1Resource}";
            }
            else if (count == 2)
            {
                result = $"{prefix}{Strings.Common.CountTexts.Count2Resource}";
            }
            else if (count == 3)
            {
                result = $"{prefix}{Strings.Common.CountTexts.Count3Resource}";
            }
            else if (count == 4)
            {
                result = $"{prefix}{Strings.Common.CountTexts.Count4Resource}";
            }
            else if (count > 4)
            {
                result = $"{prefix}{Strings.Common.CountTexts.CountMoreResources}";
            }

            return result;
        }

        public static string GetCategoryCountText(int count, bool includeNumber, bool emptyOnZero = true)
        {
            string result = string.Empty;
            string prefix = includeNumber ? $"{count} " : string.Empty;

            if (count == 0 && !emptyOnZero)
            {
                result = $"{prefix}{Strings.Common.CountTexts.Count0Category}";
            }
            else if (count == 1)
            {
                result = $"{prefix}{Strings.Common.CountTexts.Count1Category}";
            }
            else if (count == 2)
            {
                result = $"{prefix}{Strings.Common.CountTexts.Count2Category}";
            }
            else if (count == 3)
            {
                result = $"{prefix}{Strings.Common.CountTexts.Count3Category}";
            }
            else if (count == 4)
            {
                result = $"{prefix}{Strings.Common.CountTexts.Count4Category}";
            }
            else if (count > 4)
            {
                result = $"{prefix}{Strings.Common.CountTexts.CountMoreCategories}";
            }

            return result;
        }

        public static string GetYesNoText(bool value)
        {
            return value ? Strings.Common.Yes : Strings.Common.No;
        }
    }
}
