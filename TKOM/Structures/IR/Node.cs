using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TKOM.Structures.IR
{
    public class Node
    {
        public Node()
        {

        }
        public Node(Node node)
        {
            NestedLevel = node.NestedLevel;
            NewLine = node.NewLine;
            FunctionsDict = node.FunctionsDict;
            StreamWriter = node.StreamWriter;
            Scope = node.Scope;
        }

        public Node(Node node, ScopePrototype scopePrototype)
        {
            NestedLevel = node.NestedLevel;
            NewLine = node.NewLine;
            FunctionsDict = node.FunctionsDict;
            StreamWriter = node.StreamWriter;
            Scope = new Scope(scopePrototype);
        }

        public Node(Node node, ScopePrototype scopePrototype, Scope upperScope)
        {
            NestedLevel = node.NestedLevel;
            NewLine = node.NewLine;
            FunctionsDict = node.FunctionsDict;
            StreamWriter = node.StreamWriter;
            Scope = new Scope(scopePrototype, upperScope);
        }
        public StreamWriter StreamWriter { set; get; }
        public Scope Scope { set; get; }
        public int NestedLevel { set; get; }
        public bool NewLine { set; get; }
        public Dictionary<string, Block> FunctionsDict { set; get; }

    }
}