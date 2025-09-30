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
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Implementation.Messaging;
using LHQ.App.Services.Implementation.Undo;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.App.ViewModels;
using LHQ.App.ViewModels.Elements;
using LHQ.Data;
using LHQ.Data.Interfaces.Key;

namespace LHQ.App.UndoUnits
{
    internal class ImportResourcesUndoUnit : UndoCommandUnit
    {
        private readonly ShellViewModel _shellViewModel;
        private readonly RootModelViewModel _rootModel;
        private readonly IShellViewContext _shellViewContext;
        private readonly ModelContext _modelContextToImport;
        private readonly Data.Model _backupModel;
        private readonly IModelElementKey _importAsNewCategoryKey;

        public ImportResourcesUndoUnit(IShellViewContext shellViewContext, ModelContext modelContextToImport, 
            IModelElementKey importAsNewCategoryKey)
            : base(Guid.NewGuid().ToString())
        {
            _shellViewModel = shellViewContext.ShellViewModel;
            _rootModel = shellViewContext.ShellViewModel.RootModel;
            _shellViewContext = shellViewContext;
            _modelContextToImport = modelContextToImport;
            _importAsNewCategoryKey = importAsNewCategoryKey;

            InternalUpdateGetName(() => Strings.Services.UndoUnits.ImportResources.ActionName);

            _backupModel = _rootModel.SaveToModelElement();
        }

        private IMessenger Messenger => _shellViewContext.Messenger;

        private ITreeViewService TreeViewService => _shellViewContext.TreeViewService;

        protected override void InternalDo()
        {
            _shellViewModel.ChangeModelContext(_modelContextToImport);
        }

        protected override void InternalUndo()
        {
            var modelContext = _shellViewModel.ModelContext;
            modelContext.AssignModelFrom(_backupModel);
            _shellViewModel.ChangeModelContext(modelContext);
            _shellViewModel.RemoveIsDirty();
        }

        protected override UndoCommandCompleted InternalCompleted(UndoCommandKind undoKind)
        {
            _rootModel.RefreshAllResourcesCount();
            _rootModel.InvalidateTranslationCountInfo();

            // after undo, remove this undo command to be NOT able to redo import!!
            var allowReverseAction = undoKind == UndoCommandKind.Do;

            if (undoKind == UndoCommandKind.Do && _importAsNewCategoryKey != null)
            {
                var element = TreeViewService.FindElementByKey(_importAsNewCategoryKey.Serialize());
                if (element != null)
                {
                    Messenger.MultiDispatch(MessengerDefinitions.SelectNodeInTree(element));
                }
            }

            return new UndoCommandCompleted(allowReverseAction);
        }
    }
}
