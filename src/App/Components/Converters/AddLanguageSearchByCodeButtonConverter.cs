﻿#region License
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
using System.Globalization;
using System.Windows.Data;
using LHQ.App.Code;
using LHQ.App.Localization;

namespace LHQ.App.Components.Converters
{
    public class AddLanguageSearchByCodeButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = null;
            var param = parameter as string;
            bool icon = param == "icon";
            bool tooltip = param == "tooltip";

            if (value != null)
            {
                var searchByCountryCode = (bool)value;

                if (icon)
                {
                    result = VisualManager.Instance.HeaderBackgroundBrush;
                    if (searchByCountryCode)
                    {
                        result = VisualManager.Instance.ContentForegroundBrush;
                    }
                }
                else if (tooltip)
                {
                    result = searchByCountryCode
                        ? Strings.Views.LanguageSelectorView.FilterTooltipActive
                        : Strings.Views.LanguageSelectorView.FilterTooltipNotActive;
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
