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
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.Core.DependencyInjection;
using LHQ.Data;
using LHQ.Data.Extensions;
using LHQ.Data.Templating.Templates;
using LHQ.Gen.Lib;
using LHQ.Utils.Utilities;
using Exception = System.Exception;

namespace LHQ.App.Services.Implementation
{
    public class StandaloneCodeGeneratorService : AppContextServiceBase, IDisposable, IStandaloneCodeGeneratorService
    // public class StandaloneCodeGeneratorService : ShellViewContextServiceBase,  IDisposable, IStandaloneCodeGeneratorService
    {
        private readonly ThreadSafeSingleShotGuard _generating = ThreadSafeSingleShotGuard.Create(false);
        private Generator _generator;
        private bool _disposed;

        public override void ConfigureDependencies(IServiceContainer serviceContainer)
        { }

        public virtual async Task<StandaloneCodeGenerateResult> GenerateCodeAsync(string modelFileName, ModelContext modelContext)
        {
            var result = new StandaloneCodeGenerateResult();

            if (!_generating.TrySetSignal())
            {
                // report success because we are already generating and dont want to show error generating...
                return new StandaloneCodeGenerateResult { Success = true };
            }

            try
            {
                if (_generator == null)
                {
                    _generator = new Generator(GetLoggerForGenerator());
                }
                else
                {
                    _generator.UpdateLogger(GetLoggerForGenerator());
                }

                Logger.Info($"[GenerateCode] {modelFileName}");

                var outDir = Path.GetDirectoryName(modelFileName);
                var generateResult = _generator.Generate(modelFileName, null, outDir);
                var generatedFiles = generateResult.GeneratedFiles;

                foreach (var file in generatedFiles)
                {
                    var fileName = Path.Combine(outDir, file.FileName);
                    var dir = Path.GetDirectoryName(fileName);
                    if (dir != null && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    await file.WriteToDiskAsync(outDir);
                }

                result.Success = true;
                result.GeneratedFileCount = generatedFiles.Count;
            }
            catch (GeneratorException ge)
            {
                var message = $"Error generating code from LHQ file '{modelFileName}', {ge.Title}, {ge.Message}";
                Logger.Error(message, ge);
                
                result.Error = string.IsNullOrEmpty(ge.Title) ? ge.Message : ge.Title;
                result.ErrorKind = ge.Kind;
                result.ErrorCode = ge.Code;

                /*AppContext.UIService.DispatchActionOnUI(() =>
                    {
                        string title = "Code generator failed: " + ge.Title;
                        string message = ge.Message;

                        if (ge.Kind == GeneratorErrorKind.InvalidModelSchema)
                        {
                            title = "Code generator failed.";
                            message = "Invalid model schema. " + (ge.Title ?? ge.Message);
                        }
                        else if (ge.Kind == GeneratorErrorKind.TemplateValidationError)
                        {
                            title = "Code generator failed.";
                            message = "Invalid template settings. " + (ge.Title ?? ge.Message);
                        }
                        var dialogShowInfo = new DialogShowInfo(Strings.Dialogs.CodeGenerator.CodeGeneratorTitle, title, message);
                        DialogService.ShowError(dialogShowInfo);

                        if (ge.Code == GeneratorErrorCodes.CsharpNamespaceMissing)
                        {
                            AutodetectNamespace(modelFileName, modelContext);
                        }

                    }, TimeSpan.FromMilliseconds(200));*/
            }
            catch (Exception e)
            {
                var message = $"Error generating code from LHQ file '{modelFileName}'";
                Logger.Error(message, e);
            }
            finally
            {
                _generating.ResetSignal();
            }

            return result;
        }

        public string AutodetectNamespace(string modelFileName)
        {
            return _generator.AutodetectNamespace(modelFileName);
        }

        protected virtual NLog.ILogger GetLoggerForGenerator()
        {
            return Logger.GetSourceLogger() as NLog.ILogger;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _generator?.Dispose();
            }
        }
    }
}
