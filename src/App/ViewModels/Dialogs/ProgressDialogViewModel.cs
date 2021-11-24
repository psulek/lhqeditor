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
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Code.Observable;
using LHQ.App.Services.Interfaces;
using LHQ.Utils.Extensions;

namespace LHQ.App.ViewModels.Dialogs
{
    public class ProgressDialogViewModel : DialogViewModelBase
    {
        private string _text;
        private bool _completedCalled;
        private bool _userCancelled;
        private readonly IAsyncOperation _asyncOperation;
        private double _currentValue;
        private bool _showCancelButton;
        private bool _isIndeterminate;
        private double _maximumValue;
        private readonly IAsyncOperationWithProgress _asyncOperationWithProgress;
        private readonly ObservableProgress<AsyncOperationProgressData> _observableProgress;

        public ProgressDialogViewModel(IAppContext appContext, IAsyncOperation asyncOperation) : base(appContext)
        {
            _asyncOperation = asyncOperation;
            _completedCalled = false;
            _userCancelled = false;
            CancelCommand = new DelegateCommand(CancelExecute, CanCancelExecute);

            IsIndeterminate = true;
            CurrentValue = 0;
            if (asyncOperation is IAsyncOperationWithProgress asyncOperationWithProgress)
            {
                _asyncOperationWithProgress = asyncOperationWithProgress;
                IsIndeterminate = false;

                _observableProgress = ObservableProgressHelper.CreateForUi<AsyncOperationProgressData>();
            }

            UpdateProperties();
        }

        private void UpdateProperties()
        {
            ShowCancelButton = _asyncOperation.ShowCancelButton;

            if (_asyncOperationWithProgress != null)
            {
                MaximumValue = _asyncOperationWithProgress.MaximumValue;
            }
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public ICommand CancelCommand { get; }

        public bool ShowCancelButton
        {
            get => _showCancelButton;
            set => SetProperty(ref _showCancelButton, value);
        }

        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set => SetProperty(ref _isIndeterminate, value);
        }

        public double MaximumValue
        {
            get => _maximumValue;
            set => SetProperty(ref _maximumValue, value);
        }

        public double CurrentValue
        {
            get => _currentValue;
            set => SetProperty(ref _currentValue, value);
        }

        protected override string GetTitle()
        {
            return _asyncOperation?.ProgressMessage;
        }

        protected override void OnClosingWindow(CancelEventArgs e)
        {
            Cancel();
        }

        private bool CanCancelExecute(object arg)
        {
            return !_userCancelled && _asyncOperation.CanBeCancelled;
        }

        private void CancelExecute(object obj)
        {
            try
            {
                Cancel();
            }
            finally
            {
                CloseDialog();
            }
        }

        private void Cancel()
        {
            if (CanCancelExecute(null))
            {
                try
                {
                    _userCancelled = true;

                    _asyncOperation.Cancel(true);

#pragma warning disable 4014
                    CompleteActionAsync();
#pragma warning restore 4014
                }
                catch (Exception e)
                {
                    DebugUtils.Error("ProgressDialogViewModel.CancelExecute", e);
                }
            }
        }

        protected override void OnLoadedWindow(RoutedEventArgs e)
        {
            base.OnLoadedWindow(e);
#pragma warning disable 4014
            StartActionAsync();
#pragma warning restore 4014
        }

        private async Task StartActionAsync()
        {
            try
            {
                await _asyncOperation.BeforeStart();

                UpdateProperties();

                if (_observableProgress != null)
                {
                    using (_observableProgress.Observable.Subscribe(UpdateUi))
                    {
                        await Task.Run(async () => await _asyncOperationWithProgress.Start(_observableProgress.Progress));
                    }

                    void UpdateUi(AsyncOperationProgressData data)
                    {
                        CurrentValue = data.Value;
                        Text = data.Text;
                    }
                }
                else
                {
                    await _asyncOperation.Start();
                }

                CloseDialog();
#pragma warning disable 4014
                CompleteActionAsync();
#pragma warning restore 4014
            }
            catch (Exception e)
            {
                DebugUtils.Error("ProgressDialogViewModel.StartActionAsync", e);
#pragma warning disable 4014
                CompleteActionAsync();
#pragma warning restore 4014
            }
        }

        private async Task CompleteActionAsync()
        {
            if (!_completedCalled)
            {
                _completedCalled = true;
                await _asyncOperation.Completed(_userCancelled);
            }
        }
    }
}
