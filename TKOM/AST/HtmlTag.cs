using System.Collections.Generic;

namespace TKOM.AST
{
    public class HtmlTag : IInstruction
    {
        public string TagName {set; get;}
        public List<(string attributeName, string attributeValue)> Attributes {set; get;}
        public List<IInstruction> Instructions {set; get;}
    }
}