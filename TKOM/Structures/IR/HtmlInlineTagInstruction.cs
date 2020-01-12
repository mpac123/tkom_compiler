using System.Collections.Generic;
using System.IO;
using TKOM.Structures.AST;

namespace TKOM.Structures.IR
{
    public class HtmlInlineTagInstruction : Instruction
    {
        public HtmlInlineTagInstruction(ScopePrototype scopePrototype, HtmlInlineTag htmlInlineTag) : base(scopePrototype)
        {
            HtmlInlineTag = htmlInlineTag;
        }

        public HtmlInlineTag HtmlInlineTag { private set; get; }


        public override void Execute(Node node)
        {
            node.NewLine = true;
            base.Execute(node);
            node.StreamWriter.Write($"<{HtmlInlineTag.TagName}");
            foreach (var attribute in HtmlInlineTag.Attributes)
            {
                node.StreamWriter.Write($" {attribute.attributeName}=\"{attribute.attributeValue}\"");
            }
            node.StreamWriter.Write($"/>");
        }

    }
}