using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TKOM.Exceptions;

namespace TKOM.Structures.IR
{
    public abstract class Executable
    {
        public int SpacesForTab { set; get; } = 2;
        public ScopePrototype ScopePrototype { protected set; get; }
        public virtual void Execute(Node node)
        {
            Format(node.StreamWriter, node.NestedLevel, node.NewLine);
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
    }
}