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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LHQ.App.Code;
using LHQ.App.Components.VirtualTreeListViewControl;
using LHQ.App.Model;
using LHQ.App.Services.Implementation;
using LHQ.App.Services.Interfaces;
using LHQ.Data;
using LHQ.Utils.Extensions;
using LHQ.Utils.GenericTree;
using LHQ.Utils.Utilities;

namespace LHQ.App.ViewModels.Elements
{
    public static class ViewModelElementExtensions
    {
        public static void ExpandParentsToRoot(this ITreeElementViewModel viewModel, IAppContext appContext)
        {
            ArgumentValidator.EnsureArgumentNotNull(viewModel, "viewModel");
            ITreeViewService treeViewService = viewModel.ShellViewModel.ShellViewContext.TreeViewService;
            viewModel.GetParents(false, false).Run(parent =>
                {
                    if (!parent.IsVisible)
                    {
                        parent.IsVisible = true;
                    }

                    if (!parent.GetIsExpanded() && parent.IsExpandable)
                    {
                        treeViewService.Expand(parent, false);
                    }
                });
        }

        public static bool GetIsExpanded(this ITreeElementViewModel viewModel)
        {
            ArgumentValidator.EnsureArgumentNotNull(viewModel, "viewModel");
            return viewModel.ListViewNode != null && viewModel.ListViewNode.IsExpanded;
        }

        public static TreeNodeCollection GetNodeCollection(this ITreeElementViewModel viewModel)
        {
            ArgumentValidator.EnsureArgumentNotNull(viewModel, "viewModel");
            return viewModel.ListViewNode.NodeCollection;
        }

        public static CategoryViewModel ToViewModel(this CategoryElement category, ITreeElementViewModel parentElement, IShellViewContext shellViewContext)
        {
            ArgumentValidator.EnsureArgumentNotNull(category, "category");
            ArgumentValidator.EnsureArgumentNotNull(parentElement, "parentElement");

            if (parentElement.ElementType == TreeElementType.Resource)
            {
                throw new InvalidOperationException("Parent of category cannot be resource!");
            }

            return new CategoryViewModel(shellViewContext, parentElement, category);
        }

        public static ResourceViewModel ToViewModel(this ResourceElement resource, ITreeElementViewModel parentElement, IShellViewContext shellViewContext)
        {
            ArgumentValidator.EnsureArgumentNotNull(resource, "resource");
            ArgumentValidator.EnsureArgumentNotNull(parentElement, "parentElement");

            if (parentElement.ElementType == TreeElementType.Resource)
            {
                throw new InvalidOperationException("Parent of resource cannot be another resource!");
            }

            return new ResourceViewModel(shellViewContext, parentElement, resource);
        }

        public static ResourceValueViewModel ToViewModel(this ResourceValueElement resourceValue, ResourceViewModel resourceModel)
        {
            ArgumentValidator.EnsureArgumentNotNull(resourceValue, "resourceValue");

            return new ResourceValueViewModel(resourceModel, resourceValue);
        }

        public static LanguageViewModel ToViewModel(this LanguageElement language, IShellViewContext shellViewContext)
        {
            ArgumentValidator.EnsureArgumentNotNull(language, "language");

            return new LanguageViewModel(shellViewContext, language);
        }

        public static int GetParentsDepth(this ITreeElementViewModel treeElement)
        {
            int GetDepth(ITreeElementViewModel element)
            {
                var res = 0;
                if (element.ElementType != TreeElementType.Model)
                {
                    res = 1;
                    var category = (ICategoryLikeViewModel)element;
                    if (category.Parent != null)
                    {
                        res += GetDepth(category.Parent);
                    }
                }

                return res;
            }

            return GetDepth(treeElement);
        }

        public static List<string> GetParentNames(this ITreeElementViewModel treeElement, bool includeCurrent,
            bool fromRootToCurrent, bool includeRoot = true)
        {
            List<ITreeElementViewModel> parents = treeElement.GetParents(includeCurrent, fromRootToCurrent, includeRoot);
            return parents.Select(x => x.Name).ToList();
        }

        private static List<ITreeElementViewModel> GetParents(this ITreeElementViewModel treeElement, bool includeCurrent,
            bool fromRootToCurrent, bool includeRoot = true)
        {
            ArgumentValidator.EnsureArgumentNotNull(treeElement, "treeElement");

            var result = new List<ITreeElementViewModel>();
            if (includeCurrent)
            {
                result.Add(treeElement);
            }

            ITreeElementViewModel parentCategory = treeElement.Parent;
            while (parentCategory != null)
            {
                bool isRoot = parentCategory.Parent == null;

                if (!isRoot || includeRoot)
                {
                    result.Add(parentCategory);
                }
                parentCategory = parentCategory.Parent;
            }

            if (fromRootToCurrent)
            {
                result.Reverse();
            }

            return result;
        }

        public static string GenerateUniqueName(this ITreeElementViewModel treeElement, ICategoryLikeViewModel parentElement)
        {
            if (treeElement.ElementType == TreeElementType.Resource)
            {
                return NameHelper.GenerateUniqueResourceName(treeElement.Name, parentElement);
            }

            if (treeElement.ElementType == TreeElementType.Category)
            {
                return NameHelper.GenerateUniqueCategoryName(treeElement.Name, parentElement);
            }

            throw new NotSupportedException();
        }

        public static IOrderedEnumerable<T> OrderByElementType<T>(this IEnumerable<T> source, bool ascending)
            where T : ITreeElementViewModel
        {
            ArgumentValidator.EnsureArgumentNotNull(source, "source");

            return ascending
                ? source.OrderBy(x => x.ElementType)
                : source.OrderByDescending(x => x.ElementType);
        }

        public static void RemoveAllForModelElement(this TransactionUndoManager undoManager, string keyToRemove)
        {
            ArgumentValidator.EnsureArgumentNotNull(undoManager, "undoManager");

            undoManager.RemoveAll(x => x is IModelElementUndoCommand unit && unit.CanRemove(keyToRemove));
        }

        public static void IterateChildsTree(this ITreeElementViewModel startTreeElement,
            Action<GenericTreeIterationArgs<ITreeElementViewModel>> iterateAction)
        {
            ArgumentValidator.EnsureArgumentNotNull(startTreeElement, "startTreeElement");
            ArgumentValidator.EnsureArgumentNotNull(iterateAction, "iterateAction");

            InternalIterateTree(startTreeElement, 0, iterateAction);
        }

        private static void InternalIterateTree(ITreeElementViewModel parent, int level,
            Action<GenericTreeIterationArgs<ITreeElementViewModel>> iterateAction)
        {
            foreach (ITreeElementViewModel child in parent.Children)
            {
                var args = new GenericTreeIterationArgs<ITreeElementViewModel>(level, parent, child);
                iterateAction(args);
                if (args.Cancel)
                {
                    break;
                }

                InternalIterateTree(child, level + 1, iterateAction);
            }
        }

        public static CultureInfoItem ToCultureInfoItem(this CultureInfo culture, bool isoCodeAsDisplay = false)
        {
            ArgumentValidator.EnsureArgumentNotNull(culture, "culture");

            return new CultureInfoItem(culture) { NameAsDisplay = isoCodeAsDisplay };
        }

        public static CultureInfoItem GetDefaultProjectCulture(this AppConfig appConfig)
        {
            ArgumentValidator.EnsureArgumentNotNull(appConfig, "appConfig");

            return appConfig.DefaultProjectLanguage.IsNullOrEmpty()
                ? null
                : CultureCache.Instance.GetCulture(appConfig.DefaultProjectLanguage).ToCultureInfoItem();
        }
    }
}
