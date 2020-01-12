using System;
using TKOM.Exceptions;
using TKOM.Structures.IR;

namespace TKOM.Structures.AST
{
    public class SimpleCondition : ICondition
    {
        public ValueOf LeftHandSideVariable { get; set; }

        public bool EvaluateCondition(Scope scope)
        {
            var lhsInstruction = new ValueOfInstruction(scope, LeftHandSideVariable);
            var lhsToken = lhsInstruction.ReturnValue();
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
    }
}