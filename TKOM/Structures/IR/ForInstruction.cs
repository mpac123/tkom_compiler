using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TKOM.Exceptions;
using TKOM.Structures.AST;

namespace TKOM.Structures.IR
{
    public class ForInstruction : Instruction
    {
        public ForInstruction(ScopePrototype scopePrototype, ForExpression forExpression) : base(scopePrototype)
        {
            ForExpression = forExpression;
            Block = new Block(scopePrototype, new List<string> {forExpression.ElementName}, scopePrototype.FunctionName);
        }

        public ForExpression ForExpression {private set; get;}

        public Block Block {private set; get;}


        public override void Execute(Node node)
        {
            // find the exact part of given input that will be iterated
            var collection = node.Scope.FindValueOfValueOf(ForExpression.Collection);
            if (collection.Type != JTokenType.Array)
            {
                throw new RuntimeException($"The value of {collection.ToString()} is not an array and could not be iterated");
            }

            foreach (var element in collection)
            {
                var newNode = new Node(node, Block.ScopePrototype, node.Scope);
                newNode.Scope.Initialize(new List<AssignedValue> {
                        new AssignedValue(element)
                });
                Block.Execute(newNode);
            }
        }


    }
}