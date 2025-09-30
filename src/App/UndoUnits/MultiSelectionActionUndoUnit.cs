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
using System.Linq;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Implementation;
using LHQ.App.Services.Implementation.Undo;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.App.ViewModels;
using LHQ.App.ViewModels.Elements;
using LHQ.Data;

namespace LHQ.App.UndoUnits
{
    internal class MultiSelectionActionUndoUnit : UndoCommandUnit, IModelElementUndoCommand
    {
        private readonly List<string> _resourceKeys;
        private readonly ShellViewModel _shellViewModel;
        private readonly ResourceElementTranslationState _state;
        private bool _isInitialized;
        private Dictionary<string, ResourceElementTranslationState> _savedResourceStates;

        public MultiSelectionActionUndoUnit(IShellViewContext shellViewContext, List<ResourceViewModel> resources, 
            ResourceElementTranslationState state) : base(Guid.NewGuid().ToString())
        {
            _resourceKeys = resources.Select(x => x.ElementKey).ToList();
            _state = state;

            _shellViewModel = shellViewContext.ShellViewModel;

            InternalUpdateGetName(() => Strings.Services.UndoUnits.MultiSelection.ActionName);

            _isInitialized = false;
        }

        private ITreeViewService TreeViewService => UndoManager.ShellViewContext.TreeViewService;

        private TransactionUndoManager UndoManager => (TransactionUndoManager)Manager;

        private List<ResourceViewModel> FindResourcesToUpdate()
        {
            return TreeViewService.FindAllElements(x => x.ElementType == TreeElementType.Resource && _resourceKeys.Contains(x.ElementKey))
                .Cast<ResourceViewModel>().ToList();
        }

        private List<ResourceViewModel> Initialize()
        {
            List<ResourceViewModel> result = FindResourcesToUpdate();

            if (!_isInitialized)
            {
                _isInitialized = true;

                _savedResourceStates = new Dictionary<string, ResourceElementTranslationState>();

                foreach (ResourceViewModel resource in result)
                {
                    _savedResourceStates.Add(resource.ElementKey, resource.State);
                }
            }

            return result;
        }

        protected override void InternalDo()
        {
            List<ResourceViewModel> resourcesToUpdate = Initialize();

            using (TreeViewService.TemporaryBlock(TemporaryBlockType.Selection))
            {
                foreach (ResourceViewModel resource in resourcesToUpdate)
                {
                    resource.InternalSetPropertyValue(nameof(resource.State), _state);
                }
            }
        }

        protected override void InternalUndo()
        {
            using (TreeViewService.TemporaryBlock(TemporaryBlockType.Selection))
            {
                foreach (ResourceViewModel resource in FindResourcesToUpdate())
                {
                    if (_savedResourceStates.ContainsKey(resource.ElementKey))
                    {
                        resource.InternalSetPropertyValue(nameof(resource.State), _savedResourceStates[resource.ElementKey]);
                    }
                }
            }
        }

        protected override UndoCommandCompleted InternalCompleted(UndoCommandKind undoKind)
        {
            _shellViewModel.RootModel.RefreshAllResourcesCount();
            _shellViewModel.RootModel.InvalidateTranslationCountInfo();
            return UndoCommandCompleted.Default;
        }

        public bool CanRemove(string keyToRemove)
        {
            return false;
        }
    }
}
