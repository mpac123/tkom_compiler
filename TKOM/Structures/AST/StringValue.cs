using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TKOM.Exceptions;
using TKOM.Structures.IR;

namespace TKOM.Structures.AST
{
    public class StringValue : Value
    {
        public StringValue()
        {
            StringComponents = new List<IStringComponent>();
        }
        public List<IStringComponent> StringComponents { set; get; }

        public override AssignedValue GetIRValue(Scope scope)
        {
            return new AssignedValue(StringValueBuilder.Build(this, scope));
        }

        public override bool PerformComparisonOnRhs(JToken lhsToken, ConditionType conditionType, Scope scope)
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
                return (lhsLiteral == StringValueBuilder.Build(this, scope));
            }
            else if (conditionType == ConditionType.NotEqual)
            {
                return (lhsLiteral != StringValueBuilder.Build(this, scope));
            }
            throw new RuntimeException($"Cannot make comparison of type {conditionType.ToString()} when comparing with a literal.");
        }
    }
}