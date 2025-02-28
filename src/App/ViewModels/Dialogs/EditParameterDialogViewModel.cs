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

using System.Collections.Generic;
using System.Linq;
using LHQ.App.Localization;
using LHQ.App.Services.Interfaces;
using LHQ.App.Validators;
using LHQ.Utils;
using LHQ.Utils.Extensions;

namespace LHQ.App.ViewModels.Dialogs
{
    public class EditParameterDialogViewModel : DialogViewModelBase
    {
        private readonly List<string> _parametersNames;
        private readonly bool _isAddMode;
        private string _error;
        private bool _hasError;
        private string _name;
        private string _description;
        private int _order;

        public EditParameterDialogViewModel(IAppContext appContext, ObservableCollectionExt<EditResourceParameterViewModel> parameters,
            EditResourceParameterViewModel parameter) : base(appContext)
        {
            _isAddMode = parameter == null;

            if (parameter != null)
            {
                Name = parameter.Name;
                Description = parameter.Description;
                Order = parameter.Order;

                _parametersNames = parameters.Where(x => x != parameter).Select(x => x.Name).ToList();
            }
            else
            {
                _parametersNames = parameters.Select(x => x.Name).ToList();
                Order = parameters.Count == 0 ? 0 : parameters.Max(x => x.Order) + 1;
            }
        }

        public string Error
        {
            get => _error;
            set
            {
                if (SetProperty(ref _error, value))
                {
                    HasError = !_error.IsNullOrEmpty();
                }
            }
        }

        public int Order
        {
            get => _order;
            set => SetProperty(ref _order, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        protected override string GetTitle()
        {
            return _isAddMode
                ? Strings.ViewModels.EditParameter.AddTitle
                : Strings.ViewModels.EditParameter.EditTitle;
        }

        protected override bool CanSubmitDialog()
        {
            return !HasError;
        }

        protected override string ValidateProperty(string propertyName)
        {
            Error = ValidatorBase.ValidateResourceParameterName(null, Name);
            if (Error.IsNullOrEmpty())
            {
                if (_parametersNames.Any(x => x.EqualsTo(Name)))
                {
                    Error = Strings.ViewModels.EditParameter.ParameterWithSameNameExist;
                }
            }

            return Error;
        }
    }
}
