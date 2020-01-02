using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TKOM.Exceptions;
using TKOM.Structures.AST;
using TKOM.Structures.IR;

namespace TKOM.Structures.IR
{
    public class IfInstruction : Instruction
    {
        public IfInstruction(Scope scope, IfExpression ifExpression) : base(scope)
        {
            IfExpression = ifExpression;
            IfBlock = new List<Executable>();
            ElseBlock = new List<Executable>();
        }

        public IfExpression IfExpression { set; get; }

        public List<Executable> IfBlock { set; get; }
        public List<Executable> ElseBlock { set; get; }


        public override void Execute(StreamWriter streamWriter,
            Dictionary<string, Block> functions,
            int nestedLevel, bool newLine)
        {
            if (IsComparisonTrue())
            {
                PerformBlock(IfBlock, streamWriter, functions, nestedLevel, newLine);
            }
            else
            {
                PerformBlock(ElseBlock, streamWriter, functions, nestedLevel, newLine);
            }
        }

        private bool IsComparisonTrue()
        {
            var lhs = IfExpression.Condition.LeftHandSideVariable;
            var lhsInstruction = new ValueOfInstruction(Scope, lhs);
            var lhsToken = lhsInstruction.ReturnValue();

            if ((IfExpression.Condition.GetType() == typeof(SimpleCondition)))
            {
                return CheckSimpleCondition(lhsToken);
            }
            var conditionWithValue = (ConditionWithValue)IfExpression.Condition;
            if (conditionWithValue.RightHandSideVariable.GetType() == typeof(StringValue))
            {
                var rhs = (StringValue)conditionWithValue.RightHandSideVariable;
                return CheckConditionWithString(lhsToken, rhs, conditionWithValue.ConditionType);

            }
            if (conditionWithValue.RightHandSideVariable.GetType() == typeof(NumericValue))
            {
                var rhs = (NumericValue)conditionWithValue.RightHandSideVariable;
                return CheckConditionWithNumericValue(lhsToken, rhs, conditionWithValue.ConditionType);
            }
            if (conditionWithValue.RightHandSideVariable.GetType() == typeof(ValueOf))
            {
                var rhs = (ValueOf)conditionWithValue.RightHandSideVariable;
                return CheckConditionWithValueOf(lhsToken, rhs, conditionWithValue.ConditionType);
            }
            throw new RuntimeException($"Unknown type of value in condition: {conditionWithValue.RightHandSideVariable.GetType()}");
        }

        private bool CheckSimpleCondition(JToken lhsToken)
        {
            bool result;
            try
            {
                result = lhsToken.ToObject<bool>();
            }
            catch (Exception e)
            {
                throw new RuntimeException($"Error occured when parsing value in condition to boolean: {e.Message}");
            }
            return result;
        }

        private bool CheckConditionWithString(JToken lhsToken, StringValue rhs, ConditionType conditionType)
        {
            string lhsLiteral;
            try
            {
                lhsLiteral = lhsToken.ToObject<string>();
            }
            catch (Exception e)
            {
                throw new RuntimeException($"Error occured when parsing object to string in condition: {e.Message}");
            }
            if (conditionType == ConditionType.Equal)
            {
                return (lhsLiteral == StringValueBuilder.Build(rhs, Scope));
            }
            else if (conditionType == ConditionType.NotEqual)
            {
                return (lhsLiteral != StringValueBuilder.Build(rhs, Scope));
            }
            throw new RuntimeException($"Cannot make comparison of type {conditionType.ToString()} when comparing with a literal.");
        }

        private bool CheckConditionWithNumericValue(JToken lhsToken, NumericValue rhs, ConditionType conditionType)
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
            double rhsRealValue = rhs.Integer ? rhs.IntValue : rhs.RealValue;
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

        private bool CheckConditionWithValueOf(JToken lhsToken, ValueOf rhs, ConditionType conditionType)
        {
            var rhsInstruction = new ValueOfInstruction(Scope, rhs);
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