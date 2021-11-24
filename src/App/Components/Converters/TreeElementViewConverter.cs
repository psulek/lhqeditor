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
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels;
using LHQ.App.Views;

namespace LHQ.App.Components.Converters
{
    public class TreeElementViewConverter : IValueConverter
    {
        private IUIService _uiService;
        private IShellViewContext _shellViewContext;

        private ShellCacheItem InternalGetCacheItem()
        {
            return _shellViewContext?.ShellViewCache?.GetOrCreate(ShellViewCacheNames.TreeElementViewConverter,
                ShellViewCacheKeys.Default, () => new ShellCacheItem());
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UserControl result = null;
            var viewModel = value as ShellViewModelBase;
            ShellCacheItem cacheItem;

            void UnsubscribeLast()
            {
                if (cacheItem?.LastViewModel != null)
                {
                    _uiService.UnregisterSubscriber(cacheItem.LastViewModel);
                    cacheItem.LastViewModel = null;
                }
            }

            if (viewModel != null)
            {
                if (_shellViewContext == null)
                {
                    _shellViewContext = viewModel.ShellViewContext;
                }

                if (_uiService == null)
                {
                    _uiService = _shellViewContext.AppContext.UIService;
                }

                cacheItem = InternalGetCacheItem();

                if (viewModel is MultiSelectionViewModel)
                {
                    cacheItem.EnsureMultiSelectionViewCreated();
                    result = cacheItem.MultiSelectionView;
                }
                else
                {
                    var elementViewModel = (ITreeElementViewModel)viewModel;
                    TreeElementType elementType = elementViewModel.ElementType;

                    Dictionary<TreeElementType, UserControl> cachedViews = cacheItem.CachedViews;

                    if (cachedViews.ContainsKey(elementType))
                    {
                        result = cachedViews[elementType];
                    }
                    else
                    {
                        switch (elementType)
                        {
                            case TreeElementType.Model:
                            {
                                result = new ModelView();
                                break;
                            }
                            case TreeElementType.Category:
                            {
                                result = new CategoryView();
                                break;
                            }
                            case TreeElementType.Resource:
                            {
                                result = new ResourceView();
                                break;
                            }
                            default:
                            {
                                throw new ArgumentOutOfRangeException();
                            }
                        }

                        cachedViews.Add(elementType, result);
                    }
                }

                UnsubscribeLast();
                _uiService.RegisterSubscriber(viewModel);
                cacheItem.LastViewModel = viewModel;
            }
            else
            {
                cacheItem = InternalGetCacheItem();
                UnsubscribeLast();
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private class ShellCacheItem
        {
            internal ShellCacheItem()
            {
                CachedViews = new Dictionary<TreeElementType, UserControl>();
            }

            internal MultiSelectionView MultiSelectionView { get; private set; }

            internal Dictionary<TreeElementType, UserControl> CachedViews { get; }

            internal ViewModelBase LastViewModel { get; set; }

            internal void EnsureMultiSelectionViewCreated()
            {
                if (MultiSelectionView == null)
                {
                    MultiSelectionView = new MultiSelectionView();
                }
            }
        }
    }
}
