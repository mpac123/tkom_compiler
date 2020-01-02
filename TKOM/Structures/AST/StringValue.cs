using System.Collections.Generic;

namespace TKOM.Structures.AST
{
    public class StringValue : Value
    {
        public StringValue()
        {
            StringComponents = new List<IStringComponent>();
        }
        public List<IStringComponent> StringComponents { set; get; }
    }
}