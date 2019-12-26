using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TKOM.Structures.AST;

namespace TKOM.Structures.IR
{
    public class ForInstruction : Instruction
    {
        public ForInstruction(Scope scope, ForExpression forExpression) : base(scope)
        {
            ForExpression = forExpression;
            Block = new Block(scope, new List<string> {forExpression.ElementName});
        }

        public ForExpression ForExpression {private set; get;}

        public Block Block {private set; get;}


        public override void Execute(StreamWriter streamWriter, Dictionary<string, Block> functions, int nestedLevel, bool newLine)
        {
            base.Execute(streamWriter, functions, nestedLevel, newLine);

            // find the exact part of given input that will be iterated
            var valueOfInstruction = new ValueOfInstruction(Scope, ForExpression.Collection);
            var collection = valueOfInstruction.ReturnValue().ToList();

            foreach (var element in collection)
            {
                Block.Initialize(new List<AssignedValue> {
                        new AssignedValue(JsonConvert.SerializeObject(element.ToObject<string>()))
                });
                Block.Execute(streamWriter, functions, nestedLevel, true);
            }
        }


    }
}