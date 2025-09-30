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

using LHQ.App.Services.Interfaces;
using LHQ.Utils.Utilities;

namespace LHQ.App.Model
{
    public sealed class ValidationError: IValidationError
    {
        private readonly ValidationErrorLocalizationRefresh _errorRefreshCallback;

        public ValidationError(object owner, string propertyName, string error, 
            ValidationErrorLocalizationRefresh errorRefreshCallback, object data)
        {
            ArgumentValidator.EnsureArgumentNotNull(owner, "owner");
            ArgumentValidator.EnsureArgumentNotNull(errorRefreshCallback, "errorRefreshCallback");

            _errorRefreshCallback = errorRefreshCallback;

            Owner = owner;
            PropertyName = propertyName;
            Error = error;
            Data = data;

            TreeElement = owner as ITreeElementViewModel;
        }

        public ITreeElementViewModel TreeElement { get; }

        public object Owner { get; }

        public string PropertyName { get; }

        public object Data { get; set; }

        public string Error { get; set; }

        public void RefreshErrorLocalization()
        {
            Error = _errorRefreshCallback();
        }
    }
}
