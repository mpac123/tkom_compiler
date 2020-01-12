using System.Collections.Generic;
using TKOM.Structures.IR;
using TKOM.Utils;
using static TKOM.Utils.Token;

namespace TKOM.Structures.AST
{
    public interface ICondition
    {

        ValueOf LeftHandSideVariable { set; get; }
        public bool EvaluateCondition(Scope scope);

    }

    public enum ConditionType
    {
        Equal,
        NotEqual,
        LessThan,
        LessEqualThan,
        GreaterThan,
        GreaterEqualThan
    }

}