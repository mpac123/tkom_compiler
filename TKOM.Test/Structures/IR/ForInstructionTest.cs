using System.Collections.Generic;
using System.IO;
using Moq;
using TKOM.Structures.AST;
using TKOM.Structures.IR;
using Xunit;

namespace TKOM.Test.Structures.IR
{
    public class ForInstructionTest
    {
        [Fact]
        public void DefinedForInstruction_CallInsideScope_NestedScopesInitializedCorrectly()
        {
            // prepare
            var outer_scope = new Scope {
                Variables = new HashSet<string> {"model"},
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,5,8],'field2':'val2'}")}
                }
            };
            var forInstruction = new ForInstruction(outer_scope, new ForExpression {
                Collection = new ValueOf {
                    VariableName = "model",
                    NestedValue = new ValueOf {
                        VariableName = "field1"
                    }
                },
                ElementName = "element"
            });
            forInstruction.Block.NestedBlocks.Add(new ValueOfInstruction(forInstruction.Block.Scope, new ValueOf {
                VariableName = "element"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            forInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            Assert.Single(forInstruction.Scope.VariableValues);
            Assert.True(forInstruction.Scope.VariableValues.ContainsKey("model"));
            Assert.Single(forInstruction.Block.Scope.VariableValues);
            Assert.Equal("\"8\"", forInstruction.Block.Scope.VariableValues["element"].StringValue);
            streamWriter.Verify(s => s.Write("2"), Times.Once);
            streamWriter.Verify(s => s.Write("5"), Times.Once);
            streamWriter.Verify(s => s.Write("8"), Times.Once);
    

        }
    }
}