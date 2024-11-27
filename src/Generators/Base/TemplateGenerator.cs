using System;
using System.IO;
using LHQ.Data;
using LHQ.Data.Templating.Settings;
using LHQ.Data.Templating.Templates;
using LHQ.Utils.Utilities;

namespace LHQ.Generators
{
    //public abstract class TemplateGenerator<TSettings>: DisposableObject where TSettings : CodeGeneratorTemplate
    public abstract class TemplateGenerator: DisposableObject
    {
        protected readonly ModelContext ModelContext;
        protected readonly string ModelFileName;

        protected TemplateGenerator(ModelContext modelContext, string modelFileName)
        {
            ModelContext = modelContext;
            ModelFileName = modelFileName;
            TargetFolder = Path.GetDirectoryName(modelFileName);
        }
        
        protected string TargetFolder { get; }
        
        //public TSettings Settings { get; set; }
        
        public CodeGeneratorTemplate Settings { get; set; }

        public abstract void Generate();

        protected override void DoDispose()
        {
            ModelContext?.Dispose();
        }
        
        internal static string RenderToString<T>(ModelContext modelContext, GeneratorSettingsBase generatorSettings) where T : TemplateViewModel
        {
            ArgumentNullException.ThrowIfNull(modelContext);
            ArgumentNullException.ThrowIfNull(generatorSettings);
            
            if (!generatorSettings.Enabled)
            {
                return null;
            }
            
            var viewModel = (T)Activator.CreateInstance(typeof(T), modelContext, generatorSettings);

            return viewModel!.Render();
        }

        protected void RenderToFile<T>(GeneratorSettingsBase generatorSettings, string fileName) where T : TemplateViewModel
        {
            string content = RenderToString<T>(ModelContext, generatorSettings);
            if (content == null)
            {
                return;
            }

            var folder = string.IsNullOrEmpty(generatorSettings.OutputFolder)
                ? TargetFolder
                : Path.Combine(TargetFolder, generatorSettings.OutputFolder);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            fileName = Path.Combine(folder, fileName);
            File.WriteAllText(fileName, content);
        }
    }
}