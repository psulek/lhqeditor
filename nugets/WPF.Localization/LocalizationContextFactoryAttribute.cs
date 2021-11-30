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

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ScaleHQ.WPF.LHQ
{
    /// <summary>
    /// Assembly attribute to register factory which can provide LHQ strings context.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LocalizationContextFactoryAttribute : Attribute
    {
        /// <summary>
        /// Creates new instance of <see cref="LocalizationContextFactoryAttribute"/> class.
        /// </summary>
        /// <param name="typeName">Type name of localization context factory.</param>
        public LocalizationContextFactoryAttribute(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(typeName));
            }

            TypeName = typeName;
        }

        /// <summary>
        /// Creates new instance of <see cref="LocalizationContextFactoryAttribute"/> class.
        /// </summary>
        /// <param name="type">Type of localization context factory.</param>
        public LocalizationContextFactoryAttribute(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            TypeName = type.AssemblyQualifiedName;
        }

        /// <summary>
        /// Type name of localization context factory.
        /// </summary>
        public string TypeName { get; set; }
    }
}
