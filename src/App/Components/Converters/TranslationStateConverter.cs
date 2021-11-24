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
using System.Windows.Data;
using LHQ.App.Code;
using LHQ.App.Extensions;
using LHQ.Data;
using LHQ.Utils;
// ReSharper disable UnusedMember.Global

namespace LHQ.App.Components.Converters
{
    public class TranslationStateConverter : IValueConverter
    {
        private static readonly Dictionary<ResourceElementTranslationState, string> _stateImages =
            new Dictionary<ResourceElementTranslationState, string>
            {
                [ResourceElementTranslationState.New] = "state_new.png",
                [ResourceElementTranslationState.NeedsReview] = "state_needsreview.png",
                [ResourceElementTranslationState.Edited] = "state_edited.png",
                [ResourceElementTranslationState.Final] = "state_final.png"
            };

        public TranslationStateConverter()
        { }

        public TranslationStateConverter(TranslationStateConverterType type)
        {
            Type = type;
        }

        public TranslationStateConverterType Type { get; set; }

        public string GetText(ResourceElementTranslationState state)
        {
            return (this as IValueConverter).Convert(state, WellKnowTypes.System.String, TranslationStateConverterType.Text,
                CultureInfo.InvariantCulture) as string;
        }

        public string GetImageUri(ResourceElementTranslationState state)
        {
            return (this as IValueConverter).Convert(state, WellKnowTypes.System.String, TranslationStateConverterType.Image,
                CultureInfo.InvariantCulture) as string;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TranslationStateConverterType type = Type;

            if (parameter is TranslationStateConverterType converterType)
            {
                type = converterType;
            }

            if (value != null)
            {
                var translationState = (ResourceElementTranslationState)value;

                if (type == TranslationStateConverterType.Image)
                {
                    string image = _stateImages[translationState];
                    return ResourceHelper.GetImageComponentUri("/Images/" + image);
                }

                return translationState.ToDisplayName();
            }

            if (type == TranslationStateConverterType.Image)
            {
                return ResourceHelper.GetImageComponentUri("/Images/" + _stateImages[ResourceElementTranslationState.New]);
            }

            return string.Empty;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum TranslationStateConverterType
    {
        Image,
        Text
    }
}
