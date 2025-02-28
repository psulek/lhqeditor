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
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Elements;
using LHQ.Core.Model.Translation;
using LHQ.Data;
using LHQ.Utils.Extensions;

namespace LHQ.App.UndoUnits
{
    internal class AutoTranslateResourceUndoUnit<TModelElement> : TreeElementChildUndoUnit<TModelElement>
        where TModelElement : ModelElementBase, new()
    {
        private readonly TranslationResult _translationResult;
        private Dictionary<string, string> _savedResourceValues;
        private Dictionary<string, bool> _savedResourceAutoFlags;

        public AutoTranslateResourceUndoUnit(TreeElementViewModel<TModelElement> parent, ITreeElementViewModel child,
            TranslationResult translationResult)
            : base(parent, child)
        {
            if (child.ElementType != TreeElementType.Resource)
            {
                throw new InvalidOperationException("Allowed for 'Resource' element type only!");
            }

            _translationResult = translationResult ?? throw new ArgumentNullException(nameof(translationResult));
            string childName = child.Name;

            InternalUpdateGetName(() => Strings.Services.UndoUnits.AutoTranslateResource.ActionName(childName));
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
                            "AutoTranslateResourceUndoUnit.InternalDo child (key: {0} is disposed!".FormatWith(ChildElementKey));
                    }

                    var resourceViewModel = child as ResourceViewModel;
                    _savedResourceValues = new Dictionary<string, string>();
                    _savedResourceAutoFlags = new Dictionary<string, bool>();

                    if (_translationResult.HasValidTranslation && resourceViewModel != null)
                    {
                        Dictionary<string, string> translations = _translationResult.GetTranslationsByLanguage();

                        using (TreeViewService.TemporaryBlock(TemporaryBlockType.RefreshTranslationValidity))
                        {
                            foreach (ResourceValueViewModel resourceValue in resourceViewModel.Values)
                            {
                                if (!resourceValue.IsReferenceLanguage && translations.ContainsKey(resourceValue.LanguageName))
                                {
                                    _savedResourceValues.Add(resourceValue.LanguageName, resourceValue.Value);
                                    _savedResourceAutoFlags.Add(resourceValue.LanguageName, resourceValue.AutoTranslated);

                                    string translation = translations[resourceValue.LanguageName];
                                    resourceValue.SetAutoTranslated(translation, true);
                                }
                            }
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
                            "AutoTranslateResourceUndoUnit.InternalDo child (key: {0} is disposed!".FormatWith(ChildElementKey));
                    }

                    if (_savedResourceValues != null && _savedResourceValues.Count > 0 && child is ResourceViewModel resourceViewModel)
                    {
                        using (TreeViewService.TemporaryBlock(TemporaryBlockType.RefreshTranslationValidity))
                        {
                            foreach (KeyValuePair<string, string> savedPair in _savedResourceValues)
                            {
                                string languageName = savedPair.Key;
                                string originalValue = savedPair.Value;

                                ResourceValueViewModel resourceValue = resourceViewModel.Values.SingleOrDefault(x => x.LanguageName == languageName);
                                if (resourceValue != null && !resourceValue.IsReferenceLanguage)
                                {
                                    resourceValue.InternalSetPropertyValue(nameof(resourceValue.Value), originalValue);

                                    if (_savedResourceAutoFlags.ContainsKey(languageName))
                                    {
                                        bool autoTranslated = _savedResourceAutoFlags[languageName];
                                        resourceValue.InternalSetPropertyValue(nameof(resourceValue.AutoTranslated), autoTranslated);
                                    }
                                }
                            }
                        }
                    }

                    _savedResourceValues?.Clear();
                    _savedResourceAutoFlags.Clear();

                    parent.UpdateVersion(false);
                }
                else
                {
                    UndoManager.RemoveAllForModelElement(ChildElementKey);
                }
            }
        }
    }
}
