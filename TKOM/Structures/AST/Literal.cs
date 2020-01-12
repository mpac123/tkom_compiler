using TKOM.Structures.IR;

namespace TKOM.Structures.AST
{
    public class Literal : IStringComponent
    {
        public string Content {set; get;}

        public string GetValue(Scope scope)
        {
            return Content;
        }
    }
}