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
using Syncfusion.SfSkinManager;

namespace LHQ.App.Code
{
    public static class ResourcesConsts
    {
        private static readonly string[] _appResources =
        {
            "/Resources/Converters.xaml",
            "/Resources/Styles.xaml",
            "/Resources/ModelActionsContextMenu.xaml"
        };

        private static readonly string[] _syncfusionResources =
        {
            "/commonbrushes/commonbrushes.xaml",
            "/Tools/MS Control/MS control.xaml",
            "/Tools/MenuAdv/MenuAdv.xaml",
            "/Tools/dropdownbuttonadv/dropdownbuttonadv.xaml",
            "/Tools/DockingManager/DockingManager.xaml",
            "/Tools/TabControlExt/TabControlExt.xaml",
            "/Tools/Editors/Editors.xaml",
            "/Tools/SfBusyIndicator/SfBusyIndicator.xaml",
            "/tools/sfinput/sfinput.xaml"
        };

        public static string DropDownButtonStyle => ResourceHelper.BuildEmbedResourcePath("/Components/DropDownButtonStyle.xaml");

        public static string VirtualTreeListViewControl =>
            ResourceHelper.BuildEmbedResourcePath("/Components/VirtualTreeListViewControl/Generic.xaml");

        public static string TokenizedTextBox => ResourceHelper.BuildEmbedResourcePath("/Components/TokenizedTextBox/Themes/Generic.xaml");

        public static string SplitButtonStyle => ResourceHelper.BuildEmbedResourcePath("/Components/SplitButtonStyle.xaml");

        public static string SearchTextBox => ResourceHelper.BuildEmbedResourcePath("/Components/SearchTextBox/Themes/Generic.xaml");

        public static IEnumerable<ResourceData> ShellResources => GetShellResources();

        private static IEnumerable<ResourceData> GetShellResources()
        {
            VisualStyles visualStyles = VisualManager.Instance.VisualStyle;
            string syncfusionAssemblyName = $"/Syncfusion.Themes.{visualStyles}.WPF";

            foreach (string resource in _appResources)
            {
                string resourcePath = ResourceHelper.BuildEmbedResourcePath(resource);
                yield return new ResourceData(ResourceOrigin.App, resourcePath);
            }

            foreach (string resource in _syncfusionResources)
            {
                string resourcePath = ResourceHelper.BuildEmbedResourcePath(resource, syncfusionAssemblyName);
                yield return new ResourceData(ResourceOrigin.Syncfusion, resourcePath);
            }
        }
    }

    public sealed class ResourceData
    {
        public ResourceData(ResourceOrigin origin, string resource)
        {
            Resource = resource;
            Origin = origin;
        }

        public string Resource { get; }

        public ResourceOrigin Origin { get; }
    }

    public enum ResourceOrigin
    {
        App,
        Syncfusion
    }
}
