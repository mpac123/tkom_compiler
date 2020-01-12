using System.Collections.Generic;
using System.IO;

namespace TKOM.Structures.IR
{
    public abstract class Instruction : Executable
    {
        public Instruction(ScopePrototype scopePrototype)
        {
            ScopePrototype = scopePrototype;
        }
    }
}