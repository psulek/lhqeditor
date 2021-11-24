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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LHQ.App.Extensions;
using LHQ.App.Model;
using LHQ.Data;
using LHQ.Utils.Utilities;
using Syncfusion.Data.Extensions;
using Syncfusion.SfSkinManager;

namespace LHQ.App.Code
{
    public class VisualManager : ObservableObject
    {
        private const string FontSizeUnits = "11pt";
        private const string FontFamilyName = "Segoe UI";
        private static readonly FontSizeConverter _fontSizeConverter = new FontSizeConverter();

        private static readonly Lazy<VisualManager> _instance = new Lazy<VisualManager>(() => new VisualManager());

        private Brush _headerBackgroundBrush;
        private SolidColorBrush _headerForegroundBrush;
        private Brush _backgroundBrush;
        private Brush _borderBrush;
        private Brush _contentForegroundBrush;
        private Brush _contentBackgroundBrush;
        private Brush _selectionInActiveBrush;
        private Style _defaultListViewItemStyle;
        private Style _defaultMenuItemStyle;

        private Style _defaultListBoxItemStyle;
        private Style _defaultGridViewColumnHeaderStyle;

        private Dictionary<string, SolidColorBrush> _resourceStateBrushStyles;
        private Dictionary<string, SolidColorBrush> _resourceIconBrushStyles;
        private Style _multiSelectionTreeResourceStateBorderStyle;
        private Style _defaultComboBoxStyle;

        private bool _notifyPropertyChangeBlocked;
        private bool _resourcesInitialized;
        private VisualStyles? _resourcesInitializedForVisualStyles;
        private Collection<ResourceDictionary> _themedResources;
        private Brush _activeBackgroundBrush;
        private Brush _hoverBackgroundBrush;
        
        private const double DefaultFontSize = 14.666666666666666;

        private VisualManager()
        {
            _resourcesInitialized = false;
            _resourcesInitializedForVisualStyles = null;

            var defaultTheme = AppVisualTheme.Light;
            Theme = defaultTheme;
            VisualStyle = defaultTheme.ToVisualStyles();
            InternalSetTheme(defaultTheme);

            FontFamily = new FontFamily(FontFamilyName);
            
            object fontSizeObj = _fontSizeConverter.ConvertFrom(FontSizeUnits);
            FontSize = (double?)fontSizeObj ?? DefaultFontSize;
        }

        public static VisualManager Instance => _instance.Value;

        public VisualStyles VisualStyle { get; private set; }

        public AppVisualTheme Theme { get; private set; }

        public FontFamily FontFamily { get; }

        public double FontSize { get; }

        public Style TextBoxStyle { get; private set; }

        public Brush HeaderBackgroundBrush
        {
            get => _headerBackgroundBrush;
            private set => SetProperty(ref _headerBackgroundBrush, value);
        }

        public SolidColorBrush HeaderForegroundBrush
        {
            get => _headerForegroundBrush;
            private set => SetProperty(ref _headerForegroundBrush, value);
        }

        public Brush BackgroundBrush
        {
            get => _backgroundBrush;
            private set => SetProperty(ref _backgroundBrush, value);
        }

        public Brush BorderBrush
        {
            get => _borderBrush;
            private set => SetProperty(ref _borderBrush, value);
        }

        public Brush ContentForegroundBrush
        {
            get => _contentForegroundBrush;
            private set => SetProperty(ref _contentForegroundBrush, value);
        }

        public Brush ContentBackgroundBrush
        {
            get => _contentBackgroundBrush;
            private set => SetProperty(ref _contentBackgroundBrush, value);
        }

        public Brush SelectionInActiveBrush
        {
            get => _selectionInActiveBrush;
            private set => SetProperty(ref _selectionInActiveBrush, value);
        }

        public Brush ActiveBackgroundBrush
        {
            get => _activeBackgroundBrush;
            set => SetProperty(ref _activeBackgroundBrush, value);
        }

        public Brush HoverBackgroundBrush
        {
            get => _hoverBackgroundBrush;
            set => SetProperty(ref _hoverBackgroundBrush, value);
        }

        public Style DefaultListViewItemStyle
        {
            get => _defaultListViewItemStyle;
            private set => SetProperty(ref _defaultListViewItemStyle, value);
        }

        public Style DefaultMenuItemStyle
        {
            get => _defaultMenuItemStyle;
            private set => SetProperty(ref _defaultMenuItemStyle, value);
        }

        public Style DefaultListBoxItemStyle
        {
            get => _defaultListBoxItemStyle;
            private set => SetProperty(ref _defaultListBoxItemStyle, value);
        }

        public Style DefaultGridViewColumnHeaderStyle
        {
            get => _defaultGridViewColumnHeaderStyle;
            set => SetProperty(ref _defaultGridViewColumnHeaderStyle, value);
        }

        public Style DefaultComboBoxStyle
        {
            get => _defaultComboBoxStyle;
            set => SetProperty(ref _defaultComboBoxStyle, value);
        }

        public Style MultiSelectionTreeResourceStateBorderStyle
        {
            get => _multiSelectionTreeResourceStateBorderStyle;
            set => SetProperty(ref _multiSelectionTreeResourceStateBorderStyle, value);
        }

        public SolidColorBrush ModelElementIconBrush => GetTreeElementIconBrush(TreeElementType.Model);

        public SolidColorBrush CategoryElementIconBrush => GetTreeElementIconBrush(TreeElementType.Category);

        public SolidColorBrush ResourceElementIconBrush => GetTreeElementIconBrush(TreeElementType.Resource);

        protected internal override bool IsBlockedPropertyChange()
        {
            return _notifyPropertyChangeBlocked;
        }

        public void BlockChangeNotifications(bool block)
        {
            _notifyPropertyChangeBlocked = block;
        }

        public void SetTheme(AppVisualTheme theme)
        {
            InternalSetTheme(theme);
        }

        private void InternalSetTheme(AppVisualTheme theme)
        {
            if (theme == AppVisualTheme.Automatic)
            {
                throw new InvalidOperationException("Could not use 'AppVisualTheme.Automatic' directly to set theme for!");
            }

            Theme = theme;
            VisualStyle = theme.ToVisualStyles();
            RaisePropertyChanged(nameof(Theme));
            RaisePropertyChanged(nameof(VisualStyle));
        }

        public void ApplyTheme(FrameworkElement frameworkElement)
        {
            InternalApplyTheme(frameworkElement, VisualStyle);
        }

        private void InternalApplyTheme(FrameworkElement frameworkElement, VisualStyles visualStyles)
        {
            try
            {
                bool doInitialize = !_resourcesInitialized ||
                    _resourcesInitializedForVisualStyles.HasValue && _resourcesInitializedForVisualStyles.Value != visualStyles;

                if (doInitialize)
                {
                    // reset all previously stored resources for other visual style
                    HeaderBackgroundBrush = null;
                    HeaderForegroundBrush = null;
                    BackgroundBrush = null;
                    BorderBrush = null;
                    ContentForegroundBrush = null;
                    ContentBackgroundBrush = null;
                    SelectionInActiveBrush = null;
                    ActiveBackgroundBrush = null;
                    HoverBackgroundBrush = null;
                    DefaultListViewItemStyle = null;
                    DefaultMenuItemStyle = null;
                    DefaultListBoxItemStyle = null;
                    DefaultGridViewColumnHeaderStyle = null;
                    DefaultComboBoxStyle = null;

                    if (_resourceStateBrushStyles == null)
                    {
                        _resourceStateBrushStyles = new Dictionary<string, SolidColorBrush>();
                    }
                    else
                    {
                        _resourceStateBrushStyles.Clear();
                    }

                    if (_resourceIconBrushStyles == null)
                    {
                        _resourceIconBrushStyles = new Dictionary<string, SolidColorBrush>();
                    }
                    else
                    {
                        _resourceIconBrushStyles.Clear();
                    }

                    if (_themedResources == null)
                    {
                        _themedResources = new Collection<ResourceDictionary>();
                    }
                    else
                    {
                        _themedResources.Clear();
                    }
                }

                ResourceDictionary elementResources = frameworkElement.Resources;
                elementResources.MergedDictionaries.Clear();

                if (doInitialize)
                {
                    foreach (ResourceData resourceData in ResourcesConsts.ShellResources)
                    {
                        ResourceOrigin origin = resourceData.Origin;
                        string resource = resourceData.Resource;

                        var resourceDictionary = new ResourceDictionary
                        {
                            Source = new Uri(resource, UriKind.RelativeOrAbsolute)
                        };

                        if (origin == ResourceOrigin.App)
                        {
                            ProcessAppStyles(resource, resourceDictionary);
                        }
                        else if (origin == ResourceOrigin.Syncfusion)
                        {
                            ProcessSyncfusionStyles(resource, resourceDictionary);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }

                        _themedResources.Add(resourceDictionary);
                    }

                    ProcessAllResources(_themedResources);

                    _resourcesInitialized = true;
                    _resourcesInitializedForVisualStyles = visualStyles;
                }

                _themedResources.ForEach(dict => elementResources.MergedDictionaries.Add(dict));

                RaisePropertyChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ProcessAllResources(Collection<ResourceDictionary> resources)
        {
            T FindRecursive<T>(Collection<ResourceDictionary> collection, string keyToFind)
            {
                foreach (ResourceDictionary resourceDictionary in collection)
                {
                    List<string> keys = resourceDictionary.Keys.Cast<object>().Select(x => x.ToString()).ToList();
                    List<string> validKeys = keys.Where(x => x.Contains(keyToFind)).ToList();

                    if (validKeys.Count > 0)
                    {
                        return (T)resourceDictionary[validKeys.First()];
                    }

                    if (resourceDictionary.MergedDictionaries.Count > 0)
                    {
                        return FindRecursive<T>(resourceDictionary.MergedDictionaries, keyToFind);
                    }
                }

                return default;
            }

            DefaultListViewItemStyle = FindRecursive<Style>(resources, "DefaultListViewItemStyle");
            DefaultMenuItemStyle = FindRecursive<Style>(resources, "DefaultMenuItemStyle");
            DefaultListBoxItemStyle = FindRecursive<Style>(resources, "DefaultListBoxItemStyle");
            DefaultGridViewColumnHeaderStyle = FindRecursive<Style>(resources, "DefaultGridViewColumnHeaderStyle");
            DefaultComboBoxStyle = FindRecursive<Style>(resources, "DefaultComboBoxStyle");
        }

        private void ProcessAppStyles(string resource, ResourceDictionary resourceDictionary)
        {
            string resourceName = resource.ToLowerInvariant();

            if (resourceName.EndsWith("modelactionscontextmenu.xaml"))
            {
                //ModelActionsContextMenu = resourceDictionary["ModelActionsContextMenu"] as ContextMenu;
            }
            else if (resourceName.EndsWith("styles.xaml"))
            {
                AppVisualTheme[] appVisualTheme = { AppVisualTheme.Light, AppVisualTheme.Dark };
                ReadOnlyCollection<ResourceElementTranslationState> elementTranslationStates = Enum<ResourceElementTranslationState>.Values;
                ReadOnlyCollection<TreeElementType> treeElementTypes = Enum<TreeElementType>.Values;

                foreach (AppVisualTheme theme in appVisualTheme)
                {
                    foreach (ResourceElementTranslationState state in elementTranslationStates)
                    {
                        string key = $"ResourceState{state}{theme}";
                        var style = resourceDictionary[key] as SolidColorBrush;
                        if (!_resourceStateBrushStyles.ContainsKey(key))
                        {
                            _resourceStateBrushStyles.Add(key, style);
                        }
                    }

                    foreach (TreeElementType type in treeElementTypes)
                    {
                        string key = $"{type}IconColor{theme}";
                        var brush = resourceDictionary[key] as SolidColorBrush;

                        if (!_resourceIconBrushStyles.ContainsKey(key))
                        {
                            _resourceIconBrushStyles.Add(key, brush);
                        }
                    }
                }

                MultiSelectionTreeResourceStateBorderStyle = resourceDictionary["MultiSelectionTreeResourceStateBorderStyle"] as Style;
            }
        }

        public SolidColorBrush GetResourceStateBrush(ResourceElementTranslationState state)
        {
            string key = $"ResourceState{state}{Theme}";
            return _resourceStateBrushStyles.ContainsKey(key)
                ? _resourceStateBrushStyles[key]
                : null;
        }

        private SolidColorBrush GetTreeElementIconBrush(TreeElementType treeElementType)
        {
            string key = $"{treeElementType}IconColor{Theme}";
            return _resourceIconBrushStyles.ContainsKey(key)
                ? _resourceIconBrushStyles[key]
                : null;
        }

        private void ProcessSyncfusionStyles(string resource, ResourceDictionary resourceDictionary)
        {
            string resourceName = resource.ToLowerInvariant();

            if (resourceName.EndsWith("sfinput.xaml") && TextBoxStyle == null)
            {
                TextBoxStyle = resourceDictionary["TextBoxStyle"] as Style;
            }

            if (resourceName.EndsWith("commonbrushes.xaml"))
            {
                if (HeaderBackgroundBrush == null)
                {
                    HeaderBackgroundBrush = resourceDictionary["ActiveBackgroundBrush"] as Brush;
                }

                if (HeaderForegroundBrush == null)
                {
                    HeaderForegroundBrush = resourceDictionary["ActiveForegroundBrush"] as SolidColorBrush;
                }

                if (BackgroundBrush == null)
                {
                    BackgroundBrush = resourceDictionary["BackgroundBrush"] as Brush;
                }

                if (BorderBrush == null)
                {
                    BorderBrush = resourceDictionary["BorderBrush"] as Brush;
                }

                if (ContentForegroundBrush == null)
                {
                    ContentForegroundBrush = resourceDictionary["ContentForegroundBrush"] as Brush;
                }

                if (ContentBackgroundBrush == null)
                {
                    ContentBackgroundBrush = resourceDictionary["ContentBackgroundBrush"] as Brush;
                }

                if (SelectionInActiveBrush == null)
                {
                    SelectionInActiveBrush = resourceDictionary["SelectionInActiveBrush"] as Brush;
                }

                if (ActiveBackgroundBrush == null)
                {
                    ActiveBackgroundBrush = resourceDictionary["ActiveBackgroundBrush"] as Brush;
                }

                if (HoverBackgroundBrush == null)
                {
                    HoverBackgroundBrush = resourceDictionary["HoverBackgroundBrush"] as Brush;
                }
            }
        }
    }
}
