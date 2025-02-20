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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using LHQ.App.Localization;
using LHQ.App.Services.Interfaces;
using LHQ.Data.CodeGenerator;
using LHQ.Data.Templating;
using LHQ.Data.Templating.Templates;
using LHQ.Utils;
using LHQ.Utils.Extensions;
using Syncfusion.Windows.Utils;

namespace LHQ.App.ViewModels.Dialogs
{
    public class CodeGeneratorDialogViewModel : DialogViewModelBase
    {
        private CodeGeneratorTemplate _settings;
        private readonly int _modelVersion;

        public CodeGeneratorDialogViewModel(IShellViewContext shellViewContext, string templateId, 
            CodeGeneratorTemplate template = null) : base(shellViewContext.AppContext)
        {
            var shellViewModel = shellViewContext.ShellViewModel;
            ResetToDefaultCommand = new DelegateCommand(ResetToDefaultExecute);

            _modelVersion = shellViewModel.ModelContext.Model.Version;

            if (!templateId.IsNullOrEmpty())
            {
                if (template == null)
                {
                    var metadata = shellViewModel.ModelContext.GetMetadata<CodeGeneratorMetadata>(CodeGeneratorMetadataDescriptor.UID);
                    Settings = metadata.Template ?? CodeGeneratorTemplateManager.Instance.CreateTemplate(templateId);
                }
                else
                {
                    Settings = template;
                }

                var propertiesToHide = new List<string>();

                if (Settings != null)
                {
                    TemplateId = Settings.Id;
                    TemplateName = Settings.Name;

                    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(Settings);
                    foreach (PropertyDescriptor propertyDescriptor in properties)
                    {
                        var versionDependAttribute = propertyDescriptor.Attributes.OfType<ModelVersionDependAttribute>().FirstOrDefault();
                        if (versionDependAttribute != null)
                        {
                            if (_modelVersion < versionDependAttribute.MinModelVersion)
                            {
                                propertiesToHide.Add(propertyDescriptor.Name);
                            }
                        }
                    }
                }

                PropertiesToHide = new ObservableCollectionExt<string>(propertiesToHide);
            }
        }

        public ICommand ResetToDefaultCommand { get; }

        public ObservableCollection<string> PropertiesToHide { get; set; }

        public CodeGeneratorTemplate Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public string TemplateId { get; }

        public string TemplateName { get; }

        protected override string GetTitle() => Strings.Dialogs.CodeGenerator.Title;

        private void ResetToDefaultExecute(object obj)
        {
            string caption = Strings.Dialogs.CodeGenerator.ResetConfirmCaption;
            string message = Strings.Dialogs.CodeGenerator.ResetConfirmMessage;
            if (DialogService.ShowConfirm(caption, message, null) == Model.DialogResult.Yes)
            {
                Settings = CodeGeneratorTemplateManager.Instance.CreateTemplate(TemplateId);
            }
        }

        public override bool SubmitDialog()
        {
            var error = Settings.Validate();
            bool valid = error == null;

            if (!valid)
            {
                if (error.IsWarning)
                {
                    if (error.IsConfirmation)
                    {
                        valid = DialogService.ShowConfirm(Title, error.Message, error.Detail) == Model.DialogResult.Yes;
                    }
                    else
                    {
                        DialogService.ShowWarning(Title, error.Message, error.Detail);
                    }
                }
                else
                {
                    DialogService.ShowError(Title, error.Message, error.Detail);
                }
            }

            return valid;
        }
    }
}
