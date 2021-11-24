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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Extensions;
using LHQ.Utils.Extensions;
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.App.Components.TokenizedTextBox.Implementation
{
    public sealed class TokenizedTextBox : ItemsControl
    {
        public static DependencyProperty IsReadonlyProperty =
            DependencyProperty.Register("IsReadonly", typeof(bool), typeof(TokenizedTextBox),
                new UIPropertyMetadata(false));

        public static readonly DependencyProperty SearchMemberPathProperty =
            DependencyProperty.Register("SearchMemberPath", typeof(string), typeof(TokenizedTextBox),
                new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty TokenDelimiterProperty =
            DependencyProperty.Register("TokenDelimiter", typeof(string), typeof(TokenizedTextBox),
                new UIPropertyMetadata(";"));

        public static readonly DependencyProperty DelimiterKeyProperty =
            DependencyProperty.Register("DelimiterKey", typeof(Key), typeof(TokenizedTextBox),
                new UIPropertyMetadata(Key.Space));

        public static readonly DependencyProperty TokenTemplateProperty =
            DependencyProperty.Register("TokenTemplate", typeof(DataTemplate), typeof(TokenizedTextBox),
                new UIPropertyMetadata(null));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string),
            typeof(TokenizedTextBox), new UIPropertyMetadata(null, OnTextChanged));

        public static readonly DependencyProperty ValueMemberPathProperty =
            DependencyProperty.Register("ValueMemberPath", typeof(string), typeof(TokenizedTextBox),
                new UIPropertyMetadata(string.Empty));

        private RichTextBox _innerTextBox;

        private bool _surpressTextChanged;

        static TokenizedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TokenizedTextBox),
                new FrameworkPropertyMetadata(typeof(TokenizedTextBox)));
        }

        public TokenizedTextBox()
        {
            this.AddStyleFromResources(ResourcesConsts.TokenizedTextBox);
            CommandBindings.Add(new CommandBinding(TokenizedTextBoxCommands.Delete, DeleteToken));
        }

        public bool IsReadonly
        {
            get => (bool)GetValue(IsReadonlyProperty);
            set => SetValue(IsReadonlyProperty, value);
        }

        public string SearchMemberPath
        {
            get => (string)GetValue(SearchMemberPathProperty);
            set => SetValue(SearchMemberPathProperty, value);
        }

        public string TokenDelimiter
        {
            get => (string)GetValue(TokenDelimiterProperty);
            set => SetValue(TokenDelimiterProperty, value);
        }

        public Key DelimiterKey
        {
            get => (Key)GetValue(DelimiterKeyProperty);
            set => SetValue(DelimiterKeyProperty, value);
        }

        public DataTemplate TokenTemplate
        {
            get => (DataTemplate)GetValue(TokenTemplateProperty);
            set => SetValue(TokenTemplateProperty, value);
        }

        public string ValueMemberPath
        {
            get => (string)GetValue(ValueMemberPathProperty);
            set => SetValue(ValueMemberPathProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is TokenizedTextBox tokenizedTextBox)
            {
                tokenizedTextBox.OnTextChanged((string)e.OldValue, (string)e.NewValue);
            }
        }

        private void OnTextChanged(string oldValue, string newValue)
        {
            if (_innerTextBox == null || _surpressTextChanged)
            {
                return;
            }

            if (oldValue != newValue)
            {
                InitializeTokensFromText();
            }
        }

        [SuppressMessage("ReSharper", "PossibleUnintendedReferenceComparison")]
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus != null && e.NewFocus == this && e.OldFocus != _innerTextBox)
            {
                e.Handled = true;
                _innerTextBox.Focus();
            }
            else if (e.OldFocus == _innerTextBox && e.NewFocus == this)
            {
                e.Handled = true;
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_innerTextBox != null)
            {
                _innerTextBox.TextChanged -= TextBox_TextChanged;
                _innerTextBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
                _innerTextBox.LostKeyboardFocus -= TextBox_LostKeyboardFocus;
            }

            _innerTextBox = GetTemplateChild("PART_ContentHost") as RichTextBox;

            InitializeTokensFromText();

            if (_innerTextBox != null)
            {
                _innerTextBox.IsReadOnly = IsReadonly;
                _innerTextBox.TextChanged += TextBox_TextChanged;
                _innerTextBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                _innerTextBox.LostKeyboardFocus += TextBox_LostKeyboardFocus;
            }
        }

        private void TextBox_LostKeyboardFocus(object sender,
            KeyboardFocusChangedEventArgs e)
        {
            TryToAppendToken(false);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        { }

        private void TryToAppendToken(bool findTokenSeparator)
        {
            string text = _innerTextBox.CaretPosition.GetTextInRun(LogicalDirection.Backward).Trim();
            if (!text.IsNullOrEmpty())
            {
                Token token = findTokenSeparator ? ResolveToken(text) : ResolveTokenBySearchMemberPath(text);
                if (token != null)
                {
                    ReplaceTextWithToken(text, token);
                }
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            InlineUIContainer container = null;

            if (e.Key == Key.Back && _innerTextBox != null)
            {
                container =
                    _innerTextBox.CaretPosition.GetAdjacentElement(LogicalDirection.Backward) as InlineUIContainer;
            }
            else if (e.Key == Key.Delete && _innerTextBox != null)
            {
                //if the selected text is a blank space, I will assume that a token item is selected.
                //if a token item is selected, we need to move the caret position to the left of the element so we can grab the InlineUIContainer
                if (_innerTextBox.Selection.Text == " ")
                {
                    TextPointer moveTo =
                        _innerTextBox.CaretPosition.GetNextInsertionPosition(LogicalDirection.Backward);
                    _innerTextBox.CaretPosition = moveTo;
                }

                //the cursor is to the left of a token item
                container =
                    _innerTextBox.CaretPosition?.GetAdjacentElement(LogicalDirection.Forward) as InlineUIContainer;
            }
            //else if (e.Key == Key.Return)
            else if (e.Key == DelimiterKey)
            {
                TryToAppendToken(false);
            }

            //if the container is not null that means we have something to delete
            if (container?.Child is TokenItem token)
            {
                SetTextInternal(Text.Replace(token.TokenKey, ""));
            }
        }

        private void InitializeTokensFromText()
        {
            if (_innerTextBox.CaretPosition.Paragraph != null)
            {
                _innerTextBox.CaretPosition.Paragraph.Inlines.Clear();

                if (!string.IsNullOrEmpty(Text))
                {
                    string[] tokenKeys = Text.Split(new[] { TokenDelimiter },
                        StringSplitOptions.RemoveEmptyEntries);
                    foreach (string tokenKey in tokenKeys)
                    {
                        Paragraph para = _innerTextBox.CaretPosition.Paragraph;
                        var token = new Token(TokenDelimiter)
                        {
                            TokenKey = tokenKey,
                            Item = ResolveItemByTokenKey(tokenKey)
                        };
                        para.Inlines.Add(CreateTokenContainer(token));
                    }
                }
            }
        }

        private Token ResolveToken(string text)
        {
            if (text.EndsWith(TokenDelimiter))
            {
                return ResolveTokenBySearchMemberPath(text.Substring(0, text.Length - 1).Trim());
            }

            return null;
        }

        private Token ResolveTokenBySearchMemberPath(string searchText)
        {
            //create a new token and default the settings to the search text
            var token = new Token(TokenDelimiter)
            {
                TokenKey = searchText,
                Item = searchText
            };

            if (ItemsSource != null)
            {
                foreach (object item in ItemsSource)
                {
                    PropertyInfo searchProperty = item.GetType().GetProperty(SearchMemberPath);
                    if (searchProperty != null)
                    {
                        object searchValue = searchProperty.GetValue(item, null);
                        if (searchText.Equals(searchValue.ToString(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            PropertyInfo valueProperty = item.GetType().GetProperty(ValueMemberPath);
                            if (valueProperty != null)
                            {
                                token.TokenKey = valueProperty.GetValue(item, null).ToString();
                            }

                            token.Item = item;
                            break;
                        }
                    }
                }
            }

            return token;
        }

        private object ResolveItemByTokenKey(string tokenKey)
        {
            if (ItemsSource != null)
            {
                foreach (object item in ItemsSource)
                {
                    PropertyInfo property = item.GetType().GetProperty(ValueMemberPath);
                    if (property != null)
                    {
                        object value = property.GetValue(item, null);
                        if (tokenKey.Equals(value.ToString(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            return item;
                        }
                    }
                }
            }

            return tokenKey;
        }

        private void ReplaceTextWithToken(string inputText, Token token)
        {
            Paragraph paragraph = _innerTextBox.CaretPosition.Paragraph;

            // Found a Run that matched the inputText
            if (paragraph?.Inlines.FirstOrDefault(inline => inline is Run run && run.Text.EndsWith(inputText)) is Run matchedRun)
            {
                InlineUIContainer tokenContainer = CreateTokenContainer(token);
                paragraph.Inlines.InsertBefore(matchedRun, tokenContainer);

                // Remove only if the Text in the Run is the same as inputText, else split up
                if (matchedRun.Text == inputText)
                {
                    paragraph.Inlines.Remove(matchedRun);
                }
                else // Split up
                {
                    int index = (string.IsNullOrEmpty(matchedRun.Text) ? 0 : matchedRun.Text.IndexOf(inputText, StringComparison.Ordinal)) +
                        inputText.Length;
                    var tailEnd = new Run(matchedRun.Text.Substring(index));
                    paragraph.Inlines.InsertAfter(matchedRun, tailEnd);
                    paragraph.Inlines.Remove(matchedRun);
                }

                //now append the Text with the token key
                SetTextInternal(Text + token.TokenKey);
            }
        }

        private InlineUIContainer CreateTokenContainer(Token token)
        {
            return new InlineUIContainer(CreateTokenItem(token)) { BaselineAlignment = BaselineAlignment.Center };
        }

        private TokenItem CreateTokenItem(Token token)
        {
            object item = token.Item;

            var tokenItem = new TokenItem
            {
                TokenKey = token.TokenKey,
                Content = item,
                ContentTemplate = TokenTemplate,
                Focusable = false,
                AllowDelete = false
            };

            if (TokenTemplate == null)
            {
                if (!string.IsNullOrEmpty(DisplayMemberPath))
                {
                    PropertyInfo property = item.GetType().GetProperty(DisplayMemberPath);
                    if (property != null)
                    {
                        object value = property.GetValue(item, null);
                        if (value != null)
                        {
                            tokenItem.Content = value;
                        }
                    }
                }
            }

            return tokenItem;
        }

        private void DeleteToken(object sender, ExecutedRoutedEventArgs e)
        {
            Paragraph para = _innerTextBox.CaretPosition.Paragraph;

            Inline inlineToRemove = para?.Inlines
                .FirstOrDefault(inline => inline is InlineUIContainer container &&
                    ((TokenItem)container.Child).TokenKey.Equals(e.Parameter));

            if (inlineToRemove != null)
            {
                para.Inlines.Remove(inlineToRemove);
            }

            //update Text to remove delimited value
            SetTextInternal(Text.Replace(e.Parameter.ToString(), ""));
        }

        private void SetTextInternal(string text)
        {
            _surpressTextChanged = true;
            Text = text;
            _surpressTextChanged = false;
        }
    }
}
