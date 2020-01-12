using System.Collections.Generic;
using System.IO;
using Moq;
using Newtonsoft.Json.Linq;
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
            var outer_scope_prototype = new ScopePrototype {
                Variables = new HashSet<string> {"model"}
            };
            var outer_scope = new Scope(outer_scope_prototype) {
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue(JToken.Parse("{'field1':[2,5,8],'field2':'val2'}"))}
                }
            };
            var forInstruction = new ForInstruction(outer_scope_prototype, new ForExpression {
                Collection = new ValueOf {
                    VariableName = "model",
                    NestedValue = new ValueOf {
                        VariableName = "field1"
                    }
                },
                ElementName = "element"
            });
            forInstruction.Block.NestedBlocks.Add(new StringComponentInstruction(forInstruction.Block.ScopePrototype, new ValueOf {
                VariableName = "element"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            var node = new Node {
                StreamWriter = streamWriter.Object,
                NewLine = false,
                NestedLevel = 0,
                Scope = outer_scope
            };
            forInstruction.Execute(node);

            // validate
            Assert.Single(forInstruction.ScopePrototype.Variables);
            Assert.Single(forInstruction.Block.ScopePrototype.Variables);
            streamWriter.Verify(s => s.Write("2"), Times.Once);
            streamWriter.Verify(s => s.Write("5"), Times.Once);
            streamWriter.Verify(s => s.Write("8"), Times.Once);
    

        }

        [Fact]
        public void DefinedForInstruction_CallInsideScopeWithArrayOfObjects_NestedScopesInitializedCorrectly()
        {
            // prepare
            var outer_scope_prototype = new ScopePrototype {
                Variables = new HashSet<string> {"model"},
            };
            var outer_scope = new Scope(outer_scope_prototype) {
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue(JToken.Parse("{'field1':[{'prop': 2},{'prop': 5},{'prop':8}],'field2':'val2'}"))}
                }
            };
            var forInstruction = new ForInstruction(outer_scope_prototype, new ForExpression {
                Collection = new ValueOf {
                    VariableName = "model",
                    NestedValue = new ValueOf {
                        VariableName = "field1"
                    }
                },
                ElementName = "element"
            });
            forInstruction.Block.NestedBlocks.Add(new StringComponentInstruction(forInstruction.Block.ScopePrototype, new ValueOf {
                VariableName = "element",
                NestedValue = new ValueOf {
                    VariableName = "prop"
                }
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            var node = new Node {
                StreamWriter = streamWriter.Object,
                NewLine = false,
                NestedLevel = 0,
                Scope = outer_scope
            };
            forInstruction.Execute(node);

            // validate
            // Assert.Single(forInstruction.ScopePrototype.Variables);
            // Assert.True(forInstruction.Scope.VariableValues.ContainsKey("model"));
            // Assert.Single(forInstruction.Block.Scope.VariableValues);
            // Assert.Equal("8", forInstruction.Block.Scope.VariableValues["element"].);
            streamWriter.Verify(s => s.Write("2"), Times.Once);
            streamWriter.Verify(s => s.Write("5"), Times.Once);
            streamWriter.Verify(s => s.Write("8"), Times.Once);
    

        }
    }
}