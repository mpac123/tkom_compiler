using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TKOM.Exceptions;
using TKOM.Structures.AST;

namespace TKOM.Structures.IR
{
    public class FunctionCallInstruction : Instruction
    {
        public FunctionCallInstruction(Scope scope, FunctionCall functionCall) : base(scope)
        {
            FunctionCall = functionCall;
        }

        FunctionCall FunctionCall { set; get; }


        public override void Execute(StreamWriter streamWriter,
            Dictionary<string, Block> functions,
            int nestedLevel, bool newLine)
        {
            base.Execute(streamWriter, functions, nestedLevel, newLine);
            var functionBlock = functions[FunctionCall.FunctionName];
            var assignedValues = new List<AssignedValue>();
            foreach (var argumentCall in FunctionCall.ArgumentValues)
            {
                if (argumentCall.GetType() == typeof(NumericValue))
                {
                    var numericVal = (NumericValue)argumentCall;
                    assignedValues.Add(new AssignedValue
                    {
                        IsNumericValue = true,
                        NumericValue = numericVal.Integer ? numericVal.IntValue : numericVal.RealValue
                    });
                }
                else if (argumentCall.GetType() == typeof(Literal))
                {
                    var literalValue = (Literal)argumentCall;
                    assignedValues.Add(new AssignedValue(JsonConvert.SerializeObject(literalValue.Content)));
                }
                else if (argumentCall.GetType() == typeof(ValueOf))
                {
                    var valueOf = (ValueOf)argumentCall;
                    var valueOfInstruction = new ValueOfInstruction(Scope, valueOf);
                    assignedValues.Add(new AssignedValue(JsonConvert.SerializeObject(valueOfInstruction.ReturnValue())));
                }
                else
                {
                    throw new RuntimeException("Unknown type.");
                }
            }
            functionBlock.Initialize(assignedValues);
            functionBlock.Execute(streamWriter, functions, nestedLevel, newLine);
        }

    }
}