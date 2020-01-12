using System.Collections.Generic;
using TKOM.Exceptions;

namespace TKOM.Structures.IR
{
    public class ScopePrototype
    {
        public ScopePrototype(string functionName)
        {
            Variables = new HashSet<string>();
            FunctionName = functionName;
        }

        public ScopePrototype()
        {
            Variables = new HashSet<string>();
            FunctionName = "Undefined";
        }

        public HashSet<string> Variables {set; get;}
        public string FunctionName {set; get;}
        public ScopePrototype UpperScopePrototype {set; get;}

        public void TryAddVariable(string variableName)
        {
            if (!Variables.Add(variableName))
            {
                throw new SemanticsException($"Variable {variableName} already exists in the scope.");
            }
        }
        
    }
}