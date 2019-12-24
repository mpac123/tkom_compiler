using System.Collections.Generic;

namespace TKOM.AST
{
    public class Program
    {
        public List<Function> Functions {get; set;}

        public Program()
        {
            Functions = new List<Function>();
        }
    }
}