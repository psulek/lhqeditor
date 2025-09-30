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

using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels;
using LHQ.App.ViewModels.Dialogs;
using LHQ.Data;
using LHQ.Data.Templating.Templates;
using LHQ.Utils.Utilities;

namespace LHQ.App.Dialogs
{
    /// <summary>
    ///     Interaction logic for ProjectSettingsDialog.xaml
    /// </summary>
    public partial class ProjectSettingsDialog
    {
        public ProjectSettingsDialog()
        {
            InitializeComponent();
        }

        public static Result DialogShow(IShellViewContext shellViewContext)
        {
            //ArgumentValidator.EnsureArgumentNotNull(modelOptions, "modelOptions");

            var shellViewModel = shellViewContext.ShellViewModel;
            var modelOptions = shellViewModel.ModelOptions.Clone();
            // bool canChangeGeneratorTemplate = !shellViewModel.HasCodeGeneratorItemTemplate;
            // string templateId = shellViewModel.CodeGeneratorItemTemplate;

            
            using (var viewModel = new ProjectSettingsDialogViewModel(shellViewContext))
            {
                var selectedModelVersion = shellViewModel.ModelContext.Model.Version;
                viewModel.UpdrageModelVisible = selectedModelVersion < ModelConstants.CurrentModelVersion;
                bool? dialogResult = DialogShow<ProjectSettingsDialogViewModel, ProjectSettingsDialog>(viewModel);

                var isUpgradeRequested = viewModel.UpdrageModelRequested;
                
                //isUpgradeRequested = viewModel.ProjectSettings.IsUpgradeRequested;
                bool submitted = dialogResult == true && !isUpgradeRequested;
                if (submitted)
                {
                    viewModel.Save(modelOptions);
                }
                
                return new Result
                {
                    Submitted = submitted,
                    ModelOptions = modelOptions,
                    IsUpgradeRequested = isUpgradeRequested,
                    CodeGeneratorTemplate = viewModel.ProjectSettings.Template
                };
            }
        }
        
        public class Result
        {
            public bool Submitted { get; set; }

            public bool IsUpgradeRequested { get; set; }

            public CodeGeneratorTemplate CodeGeneratorTemplate { get; set; }

            public ModelOptions ModelOptions { get; set; }
        }
    }
}
