
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TKOM.Exceptions;

namespace TKOM.Structures.IR
{
    public class Block : Executable
    {
        public List<Executable> NestedBlocks;

        public Block(ScopePrototype upperScope, List<string> variables, string functionName)
        {
            NestedBlocks = new List<Executable>();
            ScopePrototype = new ScopePrototype(functionName);
            ScopePrototype.UpperScopePrototype = upperScope;
            foreach (var variable in variables)
            {
                ScopePrototype.TryAddVariable(variable);
            }
        }

        public override void Execute(Node node)
        {
            foreach (var block in NestedBlocks)
            {
                block.Execute(node);
            }
        }
    }
}