namespace TKOM.AST
{
    public class ConditionWithVariable : ICondition
    {
        public ValueOf LeftHandSideVariable { get; set; }
        public ConditionType? ConditionType { get; set; }
        public ValueOf RightHandSideVariable {set; get;}
    }
}