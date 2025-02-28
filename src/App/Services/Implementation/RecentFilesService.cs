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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Core.DependencyInjection;
using LHQ.Utils;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public sealed class RecentFilesService : AppContextServiceBase, IRecentFilesService, IHasInitialize
    {
        private const int MaxRecentItems = 15;
        private const string StorageDirectory = "";
        private const string StorageFileName = "recentfiles.dat";
        private IsolatedStorageManager _isolatedStorage;

        public bool Supported { get; } = true;

        public ObservableCollectionExt<RecentFileInfo> RecentItems { get; private set; }

        public string LastOpenedFileName
        {
            get
            {
                RecentFileInfo lastOpenedFileInfo = GetCurrentItem();
                return lastOpenedFileInfo?.FileName;
            }
            private set
            {
                if (RecentItems != null)
                {
                    foreach (RecentFileInfo recentFileInfo in RecentItems)
                    {
                        recentFileInfo.IsLastOpened = recentFileInfo.FileName.EqualsTo(value);
                    }
                }
            }
        }

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        void IHasInitialize.Initialize()
        {
            _isolatedStorage = new IsolatedStorageManager();
            LastOpenedFileName = null;
            Load();
        }

        private void Load()
        {
            RecentItems = new ObservableCollectionExt<RecentFileInfo>();

            var requireSave = false;
            var content = _isolatedStorage.Load<RecentFileInfo[]>(StorageDirectory, StorageFileName, false);
            if (content != null)
            {
                int beforeCount = content.Length;
                RecentFileInfo[] loadedItems = content.Where(x => !x.FileName.IsNullOrEmpty()).Take(MaxRecentItems).ToArray();
                requireSave = loadedItems.Length != beforeCount;

                if (loadedItems.Length > 0)
                {
                    RecentItems.AddRange(loadedItems);
                }
            }

            if (requireSave)
            {
                Save(null);
            }
        }

        public void Save(IShellViewContext shellViewContext)
        {
            UpdateExpandedKeys(shellViewContext);

            RecentFileInfo[] recentFileInfos = RecentItems.Take(MaxRecentItems).ToArray();

            if (!_isolatedStorage.Save(StorageDirectory, StorageFileName, recentFileInfos, false))
            {
                DebugUtils.Error("Error saving recent files to isolated storage!");
            }
        }

        private void UpdateExpandedKeys(IShellViewContext shellViewContext)
        {
            RecentFileInfo recentFileInfo = GetCurrentItem();
            string projectFileName = shellViewContext?.ShellViewModel.ProjectFileName;

            if (recentFileInfo == null && !projectFileName.IsNullOrEmpty())
            {
                recentFileInfo = GetRecentInfo(projectFileName);
            }

            if (recentFileInfo != null && (projectFileName.IsNullOrEmpty() || recentFileInfo.FileName.EqualsTo(projectFileName)))
            {
                string[] expandedKeys = shellViewContext?.TreeViewService.GetTreeExpandedKeys();
                if (expandedKeys != null)
                {
                    recentFileInfo.ExpandedKeys = new List<string>(expandedKeys);
                }
            }
        }

        public List<string> GetExpandedKeys(string fileName)
        {
            RecentFileInfo item = RecentItems?.SingleOrDefault(x => x.FileName.EqualsTo(fileName));
            return item?.ExpandedKeys;
        }

        private RecentFileInfo GetRecentInfo(string fileName)
        {
            return RecentItems?.SingleOrDefault(x => x.FileName.EqualsTo(fileName));
        }

        private RecentFileInfo GetCurrentItem()
        {
            return RecentItems?.SingleOrDefault(x => x.IsLastOpened);
        }

        private void Add(string fileName)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(fileName, "fileName");

            if (RecentItems.All(x => !x.FileName.EqualsTo(fileName)))
            {
                var recentFileInfo = new RecentFileInfo(fileName);

                if (RecentItems.Count == 0)
                {
                    RecentItems.Add(recentFileInfo);
                }
                else
                {
                    RecentItems.Insert(0, recentFileInfo);
                }

                while (RecentItems.Count > MaxRecentItems)
                {
                    RecentItems.RemoveAt(RecentItems.Count - 1);
                }
            }
        }

        public void Use(string fileName)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(fileName, "fileName");

            var existingItem = RecentItems.Select((info, i) => new
            {
                item = info,
                index = i
            }).SingleOrDefault(x => x.item.FileName.EqualsTo(fileName));

            if (existingItem != null)
            {
                if (existingItem.index > 0)
                {
                    RecentItems.RemoveAt(existingItem.index);
                    Add(fileName);
                }
            }
            else
            {
                Add(fileName);
            }

            LastOpenedFileName = fileName;
        }

        public void Remove(string fileName)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(fileName, "fileName");

            var existingItem = RecentItems.Select((info, i) => new
            {
                item = info,
                index = i
            }).SingleOrDefault(x => x.item.FileName.EqualsTo(fileName));

            if (existingItem != null)
            {
                if (LastOpenedFileName == existingItem.item.FileName)
                {
                    LastOpenedFileName = null;
                }

                RecentItems.RemoveAt(existingItem.index);
            }
        }

        public void ResetLastOpenedFile()
        {
            LastOpenedFileName = null;
        }

        public void Clear()
        {
            RecentItems.Clear();
            ResetLastOpenedFile();
        }
    }
}
