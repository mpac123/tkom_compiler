using System.Collections.Generic;
using System.IO;
using System.Linq;
using TKOM.Structures.AST;

namespace TKOM.Structures.IR
{
    public class HtmlTagInstruction : Instruction
    {
        public HtmlTagInstruction(ScopePrototype scopePrototype, HtmlTag htmlTag) : base(scopePrototype)
        {
            HtmlTag = htmlTag;
            Block = new List<Executable>();
        }

        public HtmlTag HtmlTag { private get; set; }
        public List<Executable> Block { set; get; }


        public override void Execute(Node node)
        {
            node.NewLine = true;
            base.Execute(node);
            node.StreamWriter.Write($"<{HtmlTag.TagName}");
            foreach (var attribute in HtmlTag.Attributes)
            {
                node.StreamWriter.Write($" {attribute.attributeName}=\"{StringValueBuilder.Build(attribute.attributeValue, node.Scope)}\"");
            }
            node.StreamWriter.Write($">");
            PerformBlock(node);
            node.StreamWriter.Write($"</{HtmlTag.TagName}>");
        }

        protected void PerformBlock(Node node)
        {
            if (Block.Count() == 0)
            {
                return;
            }
            if ((Block.Count() == 1 &&
                    (Block.First().GetType() == typeof(StringComponentInstruction)
                    || Block.First().GetType() == typeof(IfInstruction)
                    || Block.First().GetType() == typeof(FunctionCallInstruction)
                    || Block.First().GetType() == typeof(ForInstruction)))
                || (Block.All(i => i.GetType() == typeof(StringComponentInstruction))))
            {
                foreach (var instrucion in Block)
                {
                    var newNode = new Node(node);
                    newNode.NewLine = false;
                    instrucion.Execute(newNode);
                }
            }
            else
            {
                foreach (var instrucion in Block)
                {
                    var newNode = new Node(node);
                    newNode.NewLine = true;
                    newNode.NestedLevel += 1;
                    instrucion.Execute(newNode);
                }
                Format(node.StreamWriter, node.NestedLevel, true);
            }
        }

    }
}