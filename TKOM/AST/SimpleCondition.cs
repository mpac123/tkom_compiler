namespace TKOM.AST
{
    public class SimpleCondition : ICondition
    {
        public ValueOf LeftHandSideVariable { get; set; }
        public ConditionType? ConditionType { get; set; } = null;
    }
}