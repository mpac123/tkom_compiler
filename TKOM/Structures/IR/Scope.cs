using System.Collections.Generic;

namespace TKOM.Structures.IR
{
    public class Scope
    {
        public Scope()
        {
            Variables = new HashSet<string>();
            VariableValues = new Dictionary<string,AssignedValue>();
        }
        public Scope UpperScope {set; get;}
        public HashSet<string> Variables {set; get;}
        public IDictionary<string, AssignedValue> VariableValues {set; get;}
    }
}