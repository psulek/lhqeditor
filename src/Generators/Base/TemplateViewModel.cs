using System;
using System.Collections.Generic;
using System.Linq;
using LHQ.Data;
using LHQ.Data.Extensions;
using LHQ.Data.Templating.Settings;

namespace LHQ.Generators
{
    public abstract class TemplateViewModel
    {
        private ModelContext ModelContext { get; }

        public TemplateViewModel(ModelContext modelContext, GeneratorSettingsBase settings)
        {
            ModelContext = modelContext;
            Settings = settings;
            
            ModelName = ModelRoot.Name;
            ModelClassName = ModelName;

            KeysClassName = ModelName + "Keys";
            StringLocalizerClassName = ModelName + "Localizer";
            
            AvailableCultures = string.Join(",", ModelRoot.Languages.Select(x => "\"" + x.Name + "\""));
        }

        protected Dictionary<ResourceElement, ResourceInfo> AllResources { get; private set; }

        protected ResourceElementList RootResources { get; private set; }

        protected string AvailableCultures { get; }

        protected string ModelClassName { get; }

        protected string ModelName { get; }

        protected string StringLocalizerClassName { get; }

        protected string KeysClassName { get; }

        protected Model ModelRoot => ModelContext.Model;

        protected LanguageElement PrimaryLanguage => ModelContext.GetPrimaryLanguage();
        
        protected GeneratorSettingsBase Settings { get; }

        public abstract string Render();

        protected virtual string GetCommentForResourceProperty(ResourceElement resource)
        {
            var primaryResourceValue = resource.FindValueByLanguage(PrimaryLanguage.Name);
            string propertyComment = primaryResourceValue != null && !string.IsNullOrEmpty(primaryResourceValue.Value)
                ? primaryResourceValue.Value
                : resource.Description;

            if (propertyComment == null)
            {
                propertyComment = string.Empty;
            }

            bool trimmed = false;
            var idxNewLine = propertyComment.IndexOf(Environment.NewLine);

            if (idxNewLine == -1)
            {
                idxNewLine = propertyComment.IndexOf('\n');
            }

            if (idxNewLine == -1)
            {
                idxNewLine = propertyComment.IndexOf('\r');
            }

            if (idxNewLine > -1)
            {
                propertyComment = propertyComment.Substring(0, idxNewLine);
                trimmed = true;
            }

            if (propertyComment.Length > 80)
            {
                propertyComment = propertyComment.Substring(0, 80);
                trimmed = true;
            }

            if (trimmed)
            {
                propertyComment += "...";
            }

            propertyComment = propertyComment.Replace('\t', ' ');

            return propertyComment;
        }

        protected void FlattenAllResources()
        {
            RootResources = new ResourceElementList(ModelRoot.Resources.Where(x => x.ParentKey == null).ToList());
            AllResources = RootResources.ToDictionary(x => x, x => new ResourceInfo(x));
            IterateCategories(ModelRoot.Categories, string.Empty, string.Empty, RootResources);
        }

        private void IterateCategories(CategoryElementList categories, string parentsPath, string parentsPathSep,
            ResourceElementList parentResources)
        {
            foreach (var category in categories.OrderByName())
            {
                var newparentsPath = parentsPath + category.Name;
                var newparentsPathSep = string.IsNullOrEmpty(parentsPathSep)
                    ? category.Name
                    : parentsPathSep + "." + category.Name;

                if (parentResources.ContainsByName(category.Name, true))
                {
                    throw new ApplicationException(string.Format(
                        "Could not generate C# code! Resource and category name '{0}' could not be same for parent '{1}'!",
                        category.Name, parentsPathSep ?? ""));
                }

                if (category.Categories.Count > 0)
                {
                    IterateCategories(category.Categories, newparentsPath, newparentsPathSep, category.Resources);
                }

                foreach (var resource in category.Resources.OrderByName())
                {
                    var resourceInfo = new ResourceInfo(resource);
                    resourceInfo.ParentsPath = newparentsPath;
                    resourceInfo.ParentsPathWithSep = newparentsPathSep;
                    AllResources.Add(resource, resourceInfo);
                }
            }
        }

        #region ResourceInfo

        public class ResourceInfo
        {
            public ResourceElement Resource { get; set; }
            public string ParentsPath { get; set; }
            public string ParentsPathWithSep { get; set; }

            public ResourceInfo(ResourceElement resource)
            {
                Resource = resource;
            }

            public string GetResourceKey(bool withSeparator)
            {
                if (withSeparator)
                {
                    return string.IsNullOrEmpty(ParentsPathWithSep)
                        ? Resource.Name
                        : ParentsPathWithSep + "." + Resource.Name;
                }

                return string.IsNullOrEmpty(ParentsPath) ? Resource.Name : ParentsPath + Resource.Name;
            }
        }

        #endregion
    }
}