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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using Microsoft.VisualStudio.TemplateWizard;
// ReSharper disable ClassNeverInstantiated.Global

namespace LHQ.VsExtension.Wizard
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class MultiProjectWizard : ProjectWizardBase<MultiProjectWizardData>
    {
        private string _destination;
        private string _templateDir;
        private string _projectName;
        private const string TemplateFileName = "MyTemplate.vstemplate";
        private string _templateFileFullPath;
        private readonly string[] _filesToRemove = { "__PreviewImage.png", "__TemplateIcon.png"} ;//, TemplateFileName };

        private static readonly Dictionary<string, Dictionary<string, string>> _globalReplacementsDict = new Dictionary<string, Dictionary<string, string>>();

        internal static void ApplyGlobalReplacementsDictionary(string rootTemplateId, Dictionary<string, string> targetDictionary)
        {
            if (_globalReplacementsDict.ContainsKey(rootTemplateId))
            {
                targetDictionary.Clear();
                foreach (var entry in _globalReplacementsDict[rootTemplateId])
                {
                    targetDictionary.Add(entry.Key, entry.Value);
                }
            }
        }

        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);
            if (_rootTemplateId.IsNullOrEmpty())
            {
                throw new InvalidOperationException("Missing value for 'root template id'");
            }

            _destination = replacementsDictionary["$destinationdirectory$"];
            _projectName = replacementsDictionary["$safeprojectname$"];
            _templateDir = Path.GetDirectoryName(customParams[0] as string);

            _globalReplacementsDict[_rootTemplateId] = JsonUtils.CloneObject(replacementsDictionary);
        }

        protected override bool AllowInstallNugetPackages { get; } = false;

        public override void ProjectFinishedGenerating(Project project)
        {
            try
            {
                // copy all files (recursive) from template (eg VS2015) folder into target new project folder and then add project from template (vstemplate) file.
                string currentVsTemplatePath = Path.Combine(_templateDir, $"VS{_vsProductVersion}") + "\\";
                if (!Directory.Exists(currentVsTemplatePath) && !_wizardData.DefaultTemplateFolder.IsNullOrEmpty())
                {
                    currentVsTemplatePath = Path.Combine(_templateDir, _wizardData.DefaultTemplateFolder) + "\\";
                }

                if (Directory.Exists(currentVsTemplatePath))
                {
                    string[] filesToCopy = Directory.GetFiles(currentVsTemplatePath, "*.*", SearchOption.AllDirectories);
                    foreach (string file in filesToCopy)
                    {
                        string newFile = file.Substring(currentVsTemplatePath.Length, file.Length - currentVsTemplatePath.Length);
                        if (!_filesToRemove.Contains(newFile))
                        {
                            newFile = Path.Combine(_destination, newFile);
                            string newFileDir = Path.GetDirectoryName(newFile);
                            if (!string.IsNullOrEmpty(newFileDir) && !Directory.Exists(newFileDir))
                            {
                                Directory.CreateDirectory(newFileDir);
                            }

                            File.Copy(file, newFile);
                        }
                    }

                    string newFolderTemplateFile = Path.Combine(_destination, TemplateFileName);
                    _dte.Solution.AddFromTemplate(newFolderTemplateFile, _destination, _projectName);
                }

                _templateFileFullPath = Path.Combine(_destination, TemplateFileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new InvalidOperationException("Error creating project from LHQ project template!");
            }
        }

        public override void RunFinished()
        {
            if (File.Exists(_templateFileFullPath))
            {
                ProjectItem projectItem = _dte.Solution?.FindProjectItem(_templateFileFullPath);
            
                if (projectItem != null)
                {
                    Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(delegate
                        {
                            try
                            {
                                projectItem.Delete();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }

                            try
                            {
                                if (File.Exists(_templateFileFullPath))
                                {
                                    File.Delete(_templateFileFullPath);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        });
                }
            }
        }
    }
}