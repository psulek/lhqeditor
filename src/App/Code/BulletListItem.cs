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

using System.Windows;
using System.Windows.Input;

namespace LHQ.App.Code
{
    public enum BulletListItemType
    {
        Text,
        Separator,
        Hyperlink
    }

    public sealed class BulletListItem
    {
        public string Text { get; set; }

        public BulletListItemType Type { get; set; }

        public string Url { get; set; }

        public bool BulletVisible { get; set; }

        public bool CheckVisible { get; set; }

        public FontWeight FontWeight { get; set; }

        public bool IsText => Type == BulletListItemType.Text;

        public bool IsSeparator => Type == BulletListItemType.Separator;

        public bool IsHyperlink => Type == BulletListItemType.Hyperlink;

        public bool HasBulletOrCheck => BulletVisible || CheckVisible;

        public ICommand HyperlinkCommand { get; set; }

        public BulletListItem(BulletListItemType type = BulletListItemType.Text)
        {
            Type = type;
            CheckVisible = false;
            FontWeight = FontWeights.Normal;
        }

        public static BulletListItem Separator()
        {
            return new BulletListItem(BulletListItemType.Separator);
        }

        public static BulletListItem Hyperlink(string text, string url)
        {
            return Hyperlink(text, url, FontWeights.Normal);
        }

        public static BulletListItem Hyperlink(string text, ICommand command)
        {
            return Hyperlink(text, FontWeights.Normal, command);
        }

        public static BulletListItem Hyperlink(string text, FontWeight fontWeight, ICommand command)
        {
            return Hyperlink(text, null, fontWeight, command);
        }

        public static BulletListItem Hyperlink(string text, string url, FontWeight fontWeight, ICommand command = null)
        {
            return new BulletListItem(BulletListItemType.Hyperlink)
            {
                Text = text,
                Url = url,
                BulletVisible = false,
                FontWeight = fontWeight,
                CheckVisible = false,
                HyperlinkCommand = command
            };
        }

        public static BulletListItem Item(string text)
        {
            return Item(text, FontWeights.Normal);
        }

        public static BulletListItem Item(string text, FontWeight fontWeight)
        {
            return new BulletListItem
            {
                Text = text,
                BulletVisible = false,
                FontWeight = fontWeight,
                CheckVisible = false
            };
        }

        public static BulletListItem BulletItem(string text)
        {
            return BulletItem(text, FontWeights.Normal);
        }

        public static BulletListItem BulletItem(string text, FontWeight fontWeight)
        {
            return new BulletListItem
            {
                Text = text,
                BulletVisible = true,
                FontWeight = fontWeight,
                CheckVisible = false
            };
        }

        public static BulletListItem CheckItem(string text)
        {
            return CheckItem(text, FontWeights.Normal);
        }

        public static BulletListItem CheckItem(string text, FontWeight fontWeight)
        {
            return new BulletListItem
            {
                Text = text,
                BulletVisible = false,
                FontWeight = fontWeight,
                CheckVisible = true
            };
        }
    }
}
