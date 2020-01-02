using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TKOM.Structures.AST;
using TKOM.Exceptions;

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
            streamWriter.Write(ReturnValue().ToString());
        }

        public JToken ReturnValue()
        {
            var rootValue = FindValueOfVariable(ValueOf.VariableName);
            if (rootValue.IsNumericValue)
            {
                if (ValueOf.Index != null || ValueOf.NestedValue != null)
                {
                    throw new RuntimeException("Tried to access members of a numeric value.");
                }
                return rootValue.NumericValue.ToString();
            }
            var currentValueOf = ValueOf;
            JToken jToken = JToken.Parse(rootValue.StringValue);
            do
            {
                if (currentValueOf.Index != null)
                {
                    var list = jToken.ToList();
                    if (list.Count() <= currentValueOf.Index)
                    {
                        throw new RuntimeException($"The object {currentValueOf.VariableName} does not have a member of index {ValueOf.Index}");
                    }
                    jToken = list[currentValueOf.Index.Value];
                }
                currentValueOf = currentValueOf.NestedValue;
                if (currentValueOf != null)
                {
                    jToken = jToken[currentValueOf.VariableName];
                    if (jToken == null)
                    {
                        throw new RuntimeException($"The object does not have a member called {currentValueOf.VariableName}");
                    }
                }
            }
            while (currentValueOf != null);
            return jToken;
        }
    }
}