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
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.App.Services.Interfaces;
using LHQ.App.UndoUnits;
using LHQ.App.ViewModels;
using LHQ.App.ViewModels.Elements;
using LHQ.Core.Interfaces;
using LHQ.Core.Model;
using LHQ.Core.Model.Importer;
using LHQ.Data;
using LHQ.Data.Extensions;
using LHQ.Data.Interfaces;
using LHQ.Data.Interfaces.Key;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

namespace LHQ.App.Services.Implementation
{
    public partial class ResourceImportService
    {
        private class ImporterAsyncOperation : AsyncOperation
        {
            private readonly IShellViewContext _shellViewContext;
            private readonly IResourceImporter _importer;
            private readonly ResourceImportMergeMode _importMergeMode;
            private readonly bool _allowImportNewLanguages;
            private readonly string[] _files;
            private ResourceImportResult _importResult;
            private ICategoryLikeElement targetCategoryForImport;

            public ImporterAsyncOperation(IShellViewContext shellViewContext, IResourceImporter importer, 
                ResourceImportMergeMode importMergeMode, bool allowImportNewLanguages, 
                string[] files) : base(shellViewContext.AppContext)
            {
                _shellViewContext = shellViewContext;
                _importer = importer ?? throw new ArgumentNullException(nameof(importer));
                _importMergeMode = importMergeMode;
                _allowImportNewLanguages = allowImportNewLanguages;
                _files = files;
            }

            private ShellViewModel ShellViewModel => _shellViewContext.ShellViewModel;

            protected override TimeSpan GetOperationTimeout() => TimeSpan.FromMinutes(3);

            public override string ProgressMessage => Strings.Services.Import.ImportResourcesInProgress;

            public override bool ShowCancelButton { get; } = true;

            protected override async Task InternalStart()
            {
                _importResult = await Import(_importMergeMode, _importer, _files, CancellationToken);
                Result.AssignFrom(_importResult);
            }

            protected override Task InternalCompleted(bool userCancelled)
            {
                IDialogService dialogService = AppContext.DialogService;

                if (userCancelled)
                {
                    var dialogShowInfo = new DialogShowInfo(Strings.Services.Import.ImportResourcesCaption, 
                        Strings.Services.Import.ImportWasCancelledByUser);
                    
                    dialogService.ShowError(dialogShowInfo);
                }
                else
                {
                    if (_importResult.IsSuccess)
                    {
                        string message = _importResult.Message.IsNullOrEmpty()
                            ? GetImportSuccessMessage()
                            : _importResult.Message;

                        string detail = Strings.Services.Import.ImportSucceedDetail(
                            _importResult.ImportedNewResources,
                            _importResult.ImportedNewCategories,
                            _importResult.ImportedUpdateResources,
                            _importResult.ImportedUpdateCategories,
                            _importResult.ImportedNewLanguages);

                        var dialogShowInfo = new DialogShowInfo(Strings.Services.Import.ImportResourcesCaption, message, detail);
                        dialogService.ShowInfo(dialogShowInfo);
                    }
                    else
                    {
                        string message = _importResult.Message.IsNullOrEmpty()
                            ? Strings.Services.Import.ImportingResourcesFailed
                            : _importResult.Message;

                        dialogService.ShowError(new DialogShowInfo(Strings.Services.Import.ImportResourcesCaption, message));
                    }
                }

                return Task.CompletedTask;
            }

            private async Task<ResourceImportResult> Import(ResourceImportMergeMode importMergeMode,
                IResourceImporter importer, string[] fileNames, CancellationToken cancellationToken)
            {
                ResourceImportResult importResult = null;

                // TODO: IProgress ...
                try
                {
                    importResult = await importer.Import(fileNames, cancellationToken);
                }
                catch (Exception e)
                {
                    if (importResult == null)
                    {
                        importResult = new ResourceImportResult();
                    }

                    DebugUtils.Error(Strings.Services.Import.ImportingResourcesFailed, e);
                    importResult.SetError(Strings.Services.Import.ImportingResourcesFailed);
                }
                finally
                {
                    if (importResult == null)
                    {
                        importResult = new ResourceImportResult();
                    }

                    ModelContext modelContextToImport = importResult.ModelContext;
                    var modelToImport = modelContextToImport.Model;
                    List<string> languages = modelToImport.Languages
                        .Select(x => x.Name)
                        .ToList();

                    int topResourcesCount = modelToImport.Resources.Count;
                    int topCategoriesCount = modelToImport.Categories.Count;

                    if (languages.Count == 0 || (topResourcesCount == 0 && topCategoriesCount == 0))
                    {
                        importResult.SetError(Strings.Services.Import.FilesDoesNotContainResourcesToImport);
                    }
                    else
                    {
                        if (modelToImport.Categories.Count > 0 && !ShellViewModel.ModelOptions.Categories)
                        {
                            importResult.SetError(Strings.Services.Import.ImportingCategoriesIsNotAllowedMessage,
                                Strings.Services.Import.ImportingCategoriesIsNotAllowedDetail);
                        }
                        else
                        {
                            //ShellViewModel shellViewModel = AppContext.ShellViewModel;
                            IModelFileStorage modelFileStorage = AppContext.ModelFileStorage;
                            RootModelViewModel rootModel = ShellViewModel.RootModel;
                            ModelContext modelContext = modelFileStorage.Clone(ShellViewModel.ModelContext, true);

                            List<string> missingLanguages = ShellViewModel.RootModel.GetMissingLanguages(languages);
                            if (missingLanguages.Count > 0)
                            {
                                rootModel.AddMissingLanguages(missingLanguages);
                            }

                            // int importedNewCategories = 0;
                            // int importedUpdateCategories = 0;
                            // int importedNewResources = 0;
                            // int importedUpdateResources = 0;
                            // int importedNewLanguages = 0;

                            // TODO Testing if it is ok to not call 'BlockAllNotifications'
                            //using (AppContext.PropertyChangeObserver.BlockAllNotifications())
                            //ICategoryLikeElement targetCategoryForImport = null;
                            targetCategoryForImport = null;
                            bool importAsNew = importMergeMode == ResourceImportMergeMode.ImportAsNew;
                            IModelElementKey importAsNewCategoryKey = null;
                            if (importAsNew)
                            {
                                var importAsNewCategoryName = Strings.Services.Import.ImportedFolderName(DateTimeHelper.UtcNow.Ticks);
                                importAsNewCategoryName = NameHelper.GenerateUniqueCategoryName(importAsNewCategoryName, rootModel);
                                CategoryElement mainCategoryHolder = modelContext.AddCategory(importAsNewCategoryName);

                                importAsNewCategoryKey = mainCategoryHolder.Key;
                                targetCategoryForImport = mainCategoryHolder;
                                importResult.ImportedNewCategories++;
                            }
                            else
                            {
                                targetCategoryForImport = modelContext.Model;
                            }

                            if (_allowImportNewLanguages)
                            {
                                foreach (var langToImport in modelToImport.Languages)
                                {
                                    if (!modelContext.ContainsLanguage(langToImport.Name))
                                    {
                                        modelContext.AddLanguage(langToImport);
                                        importResult.ImportedNewLanguages++;
                                    }
                                }
                            }

                            var primaryLanguage = modelContext.GetPrimaryLanguage();

                            // import
                            RecursiveImport(modelToImport, targetCategoryForImport);

                            // primary language
                            primaryLanguage = modelContext.GetPrimaryLanguage();
                            if (primaryLanguage == null)
                            {
                                LanguageElement enLanguage =
                                    modelContext.Model.Languages.FindByName("en", true, CultureInfo.InvariantCulture);
                                primaryLanguage = enLanguage ?? modelContext.Model.Languages.First();
                                modelContext.SetPrimaryLanguage(primaryLanguage.Name);
                            }

                            if (importAsNew)
                            {
                                /*var mainCategoryViewModel = rootModel.AddChildCategory(targetCategoryForImport.Name) as ITreeElementViewModel;
                                rootModel.ListViewNode.Refresh(rootModel.Children);

                                mainCategoryViewModel.AddChildsElements(targetCategoryForImport.Categories);
                                mainCategoryViewModel.AddChildsElements(targetCategoryForImport.Resources);*/
                            }

                            /*rootModel.ShellViewModel.ChangeModelContext(modelContext);
                            rootModel.ShellViewModel.ProjectIsDirty = true;*/

                            void ImportResources(ResourceElementList resources, ICategoryLikeElement target)
                            {
                                foreach (ResourceElement sourceResource in resources)
                                {
                                    ResourceElement targetResource = target.Resources.FindByName(sourceResource.Name, true, CultureInfo.InvariantCulture);
                                    bool targetResourceExist = targetResource != null;
                                    bool updatedResource = false;

                                    if (targetResourceExist)
                                    {
                                        if (importer.ResourceHasDescription && sourceResource.Description != targetResource.Description)
                                        {
                                            targetResource.Description = sourceResource.Description;
                                            importResult.ImportedUpdateResources++;
                                            updatedResource = true;
                                        }
                                    }
                                    else
                                    //else if (importAsNew)
                                    {
                                        targetResource = modelContext.AddResource(sourceResource.Name, target);
                                        if (importer.ResourceHasDescription)
                                        {
                                            targetResource.Description = sourceResource.Description;
                                        }
                                        importResult.ImportedNewResources++;
                                        updatedResource = true;
                                        targetResourceExist = true;
                                    }

                                    if (targetResource == null)
                                    {
                                        continue;
                                    }

                                    // iterate resource values
                                    foreach (var sourceResourceValue in sourceResource.Values)
                                    {
                                        string languageName = sourceResourceValue.LanguageName;
                                        //var targetResourceValue = targetResourceExist ? targetResource.FindValueByLanguage(languageName) : null;
                                        var targetResourceValue = targetResource.FindValueByLanguage(languageName);

                                        if (targetResourceValue == null)
                                        {
                                            var targetModelHasLanguage = modelContext.ContainsLanguage(languageName);

                                            if (targetModelHasLanguage || _allowImportNewLanguages)
                                            {
                                                targetResourceValue = modelContext.AddResourceValue(languageName, sourceResourceValue.Value, targetResource);
                                                targetResourceValue.Assign(sourceResourceValue);
                                                if (!updatedResource)
                                                {
                                                    importResult.ImportedUpdateResources++;
                                                    updatedResource = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var isPrimaryLanguage = primaryLanguage != null && targetResourceValue.LanguageName == primaryLanguage.Name;

                                            if (!isPrimaryLanguage || importAsNew)
                                            {
                                                var assignChanged = targetResourceValue.Assign(sourceResourceValue);
                                                if (assignChanged && !updatedResource)
                                                {
                                                    importResult.ImportedUpdateResources++;
                                                    updatedResource = true;
                                                }
                                            }
                                        }
                                    }

                                    // iterate resource parameters
                                    foreach (ResourceParameterElement sourceParameter in sourceResource.Parameters)
                                    {
                                        var targetParameter = targetResourceExist
                                            ? targetResource.Parameters.FindByName(sourceParameter.Name, true, CultureInfo.InvariantCulture)
                                            : null;

                                        if (targetParameter == null)
                                        {
                                            targetParameter = modelContext.AddResourceParameter(sourceParameter.Name, targetResource);
                                            targetParameter.Assign(sourceParameter);
                                            if (!updatedResource)
                                            {
                                                importResult.ImportedUpdateResources++;
                                                updatedResource = true;
                                            }
                                        }
                                        else
                                        {
                                            var assignChanged = targetParameter.Assign(sourceParameter);
                                            if (assignChanged && !updatedResource)
                                            {
                                                importResult.ImportedUpdateResources++;
                                                updatedResource = true;
                                            }
                                        }
                                    }
                                }
                            }

                            void RecursiveImport(ICategoryLikeElement source, ICategoryLikeElement target)
                            {
                                // iterate source categories
                                foreach (CategoryElement sourceCategory in source.Categories)
                                {
                                    // if target categories does not contain source category, add it
                                    var targetCategory = target.Categories.FindByName(sourceCategory.Name, true, CultureInfo.InvariantCulture);
                                    //if (targetCategory == null && importAsNew)
                                    bool categoryAdded = false;
                                    if (targetCategory == null)
                                    {
                                        targetCategory = modelContext.AddCategory(sourceCategory.Name, target);
                                        categoryAdded = true;
                                        importResult.ImportedNewCategories++;
                                    }

                                    if (targetCategory != null)
                                    {
                                        if (importer.CategoryHasDescription && targetCategory.Description != sourceCategory.Description)
                                        {
                                            targetCategory.Description = sourceCategory.Description;

                                            if (!categoryAdded)
                                            {
                                                importResult.ImportedUpdateCategories++;
                                            }
                                        }
                                        

                                        // if source category has some child categories or child resources, do recursive import on that source category
                                        if (sourceCategory.Categories.Count > 0 || sourceCategory.Resources.Count > 0)
                                        {
                                            RecursiveImport(sourceCategory, targetCategory);
                                        }
                                    }
                                }

                                // iterate source resources
                                ImportResources(source.Resources, target);
                            }

                            if (importResult.ImportedNewResources == 0 && importResult.ImportedUpdateResources == 0)
                            {
                                importResult.SetError(Strings.Services.Import.FilesDoesNotContainResourcesToImport);
                            }
                            else
                            {
                                _shellViewContext.TransactionManager.Execute(new ImportResourcesUndoUnit(_shellViewContext, modelContext, importAsNewCategoryKey));

                                importResult.SetSuccess(GetImportSuccessMessage());
                            }
                        }
                    }
                }

                return importResult;
            }

            private string GetImportSuccessMessage()
            {
                string underParentPath = string.Empty;
                if (_importMergeMode == ResourceImportMergeMode.ImportAsNew)
                {
                    underParentPath = "\n\n" + Strings.Services.Import.ResourcesWasImportedUnderElement(GetTargetCategoryFullPath());
                }

                return Strings.Services.Import.ImportSucceedMessage(underParentPath);
            }

            private string GetTargetCategoryFullPath()
            {
                List<string> parentNames = new List<string>();
                bool hasNoParent = false;
                if (targetCategoryForImport is CategoryElement categoryForImport)
                {
                    hasNoParent = categoryForImport.Parent == null;
                    parentNames = categoryForImport.GetParentNames(true, true);
                }

                if (targetCategoryForImport is Data.Model || hasNoParent)
                {
                    parentNames.Insert(0, ShellViewModel.RootModel.Name);
                }

                var elementPaths = Ellipsis.Compact(string.Join(@"/", parentNames), 40, EllipsisFormat.Start);
                return elementPaths.StartsWith("/") ? elementPaths : "/" + elementPaths;
            }
        }
    }
}
