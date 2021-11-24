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
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Data;
using LHQ.Data.Interfaces.Key;

namespace LHQ.App.ViewModels
{
    public class EditResourceParameterViewModel : ObservableModelObject
    {
        private readonly IModelElementKeyProvider _modelElementKeyProvider;
        private string _name;
        private string _description;
        private int _order;

        public EditResourceParameterViewModel(IAppContext appContext, IModelElementKeyProvider modelElementKeyProvider,
            ResourceParameterElement resourceParameterElement)
            : base(appContext)
        {
            _modelElementKeyProvider = modelElementKeyProvider ?? throw new ArgumentNullException(nameof(modelElementKeyProvider));

            if (resourceParameterElement != null)
            {
                Name = resourceParameterElement.Name;
                Description = resourceParameterElement.Description;
                Order = resourceParameterElement.Order;
            }
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

        public int Order
        {
            get => _order;
            set => SetProperty(ref _order, value);
        }

        public ResourceParameterElement ToModelElement()
        {
            IModelElementKey newKey = _modelElementKeyProvider.GenerateKey();
            return new ResourceParameterElement(newKey)
            {
                Name = Name,
                Description = Description,
                Order = Order
            };
        }
    }
}
