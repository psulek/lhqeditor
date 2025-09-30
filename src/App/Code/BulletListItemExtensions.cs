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
using System.Windows;
using LHQ.Core.UI;
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.App.Code
{
    public static class BulletListItemExtensions
    {
        public static BulletListItem ToBulletListItem(this BulletListTextItem source, FontWeight boldWeight)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            BulletListItemType type;

            switch (source.Type)
            {
                case BulletListTextItemType.Text:
                {
                    type = BulletListItemType.Text;
                    break;
                }
                case BulletListTextItemType.Separator:
                {
                    type = BulletListItemType.Separator;
                    break;
                }
                case BulletListTextItemType.Hyperlink:
                {
                    type = BulletListItemType.Hyperlink;
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            return new BulletListItem
            {
                Type = type,
                CheckVisible = source.CheckVisible,
                BulletVisible = source.BulletVisible,
                Text = source.Text,
                Url = source.Url,
                FontWeight = source.TextBold ? boldWeight : FontWeights.Normal
            };
        }
    }
}