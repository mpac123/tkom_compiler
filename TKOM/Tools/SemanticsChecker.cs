using System.Collections.Generic;
using System.Linq;
using TKOM.Exceptions;
using TKOM.Structures.AST;
using TKOM.Structures.IR;

namespace TKOM.Tools
{
    public class SemanticsChecker
    {
        private Dictionary<string, Block> _functions;
        private Structures.AST.Program _program;

        public SemanticsChecker(Structures.AST.Program program)
        {
            _functions = new Dictionary<string, Block>();
            _program = program;
        }

        public Dictionary<string, Block> CheckAST()
        {
            ScanFunctions();
            CheckMain();
            TraverseAST();
            return _functions;
        }

        private void ScanFunctions()
        {
            foreach (var function in _program.Functions)
            {
                if (!_functions.TryAdd(function.Identifier, new Block(null, function.Arguments, function.Identifier)))
                {
                    throw new SemanticsException($"The function {function.Identifier} has already been defined.");
                }
                // check if the parameter names are all unique
                if (function.Arguments.Distinct().Count() != function.Arguments.Count())
                {
                    throw new SemanticsException($"The function {function.Identifier} has been defined with multiple parameters with the same names.");
                }
            }
        }

        private void CheckMain()
        {
            Block main_block;

            // check if function main has been defined
            if (!_functions.TryGetValue("main", out main_block))
            {
                throw new SemanticsException("There must be function main defined with one parameter");
            }

            // check if function main has been defined with exactly one parameter
            if (main_block.Scope.Variables.Count() != 1)
            {
                throw new SemanticsException("Function main must be defined with exactly one parameter");
            }
        }

        private void TraverseAST()
        {
            foreach (var function in _program.Functions)
            {
                Block functionBlock;
                _functions.TryGetValue(function.Identifier, out functionBlock);
                PopulateInstructions(function.Instructions, functionBlock.NestedBlocks, functionBlock.Scope);
            }
        }

        private void PopulateInstructions(List<IInstruction> instructions, List<Executable> block, Scope scope)
        {
            IfExpression encounteredIfExpression = null;
            foreach (var instruction in instructions)
            {
                if (encounteredIfExpression != null)
                {
                    if (instruction.GetType() == typeof(ElseExpression))
                    {
                        block.Add(PopulateIfCondition(scope, encounteredIfExpression, (ElseExpression)instruction));
                        encounteredIfExpression = null;
                        continue;
                    }
                    else
                    {
                        block.Add(PopulateIfCondition(scope, encounteredIfExpression, null));
                        encounteredIfExpression = null;
                    }
                }
                if (instruction.GetType() == typeof(IfExpression))
                {
                    encounteredIfExpression = (IfExpression)instruction;
                }
                else if (instruction.GetType() == typeof(ForExpression))
                {
                    block.Add(PopulateForInstruction(scope, (ForExpression)instruction));
                }
                else if (instruction.GetType() == typeof(ElseExpression))
                {
                    throw new SemanticsException("Else instruction must be preceeded by the if block");
                }
                else if (instruction.GetType() == typeof(FunctionCall))
                {
                    block.Add(PopulateFunctionCallInstruction(scope, (FunctionCall)instruction));
                }
                else if (instruction.GetType() == typeof(HtmlInlineTag))
                {
                    block.Add(PopulateHtmlInlineTag(scope, (HtmlInlineTag)instruction));
                }
                else if (instruction.GetType() == typeof(HtmlTag))
                {
                    block.Add(PopulateHtmlTag(scope, (HtmlTag)instruction));
                }
                else if (instruction.GetType() == typeof(Literal))
                {
                    block.Add(PopulateLiteral(scope, (Literal)instruction));
                }
                else if (instruction.GetType() == typeof(ValueOf))
                {
                    block.Add(PopulateValueOf(scope, (ValueOf)instruction));
                }
                else
                {
                    throw new SemanticsException($"Unknown instruction type: ${instruction.GetType()}");
                }
            }
            if (encounteredIfExpression != null)
            {
                block.Add(PopulateIfCondition(scope, encounteredIfExpression, null));
            }
        }

        private IfInstruction PopulateIfCondition(Scope scope, IfExpression ifExpression, ElseExpression elseExpression)
        {
            var ifInstruction = new IfInstruction(scope, ifExpression);
            PopulateInstructions(ifExpression.Instructions, ifInstruction.IfBlock, scope);
            if (elseExpression != null)
            {
                PopulateInstructions(elseExpression.Instructions, ifInstruction.ElseBlock, scope);
            }
            return ifInstruction;
        }

        private ForInstruction PopulateForInstruction(Scope scope, ForExpression forExpression)
        {
            // check if the value to be iterated over exists in the scope
            if (!scope.Variables.Contains(forExpression.Collection.VariableName))
            {
                throw new SemanticsException($"Variable {forExpression.Collection.VariableName} has not been declared in the scope of operation for in function {scope.FunctionName}.");
            }
            var forInstruction = new ForInstruction(scope, forExpression);
            PopulateInstructions(forExpression.Instructions, forInstruction.Block.NestedBlocks, forInstruction.Block.Scope);
            return forInstruction;
        }

        private FunctionCallInstruction PopulateFunctionCallInstruction(Scope scope, FunctionCall functionCall)
        {
            // check if the function has been declared in the scope
            Block function_block;
            if (!_functions.TryGetValue(functionCall.FunctionName, out function_block))
            {
                throw new SemanticsException($"Function {functionCall.FunctionName} has not been declared in the scope of the function {scope.FunctionName}.");
            }

            // check if called arguments exist in the scope
            foreach (var argument in functionCall.ArgumentValues)
            {
                if (argument.GetType() == typeof(ValueOf))
                {
                    if (!scope.Variables.Contains(((ValueOf) argument).VariableName))
                    {
                        throw new SemanticsException($"Variable {((ValueOf) argument).VariableName} has not been declared in the scope of the function {scope.FunctionName}.");
                    }
                }
            }

            var funCallInstruction = new FunctionCallInstruction(scope, functionCall);
            return funCallInstruction;

        }

        private HtmlInlineTagInstruction PopulateHtmlInlineTag(Scope scope, HtmlInlineTag htmlInlineTag)
        {
            var htmlInlineInstruction = new HtmlInlineTagInstruction(scope, htmlInlineTag);
            return htmlInlineInstruction;
        }

        private HtmlTagInstruction PopulateHtmlTag(Scope scope, HtmlTag htmlTag)
        {
            var htmlTagInstruction = new HtmlTagInstruction(scope, htmlTag);
            PopulateInstructions(htmlTag.Instructions, htmlTagInstruction.Block, scope);
            return htmlTagInstruction;
        }

        private LiteralInstruction PopulateLiteral(Scope scope, Literal literal)
        {
            var literalInstruction = new LiteralInstruction(scope, literal);
            return literalInstruction;
        }

        private ValueOfInstruction PopulateValueOf(Scope scope, ValueOf valueOf)
        {
            // check if value has been declared in the scope
            if (!scope.Variables.Contains(valueOf.VariableName))
            {
                throw new SemanticsException($"Variable {valueOf.VariableName} has not been declared in the scope of function {scope.FunctionName}.");
            }

            var valueOfInstruction = new ValueOfInstruction(scope, valueOf);
            return valueOfInstruction;
        }

    }
}