using System.Collections.Generic;
using System.IO;
using TKOM.Structures.AST;

namespace TKOM.Structures.IR
{
    public class HtmlInlineTagInstruction : Instruction
    {
        public HtmlInlineTagInstruction(Scope scope, HtmlInlineTag htmlInlineTag) : base(scope)
        {
            HtmlInlineTag = htmlInlineTag;
        }

        public HtmlInlineTag HtmlInlineTag { private set; get; }


        public override void Execute(StreamWriter streamWriter, Dictionary<string, Block> functions, int nestedLevel, bool newLine)
        {
            base.Execute(streamWriter, functions, nestedLevel, newLine);
            streamWriter.Write($"<{HtmlInlineTag.TagName}");
            foreach (var attribute in HtmlInlineTag.Attributes)
            {
                streamWriter.Write($" {attribute.attributeName}=\"{attribute.attributeValue}\"");
            }
            streamWriter.Write($"/>");
        }

    }
}