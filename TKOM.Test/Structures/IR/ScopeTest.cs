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
            var scope_prototype = new ScopePrototype
            {
                Variables = new HashSet<string> { "arg" },
            };
            var scope = new Scope(scope_prototype)
            {
                VariableValues = new Dictionary<string, AssignedValue> { { "arg", new AssignedValue(JToken.Parse("'value'")) } }
            };
            var valueOf = new ValueOf
            {
                VariableName = "arg"
            };

            // act 
            var value = scope.FindValueOfValueOf(valueOf);

            // validate
            Assert.Equal("value", value);
        }

        [Fact]
        public void ScopeWithArray_VariableIsCalled_CorrectValueIsReturned()
        {
            // prepare
            var scope_prototype = new ScopePrototype
            {
                Variables = new HashSet<string> { "array" },
            };
            var scope = new Scope(scope_prototype)
            {
                VariableValues = new Dictionary<string, AssignedValue> { { "array", new AssignedValue(JToken.Parse("[1,3,6]")) } }
            };
            var valueOf = new ValueOf
            {
                VariableName = "array",
                Index = 2
            };

            // act 
            var value = scope.FindValueOfValueOf(valueOf);

            // validate
            Assert.Equal("6", value);
        }

        [Fact]
        public void ScopeWithObjectWithArray_VariableIsCalled_CorrectValueIsReturned()
        {
            // prepare
            var scope_prototype = new ScopePrototype
            {
                Variables = new HashSet<string> { "array" },
            };
            var scope = new Scope(scope_prototype)
            {
                VariableValues = new Dictionary<string, AssignedValue> { { "array", new AssignedValue(JToken.Parse("{'object': [1,3,6]}")) } }
            };
            var valueOf = new ValueOf
            {
                VariableName = "array",
                NestedValue = new ValueOf
                {
                    VariableName = "object",
                    Index = 2
                }
            };

            // act 
            var value = scope.FindValueOfValueOf(valueOf);

            // validate
            Assert.Equal("6", value);
        }

        [Fact]
        public void ScopeWithObjectWithArrayWithNestedObject_VariableIsCalled_CorrectValueIsReturned()
        {
            // prepare
            var scope_prototype = new ScopePrototype
            {
                Variables = new HashSet<string> { "array" },
            };
            var scope = new Scope(scope_prototype)
            {
                VariableValues = new Dictionary<string, AssignedValue> { { "array", new AssignedValue(JToken.Parse("{'object': [{'el':1,'n':1},{'el':2,'n':2}]}")) } }
            };
            var valueOf = new ValueOf
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
            };

            // act 
            var value = scope.FindValueOfValueOf(valueOf);

            // validate
            Assert.Equal("2", value);
        }

        [Fact]
        public void ScopeWithVariableInUpperScope_VariableIsCalled_CorrectValueIsReturned()
        {
            // prepare
            var outer_scope_prototype = new ScopePrototype
            {
                Variables = new HashSet<string> { "arg" },
            };
            var scope_prototype = new ScopePrototype
            {
                Variables = new HashSet<string> { "arg2" },
                UpperScopePrototype = outer_scope_prototype
            };
            var scope = new Scope(scope_prototype)
            {
                UpperScope = new Scope(outer_scope_prototype)
                {
                    VariableValues = new Dictionary<string, AssignedValue> { { "arg", new AssignedValue(JToken.Parse("'value'")) } }
                },
                VariableValues = new Dictionary<string, AssignedValue> { { "arg2", new AssignedValue(JToken.Parse("'value2'")) } }
            };
            var valueOf =  new ValueOf
            {
                VariableName = "arg"
            };

            // act 
            var value = scope.FindValueOfValueOf(valueOf);

            // validate
            Assert.Equal("value", value);
        }

        [Fact]
        public void ScopeWithoutVariable_VariableIsCalled_RuntimeExceptionIsThrown()
        {
            // prepare
            var outer_scope_prototype = new ScopePrototype
            {
                Variables = new HashSet<string> { "arg" },
            };
            var scope_prototype = new ScopePrototype
            {
                Variables = new HashSet<string> { "arg2" },
                UpperScopePrototype = outer_scope_prototype
            };
            var scope = new Scope(scope_prototype)
            {
                UpperScope = new Scope(outer_scope_prototype)
                {
                    VariableValues = new Dictionary<string, AssignedValue> { { "arg", new AssignedValue(JToken.Parse("'value'")) } }
                },
                VariableValues = new Dictionary<string, AssignedValue> { { "arg2", new AssignedValue(JToken.Parse("'value2'")) } }
            };
            var valueOf = new ValueOf
            {
                VariableName = "arg3"
            };

            // act 
            var exceptionWasThrown = false;
            try
            {
                scope.FindValueOfValueOf(valueOf);
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