using System.Collections.Generic;

namespace TKOM.Structures.AST
{
    public class ForExpression : IInstruction
    {
        public ValueOf Collection { set; get; }
        public string ElementName { set; get; }
        public List<IInstruction> Instructions { set; get; }
    }
}