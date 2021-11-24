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
using System.Linq;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.Utils.GenericTree
{
    public sealed class GenericTreeIterator<TItem>
        where TItem : class
    {
        private readonly Func<TItem, IEnumerable<TItem>> _getChildsFunc;

        private readonly Func<IEnumerable<TItem>> _getRootChildsSpecialFunc;

        public GenericTreeIterator(Func<TItem, IEnumerable<TItem>> getChilds, Func<IEnumerable<TItem>> getRootChildsSpecialFunc = null)
        {
            _getChildsFunc = getChilds ?? throw new ArgumentNullException(nameof(getChilds));
            _getRootChildsSpecialFunc = getRootChildsSpecialFunc;
        }

        public void IterateTree<TData>(bool twoStagePass, TItem rootItem,
            Action<GenericTreeIterationArgs<TItem, TData>> iterateAction)
        {
            InternalIterateTree(twoStagePass, 0, rootItem, iterateAction);
        }

        public IEnumerable<TItem> IterateTree(TItem rootItem)
        {
            Func<IEnumerable<TItem>> getChilds = _getRootChildsSpecialFunc ?? (() => _getChildsFunc(rootItem));

            return InternalIterateTree(getChilds);
        }

        private IEnumerable<TItem> InternalIterateTree(Func<IEnumerable<TItem>> getChilds)
        {
            foreach (TItem item in getChilds())
            {
                yield return item;
                if (_getChildsFunc(item).Any())
                {
                    TItem childItem = item;
                    foreach (TItem item1 in InternalIterateTree(() => _getChildsFunc(childItem)))
                    {
                        yield return item1;
                    }
                }
            }
        }

        public void IterateTree(bool twoStagePass, TItem rootItem, Action<GenericTreeIterationArgs<TItem>> iterateAction)
        {
            InternalIterateTree<object>(twoStagePass, 0, rootItem, iterateAction);
        }

        // Returns true if iteration was cancelled
        private void InternalIterateTree<TData>(bool twoStagePass, int level, TItem parent,
            Action<GenericTreeIterationArgs<TItem, TData>> iterateAction)
        {
            InternalIterateTree(twoStagePass, level, parent, default, iterateAction);
        }

        private IterationResult InternalIterateTree<TData>(bool twoStagePass, int level, TItem parent, TData data,
            Action<GenericTreeIterationArgs<TItem, TData>> iterateAction)
        {
            IEnumerable<TItem> childItems;
            if (level == 0 && _getRootChildsSpecialFunc != null)
            {
                childItems = _getRootChildsSpecialFunc();
            }
            else
            {
                childItems = _getChildsFunc(parent);
            }

            foreach (TItem child in childItems)
            {
                var treeIterator = new GenericTreeIterationArgs<TItem, TData>(level, parent, child)
                {
                    Data = data
                };

                iterateAction(treeIterator);
                if (treeIterator.Cancel)
                {
                    return new IterationResult(treeIterator.Cancel, treeIterator.CancelBreaksChildLoop);
                }

                if (_getChildsFunc(child).Any())
                {
                    IterationResult iterationResult = InternalIterateTree(twoStagePass, level + 1, child, treeIterator.Data, iterateAction);
                    if (iterationResult.RequestCancelingIteration())
                    {
                        return iterationResult;
                    }
                }

                if (twoStagePass)
                {
                    treeIterator.CurrentStage = GenericTreeIterationStage.Leave;
                    iterateAction(treeIterator);
                    if (treeIterator.Cancel)
                    {
                        return new IterationResult(treeIterator.Cancel, treeIterator.CancelBreaksChildLoop);
                    }
                }
            }

            return new IterationResult();
        }

        internal class IterationResult
        {
            public IterationResult()
            {
                Cancel = false;
                CancelBreaksChildLoop = false;
            }

            public IterationResult(bool cancel, bool cancelBreaksChildLoop)
            {
                Cancel = cancel;
                CancelBreaksChildLoop = cancelBreaksChildLoop;
            }

            public IterationResult(bool cancel)
            {
                Cancel = cancel;
            }

            public bool Cancel { get; }

            public bool CancelBreaksChildLoop { get; }

            public bool RequestCancelingIteration()
            {
                return Cancel && !CancelBreaksChildLoop;
            }

            public bool RequestBreaksChildLoop()
            {
                return Cancel && CancelBreaksChildLoop;
            }
        }
    }
}
