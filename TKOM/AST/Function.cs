using System.Collections.Generic;

namespace TKOM.AST
{
    public class Function
    {
        public string Identifier {set; get;}
        public List<string> Arguments {set; get;}

        public List<IInstruction> Instructions {set; get;}

        public Function()
        {
            Arguments = new List<string>();
            Instructions = new List<IInstruction>();
        }
        
    }
}