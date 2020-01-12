using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TKOM.Exceptions;
using TKOM.Structures.IR;

namespace TKOM.Structures.AST
{
    public class ValueOf : Value, IStringComponent
    {
        public string VariableName { set; get; }
        public int? Index { set; get; }
        public ValueOf NestedValue { set; get; }

        public override AssignedValue GetIRValue(Scope scope)
        {
            var valueOfInstruction = new ValueOfInstruction(scope, this);
            return new AssignedValue(valueOfInstruction.ReturnValue());
        }

        public override bool PerformComparisonOnRhs(JToken lhsToken, ConditionType conditionType, Scope scope)
        {
            var rhsInstruction = new ValueOfInstruction(scope, this);
            var rhsToken = rhsInstruction.ReturnValue();
            switch (conditionType)
            {
                case ConditionType.Equal:
                    return CheckIfTwoValuesOfAreEqual(lhsToken, rhsToken);
                case ConditionType.NotEqual:
                    return !CheckIfTwoValuesOfAreEqual(lhsToken, rhsToken);
                case ConditionType.LessThan:
                case ConditionType.LessEqualThan:
                case ConditionType.GreaterThan:
                case ConditionType.GreaterEqualThan:
                    return TryCompareValuesOfAsNumericValues(lhsToken, rhsToken, conditionType);
                default:
                    throw new RuntimeException($"Unknown condition type: {conditionType.ToString()}");
            }
        }

        private bool CheckIfTwoValuesOfAreEqual(JToken lhs, JToken rhs)
        {
            // first, try to parse to double value
            try
            {
                var realLhs = lhs.ToObject<double>();
                var realRhs = rhs.ToObject<double>();
                return realLhs == realRhs;
            }
            catch (Exception)
            {
                try
                {
                    var stringLhs = lhs.ToObject<string>();
                    var stringRhs = rhs.ToObject<string>();
                    return stringLhs == stringRhs;
                }
                catch (Exception ex)
                {
                    throw new RuntimeException($"Values in condition could not be both parsed to numeric values nor to strings and therefore, could not be compared: {ex.Message}.");
                }
            }
        }

        private bool TryCompareValuesOfAsNumericValues(JToken lhs, JToken rhs, ConditionType conditionType)
        {
            double realLhs;
            double realRhs;
            try
            {
                realLhs = lhs.ToObject<double>();
                realRhs = rhs.ToObject<double>();
            }
            catch (Exception e)
            {
                throw new RuntimeException($"Values in condition could not be both parsed to numeric values, and therefore the condition could not be performed: {e.Message}");
            }
            switch (conditionType)
            {
                case ConditionType.LessThan:
                    return realLhs < realRhs;
                case ConditionType.LessEqualThan:
                    return realLhs <= realRhs;
                case ConditionType.GreaterThan:
                    return realLhs > realRhs;
                case ConditionType.GreaterEqualThan:
                    return realLhs >= realRhs;
                default:
                    throw new RuntimeException($"Unknown condition type: {conditionType.ToString()}");
            }
        }
    }
}