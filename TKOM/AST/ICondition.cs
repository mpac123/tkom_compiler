namespace TKOM.AST
{
    public interface ICondition
    {
        
        string LeftHandSideVariable {set; get;}
        ConditionType? ConditionType {set; get;}
    }

    public enum ConditionType
        {
            Equal,
            NotEqual,
            PointyBracketOpen,
            LessEqualThan,
            PointyBracketClose,
            GreaterEqualThan
        }
}