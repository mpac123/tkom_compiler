using System.Collections.Generic;

namespace TKOM.AST
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