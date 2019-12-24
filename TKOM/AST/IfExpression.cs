using System.Collections.Generic;

namespace TKOM.AST
{
    public class IfExpression : IInstruction
    {
        public ICondition Condition { set; get; }
        public bool Negated { set; get; }

        public List<IInstruction> Instructions { set; get; }
        public List<IInstruction> InstructionsElse { set; get; }
    }
}