// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */
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
