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

using System.Windows.Input;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Services.Interfaces;
using LHQ.Data;
using LHQ.Data.CodeGenerator;
using LHQ.Data.Templating;

namespace LHQ.App.ViewModels.Dialogs
{
    public class ProjectSettingsDialogViewModel : DialogViewModelBase
    {
        public ProjectSettingsDialogViewModel(IShellViewContext shellViewContext)
            : base(shellViewContext.AppContext)
        {
            var shellViewModel = shellViewContext.ShellViewModel;
            //var modelOptions = shellViewModel.ModelOptions.Clone();
            bool canChangeGeneratorTemplate = !shellViewModel.HasCodeGeneratorItemTemplate;
            string templateId = shellViewModel.CodeGeneratorItemTemplate;
            
            var metadata = shellViewModel.ModelContext.GetMetadata<CodeGeneratorMetadata>(CodeGeneratorMetadataDescriptor.UID);
            var template = metadata.Template ??
                (string.IsNullOrEmpty(templateId) ? null : CodeGeneratorTemplateManager.Instance.CreateTemplate(templateId));
            
            ProjectSettings = new ProjectSettingsViewModel(shellViewContext);
            ProjectSettings.CanChangeGeneratorTemplate = canChangeGeneratorTemplate;
            ProjectSettings.SetTemplate(template);
            //ProjectSettings.SelectTemplateById(templateId);
            UpgradeModelCommand = new DelegateCommand(UpgradeModelCommandExecute);
            UpdrageModelRequested = false;
        }

        public ProjectSettingsViewModel ProjectSettings { get; set; }

        public ICommand ShowHelpCommand { get; }
        
        public ICommand UpgradeModelCommand { get; }

        public bool UpdrageModelVisible { get; set; }
        public bool UpdrageModelRequested { get; set; }

        protected override string GetTitle()
        {
            return Strings.ViewModels.ProjectSettings.PageTitle;
        }

        public override bool SubmitDialog()
        {
            return ProjectSettings.Validate();
        }

        public void Save(ModelOptions modelOptions)
        {
            ProjectSettings.Save(modelOptions);
        }

        private void ShowHelpExecute(object obj)
        {
            WebPageUtils.ShowDoc(WebPageDocSection.ProjectSettings);
        }
        
        private void UpgradeModelCommandExecute(object obj)
        {
            UpdrageModelRequested = true;
            CloseDialogCommand.Execute(null);
        }
    }
}
