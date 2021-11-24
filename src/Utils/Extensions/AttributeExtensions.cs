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
using System.Reflection;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.Utils.Extensions
{
    public static class AttributeExtensions
    {
        public static List<Attribute> GetCustomAttributes(this ICustomAttributeProvider attributeProvider, Predicate<Attribute> match)
        {
            return GetCustomAttributes<Attribute>(attributeProvider, match);
        }

        public static T GetCustomAttribute<T>(this ICustomAttributeProvider attributeProvider)
            where T : Attribute
        {
            List<T> attributes = GetCustomAttributes<T>(attributeProvider);
            if (attributes.Count > 0)
            {
                return attributes[0];
            }

            return default;
        }

        public static List<T> GetCustomAttributes<T>(this ICustomAttributeProvider attributeProvider)
            where T : Attribute
        {
            return GetCustomAttributes<T>(attributeProvider, attribute => attribute is T);
        }

        public static List<T> GetCustomAttributes<T>(this ICustomAttributeProvider attributeProvider, Predicate<Attribute> match)
            where T : Attribute
        {
            var result = new List<T>();

            object[] attributes = attributeProvider.GetCustomAttributes(true);
            foreach (Attribute attribute in attributes)
            {
                if (match(attribute))
                {
                    result.Add(attribute as T);
                }
            }

            return result;
        }
    }
}
