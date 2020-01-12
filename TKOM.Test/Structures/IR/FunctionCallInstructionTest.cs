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
        public void DefinedFunction_CallFunctionWithArguments_ScopeInitializedCorrectly()
        {
            // prepare
            var variables = new List<string> { "arg1" };
            var functionBlock = new Block(null, variables, "fun");
            var functionDictionary = new Dictionary<string, Block> {
                {"function", functionBlock}
            };
            var functionCallInstruction = new FunctionCallInstruction(new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    { "model", new AssignedValue(JToken.Parse("['value1', 'value2']")) }
                }
            }, new FunctionCall
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
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            functionCallInstruction.Execute(streamWriter.Object, functionDictionary, 0, false);

            // validate
            Assert.Single(functionBlock.Scope.VariableValues);
            Assert.Equal(JToken.Parse("\"value2\""), functionBlock.Scope.VariableValues["arg1"].StringValue);
        }

        [Fact]
        public void DefinedFunction_CallFunctionWithArgumentsOutOfScope_ThrowRuntimeException()
        {
            // prepare
            var variables = new List<string> { "arg1" };
            var functionBlock = new Block(null, variables, "fun");
            var functionDictionary = new Dictionary<string, Block> {
                {"function", functionBlock}
            };
            var functionCallInstruction = new FunctionCallInstruction(new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    { "model", new AssignedValue(JToken.Parse("['value1', 'value2']")) }
                }
            }, new FunctionCall
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
            try
            {
                functionCallInstruction.Execute(streamWriter.Object, functionDictionary, 0, false);
            }
            catch (RuntimeException)
            {
                exceptionWasThrown = true;
            }
            // validate
            Assert.Empty(functionBlock.Scope.VariableValues);
            Assert.True(exceptionWasThrown);
        }

        [Fact]
        public void DefinedFunctionWithValueOfCall_CallFunctionWithArguments_ScopeInitializedCorrectly()
        {
            // prepare
            var variables = new List<string> { "arg1" };
            var functionBlock = new Block(null, variables, "fun");
            functionBlock.NestedBlocks.Add(new ValueOfInstruction(functionBlock.Scope, new ValueOf
            {
                VariableName = "arg1"
            }));
            var functionDictionary = new Dictionary<string, Block> {
                {"function", functionBlock}
            };
            var functionCallInstruction = new FunctionCallInstruction(new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    { "model", new AssignedValue(JToken.Parse("['value1', 'value2']")) }
                }
            }, new FunctionCall
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
            functionCallInstruction.Execute(streamWriter, functionDictionary, 0, false);
            streamWriter.Flush();

            // validate
            memoryStream.Position = 0;
            var streamReader = new StreamReader(memoryStream);
            Assert.Single(functionBlock.Scope.VariableValues);
            Assert.Equal(JToken.Parse("\"value2\""), functionBlock.Scope.VariableValues["arg1"].StringValue);
            Assert.Equal("\nvalue2", streamReader.ReadToEnd());
        }


        [Fact]
        public void DefinedFunctionWithValueOfCallWithSimpleValue_CallFunctionWithArguments_ScopeInitializedCorrectly()
        {
            // prepare
            var variables = new List<string> { "arg1" };
            var functionBlock = new Block(null, variables, "fun");
            functionBlock.NestedBlocks.Add(new ValueOfInstruction(functionBlock.Scope, new ValueOf
            {
                VariableName = "arg1"
            }));
            var functionDictionary = new Dictionary<string, Block> {
                {"function", functionBlock}
            };
            var functionCallInstruction = new FunctionCallInstruction(new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    { "model", new AssignedValue(JToken.Parse("'value2'")) }
                }
            }, new FunctionCall
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
            functionCallInstruction.Execute(streamWriter.Object, functionDictionary, 0, false);

            // validate
            Assert.Single(functionBlock.Scope.VariableValues);
            Assert.Equal(JToken.Parse("\"value2\""), functionBlock.Scope.VariableValues["arg1"].StringValue);
            streamWriter.Verify(s => s.Write("value2"), Times.Once);
        }
    }
}