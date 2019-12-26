using System.Collections.Generic;
using System.IO;
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
    }
}