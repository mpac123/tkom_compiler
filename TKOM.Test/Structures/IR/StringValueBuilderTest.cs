using System.Collections.Generic;
using TKOM.Structures.AST;
using TKOM.Structures.IR;
using Xunit;

namespace TKOM.Test.Structures.IR
{
    public class StringValueBuilderTest
    {
        [Fact]
        public void StringValueWithValueOf_BuildString_StringBuildCorrectly()
        {
            // prepare
            var scope = new Scope
            {
                Variables = new HashSet<string> { "model " },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'val1': 'aaa', 'val2:': 'bbb'}")}
                }
            };

            var stringValue = new StringValue
            {
                StringComponents = new List<IStringComponent> {
                    new ValueOf {
                        VariableName = "model",
                        NestedValue = new ValueOf {
                            VariableName = "val1"
                        }
                    }
                }
            };

            // act
            var result = StringValueBuilder.Build(stringValue, scope);

            // validate
            Assert.Equal("aaa", result);
        }

        [Fact]
        public void StringValueWithValueOfAndStringsAlternating_BuildString_StringBuildCorrectly()
        {
            // prepare
            var scope = new Scope
            {
                Variables = new HashSet<string> { "model " },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'val1': 'aaa', 'val2': 'bbb'}")}
                }
            };

            var stringValue = new StringValue
            {
                StringComponents = new List<IStringComponent> {
                    new Literal {
                        Content = "literal_"
                    },
                    new ValueOf {
                        VariableName = "model",
                        NestedValue = new ValueOf {
                            VariableName = "val1"
                        }
                    },
                    new ValueOf {
                        VariableName = "model",
                        NestedValue = new ValueOf {
                             VariableName = "val2"
                        }
                    },
                    new Literal {
                        Content = "_end"
                    }
                }
            };

            // act
            var result = StringValueBuilder.Build(stringValue, scope);

            // validate
            Assert.Equal("literal_aaabbb_end", result);
        }


        [Fact]
        public void StringValueWithValueOfScopeWithStringOnly_BuildString_StringBuildCorrectly()
        {
            // prepare
            var scope = new Scope
            {
                Variables = new HashSet<string> { "model " },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("\"black\"")}
                }
            };

            var stringValue = new StringValue
            {
                StringComponents = new List<IStringComponent> {
                    new ValueOf {
                        VariableName = "model"
                    },
                }
            };

            // act
            var result = StringValueBuilder.Build(stringValue, scope);

            // validate
            Assert.Equal("black", result);
        }
    }
}