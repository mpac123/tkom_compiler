using System.Collections.Generic;

namespace TKOM.Structures.AST
{
    public class HtmlInlineTag : IInstruction
    {
        public string TagName {set; get;}
        public List<(string attributeName, string attributeValue)> Attributes {set; get;}
    }
}