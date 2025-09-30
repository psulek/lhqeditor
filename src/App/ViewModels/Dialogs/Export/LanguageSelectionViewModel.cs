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

using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.ViewModels.Elements;
using LHQ.Data.Interfaces;

namespace LHQ.App.ViewModels.Dialogs.Export
{
    public class LanguageSelectionViewModel : ObservableModelObject, IModelLanguageElement
    {
        private readonly ExportDialogViewModel _owner;
        private string _englishName;
        private string _name;
        private bool _isSelected;
        private bool _isPrimary;
        private string _description;

        public LanguageSelectionViewModel(ExportDialogViewModel owner, LanguageViewModel language)
            : base(owner.AppContext)
        {
            _owner = owner;
            _englishName = language.EnglishName;
            _name = language.Name;
            _isPrimary = language.IsPrimary;
            _isSelected = language.IsPrimary;
            _description = language.IsPrimary ? Strings.Common.Primary : string.Empty;
        }

        public int Order { get; set; }

        public string EnglishName
        {
            get => _englishName;
            set => SetProperty(ref _englishName, value);
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

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (!IsPrimary)
                {
                    _isSelected = value;
                    RaisePropertyChanged();

                    _owner.NotifySelectedChanged();
                }
            }
        }

        public bool IsPrimary
        {
            get => _isPrimary;
            set => SetProperty(ref _isPrimary, value);
        }
    }
}
