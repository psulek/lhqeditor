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
using System.Threading.Tasks;
using LHQ.App.Code;
using LHQ.App.Model;
using LHQ.Core.Model;
using LHQ.Data;
using LHQ.Data.Interfaces.Key;
using LHQ.Data.Templating.Templates;

namespace LHQ.App.Services.Interfaces
{
    public interface IShellService : IShellViewContextService, IDisposable
    {
        void NotifyShellLoaded();

        bool CheckProjectIsDirty();

        ModelContext CreateModelContext(string modelName);
        OperationResult SaveModelContextToFile(string fileName, ModelContext modelContext);

        void TranslateResources(IModelElementKey[] resourceKeys,
            SuggestTranslationMode translationMode, string onlyLanguage);

        void OpenProject(string fileName);
        void CloseProject();

        event EventHandler<ShellContextEventArgs> OnShellViewEvent;

        /// <summary>
        /// Save project changes (if any) in current ShellView instance.
        /// </summary>
        /// <param name="fileName">File name of file to save. Optional, if empty than actual file name associated with ShellView is used.</param>
        /// <param name="flags">Save flags</param>
        /// <returns></returns>
        bool SaveProject(string fileName = null, SaveProjectFlags flags = SaveProjectFlags.None);
        
        void NewProject(string modelName, string primaryLanguage, ModelOptions modelOptions, 
            CodeGeneratorTemplate codeGeneratorTemplate);
        

        void StartProjectOperationIsBusy(ProjectBusyOperationType type);
        void StopProjectOperationIsBusy();
        // Task StandaloneCodeGenerate();
        void StandaloneCodeGenerate();
        
        void UpgradeModelToLatest();
    }
}