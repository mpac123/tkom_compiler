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
                parsedStringValueBuilder.Append(value.GetValue(scope));
            }
            return parsedStringValueBuilder.ToString();
        }
    }
}