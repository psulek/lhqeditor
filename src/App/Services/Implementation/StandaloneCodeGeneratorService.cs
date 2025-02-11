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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Core.DependencyInjection;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation
{
    public class StandaloneCodeGeneratorService: AppContextServiceBase, IStandaloneCodeGeneratorService
    {
        private string _generatorFilePath;

        public bool Available { get; private set; }

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        {}

        public void Initialize()
        {
            AppContext.OnAppEvent += AppContextOnOnAppEvent;
        }

        private void AppContextOnOnAppEvent(object sender, ApplicationEventArgs e)
        {
            if (e is ApplicationEventAppInitialized)
            {
                _generatorFilePath = Path.Combine(AppContext.AppFolder, "lhqcmd.exe");
                CheckAvailable();
            }
        }

        private bool CheckAvailable()
        {
            Available = File.Exists(_generatorFilePath);
            return Available;
        }

        public async Task GenerateCodeAsync(string modelFileName)
        {
            if (!CheckAvailable())
            {
                return;
            }
            
            var startInfo = new ProcessStartInfo(_generatorFilePath)
            {
                Arguments = $"\"{modelFileName}\"",
                Verb = "runas",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            var timeout = TimeSpan.FromSeconds(60);
            using (var cancellationTokenSource = new CancellationTokenSource(timeout))
            {
                await ProcessUtils.RunAsync(startInfo, cancellationTokenSource.Token);
            }
        }
    }
}
