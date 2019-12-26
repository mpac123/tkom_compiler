
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TKOM.Exceptions;

namespace TKOM.Structures.IR
{
    public class Block : Executable
    {
        public List<Executable> NestedBlocks;

        public Block(Scope upperScope, List<string> variables)
        {
            NestedBlocks = new List<Executable>();
            Scope = new Scope();
            Scope.UpperScope = upperScope;
            foreach (var variable in variables)
            {
                TryAddVariableToScope(variable);
            }
        }

        private void TryAddVariableToScope(string variable)
        {
            if (!Scope.Variables.Add(variable))
            {
                throw new SemanticsException($"Variable {variable} already exists in the scope.");
            }
        }

        // public void AddInstructionToBlock(Executable executable)
        // {

        // }

        public void Initialize(List<AssignedValue> arguments)
        {
            Scope.VariableValues = new Dictionary<string, AssignedValue>();
            // check if number of values is the same as number of arguements in the scope
            if (arguments.Count() > Scope.Variables.Count())
            {
                throw new SemanticsException($"Tried to initialize scope with variables that were not declared.");
            }
            if (arguments.Count() < Scope.Variables.Count())
            {
                throw new SemanticsException($"Some of declared variables were not initialized.");
            }
            foreach (var variableName in Scope.Variables)
            {
                Scope.VariableValues.Add(variableName, arguments.First());
                arguments = arguments.Skip(1).ToList();
            }

        }

        public override void Execute(StreamWriter streamWriter,
                    Dictionary<string, Block> functions,
                    int nestedLevel, bool newLine)
        {
            base.Execute(streamWriter, functions, nestedLevel, newLine);
            foreach (var block in NestedBlocks)
            {
                block.Execute(streamWriter, functions, nestedLevel + 1, newLine);
            }
        }
    }
}