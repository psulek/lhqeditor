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

        //public static bool DialogShow(IShellViewContext shellViewContext, ModelOptions modelOptions, out int newModelVersion)
        public static bool DialogShow(IShellViewContext shellViewContext, ModelOptions modelOptions, out bool isUpgradeRequested)
        {
            ArgumentValidator.EnsureArgumentNotNull(modelOptions, "modelOptions");

            using (var viewModel = new ProjectSettingsDialogViewModel(shellViewContext))
            {
                var selectedModelVersion = shellViewContext.ShellViewModel.ModelContext.Model.Version;
                viewModel.UpdrageModelVisible = selectedModelVersion < ModelConstants.CurrentModelVersion;
                bool? dialogResult = DialogShow<ProjectSettingsDialogViewModel, ProjectSettingsDialog>(viewModel);

                isUpgradeRequested = viewModel.UpdrageModelRequested;
                
                //isUpgradeRequested = viewModel.ProjectSettings.IsUpgradeRequested;
                bool result = dialogResult == true && !isUpgradeRequested;
                if (result)
                {
                    viewModel.Save(modelOptions);
                }
                return result;
            }
        }
    }
}
