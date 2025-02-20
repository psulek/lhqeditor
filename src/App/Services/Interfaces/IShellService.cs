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
        /// <param name="hostEnvironmentSave">Flag to true to enforce save via host environment (VS) and not directly to file.
        /// Default is false to save directly to file. But in some situations there will be <c>true</c> to use for eg. VS to trigger save on envdte document object
        /// which will in turn call associated Source Control (if any) to checkout file if is readonly (TFS server for example, not GIT).
        /// </param>
        /// <returns></returns>
        bool SaveProject(string fileName = null, bool hostEnvironmentSave = false);
        
        void NewProject(string modelName, string primaryLanguage, ModelOptions modelOptions, 
            CodeGeneratorTemplate codeGeneratorTemplate);
        

        void StartProjectOperationIsBusy(ProjectBusyOperationType type);
        void StopProjectOperationIsBusy();
        Task StandaloneCodeGenerate();
    }
}