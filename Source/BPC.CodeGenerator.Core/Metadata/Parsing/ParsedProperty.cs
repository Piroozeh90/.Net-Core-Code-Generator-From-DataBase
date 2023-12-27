using System.Diagnostics;

namespace BPC.CodeGenerator.Metadata.Parsing
{
    [DebuggerDisplay("Column: {ColumnName}, Property: {PropertyName}")]
    public class ParsedProperty
    {
        public string ColumnName { get; set; }
        public string PropertyName { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ColumnName)
                && !string.IsNullOrEmpty(PropertyName);
        }
    }
}