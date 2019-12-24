namespace TKOM.AST
{
    public class ConditionWithNumericValue : ICondition
    {
        public string LeftHandSideVariable { get; set; }
        public ConditionType? ConditionType { get; set; }

        public bool Integer {set; get;}
        public int IntValue {set; get;}
        public double RealValue {set; get;}
    }
}