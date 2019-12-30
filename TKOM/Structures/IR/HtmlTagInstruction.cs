using System.Collections.Generic;
using System.IO;
using TKOM.Structures.AST;

namespace TKOM.Structures.IR
{
    public class HtmlTagInstruction : Instruction
    {
        public HtmlTagInstruction(Scope scope, HtmlTag htmlTag) : base(scope)
        {
            HtmlTag = htmlTag;
            Block = new List<Executable>();
        }

        public HtmlTag HtmlTag {private get; set;}
        public List<Executable> Block {set; get;}


        public override void Execute(StreamWriter streamWriter, Dictionary<string, Block> functions, int nestedLevel, bool newLine)
        {
            base.Execute(streamWriter, functions, nestedLevel, newLine);
            streamWriter.Write($"<{HtmlTag.TagName}");
            foreach (var attribute in HtmlTag.Attributes)
            {
                streamWriter.Write($" {attribute.attributeName}=\"{attribute.attributeValue}\"");
            }
            streamWriter.Write($">");
            foreach (var instrucion in Block)
            {
                instrucion.Execute(streamWriter, functions, nestedLevel + 1, true);
            }
            streamWriter.Write($"\n</{HtmlTag.TagName}>");
        }

    }
}