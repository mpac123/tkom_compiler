using System.Collections.Generic;

namespace TKOM.Structures.AST
{
    public class HtmlTag : HtmlInlineTag
    {
        public HtmlTag() {
            Instructions = new List<IInstruction>();
        }
        public List<IInstruction> Instructions {set; get;}
    }
}