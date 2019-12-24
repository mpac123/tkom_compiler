namespace TKOM.AST
{
    public class ConditionWithString : ICondition
    {
        public ValueOf LeftHandSideVariable { get; set; }
        public ConditionType? ConditionType { get; set; }
        public string RightHandSideValue {set; get;}
    }
}