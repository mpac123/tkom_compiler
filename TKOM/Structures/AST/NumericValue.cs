using System;
using Newtonsoft.Json.Linq;
using TKOM.Exceptions;
using TKOM.Structures.IR;

namespace TKOM.Structures.AST
{
    public class NumericValue : Value
    {
        public bool Integer { set; get; }
        public int IntValue { set; get; }
        public double RealValue { set; get; }

        public override AssignedValue GetIRValue(Scope scope)
        {
            return new AssignedValue
            {
                IsNumericValue = true,
                NumericValue = Integer ? IntValue : RealValue
            };
        }

        public override bool PerformComparisonOnRhs(JToken lhsToken, ConditionType conditionType, Scope scope)
        {
            double lhsRealValue;
            try
            {
                lhsRealValue = lhsToken.ToObject<double>();
            }
            catch (Exception e)
            {
                throw new RuntimeException($"Cannot parse object to a number in comparison: ${e.Message}");
            }
            double rhsRealValue = this.Integer ? this.IntValue : this.RealValue;
            switch (conditionType)
            {
                case ConditionType.Equal:
                    return lhsRealValue == rhsRealValue;
                case ConditionType.NotEqual:
                    return lhsRealValue != rhsRealValue;
                case ConditionType.LessThan:
                    return lhsRealValue < rhsRealValue;
                case ConditionType.LessEqualThan:
                    return lhsRealValue <= rhsRealValue;
                case ConditionType.GreaterThan:
                    return lhsRealValue > rhsRealValue;
                case ConditionType.GreaterEqualThan:
                    return lhsRealValue >= rhsRealValue;
                default:
                    throw new RuntimeException($"Unknown condition type: {conditionType.ToString()}");

            }
        }
    }
}