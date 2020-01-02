using System.Collections.Generic;

namespace TKOM.Structures.AST
{
    public class HtmlInlineTag : IInstruction
    {
        public HtmlInlineTag()
        {
            Attributes = new List<(string attributeName, StringValue attributeValue)>();
        }
        public string TagName { set; get; }
        public List<(string attributeName, StringValue attributeValue)> Attributes { set; get; }
    }
}