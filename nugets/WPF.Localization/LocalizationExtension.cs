// /* Copyright (C) 2020 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using IFormattable = System.IFormattable;
// ReSharper disable UnusedMember.Global

namespace ScaleHQ.WPF.LHQ
{
    /// <summary>
    /// Localization markup extension for WPF projects.
    /// Works with LHQ strings context singleton returned by <see cref="LocalizationContextFactoryBase"/> descendants.
    /// </summary>
    [ContentProperty("Bindings")]
    [MarkupExtensionReturnType(typeof(string))]
    public class LocalizationExtension : MarkupExtension
    {
        private const string PropertyNameCulture = "Culture";
        private Collection<BindingBase> _bindings;
        private IFormattable _localizationContext;

        private static bool? _inDesignMode;
        private static Type _contextFactoryType;
        private static LocalizationContextFactoryBase _contextFactoryProvider;
        private static readonly Type TypeLocalizationContextFactory = typeof(LocalizationContextFactoryBase);
        private static readonly Type TypeINotifyPropertyChanged = typeof(INotifyPropertyChanged);
        private static readonly Type TypeCultureInfo = typeof(CultureInfo);

        /// <summary>
        /// Creates new instance.
        /// </summary>
        public LocalizationExtension()
        { }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="key">resource key</param>
        /// <param name="source">The source.</param>
        public LocalizationExtension(string key, IFormattable source, string designText)
        {
            Key = key;
            Source = source;
        }

        /// <summary>
        /// Resource key.
        /// </summary>
        [ConstructorArgument("key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the source to <c>StringsContext.Instance</c> property
        /// </summary>
        [ConstructorArgument("source")]
        public IFormattable Source { get; set; }

        /// <summary>
        /// Gets or sets text to be displayed in design mode when localization context is not found.
        /// </summary>
        [ConstructorArgument("designText")]
        public string DesignText { get; set; }

        /// <summary>
        /// Initializes localization extension specified context factory type.
        /// </summary>
        /// <param name="contextFactoryType">Type of the context factory which implements abstract class <see cref="LocalizationContextFactoryBase"/>.</param>
        /// <exception cref="ArgumentNullException">contextFactoryType</exception>
        public static void Initialize(Type contextFactoryType)
        {
            if (contextFactoryType == null)
            {
                throw new ArgumentNullException(nameof(contextFactoryType));
            }

            if (!TypeLocalizationContextFactory.IsAssignableFrom(contextFactoryType))
            {
                throw new InvalidOperationException(
                    $"Type '{contextFactoryType.FullName}' must implement abstract class '{TypeLocalizationContextFactory.FullName}' !");
            }

            _contextFactoryType = contextFactoryType;
        }

        /// <summary>
        /// Initializes localization extension with specified context factory provider.
        /// </summary>
        /// <param name="contextFactoryProvider">The context factory provider.</param>
        public static void Initialize(LocalizationContextFactoryBase contextFactoryProvider)
        {
            _contextFactoryProvider = contextFactoryProvider ?? throw new ArgumentNullException(nameof(contextFactoryProvider));
        }

        /// <summary>
        ///     The bindings to pass as arguments to the format string.
        /// </summary>
        public Collection<BindingBase> Bindings => _bindings ?? (_bindings = new Collection<BindingBase>());

        private static bool IsInDesignMode()
        {
            if (_inDesignMode == null)
            {
#if DEBUG
                _inDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
#else
                _inDesignMode = false;
#endif
            }

            return _inDesignMode.Value;
        }

        private IFormattable FindContextSourceInResources(FrameworkElement frameworkElement)
        {
            return FindContextSourceInResources(frameworkElement.Resources.MergedDictionaries);
        }

        private IFormattable FindContextSourceInResources(Collection<ResourceDictionary> parentCollection)
        {
            IFormattable FindRecursive(Collection<ResourceDictionary> collection)
            {
                foreach (ResourceDictionary resourceDictionary in collection)
                {
                    foreach (DictionaryEntry dictionaryEntry in resourceDictionary)
                    {
                        if (dictionaryEntry.Value is LocalizationContextSource localizationContextSource)
                        {
                            return localizationContextSource.Source;
                        }
                    }

                    if (resourceDictionary.MergedDictionaries.Count > 0)
                    {
                        return FindRecursive(resourceDictionary.MergedDictionaries);
                    }
                }

                return null;
            }

            return FindRecursive(parentCollection);
        }

        private IFormattable GetLocalizationContext(IServiceProvider serviceProvider, out DependencyObject targetObjectOut)
        {
            targetObjectOut = null;
            if (_localizationContext == null)
            {
                try
                {
                    var context = Source;

                    if (context == null)
                    {
                        context = FindContextFromAssemblyFactory(null);
                        if (context == null && serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget target)
                        {
                            var targetObject = target.TargetObject;
                            if (targetObject != null)
                            {
                                if (targetObject is DependencyObject currentDependencyObject)
                                {
                                    targetObjectOut = currentDependencyObject;
                                    DependencyObject lastCurrent = null;

                                    try
                                    {
                                        context = currentDependencyObject.TryFindParentDependencyProperty(item =>
                                            {
                                                lastCurrent = item;
                                                var localizationSource = LocalizationSourceBehavior.GetLocalizationSource(item);
                                                if (localizationSource == null && item is FrameworkElement frameworkElement)
                                                {
                                                    localizationSource = FindContextSourceInResources(frameworkElement);
                                                }

                                                return localizationSource;
                                            });
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.WriteLine($"[LHQ] {nameof(LocalizationExtension)}.{nameof(GetLocalizationContext)}() try parent DP failed, " +
                                            $"message: {e.Message}, stackTrace: {e.StackTrace}");
                                    }

                                    if (context == null)
                                    {
                                        try
                                        {
                                            if (lastCurrent != null && lastCurrent.GetParentObject() == null)
                                            {
                                                context = FindContextFromAssemblyFactory(lastCurrent);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }

                                        if (context == null && Application.Current != null && !IsInDesignMode())
                                        {
                                            context = FindContextSourceInResources(Application.Current.Resources.MergedDictionaries);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // try to find context factory on entry assembly level
                    if (context == null)
                    {
                        try
                        {
                            var contextFactoryBase = FindContextFactoryFromAssembly(Assembly.GetEntryAssembly());
                            if (contextFactoryBase != null)
                            {
                                context = contextFactoryBase.GetSingleton();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }

                    if (context != null)
                    {
                        Type localizationContextType = context.GetType();
                        PropertyInfo propertyCulture = localizationContextType.GetProperties().SingleOrDefault(x => x.Name == PropertyNameCulture);
                        if (propertyCulture == null || propertyCulture.PropertyType != TypeCultureInfo)
                        {
                            throw new InvalidOperationException(
                                $"Invalid localization context type '{localizationContextType.FullName}' ! " +
                                $"Missing property name '{PropertyNameCulture}' of type '{TypeCultureInfo.FullName}' !");
                        }

                        if (!TypeINotifyPropertyChanged.IsAssignableFrom(localizationContextType))
                        {
                            throw new InvalidOperationException(
                                $"Invalid localization context type '{localizationContextType.FullName}' ! " +
                                $"Type must implement interface '{TypeINotifyPropertyChanged.FullName}' !");
                        }

                        _localizationContext = context;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"[LHQ] {nameof(LocalizationExtension)}.{nameof(GetLocalizationContext)}() failed, " +
                        $"message: {e.Message}, stackTrace: {e.StackTrace}");
                }
            }

            return _localizationContext;
        }

        private LocalizationContextFactoryBase FindContextFactoryFromAssembly(Assembly assembly)
        {
            LocalizationContextFactoryBase result = null;

            var attributes = assembly.GetCustomAttributes<LocalizationContextFactoryAttribute>().ToList();
            if (attributes.Count > 0)
            {
                if (attributes.Count > 1)
                {
                    throw new InvalidOperationException(
                        $"Assembly '{assembly.FullName}' contains multiple [{nameof(LocalizationContextFactoryAttribute)}(...)] definitions. " +
                        "Only one attribute can be specified for assembly!");
                }

                var contextFactoryAttribute = attributes.First();

                if (!string.IsNullOrEmpty(contextFactoryAttribute.TypeName))
                {
                    var factoryType = Type.GetType(contextFactoryAttribute.TypeName, false);

                    if (factoryType == null)
                    {
                        throw new InvalidOperationException(
                            $"Could not found type '{contextFactoryAttribute.TypeName}' specified in '{typeof(LocalizationContextFactoryAttribute).FullName}' " +
                            $"in assembly '{assembly.FullName}'!");
                    }


                    try
                    {
                        var localizationContextFactory =
                            Activator.CreateInstance(factoryType) as LocalizationContextFactoryBase;

                        _contextFactoryProvider = localizationContextFactory ?? throw new InvalidOperationException(
                                                      $"Type '{factoryType.FullName}' does not implement abstract class '{TypeLocalizationContextFactory.FullName}' !");

                        result = localizationContextFactory;
                    }
                    catch (Exception e)
                    {
                        throw new InvalidOperationException(
                            $"Error creating instance of type '{factoryType.FullName}' !",
                            e);
                    }
                }
            }

            return result;
        }

        private IFormattable FindContextFromAssemblyFactory(object targetObject)
        {
            IFormattable result = null;

            LocalizationContextFactoryBase localizationContextFactory = _contextFactoryProvider;

            if (localizationContextFactory == null)
            {
                Type factoryType = _contextFactoryType;

                if (factoryType == null && targetObject != null)
                {
                    localizationContextFactory = FindContextFactoryFromAssembly(targetObject.GetType().Assembly);
                }

                if (factoryType != null && localizationContextFactory == null)
                {
                    try
                    {
                        localizationContextFactory = Activator.CreateInstance(factoryType) as LocalizationContextFactoryBase;

                        _contextFactoryProvider = localizationContextFactory ?? throw new InvalidOperationException(
                            $"Type '{factoryType.FullName}' does not implement abstract class '{TypeLocalizationContextFactory.FullName}' !");
                    }
                    catch (Exception e)
                    {
                        throw new InvalidOperationException(
                            $"Error creating instance of type '{factoryType.FullName}' !",
                            e);
                    }
                }
            }

            if (localizationContextFactory != null)
            {
                result = localizationContextFactory.GetSingleton();
            }

            return result;
        }

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IFormattable localizationContext = GetLocalizationContext(serviceProvider, out var _);
            if (localizationContext == null)
            {
                return IsInDesignMode() && !string.IsNullOrEmpty(DesignText) ? DesignText : $"@@{Key}@@";
            }

            var binding = new Binding(PropertyNameCulture)
            {
                Source = localizationContext, 
                Mode = BindingMode.TwoWay
            };

            if (_bindings == null || _bindings.Count == 0)
            {
                binding.Converter = new LocalizationConverter(localizationContext, Key);
                object value = binding.ProvideValue(serviceProvider);
                return value;
            }

            var multiBinding = new MultiBinding
            {
                Mode = BindingMode.TwoWay,
                Converter = new LocalizationConverter(localizationContext, Key, _bindings)
            };
            
            multiBinding.Bindings.Add(binding);

            foreach (var item in _bindings)
            {
                multiBinding.Bindings.Add(item);
            }

            return multiBinding.ProvideValue(serviceProvider);
        }
    }
}