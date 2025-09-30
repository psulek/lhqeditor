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
using System.Runtime.CompilerServices;
using LHQ.App.Model;
using LHQ.App.Services.Implementation.Undo;
using LHQ.App.Services.Interfaces;
using LHQ.App.Services.Interfaces.Undo;
using LHQ.Data;
using LHQ.Data.Interfaces;
using LHQ.Data.Interfaces.Key;
using LHQ.Utils.Extensions;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace LHQ.App.ViewModels.Elements
{
    public abstract class ViewModelElementBase<TModelElement> : ShellViewModelBase, IViewModelElement
        where TModelElement : ModelElementBase, new()
    {
        private string _elementKey;
        private string _parentElementKey;

        protected ViewModelElementBase(IShellViewContext shellViewContext, TModelElement modelElement, bool subscribeToLocalizationChange = true)
            : base(shellViewContext, subscribeToLocalizationChange)
        {
            BuildModelStageInfo = BuildModelStage.NotCalled;

#if DEBUG
            ShowVersion = true;
#else
            ShowVersion = false;
#endif

            TypeOfModelElement = GetModelElementType().ToString();
        }

        private int OriginalVersion { get; set; }

        protected bool ModelBuildCompleted => BuildModelStageInfo == BuildModelStage.Complete;

        private BuildModelStage BuildModelStageInfo { get; set; }

        public string ElementKey
        {
            get => _elementKey;
            private set => SetProperty(ref _elementKey, value);
        }

        public string ParentElementKey
        {
            get => _parentElementKey;
            protected set => SetProperty(ref _parentElementKey, value);
        }

        public abstract string Name { get; set; }

        public int Version { get; private set; }

        public string VersionUID { get; private set; }

        public bool ShowVersion { get; }

        public string TypeOfModelElement { get; }

        public bool HasChanges => Version != OriginalVersion;

        protected bool IsBuildModelInProgress => BuildModelStageInfo == BuildModelStage.InProgress;

        public abstract ModelElementType GetModelElementType();

        protected void ResetBuildStage()
        {
            BuildModelStageInfo = BuildModelStage.NotCalled;
        }

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(storage, value))
            {
                return false;
            }

            if (IsBuildModelInProgress)
            {
                storage = value;
                return true;
            }

            // ReSharper disable once ExplicitCallerInfoArgument
            return base.SetProperty(ref storage, value, propertyName);
        }

        protected void MarkBuildModelCompleted(bool notifyEvent)
        {
            BuildModelStageInfo = BuildModelStage.Complete;

            if (notifyEvent)
            {
                OnBuildModelComplete();
            }
        }

        protected void BuildModel(TModelElement modelElement)
        {
            if (modelElement == null)
            {
                MarkBuildModelCompleted(false);
                return;
            }

            if (BuildModelStageInfo == BuildModelStage.NotCalled)
            {
                using (TransactionManager.TemporaryBlockUndoManager())
                {
                    BuildModelStageInfo = BuildModelStage.InProgress;

                    if (modelElement is IModelNamedElement modelNamedElement)
                    {
                        Name = modelNamedElement.Name;
                    }

                    ElementKey = modelElement.Key.Serialize();

                    var element = modelElement as ModelElement;
                    ParentElementKey = element?.ParentKey != null
                        ? element.ParentKey.Serialize()
                        : string.Empty;

                    Version = 1;
                    VersionUID = Guid.NewGuid().ToString();

                    BuildModelInternal(modelElement);

                    OriginalVersion = Version;

                    MarkBuildModelCompleted(true);
                }
            }
            else
            {
                throw new InvalidOperationException("BuildModel for {0} is called 2nd time!".FormatWith(GetType().Name));
            }
        }

        protected virtual void OnBuildModelComplete()
        { }

        protected IUndoCommand CreatePropertyChangeUndoCommand(TreeElementViewModel<TModelElement> treeViewModelElement,
            ITreeElementViewModel parentTreeElement, string propertyName, object newValue, object oldValue,
            Func<string> transactionDescription)
        {
            return CreatePropertyChangeUndoCommand(treeViewModelElement as ITreeElementViewModel, parentTreeElement,
                propertyName, newValue, oldValue, transactionDescription);
        }

        protected IUndoCommand CreatePropertyChangeUndoCommand(ITreeElementViewModel treeViewModelElement,
            ITreeElementViewModel parentTreeElement, string propertyName, object newValue, object oldValue,
            Func<string> transactionDescription)
        {
            return CreatePropertyChangeUndoCommand(treeViewModelElement as ViewModelElementBase<TModelElement>,
                parentTreeElement, propertyName, newValue, oldValue, transactionDescription);
        }

        protected IUndoCommand CreatePropertyChangeUndoCommand(ViewModelElementBase<TModelElement> viewModelElement,
            ITreeElementViewModel parentTreeElement, string propertyName, object newValue, object oldValue,
            Func<string> transactionDescription)
        {
            string undoCommandUID = CreateUndoCommandUID(viewModelElement, propertyName);

            if (TransactionManager.IsBlockedUndoRedo)
            {
                return new UndoCommand(transactionDescription, () => viewModelElement.InternalSetPropertyValue(propertyName, newValue), () => { },
                    undoCommandUID);
            }

            return new ViewModelPropertyChangeUndoCommand<TModelElement>(viewModelElement, parentTreeElement,
                propertyName, newValue, oldValue, transactionDescription, undoCommandUID);
        }

        private static string CreateUndoCommandUID(ViewModelElementBase<TModelElement> viewModelElement, string propertyName)
        {
            return $"{viewModelElement.TypeOfModelElement}-{viewModelElement.ElementKey}-{propertyName}";
        }

        protected internal RootModelViewModel GetRootModel()
        {
            return ShellViewModel.RootModel;
        }

        protected internal override void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (BuildModelStageInfo == BuildModelStage.NotCalled)
            {
                throw new InvalidOperationException(
                    "BuildModel for {0} must be called prior raising any property changed event!".FormatWith(GetType().Name));
            }

            // ReSharper disable once ExplicitCallerInfoArgument
            base.RaisePropertyChanged(propertyName);
        }

        protected IModelElementKey ParseModelElementKey(string key)
        {
            IModelElementKeyProvider keyProvider = ShellViewModel.ModelContext.KeyProvider;
            return keyProvider.TryParseKey(key, out IModelElementKey parentKey) ? parentKey : null;
        }

        public IModelElementKey GetElementKey()
        {
            return ParseModelElementKey(ElementKey);
        }

        protected IModelElementKey GenerateElementKey()
        {
            IModelElementKeyProvider keyProvider = ShellViewModel.ModelContext.KeyProvider;
            return keyProvider.GenerateKey();
        }

        protected void UpdateVersion(int value)
        {
            if (value != Version)
            {
                Version = value;
                VersionUID = Guid.NewGuid().ToString();
                RaisePropertyChanged(nameof(Version));
                RaisePropertyChanged(nameof(VersionUID));
            }
        }

        public void UpdateVersion(bool increase)
        {
            int newVersion = Version;

            if (increase)
            {
                newVersion++;
            }
            else
            {
                newVersion--;
            }

            UpdateVersion(newVersion);
        }

        internal TModelElement SaveToModelElement()
        {
            var modelElementBase = new TModelElement();

            IModelElementKey modelElementKey = GetElementKey() ?? GenerateElementKey();
            ShellViewModel.ModelContext.UpdateKey(modelElementBase, modelElementKey);

            if (Version != OriginalVersion)
            {
                if (Version > OriginalVersion)
                {
                    OriginalVersion++;
                }
            }

            if (modelElementBase is ModelElement modelElement && !ParentElementKey.IsNullOrEmpty())
            {
                modelElement.ParentKey = ParseModelElementKey(ParentElementKey);
            }

            if (modelElementBase is IModelNamedElement modelNamedElement)
            {
                modelNamedElement.Name = Name;
            }

            InternalSaveToModelElement(modelElementBase);

            ShellViewModel.NotifySaveToElement(this, modelElementBase);

            return modelElementBase;
        }

        protected abstract void BuildModelInternal(TModelElement modelElement);

        internal abstract void InternalSaveToModelElement(TModelElement modelElement);

        internal abstract bool InternalSetPropertyValue(string propertyName, object value);

        private enum BuildModelStage
        {
            NotCalled,
            InProgress,
            Complete
        }
    }
}
