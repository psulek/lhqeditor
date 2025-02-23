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
using LHQ.App.Extensions;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Core.DependencyInjection;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation
{
    public class AppConfigFactory : AppContextServiceBase, IAppConfigFactory
    {
        public AppConfig Current { get; private set; }

        private IAppConfigStorage _storage;

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        {
            _storage = serviceContainer.Get<IAppConfigStorage>();
        }

        public void Set(AppConfig appConfig)
        {
            Current = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            Save();
        }

        public virtual AppConfig CreateDefault()
        {
            var newConfig = new AppConfig
            {
                UILanguage = CultureCache.English.Name,
                OpenLastProjectOnStartup = true,
                DefaultProjectLanguage = CultureCache.English.Name,
                CheckUpdatesOnAppStart = true,
                EnableTranslation = false,
                LockTranslationsWithResource = null,
                RunTemplateAfterSave = false
            };
            newConfig.UpdateVersion(AppContext.AppCurrentVersion);
            return newConfig;
        }

        public void Initialize()
        {
            try
            {
                Current = Load();
                bool requireSave = Current == null;
                if (requireSave)
                {
                    Current = CreateDefault();
                }

                if (Current.Version.IsNullOrEmpty())
                {
                    Current.UpdateVersion(AppContext.AppCurrentVersion);
                }

                if (!requireSave)
                {
                    requireSave = UpgradeIfRequired();
                }

                if (requireSave)
                {
                    Save();
                }

                Loaded(Current);
            }
            catch (Exception e)
            {
                DebugUtils.Error("AppConfigFactory.InitializeAsync, " + e.GetFullExceptionDetails());
            }
        }

        protected virtual void Loaded(AppConfig appConfig)
        { }

        public virtual void Save()
        {
            _storage.Save(Current);
        }

        private AppConfig Load()
        {
            return _storage.Load();
        }

        protected virtual bool UpgradeIfRequired()
        {
            var requireSave = false;

            Version loadedConfigVersion = Current.GetVersion();
            Version appCurrentVersion = AppContext.AppCurrentVersion;

            if (appCurrentVersion.CompareTo(loadedConfigVersion) > 0)
            {
                requireSave = true;
                Current.UpdateVersion(appCurrentVersion);

                //TODO: Add some upgrade rules later, when required!
                // loadedConfig.NewProperty = "defaultValue";
            }

            // UILanguage
            bool validUILanguage = !Current.UILanguage.IsNullOrEmpty() && CultureCache.Instance.ExistCulture(Current.UILanguage);
            if (!validUILanguage)
            {
                requireSave = true;
                Current.UILanguage = CultureCache.English.Name;
            }

            // DefaultProjectLanguage
            bool validDefaultProjectLanguage = !Current.DefaultProjectLanguage.IsNullOrEmpty() &&
                CultureCache.Instance.ExistCulture(Current.DefaultProjectLanguage);
            if (!validDefaultProjectLanguage)
            {
                requireSave = true;
                Current.DefaultProjectLanguage = CultureCache.English.Name;
            }

            return requireSave;
        }
    }
}
