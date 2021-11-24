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
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Core.DependencyInjection;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation
{
    public abstract class StorageServiceBase<TStorage> : AppContextServiceBase, IStorageService<TStorage>, IHasInitialize
        where TStorage : IStorage, new()
    {
        private string _storageDirectory;
        private readonly IsolatedStorageManager _isolatedStorage;

        private static TStorage _cache;
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object _sync = new object();

        protected StorageServiceBase()
        {
            _isolatedStorage = new IsolatedStorageManager();
        }

        public TStorage Current => GetOrLoad();

        protected abstract string StorageFileName { get; }

        private TStorage Cache
        {
            get => _cache;
            set => _cache = value;
        }

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        {
            _storageDirectory = AppContext.RunInVsPackage ? "VS" : "APP";
        }

        public virtual void Initialize()
        {
            GetOrLoad(true);
        }

        private TStorage GetOrLoad(bool initPhase = false)
        {
            string header = $"{nameof(GetOrLoad)}(initPhase: {initPhase})";

            if (Cache == null)
            {
                TStorage content = default;
                try
                {
                    content = _isolatedStorage.Load<TStorage>(_storageDirectory, StorageFileName, true);
                }
                catch (Exception e)
                {
                    DebugUtils.Error($"{header} load storage data failed", e);
                }

                lock (_sync)
                {
                    if (Cache == null)
                    {
                        try
                        {
                            bool save = content == null &&
                                initPhase; // only allow save on init phase to not fall into 'deadlock circle save/getorload bug'

                            if (save)
                            {
                                content = new TStorage();
                            }

                            Cache = content;

                            if (save)
                            {
                                Save();
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            }

            return Cache;
        }

        public void Update(Action<TStorage> updatePredicate)
        {
            if (updatePredicate == null)
            {
                throw new ArgumentNullException(nameof(updatePredicate));
            }

            lock (_sync)
            {
                TStorage current = Current;
                updatePredicate(current);

                Save();
            }
        }


        private void Save()
        {
            if (Cache != null)
            {
                _isolatedStorage.Save(_storageDirectory, StorageFileName, Cache, true);
            }
        }

        private void InternalReset()
        {
            lock (_sync)
            {
                Cache = new TStorage();
                Save();
            }
        }

#if DEBUG
        public void Reset()
        {
            InternalReset();
        }
#endif
    }
}