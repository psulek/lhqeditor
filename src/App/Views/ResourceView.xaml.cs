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

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using LHQ.App.Extensions;
using LHQ.App.Model.Messaging;
using LHQ.App.Services.Implementation.Messaging;
using LHQ.App.ViewModels;
using LHQ.App.ViewModels.Elements;

namespace LHQ.App.Views
{
    public partial class ResourceView : IMessageHandler
    {
        public ResourceView()
        {
            InitializeComponent();
            this.RegisterMessengerHandler();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            BindingExpression bindingExpression = ResourceName.GetBindingExpression(TextBox.TextProperty);
            bindingExpression?.ValidateWithoutUpdate();
        }

        public string HandlerName { get; } = "ResourceView";

        void IMessageHandler.HandleMessage(IMessengerData data)
        {
            if (data is MessengerDataFocusAutoSelectName focusAutoSelectName)
            {
                ViewModelBase viewModel = this.GetViewModel<ShellViewModelBase>();
                if (viewModel == focusAutoSelectName.Data)
                {
                    ResourceName.Focus();
                    ResourceName.SelectAll();
                }
            }
            else if (data == MessengerDefinitions.FocusPrimaryResourceValue)
            {
                if (this.GetViewModel<ShellViewModelBase>() is ResourceViewModel resourceViewModel)
                {
                    ResourceValueViewModel resourceValueViewModel = resourceViewModel.Values.Single(x => x.IsReferenceLanguage);
                    resourceValueViewModel.IsAutoFocus = true;
                }
            }
        }
    }
}
