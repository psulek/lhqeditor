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

using LHQ.App.Services.Interfaces;
using LHQ.App.ViewModels.Dialogs;
using LHQ.Data;
using LHQ.Data.Templating.Templates;
using LHQ.Utils.Extensions;

namespace LHQ.App.Dialogs
{
    /// <summary>
    ///     Interaction logic for NewProjectView.xaml
    /// </summary>
    public partial class NewProjectDialog
    {
        public NewProjectDialog()
        {
            InitializeComponent();
        }

        public static Result DialogShow(IShellViewContext shellViewContext, string modelName, NewProjectDialogViewModel.VsExtraInfo vsExtraInfo = null)
        {
            string primaryLanguage = null;
            var openLanguageSettings = false;
            ModelOptions modelOptions = null;
            CodeGeneratorTemplate codeGeneratorTemplate = null;

            using (var viewModel = new NewProjectDialogViewModel(shellViewContext))
            {
                viewModel.SetVsExtraInfo(vsExtraInfo);
                viewModel.ModelName = modelName;
                
                bool resourcesUnderRootVisible = true;
                if (shellViewContext.AppContext.RunInVsPackage && viewModel.Template != null)
                {
                    resourcesUnderRootVisible = !viewModel.Template.ModelFeatures.IsFlagSet(ModelFeatures.HideResourcesUnderRoot);
                }

                viewModel.ProjectSettings.ResourcesUnderRootVisible = resourcesUnderRootVisible;
                bool? dialogResult = DialogShow<NewProjectDialogViewModel, NewProjectDialog>(viewModel);
                bool submitted = dialogResult == true;
                if (submitted)
                {
                    modelName = viewModel.ModelName;
                    primaryLanguage = viewModel.LanguageSelector.SelectedLanguage.CultureName;
                    openLanguageSettings = viewModel.OpenLanguageSettings;
                    codeGeneratorTemplate = viewModel.Template;
                    modelOptions = new ModelOptions();
                    viewModel.ProjectSettings.Save(modelOptions);
                }

                return new Result
                {
                    Submitted = submitted,
                    ModelName = modelName,
                    PrimaryLanguage = primaryLanguage,
                    OpenLanguageSettings = openLanguageSettings,
                    ModelOptions = modelOptions,
                    Template = codeGeneratorTemplate
                };
            }
        }

        public class Result
        {
            public bool Submitted { get; set; }

            public string ModelName { get; set; }

            public string PrimaryLanguage { get; set; }

            public CodeGeneratorTemplate Template { get; set; }

            public bool OpenLanguageSettings { get; set; }

            public ModelOptions ModelOptions { get; set; }
        }
    }
}
