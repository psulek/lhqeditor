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
using System.Runtime.InteropServices;
using System.Windows;
using LHQ.App.Code;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels;
using LHQ.App.ViewModels.Elements;
using LHQ.Core.DependencyInjection;
using LHQ.Data;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation
{
    public class TreeElementClipboardManager : ShellViewContextServiceBase, ITreeElementClipboardManager
    {
        private static readonly DataFormat _dataFormat;

        static TreeElementClipboardManager()
        {
            string fullName = typeof(TreeElementClipboardManager).AssemblyQualifiedName;
            _dataFormat = DataFormats.GetDataFormat(fullName + "TreeElementClipboardData");
        }

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        public void SetSelectionToClipboard(bool isMarkedForCut, CopyElementSpecialTag specialCopyTag = CopyElementSpecialTag.None)
        {
            TreeSelectionInfo selectionInfo = TreeViewService.GetSelection();
            if (!selectionInfo.IsEmpty)
            {
                if (specialCopyTag == CopyElementSpecialTag.None)
                {
                    if (!isMarkedForCut) // if is not CUT, mark all tree element nodes as not marked for cut!
                    {
                        TreeViewService.GetChildsAsFlatList(null).Run(x => x.IsMarkedForCut = false);
                    }

                    RootModelViewModel rootModelViewModel = TreeViewService.RootModel;
                    ShellViewModel shellViewModel = ShellViewModel;
                    Guid shellInstanceId = shellViewModel.ShellInstanceId;
                    Version shellVersion = AppContext.AppCurrentVersion;

                    List<ITreeElementViewModel> selectedElements = selectionInfo.GetSelectedElements();
                    selectedElements.ForEach(x => x.IsMarkedForCut = isMarkedForCut);

                    List<ModelElementBase> selectedModelElements = selectedElements.Select(x => x.SaveToModelElement()).ToList();

                    string serializedContent = TreeElementClipboardData.Serialize(shellInstanceId,
                        shellVersion, isMarkedForCut, rootModelViewModel.ElementKey, selectedModelElements);

                    var dataObject = new DataObject();
                    dataObject.SetData(_dataFormat.Name, serializedContent);

                    Clipboard.Clear();
                    Clipboard.SetDataObject(dataObject);
                }
                else
                {
                    SpecialCopy(selectionInfo, specialCopyTag);
                }
            }
        }

        private void SpecialCopy(TreeSelectionInfo selectionInfo, CopyElementSpecialTag specialCopyTag)
        {
            switch (specialCopyTag)
            {
                case CopyElementSpecialTag.XamlMarkup:
                {
                    if (selectionInfo.ElementIsResource)
                    {
                        string modelName = ShellViewModel.RootModel.Name;
                        string fullPath = $"{modelName}Keys+";
                        var parentRecursiveNames = selectionInfo.ElementAsResource.ParentCategory.GetParentNames(true, true, false);
                        fullPath += string.Join("+", parentRecursiveNames);
                        fullPath += "." + selectionInfo.ElementAsResource.Name;

                        Clipboard.Clear();
                        Clipboard.SetText(@"{lhq:Localization Key={x:Static " + fullPath + "}}");
                    }
                    break;
                }
                case CopyElementSpecialTag.CSharpCode:
                {
                    if (selectionInfo.ElementIsResource)
                    {
                        string modelName = ShellViewModel.RootModel.Name;
                        string fullPath = $"{modelName}.";
                        var parentRecursiveNames = selectionInfo.ElementAsResource.ParentCategory.GetParentNames(true, true, false);
                        fullPath += string.Join(".", parentRecursiveNames) + "." + selectionInfo.ElementAsResource.Name;

                        Clipboard.Clear();
                        Clipboard.SetText(fullPath);
                    }
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(specialCopyTag), specialCopyTag, null);
                }
            }
        }

        public bool HasValidClipboardData()
        {
            return HasValidClipboardData(out _);
        }

        private bool HasValidClipboardData(out IDataObject dataObject)
        {
            dataObject = null;

            try
            {
                if (!Clipboard.ContainsData(_dataFormat.Name))
                {
                    return false;
                }

                dataObject = Clipboard.GetDataObject();
            }
            catch (COMException e)
            {
                Console.WriteLine(e);
            }

            return dataObject != null && dataObject.GetDataPresent(_dataFormat.Name);
        }

        public TreeElementClipboardData GetClipboardData()
        {
            TreeElementClipboardData result = null;

            if (HasValidClipboardData(out IDataObject dataObject))
            {
                var serializedContent = dataObject.GetData(_dataFormat.Name) as string;
                if (!serializedContent.IsNullOrEmpty())
                {
                    Version shellVersion = AppContext.AppCurrentVersion;
                    result = TreeElementClipboardData.Deserialize(shellVersion, serializedContent);
                }
            }

            return result;
        }

        public bool IsSameModel(TreeElementClipboardData clipboardData)
        {
            ArgumentValidator.EnsureArgumentNotNull(clipboardData, "clipboardData");

            string rootModelVElementKey = TreeViewService.RootModel.ElementKey;
            return clipboardData.RootModelElementKey == rootModelVElementKey;
        }

        public bool IsSameShellInstance(TreeElementClipboardData clipboardData)
        {
            ArgumentValidator.EnsureArgumentNotNull(clipboardData, "clipboardData");

            ShellViewModel shellViewModel = ShellViewModel;
            Guid shellInstanceId = shellViewModel.ShellInstanceId;
            Version shellVersion = AppContext.AppCurrentVersion;
            string rootModelVElementKey = TreeViewService.RootModel.ElementKey;

            return clipboardData.ShellInstanceId.EqualsTo(shellInstanceId)
                && clipboardData.RootModelElementKey == rootModelVElementKey
                && clipboardData.ShellVersion.Equals(shellVersion);
        }

        public void Clear()
        {
            if (HasValidClipboardData(out _))
            {
                Clipboard.Clear();
            }
        }
    }
}
