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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using LHQ.App.Code;
using LHQ.App.Extensions;
using Syncfusion.Windows.Controls.Input;
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.App.Components
{
    public sealed class SearchTextBox : SfTextBoxExt
    {
        public static DependencyProperty LabelTextProperty =
            DependencyProperty.Register(
                "LabelText",
                typeof(string),
                typeof(SearchTextBox));

        public static DependencyProperty LabelTextColorProperty =
            DependencyProperty.Register(
                "LabelTextColor",
                typeof(Brush),
                typeof(SearchTextBox));

        public static DependencyProperty SearchModeProperty =
            DependencyProperty.Register(
                "SearchMode",
                typeof(SearchMode),
                typeof(SearchTextBox),
                new PropertyMetadata(SearchMode.Instant));

        private static readonly DependencyPropertyKey _hasTextPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "HasText",
                typeof(bool),
                typeof(SearchTextBox),
                new PropertyMetadata());

        public static DependencyProperty HasTextProperty = _hasTextPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey _isMouseLeftButtonDownPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "IsMouseLeftButtonDown",
                typeof(bool),
                typeof(SearchTextBox),
                new PropertyMetadata());

        public static DependencyProperty IsMouseLeftButtonDownProperty = _isMouseLeftButtonDownPropertyKey.DependencyProperty;

        public static DependencyProperty SearchEventTimeDelayProperty =
            DependencyProperty.Register(
                "SearchEventTimeDelay",
                typeof(Duration),
                typeof(SearchTextBox),
                new FrameworkPropertyMetadata(
                    new Duration(new TimeSpan(0, 0, 0, 0, 500)),
                    OnSearchEventTimeDelayChanged));

        public static readonly RoutedEvent SearchEvent =
            EventManager.RegisterRoutedEvent(
                "Search",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(SearchTextBox));

        public static DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Uri), typeof(SearchTextBox),
                new PropertyMetadata(ResourceHelper.GetPackUri(@"/Components/SearchTextBox/Images/search.png")));

        public static DependencyProperty IconEmptyTextProperty =
            DependencyProperty.Register("IconEmptyText", typeof(Uri), typeof(SearchTextBox),
                new PropertyMetadata(ResourceHelper.GetPackUri(@"/Components/SearchTextBox/Images/clear.png")));

        private readonly DispatcherTimer _searchEventDelayTimer;

        private bool _tempBlockSearchEventDelayTimer;

        static SearchTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SearchTextBox),
                new FrameworkPropertyMetadata(typeof(SearchTextBox)));
        }

        public SearchTextBox()
        {
            this.AddStyleFromResources(ResourcesConsts.SearchTextBox);

            _searchEventDelayTimer = new DispatcherTimer
            {
                Interval = SearchEventTimeDelay.TimeSpan
            };
            _searchEventDelayTimer.Tick += OnSeachEventDelayTimerTick;
        }

        public Uri Icon
        {
            get => (Uri)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public Uri IconEmptyText
        {
            get => (Uri)GetValue(IconEmptyTextProperty);
            set => SetValue(IconEmptyTextProperty, value);
        }

        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        public Brush LabelTextColor
        {
            get => (Brush)GetValue(LabelTextColorProperty);
            set => SetValue(LabelTextColorProperty, value);
        }

        public SearchMode SearchMode
        {
            get => (SearchMode)GetValue(SearchModeProperty);
            set => SetValue(SearchModeProperty, value);
        }

        public bool HasText
        {
            get => (bool)GetValue(HasTextProperty);
            private set => SetValue(_hasTextPropertyKey, value);
        }

        public Duration SearchEventTimeDelay
        {
            get => (Duration)GetValue(SearchEventTimeDelayProperty);
            set => SetValue(SearchEventTimeDelayProperty, value);
        }

        public bool IsMouseLeftButtonDown
        {
            get => (bool)GetValue(IsMouseLeftButtonDownProperty);
            private set => SetValue(_isMouseLeftButtonDownPropertyKey, value);
        }

        // ReSharper disable once EventNeverSubscribedTo.Global
        public event EventHandler<SearchEventArgs> OnSearch;

        private void OnSeachEventDelayTimerTick(object o, EventArgs e)
        {
            _searchEventDelayTimer.Stop();
            RaiseSearchEvent();
        }

        private static void OnSearchEventTimeDelayChanged(
            DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is SearchTextBox stb)
            {
                stb._searchEventDelayTimer.Interval = ((Duration)e.NewValue).TimeSpan;
                stb._searchEventDelayTimer.Stop();
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            HasText = Text.Length != 0;

            if (!_tempBlockSearchEventDelayTimer && SearchMode == SearchMode.Instant)
            {
                _searchEventDelayTimer.Stop();
                _searchEventDelayTimer.Start();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_SearchIconBorder") is Border iconBorder)
            {
                iconBorder.MouseLeftButtonDown += IconBorder_MouseLeftButtonDown;
                iconBorder.MouseLeftButtonUp += IconBorder_MouseLeftButtonUp;
                iconBorder.MouseLeave += IconBorder_MouseLeave;
            }
        }

        private void IconBorder_MouseLeftButtonDown(object obj, MouseButtonEventArgs e)
        {
            IsMouseLeftButtonDown = true;
        }

        private void IconBorder_MouseLeftButtonUp(object obj, MouseButtonEventArgs e)
        {
            if (!IsMouseLeftButtonDown)
            {
                return;
            }

            if (HasText && SearchMode == SearchMode.Instant)
            {
                _tempBlockSearchEventDelayTimer = true;
                try
                {
                    Text = "";
                }
                finally
                {
                    _tempBlockSearchEventDelayTimer = false;
                }
                RaiseSearchEvent();
            }

            if (HasText && SearchMode == SearchMode.Delayed)
            {
                RaiseSearchEvent();
            }

            IsMouseLeftButtonDown = false;
        }

        private void IconBorder_MouseLeave(object obj, MouseEventArgs e)
        {
            IsMouseLeftButtonDown = false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape && SearchMode == SearchMode.Instant)
            {
                Text = "";
            }
            else if ((e.Key == Key.Return || e.Key == Key.Enter) &&
                SearchMode == SearchMode.Delayed)
            {
                RaiseSearchEvent();
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        private void RaiseSearchEvent()
        {
            BindingExpression bindingExpression = GetBindingExpression(TextProperty);
            bindingExpression?.UpdateSource();

            var args = new RoutedEventArgs(SearchEvent);
            RaiseEvent(args);

            RaiseOnSearch(Text);
        }

        // ReSharper disable once EventNeverSubscribedTo.Global
        public event RoutedEventHandler Search
        {
            add => AddHandler(SearchEvent, value);
            remove => RemoveHandler(SearchEvent, value);
        }

        private void RaiseOnSearch(string searchText)
        {
            OnSearch?.Invoke(this, new SearchEventArgs
            {
                Text = searchText
            });
        }
    }
}
