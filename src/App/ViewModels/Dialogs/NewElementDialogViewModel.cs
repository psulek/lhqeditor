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

using System.Collections.Generic;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.Validators;
using LHQ.App.ViewModels.Elements;
using LHQ.Data;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.ViewModels.Dialogs
{
    public class NewElementDialogViewModel : DialogViewModelBase
    {
        private string _elementName;
        private string _error;
        private bool _hasError;
        private readonly IShellViewContext _shellViewContext;

        public NewElementDialogViewModel(IShellViewContext shellViewContext, ICategoryLikeViewModel parentElement,
            List<string> parentNames, bool createResource)
            : base(shellViewContext.AppContext)
        {
            _shellViewContext = shellViewContext;
            ParentElement = parentElement;
            ParentName = parentElement.Name;
            ParentIsModel = parentElement.ElementType == TreeElementType.Model;
            ParentNames = parentNames;
            NewElementIsResource = createResource;

            var keyProvider = shellViewContext.ShellViewModel.ModelContext.KeyProvider;
            var generatedKey = keyProvider.GenerateKey();
            NewElementDummy = createResource
                ? (ITreeElementViewModel)new ResourceViewModel(shellViewContext, parentElement, new ResourceElement(generatedKey))
                : new CategoryViewModel(shellViewContext, parentElement, new CategoryElement(generatedKey));

            CaptionText = NewElementIsResource
                ? Strings.ViewModels.NewElement.ResourceNameCaption
                : Strings.ViewModels.NewElement.CategoryNameCaption;

            ElementName = NewElementIsResource
                ? NameHelper.GenerateUniqueResourceName(Strings.ViewModels.NewElement.NewResourceTemplateName,
                    parentElement)
                : NameHelper.GenerateUniqueCategoryName(Strings.ViewModels.NewElement.NewCategoryTemplateName,
                    parentElement);

            ParentName = Ellipsis.Compact(string.Join(@"/", parentNames), 40, EllipsisFormat.Start);
        }

        public ITreeElementViewModel ParentElement { get; }

        public ITreeElementViewModel NewElementDummy { get; }

        public string ElementName
        {
            get => _elementName;
            set => SetProperty(ref _elementName, value);
        }

        public string ParentName { get; }

        public string CaptionText { get; }

        public bool ParentIsModel { get; }

        public bool NewElementIsResource { get; }

        public List<string> ParentNames { get; }

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

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        protected override string GetTitle()
        {
            return NewElementIsResource
                ? Strings.ViewModels.NewElement.NewResource
                : Strings.ViewModels.NewElement.NewCategory;
        }

        protected override bool CanSubmitDialog()
        {
            return !HasError;
        }

        protected override string ValidateProperty(string propertyName)
        {
            string err;
            if (ElementName.IsNullOrEmpty())
            {
                err = NewElementIsResource
                    ? Strings.ViewModels.NewElement.ResourceNameIsRequired
                    : Strings.ViewModels.NewElement.CategoryNameIsRequired;
            }
            else
            {
                var validatorContext = _shellViewContext.ValidatorContext;

                if (NewElementIsResource)
                {
                    err = ResourceValidator.ValidateName(validatorContext, _elementName);
                    if (err.IsNullOrEmpty())
                    {
                        err = ResourceValidator.ValidateNameAlreadyUsed(validatorContext, _elementName,
                            ParentElement);
                    }
                }
                else
                {
                    err = CategoryValidator.ValidateName(validatorContext, _elementName);
                    if (err.IsNullOrEmpty())
                    {
                        err = CategoryValidator.ValidateNameAlreadyUsed(validatorContext, _elementName,
                            ParentElement);
                    }
                }
            }

            Error = err;
            return Error;
        }
    }
}
