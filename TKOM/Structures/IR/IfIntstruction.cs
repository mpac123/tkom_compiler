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
        public IfInstruction(ScopePrototype scopePrototype, IfExpression ifExpression) : base(scopePrototype)
        {
            IfExpression = ifExpression;
            IfBlock = new List<Executable>();
            ElseBlock = new List<Executable>();
        }

        public IfExpression IfExpression { set; get; }

        public List<Executable> IfBlock { set; get; }
        public List<Executable> ElseBlock { set; get; }


        public override void Execute(Node node)
        {
            if (IsComparisonTrue(node.Scope))
            {
                foreach (var instrucion in IfBlock)
                {
                    instrucion.Execute(node);
                }
            }
            else
            {
                foreach (var instrucion in ElseBlock)
                {
                    instrucion.Execute(node);
                }
            }
        }

        private bool IsComparisonTrue(Scope scope)
        {
                if (IfExpression.Negated)
                {
                    return !IfExpression.Condition.EvaluateCondition(scope);
                }
                return IfExpression.Condition.EvaluateCondition(scope);
        }
        
    }
}