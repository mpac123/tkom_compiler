using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public void Execute(string model, string path_out, bool addDeclaration)
        {
            var main_fun = _functions_dict["main"];
            main_fun.Initialize(new List<AssignedValue> {
                new AssignedValue(model)
            });
            using (var f = new StreamWriter(path_out))
            {
                if (addDeclaration)
                {
                    f.Write("<!DOCTYPE html>\n<meta charset=\"UTF-8\">");
                }
                main_fun.Execute(f, _functions_dict, 0, false);
            }
        }
    }
}