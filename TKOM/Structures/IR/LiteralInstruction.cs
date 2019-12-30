using System.Collections.Generic;
using System.IO;
using TKOM.Structures.AST;

namespace TKOM.Structures.IR
{
    public class LiteralInstruction : Instruction
    {
        public LiteralInstruction(Scope upperScope, Literal literal) : base(upperScope)
        {
            Literal = literal;
        }

        public Literal Literal { private set; get; }

        public override void Execute(StreamWriter streamWriter,
            Dictionary<string, Block> functions,
            int nestedLevel, bool newLine)
        {
            base.Execute(streamWriter, functions, nestedLevel, newLine);
            streamWriter.Write(Literal.Content);
        }
    }
}