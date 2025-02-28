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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LHQ.App.Code;
using LHQ.App.Extensions;
using LHQ.Data;
using LHQ.Utils.Utilities;

// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.App.Components
{
    public class TranslationStateTemplateSelector : DataTemplateSelector
    {
        public static TranslationStateTemplateSelector Instance = new TranslationStateTemplateSelector();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate datatemplate = null;

            if (item != null)
            {
                var contentPresenter = container as ContentPresenter;
                var translationStateSelector = contentPresenter.FindParent<TranslationStateSelector>();
                var state = (ResourceElementTranslationState)item;
                string key = $"State{state}DataTemplate";

                datatemplate = translationStateSelector.Resources[key] as DataTemplate;
            }

            return datatemplate;
        }
    }

    /// <summary>
    ///     Interaction logic for TranslationStateSelector.xaml
    /// </summary>
    public partial class TranslationStateSelector: INotifyPropertyChanged
    {
        public static readonly DependencyProperty SelectedStateProperty =
            DependencyProperty.Register(
                "SelectedState",
                typeof(ResourceElementTranslationState),
                typeof(TranslationStateSelector), 
                new PropertyMetadata(ResourceElementTranslationState.New, SelectedStateChanged));

        public TranslationStateSelector()
        {
            InitializeComponent();

            StateSelectedCommand = new DelegateCommand(StateSelectedExecute);
            UpdateStates();
        }

        public bool SelectedStateIsNew => SelectedState == ResourceElementTranslationState.New;

        public bool SelectedStateIsEdited => SelectedState == ResourceElementTranslationState.Edited;

        public bool SelectedStateIsNeedsReview => SelectedState == ResourceElementTranslationState.NeedsReview;

        public bool SelectedStateIsFinal => SelectedState == ResourceElementTranslationState.Final;

        public Brush StateIsNewBackground { get; private set; }
        public Brush StateIsEditedBackground { get; private set; }
        public Brush StateIsNeedsReviewBackground { get; private set; }
        public Brush StateIsFinalBackground { get; private set; }

        public ICommand StateSelectedCommand { get; }

        public ResourceElementTranslationState SelectedState
        {
            get => (ResourceElementTranslationState)GetValue(SelectedStateProperty);
            set => SetValue(SelectedStateProperty, value);
        }

        private static void SelectedStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TranslationStateSelector control)
            {
                control.UpdateStates();
            }
        }

        private void UpdateStates()
        {
            StateIsNewBackground = SelectedStateIsNew ? VisualManager.Instance.BackgroundBrush : null;
            StateIsEditedBackground = SelectedStateIsEdited ? VisualManager.Instance.BackgroundBrush : null;
            StateIsNeedsReviewBackground = SelectedStateIsNeedsReview ? VisualManager.Instance.BackgroundBrush : null;
            StateIsFinalBackground = SelectedStateIsFinal ? VisualManager.Instance.BackgroundBrush : null;

            RaisePropertyChanged(null);
        }

        private void StateSelectedExecute(object parameter)
        {
            SelectedState = Enum<ResourceElementTranslationState>.Parse(parameter as string);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
