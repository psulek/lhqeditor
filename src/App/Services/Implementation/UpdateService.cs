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
using System.IO;
using System.Threading.Tasks;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Core.DependencyInjection;
using NuGet.Versioning;
using Velopack;
using Velopack.Locators;
using Velopack.Sources;

namespace LHQ.App.Services.Implementation
{
    public class UpdateService : AppContextServiceBase, IUpdateService
    {
        private UpdateManager _updateManager;
        private DateTime _lastUpdateCheckTime = DateTime.MinValue;
        private UpdateCheckInfo _lastUpdateCheckInfo = null;

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        {}

        public virtual async Task<UpdateCheckInfo> CheckForUpdate()
        {
            var updateInfo = await CheckForUpdateInternal();
            if (updateInfo != null)
            {
                return new UpdateCheckInfo(ToVersion(updateInfo.TargetFullRelease.Version),
                    Array.Empty<string>(), Array.Empty<string>());
            }

            return null;
        }
        
        private async Task<UpdateInfo> CheckForUpdateInternal()
        {
            var useLocal = true;
// #if DEGUG
//             useLocal = true;
// #endif
            IUpdateSource updateSource = null;

            if (useLocal)
            {
                var path = "C:\\dev\\github\\psulek\\lhqeditor\\src\\App\\Releases\\";
                var localDir = new DirectoryInfo(path);
                if (localDir.Exists)
                {
                    updateSource = new SimpleFileSource(localDir);
                }
            }
            else
            {
                // NO: Do check only if Velopack version of App is installed (not runned from portable or from IDE)
                if (VelopackLocator.Current.CurrentlyInstalledVersion != null)
                {
                    updateSource = new GithubSource("https://github.com/psulek/lhqeditor", null, false);
                }
            }

            if (updateSource != null)
            {
                _updateManager = new UpdateManager(updateSource);
                return await _updateManager.CheckForUpdatesAsync();
                
            }

            return null;
        }

        public virtual bool InplaceUpgradeSupported()
        {
            return true;
        }

        public async Task<bool> UpdateToNewVersion()
        {
            var newVersion = await CheckForUpdateInternal();
            if (newVersion == null)
            {
                return false;
            }
            
            // download new version
            await _updateManager.DownloadUpdatesAsync(newVersion);

            // install new version and restart app
            _updateManager.ApplyUpdatesAndRestart(newVersion);
            
            return true;
        }

        private Version ToVersion(SemanticVersion semanticVersion)
        {
            return new Version(semanticVersion.Major, semanticVersion.Minor, semanticVersion.Patch);
        }
    }
}