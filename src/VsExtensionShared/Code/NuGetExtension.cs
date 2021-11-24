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
using System.IO;
using System.Linq;
using EnvDTE;
using LHQ.Utils.Extensions;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;
using Task = System.Threading.Tasks.Task;

// ReSharper disable UnusedMember.Global

namespace LHQ.VsExtension.Code
{
    public static class NuGetExtension
    {
        public static void RestorePackages(this Project project)
        {
            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));

            var installer = componentModel.GetService<IVsPackageInstaller>();
            var uninstaller = componentModel.GetService<IVsPackageUninstaller>();
            var installerServices = componentModel.GetService<IVsPackageInstallerServices>();

            var installedPackages = installerServices.GetInstalledPackages().ToList();

            installedPackages.ForEach(p => uninstaller.UninstallPackage(project, p.Id, false));
            installedPackages.ForEach(p => installer.InstallPackage(null, project, p.Id, p.VersionString, true));
        }

        public static async Task InstallPackage(this Project project, string packageId, string version, bool openReadmeTxt)
        {
            try
            {
                var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
                IVsPackageInstallerServices installerServices = componentModel.GetService<IVsPackageInstallerServices>();

                bool isPackageInstalled = installerServices.IsPackageInstalled(project, packageId);

                // if package is installed (usually when new project wizard) check if project template nuget version is not lower then required package version
                if (isPackageInstalled)
                {
                    var packageMetadata = installerServices.GetInstalledPackages().SingleOrDefault(x => x.Id == packageId);
                    if (packageMetadata != null && version != packageMetadata.VersionString)
                    {
                        isPackageInstalled = false;

                        // do not open readme when package with old version is already installed in project!
                        openReadmeTxt = false;
                    }
                }

                if (!isPackageInstalled)
                {
                    var installer = componentModel.GetService<IVsPackageInstaller>();
                    DebugUtils.Log($"Installing nuget package '{packageId}'@{version}...");
                    installer.InstallPackage(null, project, packageId, version, false);

                    if (openReadmeTxt)
                    {
                        TimeSpan timeout = TimeSpan.FromSeconds(10);

                        await Task.Run(async () =>
                            {
                                var startTime = DateTime.Now;
                                IVsPackageMetadata packageMetadata = null;
                                while ((DateTime.Now - startTime) < timeout && packageMetadata == null)
                                {
                                    try
                                    {
                                        packageMetadata = installerServices.GetInstalledPackages().SingleOrDefault(x => x.Id == packageId);

                                        if (packageMetadata == null)
                                        {
                                            await Task.Delay(TimeSpan.FromSeconds(1));
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                }

                                if (packageMetadata != null && Directory.Exists(packageMetadata.InstallPath))
                                {
                                    var readmeFile = Path.Combine(packageMetadata.InstallPath, "readme.txt");
                                    if (File.Exists(readmeFile))
                                    {
                                        VsPackageService.DTE.ItemOperations.Navigate(readmeFile, vsNavigateOptions.vsNavigateOptionsNewWindow);
                                    }
                                }
                            });
                    }
                }
                else
                {
                    DebugUtils.Log($"Installing nuget package '{packageId}' skipped because its already installed.");
                }
            }
            catch (Exception e)
            {
                bool containPackageId = e.Message.IndexOf(packageId, StringComparison.InvariantCultureIgnoreCase) > -1;
                bool containVersion = string.IsNullOrEmpty(version) || e.Message.IndexOf(version, StringComparison.InvariantCultureIgnoreCase) > -1;

                if (containPackageId && containVersion)
                {
                    string versionStr = string.IsNullOrEmpty(version) ? string.Empty : $" version {version}";
                    string error = $"Failed to install nuget package '{packageId}'{versionStr}.\n" +
                        $"Please manually install NuGet package '{packageId}'{versionStr} in Nuget Package Manager.";
                    
                    VsPackageService.AddMessageToOutput(error, OutputMessageType.Error);
                }
            }
        }
    }
}