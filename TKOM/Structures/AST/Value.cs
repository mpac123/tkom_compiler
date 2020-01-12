using Newtonsoft.Json.Linq;
using TKOM.Structures.IR;

namespace TKOM.Structures.AST
{
    public abstract class Value : IInstruction
    {
        public abstract AssignedValue GetIRValue(Scope scope);
        public abstract bool PerformComparisonOnRhs(JToken lhsToken, ConditionType conditionType, Scope scope);
        
    }
}