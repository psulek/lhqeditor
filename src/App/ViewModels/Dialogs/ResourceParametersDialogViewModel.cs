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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Dialogs;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Data;
using LHQ.Data.Interfaces.Key;
using LHQ.Utils;

namespace LHQ.App.ViewModels.Dialogs
{
    public class ResourceParametersDialogViewModel : DialogViewModelBase
    {
        private readonly IShellViewContext _shellViewContext;
        private ObservableCollectionExt<EditResourceParameterViewModel> _parameters;
        private ICollectionView _parametersView;
        private EditResourceParameterViewModel _selectedParameter;
        private bool _parametersUsageInfoVisible;

        public ResourceParametersDialogViewModel(IShellViewContext shellViewContext,
            ResourceParameterElementList parameters) : base(shellViewContext.AppContext)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            _shellViewContext = shellViewContext;

            IAppContext appContext = shellViewContext.AppContext;
            IModelElementKeyProvider modelElementKeyProvider = shellViewContext.ModelElementKeyProvider;

            Parameters = new ObservableCollectionExt<EditResourceParameterViewModel>(
                parameters.Select(x => new EditResourceParameterViewModel(appContext, modelElementKeyProvider, x)));

            var parametersView = (ListCollectionView)CollectionViewSource.GetDefaultView(Parameters);
            parametersView.CustomSort = new EditResourceParameterViewModelComparer();
            ParametersView = parametersView;

            AppStateStorage appStateStorage = AppContext.AppStateStorageService.Current;
            ParametersUsageInfoVisible = appStateStorage.ParametersUsageInfoVisible;

            AddParameterCommand = new DelegateCommand(AddParameterExecute);
            EditParameterCommand = new DelegateCommand(EditParameterExecute, EditParameterCanExecute);
            RemoveParameterCommand = new DelegateCommand(RemoveParameterExecute, RemoveParameterCanExecute);
            MoveParameterCommand = new DelegateCommand(MoveParameterExecute, MoveParameterCanExecute);

            ShowHideParameterHintCommand = new DelegateCommand(ShowHideParameterHintExecute);

            PreviewMouseDoubleClickCommand = new DelegateCommand<MouseButtonEventArgs>(OnPreviewMouseDoubleClick);
            ShowHelpCommand = new DelegateCommand(ShowHelpExecute);

            HintPanelViewModel = new HintPanelViewModel(_shellViewContext, AppHintType.ResourceParameters);
        }

        public ICommand ShowHelpCommand { get; }

        public HintPanelViewModel HintPanelViewModel { get; }

        public DelegateCommand<MouseButtonEventArgs> PreviewMouseDoubleClickCommand { get; }

        public ICommand AddParameterCommand { get; }

        public ICommand EditParameterCommand { get; }

        public ICommand RemoveParameterCommand { get; }

        public ICommand MoveParameterCommand { get; }

        public ICommand ShowHideParameterHintCommand { get; }

        public string ShowHideParameterHintTooltip { get; set; }

        public bool ParametersUsageInfoVisible
        {
            get => _parametersUsageInfoVisible;
            set
            {
                SetProperty(ref _parametersUsageInfoVisible, value);
                ShowHideParameterHintTooltip = value ? Strings.Common.ClickToHideThisHint : Strings.Common.ClickToShowThisHint;
                RaisePropertyChanged(nameof(ShowHideParameterHintTooltip));
            }
        }

        public ObservableCollectionExt<EditResourceParameterViewModel> Parameters
        {
            get => _parameters;
            set => SetProperty(ref _parameters, value);
        }

        public ICollectionView ParametersView
        {
            get => _parametersView;
            set => SetProperty(ref _parametersView, value);
        }

        public EditResourceParameterViewModel SelectedParameter
        {
            get => _selectedParameter;
            set => SetProperty(ref _selectedParameter, value);
        }

        private bool MoveParameterCanExecute(object arg)
        {
            bool up = arg.ToString() == "up";

            return SelectedParameter != null && (up ? SelectedParameter.Order > 0 : SelectedParameter.Order < (Parameters.Count-1));
        }

        private void MoveParameterExecute(object arg)
        {
            bool up = arg.ToString() == "up";

            EditResourceParameterViewModel parameter = SelectedParameter;
            if (parameter != null)
            {
                if (up)
                {
                    if (parameter.Order > 0)
                    {
                        int currentOrder = parameter.Order;
                        int newOrder = currentOrder - 1;
                        EditResourceParameterViewModel otherParam = Parameters.Single(p => p.Order == newOrder);
                        otherParam.Order = currentOrder;
                        parameter.Order = newOrder;
                    }
                }
                else
                {
                    if (parameter.Order < (Parameters.Count-1))
                    {
                        int currentOrder = parameter.Order;
                        int newOrder = currentOrder + 1;
                        EditResourceParameterViewModel otherParam = Parameters.Single(p => p.Order == newOrder);
                        otherParam.Order = currentOrder;
                        parameter.Order = newOrder;
                    }
                }

                ParametersView.Refresh();
            }
        }

        private bool RemoveParameterCanExecute(object arg)
        {
            return SelectedParameter != null;
        }

        private void RemoveParameterExecute(object obj)
        {
            string name = SelectedParameter.Name;
            int position = SelectedParameter.Order;
            var caption = Strings.ViewModels.ResourceParameters.RemoveParameterCaption;
            string message = Strings.ViewModels.ResourceParameters.RemoveParameterMessage(name, position);

            if (DialogService.ShowConfirm(new DialogShowInfo(caption, message)).DialogResult == Model.DialogResult.Yes)
            {
                Parameters.Remove(SelectedParameter);

                ParametersView.Refresh();
            }
        }

        private bool EditParameterCanExecute(object arg)
        {
            return SelectedParameter != null;
        }

        private void EditParameterExecute(object obj)
        {
            EditParameterDialog.Result dialogResult = EditParameterDialog.DialogShow(AppContext, Parameters, SelectedParameter);
            if (dialogResult.Submitted)
            {
                SelectedParameter.Name = dialogResult.Name;
                SelectedParameter.Description = dialogResult.Description;

                ParametersView.Refresh();
            }
        }

        private void AddParameterExecute(object obj)
        {
            EditParameterDialog.Result dialogResult = EditParameterDialog.DialogShow(AppContext, Parameters, null);
            if (dialogResult.Submitted)
            {
                var modelElementKeyProvider = _shellViewContext.ModelElementKeyProvider;
                var newParameter = new EditResourceParameterViewModel(AppContext, modelElementKeyProvider, null)
                {
                    Name = dialogResult.Name,
                    Description = dialogResult.Description,
                    Order = dialogResult.Order
                };

                Parameters.Add(newParameter);

                ParametersView.Refresh();
            }
        }

        private void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (EditParameterCommand.CanExecute(null))
            {
                EditParameterCommand.Execute(null);
            }
        }

        protected override string GetTitle()
        {
            return Strings.ViewModels.ResourceParameters.PageTitle;
        }

        private class EditResourceParameterViewModelComparer : Comparer<EditResourceParameterViewModel>
        {
            public override int Compare(EditResourceParameterViewModel x, EditResourceParameterViewModel y)
            {
                int result;

                if (x.Order == y.Order)
                {
                    result = 0;
                }
                else
                {
                    result = x.Order > y.Order ? 1 : -1;
                }

                return result;
            }
        }

        private void ShowHideParameterHintExecute(object obj)
        {
            ParametersUsageInfoVisible = !ParametersUsageInfoVisible;

            AppContext.AppStateStorageService.Update(storage =>
                {
                    storage.ParametersUsageInfoVisible = ParametersUsageInfoVisible;
                });
        }

        private void ShowHelpExecute(object obj)
        {
            WebPageUtils.ShowDoc(WebPageDocSection.ResourceParameters);
        }
    }
}
