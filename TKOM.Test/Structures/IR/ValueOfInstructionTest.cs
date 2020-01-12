using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TKOM.Exceptions;
using TKOM.Structures.AST;
using TKOM.Structures.IR;
using Xunit;

namespace TKOM.Test.Structures.IR
{
    public class ValueOfIntructionTest
    {
        [Fact]
        public void ScopeWithVariable_VariableIsCalled_CorrectValueIsReturned()
        {
            // prepare
            var scope = new Scope
            {
                UpperScope = null,
                Variables = new HashSet<string> { "arg" },
                VariableValues = new Dictionary<string, AssignedValue> { { "arg", new AssignedValue(JToken.Parse("'value'")) } }
            };
            var valueOfInstruction = new ValueOfInstruction(scope, new ValueOf
            {
                VariableName = "arg"
            });

            // act 
            var value = valueOfInstruction.ReturnValue();

            // validate
            Assert.Equal("value", value);
        }

        [Fact]
        public void ScopeWithArray_VariableIsCalled_CorrectValueIsReturned()
        {
            // prepare
            var scope = new Scope
            {
                UpperScope = null,
                Variables = new HashSet<string> { "array" },
                VariableValues = new Dictionary<string, AssignedValue> { { "array", new AssignedValue(JToken.Parse("[1,3,6]")) } }
            };
            var valueOfInstruction = new ValueOfInstruction(scope, new ValueOf
            {
                VariableName = "array",
                Index = 2
            });

            // act 
            var value = valueOfInstruction.ReturnValue();

            // validate
            Assert.Equal("6", value);
        }

        [Fact]
        public void ScopeWithObjectWithArray_VariableIsCalled_CorrectValueIsReturned()
        {
            // prepare
            var scope = new Scope
            {
                UpperScope = null,
                Variables = new HashSet<string> { "array" },
                VariableValues = new Dictionary<string, AssignedValue> { { "array", new AssignedValue(JToken.Parse("{'object': [1,3,6]}")) } }
            };
            var valueOfInstruction = new ValueOfInstruction(scope, new ValueOf
            {
                VariableName = "array",
                NestedValue = new ValueOf
                {
                    VariableName = "object",
                    Index = 2
                }
            });

            // act 
            var value = valueOfInstruction.ReturnValue();

            // validate
            Assert.Equal("6", value);
        }

        [Fact]
        public void ScopeWithObjectWithArrayWithNestedObject_VariableIsCalled_CorrectValueIsReturned()
        {
            // prepare
            var scope = new Scope
            {
                UpperScope = null,
                Variables = new HashSet<string> { "array" },
                VariableValues = new Dictionary<string, AssignedValue> { { "array", new AssignedValue(JToken.Parse("{'object': [{'el':1,'n':1},{'el':2,'n':2}]}")) } }
            };
            var valueOfInstruction = new ValueOfInstruction(scope, new ValueOf
            {
                VariableName = "array",
                NestedValue = new ValueOf
                {
                    VariableName = "object",
                    Index = 1,
                    NestedValue = new ValueOf
                    {
                        VariableName = "n"
                    }
                }
            });

            // act 
            var value = valueOfInstruction.ReturnValue();

            // validate
            Assert.Equal("2", value);
        }

        [Fact]
        public void ScopeWithVariableInUpperScope_VariableIsCalled_CorrectValueIsReturned()
        {
            // prepare
            var scope = new Scope
            {
                UpperScope = new Scope
                {
                    Variables = new HashSet<string> { "arg" },
                    VariableValues = new Dictionary<string, AssignedValue> { { "arg", new AssignedValue(JToken.Parse("'value'")) } }
                },
                Variables = new HashSet<string> { "arg2" },
                VariableValues = new Dictionary<string, AssignedValue> { { "arg2", new AssignedValue(JToken.Parse("'value2'")) } }
            };
            var valueOfInstruction = new ValueOfInstruction(scope, new ValueOf
            {
                VariableName = "arg"
            });

            // act 
            var value = valueOfInstruction.ReturnValue();

            // validate
            Assert.Equal("value", value);
        }

        [Fact]
        public void ScopeWithoutVariable_VariableIsCalled_RuntimeExceptionIsThrown()
        {
            // prepare
            var scope = new Scope
            {
                UpperScope = new Scope
                {
                    Variables = new HashSet<string> { "arg" },
                    VariableValues = new Dictionary<string, AssignedValue> { { "arg", new AssignedValue(JToken.Parse("'value'")) } }
                },
                Variables = new HashSet<string> { "arg2" },
                VariableValues = new Dictionary<string, AssignedValue> { { "arg2", new AssignedValue(JToken.Parse("'value2'")) } }
            };
            var valueOfInstruction = new ValueOfInstruction(scope, new ValueOf
            {
                VariableName = "arg3"
            });

            // act 
            var exceptionWasThrown = false;
            try
            {
                valueOfInstruction.ReturnValue();
            }
            catch (RuntimeException)
            {
                exceptionWasThrown = true;
            }

            // validate
            Assert.True(exceptionWasThrown);
        }
    }
}