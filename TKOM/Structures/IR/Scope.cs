using System.Collections.Generic;

namespace TKOM.Structures.IR
{
    public class Scope
    {
        public Scope(string functionName)
        {
            Variables = new HashSet<string>();
            VariableValues = new Dictionary<string,AssignedValue>();
            FunctionName = functionName;
        }

        public Scope()
        {
            Variables = new HashSet<string>();
            VariableValues = new Dictionary<string,AssignedValue>();
            FunctionName = "Undefined";
        }
        public Scope UpperScope {set; get;}
        public HashSet<string> Variables {set; get;}
        public IDictionary<string, AssignedValue> VariableValues {set; get;}
        public string FunctionName {set; get;}
    }
}