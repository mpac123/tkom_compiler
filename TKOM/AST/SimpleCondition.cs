namespace TKOM.AST
{
    public class SimpleCondition : ICondition
    {
        public string LeftHandSideVariable { get; set; }
        public ConditionType? ConditionType { get; set; } = null;
    }
}