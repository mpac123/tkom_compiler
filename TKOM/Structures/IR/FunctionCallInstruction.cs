using System.Collections.Generic;
using System.IO;
using System.Text;
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
            var functionBlock = functions[FunctionCall.FunctionName];
            var assignedValues = new List<AssignedValue>();
            foreach (var argumentCall in FunctionCall.ArgumentValues)
            {
                assignedValues.Add(argumentCall.GetIRValue(Scope));
            }
            functionBlock.Initialize(assignedValues);
            functionBlock.Execute(streamWriter, functions, nestedLevel, newLine);
        }

    }
}