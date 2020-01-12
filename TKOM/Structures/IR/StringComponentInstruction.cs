using System.Collections.Generic;
using System.IO;
using TKOM.Structures.AST;

namespace TKOM.Structures.IR
{
    public class StringComponentInstruction : Instruction
    {
        public StringComponentInstruction(ScopePrototype upperScope, IStringComponent literal) : base(upperScope)
        {
            Literal = literal;
        }

        public IStringComponent Literal { private set; get; }

        public override void Execute(Node node)
        {
            base.Execute(node);
            node.StreamWriter.Write(Literal.GetValue(node.Scope));
        }
    }
}