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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using EnvDTE;
using LHQ.App.Dialogs;
using LHQ.App.Services.Implementation;
using LHQ.App.ViewModels.Dialogs;
using LHQ.Utils.Utilities;
using LHQ.Data;
using LHQ.Data.CodeGenerator;
using LHQ.Data.ModelStorage;
using LHQ.Utils.Extensions;
using Microsoft.VisualStudio.TemplateWizard;
// ReSharper disable UnusedType.Global

namespace LHQ.VsExtension.Wizard
{
    public class ItemWizard : WizardBase<ItemWizardData>
    {
        private const string DataSafeItemName = "$safeitemname$";
        private const string DataProjectName = "$projectname$";
        private const string DataItemtemplate = "$lhq_itemtemplate$";
        private const string ExtVsTemplate = ".vstemplate";

        private static NewProjectDialog.Result _dialogResult;

        protected override bool WizardDataIsRequired { get; } = false;

        protected override bool AllowInstallNugetPackages { get; } = true;

        protected override WizardType WizardType { get; } = WizardType.Item;

        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind,
            object[] customParams)
        {
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

            (string templateName, string templateDesc, string templateId) = GetVsTemplateInfo(replacementsDictionary, customParams);

            var modelName = replacementsDictionary.ContainsKey(DataSafeItemName) ? replacementsDictionary[DataSafeItemName] : "Default";

            string projectName = GetProjectName(_dte);
            replacementsDictionary[DataProjectName] = projectName;

            ShowDialog(modelName, projectName, templateName, templateDesc, templateId);

            if (_dialogResult.Submitted)
            {
                var modelContext = new ModelContext(ModelContextOptions.Default);
                modelContext.CreateModel(_dialogResult.ModelName);
                modelContext.AddLanguage(CultureCache.Instance.GetCulture(_dialogResult.PrimaryLanguage), true);
                modelContext.Model.Options.AssignFrom(_dialogResult.ModelOptions);

                var codeGeneratorMetadata = modelContext.GetMetadata<CodeGeneratorMetadata>(CodeGeneratorMetadataDescriptor.UID);
                codeGeneratorMetadata.Template = _dialogResult.Template;
                codeGeneratorMetadata.TemplateId = templateId;

                var modelFileStorage = new ModelFileStorage();
                modelFileStorage.Initialize();
                string fileContent = modelFileStorage.Save(modelContext, ModelSaveOptions.Indent);
                replacementsDictionary["$lhq_model$"] = fileContent;
            }
        }

        private (string templateName, string templateDesc, string templateId) GetVsTemplateInfo(
            Dictionary<string, string> replacementsDictionary, object[] customParams)
        {
            string templateName = null;
            string templateDesc = null;
            string templateId = null;

            if (replacementsDictionary.ContainsKey(DataItemtemplate))
            {
                templateId = replacementsDictionary[DataItemtemplate];
            }

            if (customParams != null && customParams.Length > 0)
            {
                string fileName = customParams[0] as string;
                if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName) && Path.GetExtension(fileName) == ExtVsTemplate)
                {
                    XDocument xml = XDocument.Parse(File.ReadAllText(fileName));

                    if (xml.Root?.Name.LocalName == "VSTemplate" &&
                        xml.Root.Attribute("xmlns")?.Value == "http://schemas.microsoft.com/developer/vstemplate/2005")
                    {
                        XElement templateData = xml.Root.Elements().SingleOrDefault(x=>x.Name.LocalName == "TemplateData");
                        if (templateData != null)
                        {
                            foreach (var element in templateData.Elements())
                            {
                                if (element.Name.LocalName == "Name")
                                {
                                    templateName = element.Value;
                                }
                                else if (element.Name.LocalName == "Description")
                                {
                                    templateDesc = element.Value;
                                }
                            }
                        }
                    }
                }
            }

            return (templateName, templateDesc, templateId);
        }

        private static void ShowDialog(string modelName, string projectName, string templateName, 
            string templateDescription, string templateId)
        {
            VsPackageService.ShowIsolatedAppWindow(shellViewModel =>
                {
                    shellViewModel.CodeGeneratorItemTemplate = templateId;
                    var extraInfo = new NewProjectDialogViewModel.VsExtraInfo(projectName, templateName, templateDescription, templateId);
                    _dialogResult = NewProjectDialog.DialogShow(shellViewModel.ShellViewContext, modelName, extraInfo);
                });
        }

        public override void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            base.ProjectItemFinishedGenerating(projectItem);

            if (AllowInstallNugetPackages)
            {
                InstallNugetPackages(projectItem.ContainingProject, true);
            }

            short fileCount = projectItem.FileCount;
            for (short i = 0; i < fileCount; i++)
            {
                string fileName = projectItem.FileNames[i];
                if (fileName.EndsWith(".lhq.tt", StringComparison.OrdinalIgnoreCase))
                {
                    string dependentUpon = fileName.Substring(0, fileName.Length - 3);
                    try
                    {
                        projectItem.Properties.Item("DependentUpon").Value = Path.GetFileName(dependentUpon);
                    }
                    catch (ArgumentException)
                    {
                        // (E_INVALIDARG)
                        DebugUtils.Log("ItemWizard->ProjectItemFinishedGenerating() failed to set DependentUpon [warning]");
                    }
                }
            }
        }

        public override bool ShouldAddProjectItem(string filePath)
        {
            return _dialogResult.Submitted;
        }

        public override void BeforeOpeningFile(ProjectItem projectItem)
        {
            System.Runtime.Remoting.Messaging.CallContext.LogicalSetData("CreatedFromItemWizard", true);
        }

        public override void RunFinished()
        {
            System.Runtime.Remoting.Messaging.CallContext.LogicalSetData("CreatedFromItemWizard", false);
        }
    }
}
