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
        public FunctionCallInstruction(ScopePrototype scopePrototype, FunctionCall functionCall) : base(scopePrototype)
        {
            FunctionCall = functionCall;
        }

        FunctionCall FunctionCall { set; get; }


        public override void Execute(Node node)
        {
            var functionBlock = node.FunctionsDict[FunctionCall.FunctionName];
            var assignedValues = new List<AssignedValue>();
            foreach (var argumentCall in FunctionCall.ArgumentValues)
            {
                assignedValues.Add(argumentCall.GetIRValue(node.Scope));
            }
            var newNode = new Node(node, functionBlock.ScopePrototype, null);
            newNode.Scope.Initialize(assignedValues);

            functionBlock.Execute(newNode);
        }

    }
}