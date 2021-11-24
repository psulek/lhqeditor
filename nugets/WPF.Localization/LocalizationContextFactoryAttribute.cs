// /* Copyright (C) 2020 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
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
