using System.Collections.Generic;
using System.IO;
using System.Linq;
using TKOM.Exceptions;

namespace TKOM.Structures.IR
{
    public abstract class Executable
    {
        public int SpacesForTab { set; get; } = 2;
        public Scope Scope { protected set; get; }
        public virtual void Execute(StreamWriter streamWriter,
                Dictionary<string, Block> functions,
                int nestedLevel, bool newLine)
        {
            Format(streamWriter, nestedLevel, newLine);
        }

        protected void Format(StreamWriter streamWriter, int nestedLevel, bool newLine)
        {
            if (newLine)
            {
                streamWriter.Write('\n');
                for (int i = 0; i < nestedLevel * SpacesForTab; i++)
                {
                    streamWriter.Write(" ");
                }
            }
        }

        protected AssignedValue FindValueOfVariable(string variableName)
        {
            Scope scope = Scope;
            AssignedValue result;
            while (scope != null)
            {
                if (scope.VariableValues.TryGetValue(variableName, out result))
                {
                    return result;
                }
                scope = scope.UpperScope;
            }
            throw new RuntimeException($"Value {variableName} could not be found");
        }

        protected void PerformBlock(List<Executable> block, StreamWriter streamWriter,
            Dictionary<string, Block> functions, int nestedLevel, bool newLine)
        {
            if (block.Count() == 0)
            {
                return;
            }
            if ((block.Count() == 1 &&
                    (block.First().GetType() == typeof(LiteralInstruction)
                    || block.First().GetType() == typeof(ValueOfInstruction)
                    || block.First().GetType() == typeof(IfInstruction)
                    || block.First().GetType() == typeof(FunctionCallInstruction)
                    || block.First().GetType() == typeof(ForInstruction)))
                || (block.All(i => i.GetType() == typeof(LiteralInstruction)
                    || i.GetType() == typeof(ValueOfInstruction))))
            {
                foreach (var instrucion in block)
                {
                    instrucion.Execute(streamWriter, functions, nestedLevel, false);
                }
            }
            else
            {
                foreach (var instrucion in block)
                {
                    instrucion.Execute(streamWriter, functions, nestedLevel + 1, true);
                }
                Format(streamWriter, nestedLevel, true);
            }
        }
    }
}