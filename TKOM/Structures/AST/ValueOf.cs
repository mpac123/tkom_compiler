namespace TKOM.Structures.AST
{
    public class ValueOf : Value, IStringComponent
    {
        public string VariableName { set; get; }
        public int? Index { set; get; }
        public ValueOf NestedValue { set; get; }
    }
}