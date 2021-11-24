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
using System.Diagnostics.CodeAnalysis;
using LHQ.Utils.Extensions;
using Microsoft.VisualStudio.TemplateWizard;
// ReSharper disable UnusedType.Global

namespace LHQ.VsExtension.Wizard
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class ProjectWizard : ProjectWizardBase<ProjectWizardData>
    {
        protected override bool AllowInstallNugetPackages { get; } = true;

        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

            // if root template id is not null, means this project wizard is for child template ...
            if (!_rootTemplateId.IsNullOrEmpty())
            {
                MultiProjectWizard.ApplyGlobalReplacementsDictionary(_rootTemplateId, replacementsDictionary);
            }
        }
    }
}