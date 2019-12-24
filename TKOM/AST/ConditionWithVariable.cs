namespace TKOM.AST
{
    public class ConditionWithVariable : ICondition
    {
        public string LeftHandSideVariable { get; set; }
        public ConditionType? ConditionType { get; set; }
        public string RightHandSideVariable {set; get;}
    }
}