using Newtonsoft.Json.Linq;

namespace TKOM.Structures.IR
{
    public class AssignedValue
    {
        public AssignedValue()
        {

        }
        public AssignedValue(JToken jValue)
        {
            StringValue = jValue;
        }
        public bool IsNumericValue { set; get; }
        public JToken StringValue { set; get; }
        public double NumericValue { set; get; }
    }
}