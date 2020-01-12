using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TKOM.Structures.IR;

namespace TKOM.Tools
{
    public class Executor
    {
        private Dictionary<string, Block> _functions_dict;
        public Executor(Dictionary<string, Block> dict)
        {
            _functions_dict = dict;
        }

        public void Execute(JToken model, string path_out, bool addDeclaration)
        {
            var main_fun = _functions_dict["main"];
            using (var f = new StreamWriter(path_out))
            {
                var node = new Node
                {
                    StreamWriter = f,
                    NestedLevel = 0,
                    NewLine = false,
                    FunctionsDict = _functions_dict,
                    Scope = new Scope(main_fun.ScopePrototype)
                };
                node.Scope.Initialize(new List<AssignedValue> {
                new AssignedValue(model)});
                if (addDeclaration)
                {
                    f.Write("<!DOCTYPE html>\n<meta charset=\"UTF-8\">");
                }
                main_fun.Execute(node);
            }
        }
    }
}