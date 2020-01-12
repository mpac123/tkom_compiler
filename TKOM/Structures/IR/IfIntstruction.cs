using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TKOM.Exceptions;
using TKOM.Structures.AST;
using TKOM.Structures.IR;

namespace TKOM.Structures.IR
{
    public class IfInstruction : Instruction
    {
        public IfInstruction(Scope scope, IfExpression ifExpression) : base(scope)
        {
            IfExpression = ifExpression;
            IfBlock = new List<Executable>();
            ElseBlock = new List<Executable>();
        }

        public IfExpression IfExpression { set; get; }

        public List<Executable> IfBlock { set; get; }
        public List<Executable> ElseBlock { set; get; }


        public override void Execute(StreamWriter streamWriter,
            Dictionary<string, Block> functions,
            int nestedLevel, bool newLine)
        {
            if (IsComparisonTrue())
            {
                foreach (var instrucion in IfBlock)
                {
                    instrucion.Execute(streamWriter, functions, nestedLevel, false);
                }
            }
            else
            {
                foreach (var instrucion in ElseBlock)
                {
                    instrucion.Execute(streamWriter, functions, nestedLevel, false);
                }
            }
        }

        private bool IsComparisonTrue()
        {
                if (IfExpression.Negated)
                {
                    return !IfExpression.Condition.EvaluateCondition(Scope);
                }
                return IfExpression.Condition.EvaluateCondition(Scope);
        }
        
    }
}