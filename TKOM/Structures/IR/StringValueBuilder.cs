using System.Text;
using Newtonsoft.Json;
using TKOM.Structures.AST;

namespace TKOM.Structures.IR
{
    public static class StringValueBuilder
    {
        public static string Build(StringValue stringValue, Scope scope)
        {
            var parsedStringValueBuilder = new StringBuilder();
            foreach (var value in stringValue.StringComponents)
            {
                if (value.GetType() == typeof(Literal))
                {
                    parsedStringValueBuilder.Append(((Literal)value).Content);
                }
                else if (value.GetType() == typeof(ValueOf))
                {
                    var valueOfInstruction = new ValueOfInstruction(scope, (ValueOf)value);
                    parsedStringValueBuilder.Append(valueOfInstruction.ReturnValue());
                }
            }
            return parsedStringValueBuilder.ToString();
        }
    }
}