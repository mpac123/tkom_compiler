using System.Collections.Generic;

namespace TKOM.Structures.AST
{
    public class HtmlTag : HtmlInlineTag
    {
        public List<IInstruction> Instructions {set; get;}
    }
}