using BPC.CodeGenerator.Metadata.Generation;
using BPC.CodeGenerator.Options;

namespace BPC.CodeGenerator.Scripts
{
    public class ContextScriptVariables : ScriptVariablesBase
    {
        public ContextScriptVariables(EntityContext entityContext, GeneratorOptions generatorOptions, TemplateOptions templateOptions)
            : base(generatorOptions, templateOptions)
        {
            EntityContext = entityContext;
        }

        public EntityContext EntityContext { get; }
    }
}