using System;
using System.Collections.Generic;
using System.Reflection;
using LHQ.Data;
using LHQ.Data.Extensions;
using LHQ.Data.ModelStorage;
using LHQ.Data.Templating.Templates;
using LHQ.Generators.TemplateGenerators;

namespace LHQ.Generators
{
    public static class TemplateGeneratorFactory
    {
        private static readonly Dictionary<string, Type> Generators = new Dictionary<string, Type>();
        
        static TemplateGeneratorFactory()
        {
            Register<CSharpNetCoreTemplateGenerator>();
        }

        private static void Register<T>()
        {
            var type = typeof(T);
            var generatorIdAttribute = type.GetCustomAttribute<TemplateGeneratorIdAttribute>();
            if (generatorIdAttribute == null)
            {
                throw new InvalidOperationException($"Generator '{type.Name}' does not have attribute '{nameof(TemplateGeneratorIdAttribute)}'!");
            }

            if (!Generators.TryAdd(generatorIdAttribute.Id, type))
            {
                throw new InvalidOperationException($"Generator with id '{generatorIdAttribute.Id}' already registered!");
            }
        }

        internal static ModelContext LoadModel(string modelFileName)
        {
            return LoadModel(modelFileName, out _, out _);
        }

        internal static ModelContext LoadModel(string modelFileName, out CodeGeneratorTemplate codeGeneratorTemplate, out Type generatorType)
        {
            if (string.IsNullOrEmpty(modelFileName))
            {
                throw new ApplicationException("Strings file name is empty!");
            }

            var modelContext = new ModelContext(ModelContextOptions.Default);
            var loadResult = ModelSerializerManager.DeserializeFrom(modelFileName, modelContext);

            if (loadResult.Status != ModelLoadStatus.Success)
            {
                throw new ApplicationException("Error loading strings file, " + loadResult.Status);
            }

            codeGeneratorTemplate = modelContext.GetCodeGeneratorTemplate(null);
            if (codeGeneratorTemplate == null ||
                !Generators.TryGetValue(codeGeneratorTemplate.Id, out generatorType))
            {
                throw new ApplicationException("Could not find code generator template!");
            }
            
            return modelContext;
        }
        
        public static void Generate(string modelFileName)
        {
            using (var modelContext = LoadModel(modelFileName, out var codeGeneratorTemplate, out var generatorType))
            {
                //using (var generator = (TemplateGenerator<CodeGeneratorTemplate>)Activator.CreateInstance(generatorType, modelContext, modelFileName))
                using (var generator = (TemplateGenerator)Activator.CreateInstance(generatorType, modelContext, modelFileName))
                {
                    generator.Settings = codeGeneratorTemplate;
                    generator.Generate();
                }
            }
        }
    }
}