using BPC.CodeGenerator.Metadata.Generation;
using BPC.CodeGenerator.Options;

namespace BPC.CodeGenerator.Scripts
{
    public class ModelScriptVariables : ScriptVariablesBase
    {
        public ModelScriptVariables(Model model, GeneratorOptions generatorOptions, TemplateOptions templateOptions)
            : base(generatorOptions, templateOptions)
        {
            Model = model;
        }

        public Model Model { get; }
    }
}