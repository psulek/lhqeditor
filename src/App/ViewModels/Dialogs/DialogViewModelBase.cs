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

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Services.Interfaces;

// ReSharper disable VirtualMemberNeverOverridden.Global

namespace LHQ.App.ViewModels.Dialogs
{
    public abstract class DialogViewModelBase : ViewModelBase
    {
        private string _title;
        private Window _dialogWindow;
        private bool? _dialogResult;
        private bool _initializeCompleted;

        protected DialogViewModelBase(IAppContext appContext)
            : base(appContext, true)
        {
            _title = string.Empty;
            _dialogResult = null;
            
            InitializeCommands();
        }

        public string Title
        {
            get => _title;
            private set => SetProperty(ref _title, value);
        }

        private bool HasDialogWindow => _dialogWindow != null;

        public DelegateCommand<KeyEventArgs> KeyDownCommand { get; private set; }

        public DelegateCommand<RoutedEventArgs> LoadedWindowCommand { get; private set; }

        public DelegateCommand<CancelEventArgs> ClosingWindowCommand { get; private set; }

        public ICommand CloseDialogCommand { get; private set; }

        public ICommand CancelDialogCommand { get; private set; }

        public ICommand SubmitDialogCommand { get; private set; }

        protected bool? DialogResult => HasDialogWindow ? _dialogWindow.DialogResult : _dialogResult;

        private void InitializeCommands()
        {
            try
            {
                KeyDownCommand = new DelegateCommand<KeyEventArgs>(OnKeyDown);
                LoadedWindowCommand = new DelegateCommand<RoutedEventArgs>(OnLoadedWindow);
                ClosingWindowCommand = new DelegateCommand<CancelEventArgs>(OnClosingWindow);

                CloseDialogCommand = new DelegateCommand(CloseDialogExecute, CloseDialogCanExecute);
                SubmitDialogCommand = new DelegateCommand(SubmitDialogExecute, SubmitDialogCanExecute);
                CancelDialogCommand = new DelegateCommand(CancelDialogExecute);
            }
            finally
            {
                _initializeCompleted = true;
            }
        }

        protected internal override bool IsBlockedPropertyChange()
        {
            if (!_initializeCompleted)
            {
                return true;
            }
            
            return base.IsBlockedPropertyChange();
        }

        protected virtual void OnLoadedWindow(RoutedEventArgs e)
        {
            LocalizeStrings();
            _dialogWindow = e.Source as Window;
            var inputBinding = new InputBinding(CloseDialogCommand, new KeyGesture(Key.Escape));
            _dialogWindow?.InputBindings.Add(inputBinding);
        }

        protected virtual void OnClosingWindow(CancelEventArgs e)
        {}

        protected void ExecuteSubmitDialogCommand()
        {
            if (SubmitDialogCommand.CanExecute(null))
            {
                SubmitDialogCommand.Execute(null);
            }
        }

        private bool CloseDialogCanExecute(object arg)
        {
            return CanCloseDialog();
        }

        protected virtual bool CanCloseDialog()
        {
            return true;
        }

        private void CancelDialogExecute(object obj)
        {
            CancelDialog();
        }

        protected virtual void CancelDialog()
        {
            CloseDialog();
        }

        private void CloseDialogExecute(object obj)
        {
            CloseDialog();
        }

        protected void CloseDialog()
        {
            _dialogWindow?.Close();
        }

        private bool SubmitDialogCanExecute(object arg)
        {
            return CanSubmitDialog();
        }

        protected virtual bool CanSubmitDialog()
        {
            return true;
        }

        protected virtual bool AutoCloseOnSubmit => true;

        private void SubmitDialogExecute(object obj)
        {
            if (SubmitDialog())
            {
                if (HasDialogWindow)
                {
                    if (AutoCloseOnSubmit)
                    {
                        _dialogWindow.DialogResult = true;
                    }
                }
                else
                {
                    _dialogResult = true;
                }
            }
        }

        public virtual bool SubmitDialog()
        {
            return true;
        }

        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                HandleEnter();
            }
        }

        protected virtual void HandleEnter()
        {
            if (SubmitDialogCommand.CanExecute(null))
            {
                SubmitDialogExecute(null);
            }
        }

        protected abstract string GetTitle();

        protected override void DoLocalizeStrings()
        {
            Title = AppContext.GetTitleForDialog(GetTitle());
        }
    }
}
