using System.Collections.Generic;
using TKOM.Structures.IR;

namespace TKOM.Structures.AST
{
    public class ElseExpression : IInstruction
    {
        public List<IInstruction> Instructions { set; get; }
        public ElseExpression()
        {
            Instructions = new List<IInstruction>();
        }
    }
}