using LHQ.Data;
using LHQ.Data.Templating.Templates.NetCore;
using LHQ.Generators.NetCore;

namespace LHQ.Generators.TemplateGenerators
{
    [TemplateGeneratorId("NetCoreResxCsharp01")]
    //public class CSharpNetCoreTemplateGenerator : TemplateGenerator<NetCoreResxCsharp01Template>
    public class CSharpNetCoreTemplateGenerator : TemplateGenerator
    {
        public CSharpNetCoreTemplateGenerator(ModelContext modelContext, string modelFileName) : base(modelContext, modelFileName)
        {}

        public override void Generate()
        {
            var settings = (NetCoreResxCsharp01Template)Settings;
            var modelName = ModelContext.Model.Name;
            
            RenderToFile<CSharpTemplateViewModel>(settings.CSharp, modelName + ".gen.cs");
        }
    }
}