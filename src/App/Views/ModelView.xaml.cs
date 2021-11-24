// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using LHQ.App.Extensions;
using LHQ.App.Model.Messaging;
using LHQ.App.Services.Implementation.Messaging;
using LHQ.App.ViewModels;

namespace LHQ.App.Views
{
    public partial class ModelView : IMessageHandler
    {
        public ModelView()
        {
            InitializeComponent();
            this.RegisterMessengerHandler();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BindingExpression bindingExpression = ModelName.GetBindingExpression(TextBox.TextProperty);
            bindingExpression?.ValidateWithoutUpdate();
        }

        public string HandlerName { get; } = "ModelView";

        void IMessageHandler.HandleMessage(IMessengerData data)
        {
            if (data is MessengerDataFocusAutoSelectName focusAutoSelectName)
            {
                ViewModelBase viewModel = this.GetViewModel<ShellViewModelBase>();
                if (viewModel == focusAutoSelectName.Data)
                {
                    ModelName.Focus();
                    ModelName.SelectAll();
                }
            }
        }
    }
}
