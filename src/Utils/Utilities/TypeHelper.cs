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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using LHQ.Utils.Extensions;
// ReSharper disable UnusedMember.Global

namespace LHQ.Utils.Utilities
{
    public static class TypeHelper
    {
        /// <summary>
        /// Creates similar string as 'Type.AssemblyQualifiedName' but excludes 'Version=' and 'Culture=' parts.
        /// </summary>
        public static string GetSimpleAssemblyQualifiedName(this Type pluginModuleType)
        {
            return pluginModuleType.AssemblyQualifiedName;

            // var assemblyName = pluginModuleType.Assembly.GetName();
            // return $"{pluginModuleType.FullName}, {assemblyName.Name}, PublicKeyToken={assemblyName.GetPublicKeyTokenAsHex()}";
        }
        
        public static List<TInstanceInterface> CreateInstancesFromTypedAttributes<TAttribute, TInstanceInterface>(string operationName,
            Assembly[] assemblies = null)
            where TAttribute : Attribute, ITypedAttribute
            where TInstanceInterface : class
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(operationName, "operationName");
            string attributeName = typeof(TAttribute).Name;
            string interfaceFullName = typeof(TInstanceInterface).FullName;

            var result = new List<TInstanceInterface>();

            try
            {
                if (assemblies == null)
                {
                    assemblies = AppDomain.CurrentDomain.GetAssemblies();
                }

                foreach (Assembly assembly in assemblies)
                {
                    try
                    {
                        List<TAttribute> attributes = AttributeExtensions.GetCustomAttributes<TAttribute>(assembly);
                        foreach (TAttribute attribute in attributes)
                        {
                            if (attribute.TypeName.IsNullOrEmpty())
                            {
                                DebugUtils.Warning("{0}, skipping '{1}' attribute because its 'TypeName' is null or empty!"
                                    .FormatWith(operationName, attributeName));
                            }
                            else
                            {
                                Type type = Type.GetType(attribute.TypeName, false);
                                if (type == null)
                                {
                                    DebugUtils.Warning("{0}, could not find type '{1}' defined in '{2}' attribute."
                                        .FormatWith(operationName, attribute.TypeName, attributeName));
                                }
                                else
                                {
                                    object instance = Activator.CreateInstance(type);
                                    if (instance == null)
                                    {
                                        DebugUtils.Error("{0}, unable to create instance of type '{1}' defined in '{2}' attribute."
                                            .FormatWith(operationName, type.AssemblyQualifiedName, attributeName));
                                    }
                                    else
                                    {
                                        var @interface = instance as TInstanceInterface;
                                        if (@interface == null)
                                        {
                                            DebugUtils.Error(
                                                "{0}, type '{1}' (defined in '{2}' attribute) does not implemented required interface '{3}'!"
                                                    .FormatWith(operationName, type.AssemblyQualifiedName, attributeName, interfaceFullName));
                                        }
                                        else
                                        {
                                            if (result.Any(x => x.GetType() == type))
                                            {
                                                DebugUtils.Warning(
                                                    "{0}, type '{1}' (defined in '{2}' attribute) is already loaded, so this is duplicate registration of same attribute!"
                                                        .FormatWith(operationName, type.AssemblyQualifiedName, attributeName));
                                            }
                                            else
                                            {
                                                result.Add(@interface);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        DebugUtils.Error($"TypeHelper.CreateInstancesFromTypedAttributes('{operationName}') "+
                            $"load types of '{interfaceFullName}' from assembly '{assembly.FullName}' failed", e);
                    }
                }
            }
            catch (Exception e)
            {
                DebugUtils.Error($"TypeHelper.CreateInstancesFromTypedAttributes('{operationName}') " +
                    $"load types of '{interfaceFullName}' failed", e);
            }

            return result;
        }

        public static List<Type> LoadTypesFromTypedAttributes<TAttribute, TInstanceInterface>(string operationName,
            Assembly[] assemblies = null)
            where TAttribute : Attribute, ITypedAttribute
            where TInstanceInterface : class
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(operationName, "operationName");
            string attributeName = typeof(TAttribute).Name;
            Type interfaceType = typeof(TInstanceInterface);
            string interfaceFullName = interfaceType.FullName;

            var result = new List<Type>();

            try
            {
                if (assemblies == null)
                {
                    assemblies = AppDomain.CurrentDomain.GetAssemblies();
                }

                foreach (Assembly assembly in assemblies)
                {
                    try
                    {
                        List<TAttribute> attributes = AttributeExtensions.GetCustomAttributes<TAttribute>(assembly);
                        foreach (TAttribute attribute in attributes)
                        {
                            if (attribute.TypeName.IsNullOrEmpty())
                            {
                                DebugUtils.Warning("{0}, skipping '{1}' attribute because its 'TypeName' is null or empty!"
                                    .FormatWith(operationName, attributeName));
                            }
                            else
                            {
                                Type type = Type.GetType(attribute.TypeName, false);
                                if (type == null)
                                {
                                    DebugUtils.Warning("{0}, could not find type '{1}' defined in '{2}' attribute."
                                        .FormatWith(operationName, attribute.TypeName, attributeName));
                                }
                                else
                                {
                                    if (!interfaceType.IsAssignableFrom(type))
                                    {
                                        DebugUtils.Error("{0}, type '{1}' does not implement interface '{2}'."
                                            .FormatWith(operationName, type.AssemblyQualifiedName, interfaceFullName));
                                    }
                                    else
                                    {
                                        if (result.Any(x => x == type))
                                        {
                                            DebugUtils.Warning(
                                                "{0}, type '{1}' (defined in '{2}' attribute) is already loaded, so this is duplicate registration of same attribute!"
                                                    .FormatWith(operationName, type.AssemblyQualifiedName, attributeName));
                                        }
                                        else
                                        {
                                            result.Add(type);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        DebugUtils.Error($"TypeHelper.LoadTypesFromTypedAttributes('{operationName}') " +
                            $"load types of '{interfaceFullName}' from assembly '{assembly.FullName}' failed", e);
                    }
                }
            }
            catch (Exception e)
            {
                DebugUtils.Error($"TypeHelper.LoadTypesFromTypedAttributes('{operationName}') " +
                    $"load types of '{interfaceFullName}' failed", e);
            }

            return result;
        }

        public static Type GetTypeFromConfig<T>(string configKey, bool throwOnError)
        {
            string setting = ConfigurationManager.AppSettings[configKey];
            Type requestedType = typeof(T);
            if (setting.IsNullOrEmpty())
            {
                if (throwOnError)
                {
                    throw new InvalidOperationException(
                        "Could not create instance of type '{0}', no setting found under key '{1}'!".FormatWith(
                            requestedType.AssemblyQualifiedName, configKey));
                }
            }

            Type type = Type.GetType(setting, throwOnError);
            if (type != null)
            {
                if (!requestedType.IsAssignableFrom(type))
                {
                    throw new InvalidOperationException("Type '{0}' (under config key '{1}') does not implement type '{2}'!"
                        .FormatWith(type.AssemblyQualifiedName, configKey, requestedType.AssemblyQualifiedName));
                }
            }

            return type;
        }

        public static T CreateInstanceFromConfig<T>(string configKey, bool throwOnError)
        {
            Type type = GetTypeFromConfig<T>(configKey, throwOnError);
            T result = default;
            if (type != null)
            {
                result = (T)Activator.CreateInstance(type);
            }

            return result;
        }
    }
}
