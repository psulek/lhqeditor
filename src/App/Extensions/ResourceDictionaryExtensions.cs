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
using System.Diagnostics;
using System.Text;
using System.Windows;
using LHQ.Utils.Extensions;

namespace LHQ.App.Extensions
{
    public static class ResourceDictionaryExtensions
    {
        // ReSharper disable once UnusedMember.Global
        [Conditional("DEBUG")]
        public static void DumpToDebug(this ResourceDictionary dictionary)
        {
            var sb = new StringBuilder(new string(' ', 80) + "\n" +
                $"Merged dictionaries ({dictionary.MergedDictionaries.Count}):\n");

            for (var i = 0; i < dictionary.MergedDictionaries.Count; i++)
            {
                ResourceDictionary dict = dictionary.MergedDictionaries[i];
                sb.AppendLine($"#{i + 1} " + dict.Source);
            }
            sb.AppendLine(new string(' ', 80));

            DebugUtils.Log(sb.ToString());
        }

        public static void AddStyleFromResources(this FrameworkElement frameworkElement, string resourcePath)
        {
            AddStyleFromResources(frameworkElement, new[] { resourcePath });
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static void AddStyleFromResources(this FrameworkElement frameworkElement, IEnumerable<string> resourcesPaths)
        {
            Collection<ResourceDictionary> resourceDictionaries = frameworkElement.Resources.MergedDictionaries;
            foreach (string resource in resourcesPaths)
            {
                resourceDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri(resource, UriKind.RelativeOrAbsolute)
                });
            }
        }
    }
}
