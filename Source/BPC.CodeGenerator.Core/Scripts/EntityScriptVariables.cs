using BPC.CodeGenerator.Metadata.Generation;
using BPC.CodeGenerator.Options;

namespace BPC.CodeGenerator.Scripts
{
    public class EntityScriptVariables : ScriptVariablesBase
    {
        public EntityScriptVariables(Entity entity, GeneratorOptions generatorOptions, TemplateOptions templateOptions)
            : base(generatorOptions, templateOptions)
        {
            Entity = entity;
        }

        public Entity Entity { get; }
    }
}