using System.Collections.Generic;

namespace TKOM.AST
{
    public class HtmlTag : HtmlInlineTag
    {
        public List<IInstruction> Instructions {set; get;}
    }
}