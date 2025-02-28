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
using System.Windows.Controls;
using System.Windows.Data;
using LHQ.App.Extensions;
using LHQ.App.Model.Messaging;
using LHQ.App.Services.Implementation.Messaging;
using LHQ.App.ViewModels;

namespace LHQ.App.Views
{
    /// <summary>
    ///     Interaction logic for CategoryView.xaml
    /// </summary>
    public partial class CategoryView : IMessageHandler
    {
        public CategoryView()
        {
            InitializeComponent();

            this.RegisterMessengerHandler();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BindingExpression bindingExpression = CategoryName.GetBindingExpression(TextBox.TextProperty);
            bindingExpression?.ValidateWithoutUpdate();
        }

        public string HandlerName { get; } = "CategoryView";

        void IMessageHandler.HandleMessage(IMessengerData data)
        {
            if (data is MessengerDataFocusAutoSelectName focusAutoSelectName)
            {
                ViewModelBase viewModel = this.GetViewModel<ShellViewModelBase>();
                if (viewModel == focusAutoSelectName.Data)
                {
                    CategoryName.Focus();
                    CategoryName.SelectAll();
                }
            }
        }
    }
}
