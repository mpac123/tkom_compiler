using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using Newtonsoft.Json.Linq;
using TKOM.Exceptions;
using TKOM.Structures.AST;
using TKOM.Structures.IR;
using Xunit;

namespace TKOM.Test.Structures.IR
{
    public class FunctionCallInstructionTest
    {

        [Fact]
        public void DefinedFunction_CallFunctionWithArgumentsOutOfScope_ThrowRuntimeException()
        {
            // prepare
            var variables = new List<string> { "arg1" };
            var functionBlock = new Block(null, variables, "fun");
            var functionDictionary = new Dictionary<string, Block> {
                {"function", functionBlock}
            };
            var scope_prototype = new ScopePrototype
            {
                Variables = new HashSet<string> { "model" },
            };
            var scope = new Scope(scope_prototype)
            {
                VariableValues = new Dictionary<string, AssignedValue> {
                    { "model", new AssignedValue(JToken.Parse("['value1', 'value2']")) }
                }
            };
            var functionCallInstruction = new FunctionCallInstruction(scope_prototype, new FunctionCall
            {
                FunctionName = "function",
                ArgumentValues = new List<Value> {
                    new ValueOf {
                        VariableName = "model",
                        Index = 2
                    }
                }
            });

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            var exceptionWasThrown = false;
            var node = new Node
            {
                StreamWriter = streamWriter.Object,
                NewLine = false,
                NestedLevel = 0,
                Scope = scope,
                FunctionsDict = functionDictionary
            };
            try
            {
                functionCallInstruction.Execute(node);
            }
            catch (RuntimeException)
            {
                exceptionWasThrown = true;
            }
            // validate
            Assert.True(exceptionWasThrown);
        }

        [Fact]
        public void DefinedFunctionWithValueOfCall_CallFunctionWithArguments_ScopeInitializedCorrectly()
        {
            // prepare
            var variables = new List<string> { "arg1" };
            var functionBlock = new Block(null, variables, "fun");
            functionBlock.NestedBlocks.Add(new StringComponentInstruction(functionBlock.ScopePrototype, new ValueOf
            {
                VariableName = "arg1"
            }));
            var functionDictionary = new Dictionary<string, Block> {
                {"function", functionBlock}
            };
            var scope_prototype = new ScopePrototype
            {
                Variables = new HashSet<string> { "model" },
            };
            var scope = new Scope(scope_prototype)
            {
                VariableValues = new Dictionary<string, AssignedValue> {
                    { "model", new AssignedValue(JToken.Parse("['value1', 'value2']")) }
                }
            };
            var functionCallInstruction = new FunctionCallInstruction(scope_prototype, new FunctionCall
            {
                FunctionName = "function",
                ArgumentValues = new List<Value> {
                    new ValueOf {
                        VariableName = "model",
                        Index = 1
                    }
                }
            });

            // act
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            var node = new Node
            {
                StreamWriter = streamWriter,
                NewLine = false,
                NestedLevel = 0,
                Scope = scope,
                FunctionsDict = functionDictionary
            };
            functionCallInstruction.Execute(node);
            streamWriter.Flush();

            // validate
            memoryStream.Position = 0;
            var streamReader = new StreamReader(memoryStream);
            //Assert.Equal("\nvalue2", streamReader.ReadToEnd());
            Assert.Equal("value2", streamReader.ReadToEnd());
        }


        [Fact]
        public void DefinedFunctionWithValueOfCallWithSimpleValue_CallFunctionWithArguments_ScopeInitializedCorrectly()
        {
            // prepare
            var variables = new List<string> { "arg1" };
            var functionBlock = new Block(null, variables, "fun");
            functionBlock.NestedBlocks.Add(new StringComponentInstruction(functionBlock.ScopePrototype, new ValueOf
            {
                VariableName = "arg1"
            }));
            var functionDictionary = new Dictionary<string, Block> {
                {"function", functionBlock}
            };
            var scope_prototype = new ScopePrototype
            {
                Variables = new HashSet<string> { "model" },
            };
            var scope = new Scope(scope_prototype)
            {
                VariableValues = new Dictionary<string, AssignedValue> {
                    { "model", new AssignedValue(JToken.Parse("'value2'")) }
                }
            };
            var functionCallInstruction = new FunctionCallInstruction(scope_prototype, new FunctionCall
            {
                FunctionName = "function",
                ArgumentValues = new List<Value> {
                    new ValueOf {
                        VariableName = "model"
                    }
                }
            });

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            var node = new Node
            {
                StreamWriter = streamWriter.Object,
                NewLine = false,
                NestedLevel = 0,
                Scope = scope,
                FunctionsDict = functionDictionary
            };
            functionCallInstruction.Execute(node);

            // validate
            streamWriter.Verify(s => s.Write("value2"), Times.Once);
        }

        [Fact]
        public void DefinedFunctionWithRecursiveFunctionCall_CallFunctionWithArguments_TwoDifferentScopesUsed()
        {
            // prepare
            var scope_prototype = new ScopePrototype
            {
                Variables = new HashSet<string> { "arg1", "stopcondition" },
            };
            var variables = new List<string> { "arg1", "stopcondition" };
            var functionBlock = new Block(null, variables, "function");
            functionBlock.NestedBlocks.Add(new StringComponentInstruction(functionBlock.ScopePrototype, new ValueOf
            {
                VariableName = "arg1"
            }));
            functionBlock.NestedBlocks.Add(new IfInstruction(scope_prototype, new IfExpression
            {
                Condition = new SimpleCondition
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "stopcondition"
                    }
                },
                Negated = true
            })
            {
                IfBlock = new List<Executable> {
                    new FunctionCallInstruction(scope_prototype, new FunctionCall {
                        FunctionName = "function",
                        ArgumentValues = new List<Value> {
                            new StringValue {
                                StringComponents = new List<IStringComponent> {
                                    new Literal { Content = "another_value"}
                                }
                            },
                            new StringValue {
                                StringComponents = new List<IStringComponent> {
                                    new Literal { Content = "true"}
                                }
                            },
                        }
                    })
                }
            });
            functionBlock.NestedBlocks.Add(new StringComponentInstruction(functionBlock.ScopePrototype, new ValueOf
            {
                VariableName = "arg1"
            }));
            var functionDictionary = new Dictionary<string, Block> {
                {"function", functionBlock}
            };
            var scope = new Scope(scope_prototype)
            {
                VariableValues = new Dictionary<string, AssignedValue> {
                    { "arg1", new AssignedValue(JToken.Parse("'value2'")) }
                }
            };
            var functionCallInstruction = new FunctionCallInstruction(scope_prototype, new FunctionCall
            {
                FunctionName = "function",
                ArgumentValues = new List<Value> {
                    new ValueOf {
                        VariableName = "arg1"
                    },
                    new StringValue {
                        StringComponents = new List<IStringComponent> {
                            new Literal { Content = "false"}
                        }
                    }
                }
            });

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            var node = new Node
            {
                StreamWriter = streamWriter.Object,
                NewLine = false,
                NestedLevel = 0,
                Scope = scope,
                FunctionsDict = functionDictionary
            };
            functionCallInstruction.Execute(node);

            // validate
            streamWriter.Verify(s => s.Write("value2"), Times.Exactly(2));
            streamWriter.Verify(s => s.Write("another_value"), Times.Exactly(2));
        }
    }
}