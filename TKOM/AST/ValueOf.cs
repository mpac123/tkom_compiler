namespace TKOM.AST
{
    public class ValueOf : IInstruction
    {
        public string VariableName { set; get; }
        public int? Index { set; get; }
        public ValueOf NestedValue { set; get; }
    }
}