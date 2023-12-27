using System.Collections.Generic;
using System.Diagnostics;

namespace BPC.CodeGenerator.Metadata.Parsing
{
    [DebuggerDisplay("This: {ThisPropertyName}, Other: {OtherPropertyName}")]
    public class ParsedRelationship
    {
        public ParsedRelationship()
        {
            ThisProperties = new List<string>();
        }

        public string ThisPropertyName { get; set; }
        public List<string> ThisProperties { get; private set; }

        public string OtherPropertyName { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ThisPropertyName)
                && !string.IsNullOrEmpty(OtherPropertyName)
                && ThisProperties.Count > 0;
        }

    }
}