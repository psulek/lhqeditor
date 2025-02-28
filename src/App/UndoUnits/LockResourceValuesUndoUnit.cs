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
using LHQ.App.ViewModels.Elements;
using LHQ.Data;
using LHQ.Utils.Extensions;

namespace LHQ.App.UndoUnits
{
    internal class LockResourceValuesUndoUnit<TModelElement> : TreeElementChildUndoUnit<TModelElement>
        where TModelElement : ModelElementBase, new()
    {
        private readonly bool _lockAll;
        private Dictionary<string, bool> _savedResourceLockedFlags;

        public LockResourceValuesUndoUnit(TreeElementViewModel<TModelElement> parent,
            ITreeElementViewModel child, bool lockAll) : base(parent, child)
        {
            if (child.ElementType != TreeElementType.Resource)
            {
                throw new InvalidOperationException("Allowed for 'Resource' element type only!");
            }

            _lockAll = lockAll;
            string childName = child.Name;
            
            InternalUpdateGetName(() => Strings.Services.UndoUnits.LockResourceValues.ActionName(childName));
        }

        protected override void InternalDo()
        {
            if (Validate(out TreeElementViewModel<TModelElement> parent))
            {
                ITreeElementViewModel child = FindChild();

                if (child != null && child.ElementType == TreeElementType.Resource)
                {
                    if (child.IsDisposed)
                    {
                        throw new ObjectDisposedException(
                            "LockResourceValuesUndoUnit.InternalDo child (key: {0} is disposed!".FormatWith(ChildElementKey));
                    }

                    var resourceViewModel = child as ResourceViewModel;
                    _savedResourceLockedFlags = new Dictionary<string, bool>();

                    if (resourceViewModel != null)
                    {
                        foreach (ResourceValueViewModel resourceValue in resourceViewModel.Values)
                        {
                            _savedResourceLockedFlags.Add(resourceValue.LanguageName, resourceValue.Locked);

                            resourceValue.InternalSetPropertyValue(nameof(resourceValue.Locked), _lockAll);
                        }
                    }

                    parent.UpdateVersion(true);
                }
                else
                {
                    UndoManager.RemoveAllForModelElement(ChildElementKey);
                }
            }
        }

        protected override void InternalUndo()
        {
            if (Validate(out TreeElementViewModel<TModelElement> parent))
            {
                ITreeElementViewModel child = FindChild();

                if (child != null && child.ElementType == TreeElementType.Resource)
                {
                    if (child.IsDisposed)
                    {
                        throw new ObjectDisposedException(
                            "LockResourceValuesUndoUnit.InternalDo child (key: {0} is disposed!".FormatWith(ChildElementKey));
                    }

                    if (child is ResourceViewModel resourceViewModel)
                    {
                        foreach (KeyValuePair<string, bool> savedPair in _savedResourceLockedFlags)
                        {
                            string languageName = savedPair.Key;
                            bool originalLocked = savedPair.Value;

                            ResourceValueViewModel resourceValue = resourceViewModel.Values.SingleOrDefault(x => x.LanguageName == languageName);
                            resourceValue?.InternalSetPropertyValue(nameof(resourceValue.Locked), originalLocked);
                        }
                    }

                    _savedResourceLockedFlags.Clear();
                    parent.UpdateVersion(true);
                }
                else
                {
                    UndoManager.RemoveAllForModelElement(ChildElementKey);
                }
            }
        }
    }
}
