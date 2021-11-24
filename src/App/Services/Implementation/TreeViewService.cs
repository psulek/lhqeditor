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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LHQ.App.Components.VirtualTreeListViewControl;
using LHQ.App.Model;
using LHQ.App.Services.Implementation.Messaging;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Elements;
using LHQ.Core.DependencyInjection;
using LHQ.Utils.GenericTree;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public partial class TreeViewService : ShellViewContextServiceBase, ITreeViewService
    {
        private TranslationStateValidityProcess _translationValidityProcess;

        private PropertyValidityProcess _validityProcess;

        public RootModelViewModel RootModel => ShellViewModel.RootModel;

        private TranslationStateValidityProcess TranslationValidityProcess =>
            _translationValidityProcess ??
            (_translationValidityProcess = new TranslationStateValidityProcess(ShellViewContext));

        private PropertyValidityProcess ValidityProcess =>
            _validityProcess ?? (_validityProcess = new PropertyValidityProcess(ShellViewContext));

        private IMessenger Messenger => ShellViewContext.Messenger;

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        public bool IsTempraryBlocked(TemporaryBlockType blockType)
        {
            switch (blockType)
            {
                case TemporaryBlockType.Selection:
                {
                    return BlockedSelectionScope.IsBlocked;
                }
                case TemporaryBlockType.RefreshTranslationValidity:
                {
                    return BlockedRefreshTranslationValidityScope.IsBlocked;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(blockType), blockType, null);
                }
            }
        }

        public IDisposable TemporaryBlock(TemporaryBlockType blockType)
        {
            switch (blockType)
            {
                case TemporaryBlockType.Selection:
                {
                    return new BlockedSelectionScope();
                }
                case TemporaryBlockType.RefreshTranslationValidity:
                {
                    return new BlockedRefreshTranslationValidityScope();
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(blockType), blockType, null);
                }
            }
        }

        public IEnumerable<ITreeElementViewModel> GetChildsAsFlatList(ITreeElementViewModel startElement)
        {
            RootModelViewModel rootModelViewModel = RootModel;
            Func<IEnumerable<ITreeElementViewModel>> rootChildsSpecialFunc =
                startElement == null ? () => rootModelViewModel.Children : (Func<IEnumerable<ITreeElementViewModel>>)null;
            var treeIterator = new GenericTreeIterator<ITreeElementViewModel>(arg => arg.Children, rootChildsSpecialFunc);
            return treeIterator.IterateTree(startElement);
        }

        private GenericTreeIterator<ITreeElementViewModel> GetTreeIterator(ITreeElementViewModel rootElement)
        {
            RootModelViewModel rootModelViewModel = RootModel;
            Func<IEnumerable<ITreeElementViewModel>> rootChildsSpecialFunc =
                rootElement == null ? () => rootModelViewModel.Children : (Func<IEnumerable<ITreeElementViewModel>>)null;
            return new GenericTreeIterator<ITreeElementViewModel>(arg => arg.Children, rootChildsSpecialFunc);
        }

        public List<ITreeElementViewModel> GetSelectedElements()
        {
            return RootModel.SelectedTreeElements;
        }

        public void ApplyTreeExpandedKeys(IEnumerable<string> source)
        {
            ArgumentValidator.EnsureArgumentNotNull(source, "source");

            var expandedKeys = new HashSet<string>(source);

            List<ITreeElementViewModel> elementsToExpand = FindAllElements(x => expandedKeys.Contains(x.ElementKey)).ToList();

            if (expandedKeys.Contains(RootModel.ElementKey))
            {
                elementsToExpand.Add(RootModel);
            }

            if (elementsToExpand.Count > 0)
            {
                ExpandMultiple(elementsToExpand);
            }
        }

        public string[] GetTreeExpandedKeys()
        {
            var expandedKeys = new HashSet<string>();

            void OptionallyAddKey(ITreeElementViewModel element)
            {
                if (element.IsExpandable && element.GetIsExpanded() && !expandedKeys.Contains(element.ElementKey))
                {
                    expandedKeys.Add(element.ElementKey);
                }
            }

            RootModelViewModel rootModel = RootModel;
            OptionallyAddKey(rootModel);

            rootModel.IterateChildsTree(args => { OptionallyAddKey(args.Current); });

            return expandedKeys.ToArray();
        }

        public TreeSelectionInfo GetSelection()
        {
            return RootModel.GetSelection();
        }

        public IEnumerable<ITreeElementViewModel> FindAllElements(Predicate<ITreeElementViewModel> predicate)
        {
            GenericTreeIterator<ITreeElementViewModel> treeIterator = GetTreeIterator(null);
            foreach (ITreeElementViewModel treeElementViewModel in treeIterator.IterateTree(null))
            {
                if (predicate(treeElementViewModel))
                {
                    yield return treeElementViewModel;
                }
            }
        }

        public ITreeElementViewModel FindElementByKey(string elementKey)
        {
            if (RootModel.ElementKey == elementKey)
            {
                return RootModel;
            }

            // TODO: Rewrite treeiterator to find first requested and exit lopping whole tree!
            return GetChildsAsFlatList(null).SingleOrDefault(x => x.ElementKey == elementKey);
        }

        public void Expand<T>(T element, bool includeChilds)
            where T : class, ITreeElementViewModel
        {
            ExpandOrCollapse(element, includeChilds, true);
        }

        private void ExpandMultiple<T>(List<T> elements)
            where T : class, ITreeElementViewModel
        {
            if (elements.Count > 0)
            {
                TreeNodeCollection nodeCollection = elements.First().GetNodeCollection();
                using (nodeCollection.DelayedInvalidate())
                {
                    using (ShellViewContext.PropertyChangeObserver.BlockNotifications(elements.Cast<ObservableModelObject>(), true))
                    {
                        foreach (T item in elements)
                        {
                            VirtualTreeListViewNode listViewNode = item.ListViewNode;
                            if (item.IsExpandable)
                            {
                                listViewNode.IsExpanded = true;
                            }
                        }
                    }
                }
            }
        }

        public void Collapse<T>(T element, bool includeChilds)
            where T : class, ITreeElementViewModel
        {
            ExpandOrCollapse(element, includeChilds, false);
        }

        public void SelectMultiple<T>(IEnumerable<T> elements)
            where T : class, ITreeElementViewModel
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            Messenger.SingleDispatch(MessengerDefinitions.SelectNodesInTree(elements));
        }

        public void Select<T>(T element)
            where T : class, ITreeElementViewModel
        {
            List<ITreeElementViewModel> selectedElements = GetSelectedElements();
            // if currently selected (and only one) element is same element
            if (selectedElements.Count == 1 && selectedElements.First() == element)
            {
                return;
            }

            Messenger.SingleDispatch(MessengerDefinitions.SelectNodesInTree(new ITreeElementViewModel[] { element }));
        }

        public void Unselect<T>(T element)
            where T : class, ITreeElementViewModel
        {
            Messenger.SingleDispatch(MessengerDefinitions.DeSelectNodesInTree(new ITreeElementViewModel[] { element }));
        }

        public void ResetTree()
        {
            List<ITreeElementViewModel> allElements = GetChildsAsFlatList(null).ToList();
            RootModel.Children.Clear();

            AppContext.UIService.UnregisterSubscribers(allElements);

            ValidityProcess.Reset();

            Messenger.SingleDispatch(MessengerDefinitions.ResetTree);
            ShellViewModel.ResetTree();
        }

        private void ExpandOrCollapse<T>(T element, bool includeChilds, bool expand)
            where T : class, ITreeElementViewModel
        {
            ArgumentValidator.EnsureArgumentNotNull(element, "element");

            TreeNodeCollection nodeCollection = element.GetNodeCollection();
            using (nodeCollection.DelayedInvalidate())
            {
                if (includeChilds)
                {
                    var elementWithChilds = new List<ITreeElementViewModel> { element };
                    elementWithChilds.AddRange(GetChildsAsFlatList(element));

                    using (ShellViewContext.PropertyChangeObserver.BlockNotifications(elementWithChilds.Cast<ObservableModelObject>(), true))
                    {
                        if (element.IsExpandable)
                        {
                            element.ListViewNode.SetExpand(expand, true);
                        }
                    }
                }
                else
                {
                    if (element.IsExpandable)
                    {
                        element.ListViewNode.IsExpanded = expand;
                    }
                }
            }
        }

        public void NotifyTranslationValidChanged()
        {
            TranslationValidityProcess.Notify();
        }

        public void NotifyPropertyValidator()
        {
            ValidityProcess.Notify();
        }

        public void ForceRefreshPropertyValidator()
        {
            ValidityProcess.ForceRefresh();
        }
    }
}
