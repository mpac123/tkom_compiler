using System.Collections.Generic;

namespace TKOM.AST
{
    public class FunctionCall : IInstruction
    {
        public string FunctionName {set; get;}
        public List<ValueOf> ArgumentValues {set; get;} 

    }
}