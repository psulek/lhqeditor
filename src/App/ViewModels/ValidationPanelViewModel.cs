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

using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Model;
using LHQ.App.Services.Implementation.Messaging;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Elements;
// ReSharper disable UnusedMember.Global

namespace LHQ.App.ViewModels
{
    public class ValidationPanelViewModel : ObservableModelObject
    {
        private ValidationError _selectedError;

        public ValidationPanelViewModel(IShellViewContext shellViewContext) : base(shellViewContext.AppContext, true)
        {
            ShellViewContext = shellViewContext;
            PreviewMouseDoubleClickCommand = new DelegateCommand(PreviewMouseDoubleClickExecute, PreviewMouseDoubleClickCanExecute);
            ErrorsKeyDownCommand = new DelegateCommand<KeyEventArgs>(ErrorsKeyDownExecute);
        }

        private IShellViewContext ShellViewContext { get; }

        public IValidatorContext ValidatorContext => ShellViewContext.ValidatorContext;

        public IMessenger Messenger => ShellViewContext.Messenger;

        public ITreeViewService TreeViewService => ShellViewContext.TreeViewService;

        public ICommand PreviewMouseDoubleClickCommand { get; }

        public DelegateCommand<KeyEventArgs> ErrorsKeyDownCommand { get; }

        public ValidationError SelectedError
        {
            get => _selectedError;
            set => SetProperty(ref _selectedError, value);
        }

        private void ErrorsKeyDownExecute(KeyEventArgs args)
        {
            if (args.Key == Key.Enter)
            {
                GoToSource(SelectedError);
            }
        }

        private void PreviewMouseDoubleClickExecute(object obj)
        {
            GoToSource(SelectedError);
        }

        private bool PreviewMouseDoubleClickCanExecute(object arg)
        {
            return SelectedError != null;
        }

        private void GoToSource(ValidationError selectedError)
        {
            ITreeElementViewModel treeElement = selectedError?.TreeElement;
            if (treeElement != null && !treeElement.IsDisposed)
            {
                treeElement.ExpandParentsToRoot(AppContext);
                Messenger.SingleDispatch(MessengerDefinitions.ReSelectNodeInTree(treeElement));
            }
        }
    }
}
