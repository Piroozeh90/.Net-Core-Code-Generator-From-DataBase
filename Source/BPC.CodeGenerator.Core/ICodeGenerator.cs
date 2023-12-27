using BPC.CodeGenerator.Options;

namespace BPC.CodeGenerator
{
    public interface ICodeGenerator
    {
        bool Generate(GeneratorOptions options);
    }
}