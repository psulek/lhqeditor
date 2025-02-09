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
using LHQ.App.Components.VirtualTreeListViewControl;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels;
using LHQ.App.ViewModels.Elements;
using LHQ.Core.Model.Translation;
using LHQ.Data;
using LHQ.Utils;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace LHQ.App.Model
{
    public interface ITreeElementViewModel : IViewModelElement, IUiLanguageChangeSubscriber
    {
        bool IsTranslationValid { get; set; }

        ITreeElementViewModel Parent { get; }

        ObservableCollectionExt<ITreeElementViewModel> Children { get; }

        bool IsExpandable { get; set; }

        bool IsExpanded { get; }

        bool IsVisible { get; set; }

        bool IsMarkedForCut { get; set; }
        
        bool IsMarkedForExport { get; set; }

        TreeElementType ElementType { get; }

        ShellViewModel ShellViewModel { get; }

        bool ElementIsModel { get; }

        bool ElementIsCategory { get; }

        bool ElementIsResource { get; }

        /// <summary>
        ///     when set to true it flushed current binding UI element to source view model property
        /// </summary>
        bool FlushCurrentChange { get; set; }

        VirtualTreeListViewNode ListViewNode { get; set; }

        bool FocusName { get; set; }

        bool HasHighlightInlines { get; set; }

        bool GetHasChildren();

        void UpdateParent(ITreeElementViewModel newParent);

        void Move(List<ITreeElementViewModel> movedChilds);

        void InternalMove(List<ITreeElementViewModel> movedChilds);

        void AddChildsElements<T>(IEnumerable<T> elements)
            where T : ModelElementBase;

        void PasteFromClipboard(TreeElementClipboardData clipboardData);

        ITreeElementViewModel AddChildSorted(ITreeElementViewModel childViewModel);

        void RemoveChild(ITreeElementViewModel child);

        ModelElementBase SaveToModelElement();

        ITreeElementViewModel CreateFromModelElement(ModelElementBase modelElement, ITreeElementViewModel parentElement);

        void RestoreSelectedElement(ITreeElementViewModel treeElement);

        void AutoTranslateResource(TranslationResult translationResult, ResourceViewModel resourceViewModel);

        void LockResourceValues(ResourceViewModel resourceViewModel, bool lockAll);
        void RefreshListViewNode();
    }
}
