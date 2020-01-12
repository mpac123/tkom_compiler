using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TKOM.Structures.AST;
using TKOM.Exceptions;
using Newtonsoft.Json;
using System.Text;

namespace TKOM.Structures.IR
{
    public class ValueOfInstruction : Instruction
    {
        public ValueOfInstruction(Scope scope, ValueOf valueOf) : base(scope)
        {
            ValueOf = valueOf;
        }

        public ValueOf ValueOf { private set; get; }


        public override void Execute(StreamWriter streamWriter,
            Dictionary<string, Block> functions,
            int nestedLevel, bool newLine)
        {
            base.Execute(streamWriter, functions, nestedLevel, newLine);
            streamWriter.Write(ReturnValue().ToObject<string>());
        }

        public JToken ReturnValue()
        {
            var rootValue = FindValueOfVariable(ValueOf.VariableName);
            var rootName = ValueOf.VariableName;
            if (rootValue.IsNumericValue)
            {
                if (ValueOf.Index != null || ValueOf.NestedValue != null)
                {
                    throw new RuntimeException("Tried to access members of a numeric value.");
                }
                return rootValue.NumericValue.ToString();
            }
            var currentValueOf = ValueOf;
            var alreadyParsedBuilder = new StringBuilder(rootName);
            JToken jToken = rootValue.StringValue;
            do
            {
                if (currentValueOf.Index != null)
                {
                    var list = jToken.ToList();
                    if (list.Count() <= currentValueOf.Index)
                    {
                        throw new RuntimeException($"The object {alreadyParsedBuilder.ToString()} does not have a member of index {currentValueOf.Index}.");
                    }
                    jToken = list[currentValueOf.Index.Value];
                    alreadyParsedBuilder.Append($"[{currentValueOf.Index}]");
                }
                currentValueOf = currentValueOf.NestedValue;
                if (currentValueOf != null)
                {
                    jToken = jToken[currentValueOf.VariableName];
                    if (jToken == null)
                    {
                        throw new RuntimeException($"The object {alreadyParsedBuilder.ToString()} does not have a member called {currentValueOf.VariableName}.");
                    }
                    alreadyParsedBuilder.Append($".{currentValueOf.VariableName}");
                }
            }
            while (currentValueOf != null);
            return jToken;
        }
    }
}