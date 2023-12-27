using System.Diagnostics;

namespace BPC.CodeGenerator.Metadata.Parsing
{
    [DebuggerDisplay("Entity: {EntityClass}, Property: {ContextProperty}")]
    public class ParsedEntitySet
    {
        public string EntityClass { get; set; }
        public string ContextProperty { get; set; }
    }
}