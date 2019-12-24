using System.Collections.Generic;

namespace TKOM.AST
{
    public class ForExpression : IInstruction
    {
        public ValueOf Collection { set; get; }
        public string ElementName { set; get; }
        public List<IInstruction> Instructions { set; get; }
    }
}