namespace TKOM.Structures.IR
{
    public class AssignedValue
    {
        public AssignedValue()
        {

        }
        public AssignedValue(string stringValue)
        {
            StringValue = stringValue;
        }
        public bool IsNumericValue { set; get; }
        public string StringValue { set; get; }
        public double NumericValue { set; get; }
    }
}