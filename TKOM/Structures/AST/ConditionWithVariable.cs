namespace TKOM.Structures.AST
{
    public class ConditionWithValue : ICondition
    {
        public ValueOf LeftHandSideVariable { get; set; }
        public ConditionType? ConditionType { get; set; }
        public Value RightHandSideVariable {set; get;}
    }
}