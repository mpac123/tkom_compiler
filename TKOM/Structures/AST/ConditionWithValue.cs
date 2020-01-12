using TKOM.Structures.IR;

namespace TKOM.Structures.AST
{
    public class ConditionWithValue : ICondition
    {
        public ValueOf LeftHandSideVariable { get; set; }
        public ConditionType ConditionType { get; set; }
        public Value RightHandSideVariable {set; get;}

        public bool EvaluateCondition(Scope scope)
        {
            var lhsInstruction = new ValueOfInstruction(scope, LeftHandSideVariable);
            var lhsToken = lhsInstruction.ReturnValue();

            return RightHandSideVariable.PerformComparisonOnRhs(lhsToken, ConditionType, scope);
        }
    }
}