using BPC.CodeGenerator.Metadata.Generation;
using BPC.CodeGenerator.Options;
using Microsoft.Extensions.Logging;

namespace BPC.CodeGenerator.Scripts
{
    public class ModelScriptTemplate : ScriptTemplateBase<ModelScriptVariables>
    {
        private Model _model;

        public ModelScriptTemplate(ILoggerFactory loggerFactory, GeneratorOptions generatorOptions, TemplateOptions templateOptions)
            : base(loggerFactory, generatorOptions, templateOptions)
        {
        }

        public void RunScript(Model model)
        {
            _model = model;

            WriteCode();
        }

        protected override ModelScriptVariables CreateVariables()
        {
            return new ModelScriptVariables(_model, GeneratorOptions, TemplateOptions);
        }
    }
}