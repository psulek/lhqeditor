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
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using LHQ.App.Model;
using LHQ.App.Model.Messaging;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels;

// ReSharper disable UnusedMember.Global

namespace LHQ.App.Extensions
{
    public static class FrameworkElementExtensions
    {
        private static readonly XmlWriterSettings _xmlWriterSettings = new XmlWriterSettings
        {
            Indent = true,
            ConformanceLevel = ConformanceLevel.Fragment,
            OmitXmlDeclaration = true,
            NamespaceHandling = NamespaceHandling.OmitDuplicates
        };

        private static bool? _inDesignMode;

        public static bool IsInDesignMode => GetIsInDesignMode();

        private static bool GetIsInDesignMode()
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

        public static T CloneWpfObject<T>(T obj)
        {
            var sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb, _xmlWriterSettings))
            {
                var mgr = new XamlDesignerSerializationManager(writer) { XamlWriterMode = XamlWriterMode.Expression };
                XamlWriter.Save(obj, mgr);
                return (T)XamlReader.Parse(sb.ToString());
            }
        }

        public static object TryFindResource(this FrameworkElement element, object resourceKey)
        {
            while (element != null)
            {
                object value = element.Resources.TryGetValue(resourceKey);
                if (value != null)
                {
                    return value;
                }
                element = element.Parent as FrameworkElement;
            }

            return Application.Current.Resources.TryGetValue(resourceKey);
        }

        private static object TryGetValue(this ResourceDictionary dictionary, object key)
        {
            object result = null;
            // ReSharper disable once SuspiciousTypeConversion.Global
            var dict = dictionary as IDictionary<object, object>;
            dict?.TryGetValue(key, out result);
            return result;
        }

        public static TViewModel GetViewModel<TViewModel>(this UserControl userControl) where TViewModel: ShellViewModelBase
        {
            return userControl.DataContext as TViewModel;
        }

        public static IMessenger GetMessenger<TViewModel>(this UserControl userControl) where TViewModel : ShellViewModelBase
        {
            return userControl.GetViewModel<TViewModel>().ShellViewModel.Messenger;
        }

        public static void RegisterMessengerHandler<T>(this T userControl) where T : FrameworkElement, IMessageHandler
        {
            userControl.DataContextChanged += (sender, e) =>
                {
                    IMessenger GetValidMessenger(object source)
                    {
                        var viewModel = (ObservableModelObject)source;

                        IMessenger messenger = null;
                        if (viewModel is ShellViewModelBase shellViewModel)
                        {
                            messenger = shellViewModel.ShellViewContext.Messenger;
                        }

                        if (messenger == null && viewModel is IHasShellViewContext hasShellViewContext)
                        {
                            messenger = hasShellViewContext.ShellViewContext.Messenger;
                        }

                        return messenger;
                    }

                    if (e.OldValue == null && e.NewValue != null)
                    {
                        var messenger = GetValidMessenger(e.NewValue);
                        messenger?.Register(userControl);
                    }
                    else if (e.NewValue == null && e.OldValue != null)
                    {
                        var messenger = GetValidMessenger(e.OldValue);
                        messenger?.Unregister(userControl);
                    }
                };
        }

        public static T GetVisualChild<T>(this DependencyObject referenceVisual)
            where T : Visual
        {
            Visual child = null;
            int childCount = VisualTreeHelper.GetChildrenCount(referenceVisual);

            for (var i = 0; i < childCount; i++)
            {
                child = VisualTreeHelper.GetChild(referenceVisual, i) as Visual;

                if (child is T)
                {
                    break;
                }

                if (child != null)
                {
                    child = GetVisualChild<T>(child);
                    break;
                }
            }

            return child as T;
        }
    }
}
