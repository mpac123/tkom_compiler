using System.Collections.Generic;
using System.IO;
using Moq;
using TKOM.Exceptions;
using TKOM.Structures.AST;
using TKOM.Structures.IR;
using Xunit;

namespace TKOM.Test.Structures.IR
{
    public class IfInstructionTest
    {
        [Fact]
        public void DefinedIfInstructionWithSimpleConditionThatShouldYieldTrue_CallInsideScope_ConditionEvaluatedCorrectly()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,5,8],'field2':'val2'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new SimpleCondition
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field1",
                            Index = 1
                        }
                    }
                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was true"), Times.Once);
        }

        [Fact]
        public void DefinedIfInstructionWithSimpleConditionThatShouldYieldFalse_CallInsideScope_ConditionEvaluatedCorrectly()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'val2'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new SimpleCondition
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field1",
                            Index = 1
                        }
                    }
                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was false"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was true"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithSimpleConditionWithBoolean_CallInsideScope_ConditionEvaluatedCorrectly()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'true'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new SimpleCondition
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field2"
                        }
                    }
                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was true"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was false"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithSimpleConditionWithBooleanStartingWithCapital_CallInsideScope_ConditionEvaluatedCorrectly()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'False'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new SimpleCondition
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field2"
                        }
                    }
                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was false"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was true"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithEqualConditionWithLiteral_CallInsideScope_ShouldPerformInstructionsInIfBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field2"
                        }
                    },
                    ConditionType = ConditionType.Equal,
                    RightHandSideVariable = new Literal
                    {
                        Content = "literal"
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was true"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was false"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithEqualConditionWithLiteral_CallInsideScope_ShouldPerformInstructionsInElseBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'different_literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field2"
                        }
                    },
                    ConditionType = ConditionType.Equal,
                    RightHandSideVariable = new Literal
                    {
                        Content = "literal"
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was false"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was true"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithNotEqualConditionWithLiteral_CallInsideScope_ShouldPerformInstructionsInElseBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field2"
                        }
                    },
                    ConditionType = ConditionType.NotEqual,
                    RightHandSideVariable = new Literal
                    {
                        Content = "literal"
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was false"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was true"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithNotEqualConditionWithLiteral_CallInsideScope_ShouldPerformInstructionsInInBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'different_literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field2"
                        }
                    },
                    ConditionType = ConditionType.NotEqual,
                    RightHandSideVariable = new Literal
                    {
                        Content = "literal"
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was true"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was false"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithEqualConditionWithNumericIntValue_CallInsideScope_ShouldPerformInstructionsInIfBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field1",
                            Index = 2
                        }
                    },
                    ConditionType = ConditionType.Equal,
                    RightHandSideVariable = new NumericValue
                    {
                        IntValue = 8,
                        Integer = true
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was true"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was false"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithEqualConditionWithNumericRealValue_CallInsideScope_ShouldPerformInstructionsInIfBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field1",
                            Index = 2
                        }
                    },
                    ConditionType = ConditionType.Equal,
                    RightHandSideVariable = new NumericValue
                    {
                        RealValue = 8.0
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was true"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was false"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithEqualConditionWithNumericRealValue_CallInsideScope_ShouldPerformInstructionsInElseBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field1",
                            Index = 2
                        }
                    },
                    ConditionType = ConditionType.Equal,
                    RightHandSideVariable = new NumericValue
                    {
                        RealValue = 8.256
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was false"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was true"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithEqualConditionWithNumericRealValue_CallInsideScopeWithRealValue_ShouldPerformInstructionsInIfBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8.256],'field2':'literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field1",
                            Index = 2
                        }
                    },
                    ConditionType = ConditionType.Equal,
                    RightHandSideVariable = new NumericValue
                    {
                        RealValue = 8.256
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was true"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was false"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithLessThanConditionWithNumericRealValue_CallInsideScope_ShouldPerformInstructionsInIfBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field1",
                            Index = 2
                        }
                    },
                    ConditionType = ConditionType.LessThan,
                    RightHandSideVariable = new NumericValue
                    {
                        RealValue = 8.256
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was true"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was false"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithLessEqualThanConditionWithNumericRealValue_CallInsideScope_ShouldPerformInstructionsInIfBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field1",
                            Index = 2
                        }
                    },
                    ConditionType = ConditionType.LessEqualThan,
                    RightHandSideVariable = new NumericValue
                    {
                        RealValue = 8.256
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was true"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was false"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithGreaterThanConditionWithNumericRealValue_CallInsideScope_ShouldPerformInstructionsInElseBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field1",
                            Index = 2
                        }
                    },
                    ConditionType = ConditionType.GreaterThan,
                    RightHandSideVariable = new NumericValue
                    {
                        RealValue = 8.256
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was false"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was true"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithGreaterEqualThanConditionWithNumericRealValue_CallInsideScope_ShouldPerformInstructionsInElseBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field1",
                            Index = 2
                        }
                    },
                    ConditionType = ConditionType.GreaterEqualThan,
                    RightHandSideVariable = new NumericValue
                    {
                        RealValue = 8.256
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was false"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was true"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithGreaterEqualThanConditionWithNumericRealValue_CallInsideScopeWhereValueIsString_RuntimeExceptionIsThrown()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field2"
                        }
                    },
                    ConditionType = ConditionType.GreaterEqualThan,
                    RightHandSideVariable = new NumericValue
                    {
                        RealValue = 8.256
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            var exceptionWasThrown = false;
            try
            {
                ifInstruction.Execute(streamWriter.Object, null, 0, false);
            }
            catch (RuntimeException)
            {
                exceptionWasThrown = true;
            }

            // validate
            Assert.True(exceptionWasThrown);
        }

        [Fact]
        public void DefinedIfInstructionWithEqualConditionWithValueOf_CallInsideScopeWhereBothValuesAreStrings_ShouldPerformInstructionsInIfBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal', 'field3': 'literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field2"
                        }
                    },
                    ConditionType = ConditionType.Equal,
                    RightHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field3"
                        }
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was true"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was false"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithEqualConditionWithValueOf_CallInsideScopeWhereBothValuesAreStrings_ShouldPerformInstructionsInElseBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal', 'field3': 'different_literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field2"
                        }
                    },
                    ConditionType = ConditionType.Equal,
                    RightHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field3"
                        }
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was false"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was true"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithNotEqualConditionWithValueOf_CallInsideScopeWhereBothValuesAreStrings_ShouldPerformInstructionsInIfBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal', 'field3': 'different_literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field2"
                        }
                    },
                    ConditionType = ConditionType.NotEqual,
                    RightHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field3"
                        }
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was true"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was false"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithNotEqualConditionWithValueOf_CallInsideScopeWhereBothValuesAreStrings_ShouldPerformInstructionsInElseBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal', 'field3': 'literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field2"
                        }
                    },
                    ConditionType = ConditionType.NotEqual,
                    RightHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field3"
                        }
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was false"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was true"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithLessTahnConditionWithValueOf_CallInsideScopeWhereBothValuesAreStrings_RuntimeExceptionIsThrown()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal', 'field3': 'literal'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field2"
                        }
                    },
                    ConditionType = ConditionType.LessThan,
                    RightHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field3"
                        }
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            var exceptionWasThrown = false;
            try
            {

                ifInstruction.Execute(streamWriter.Object, null, 0, false);
            }
            catch (RuntimeException)
            {
                exceptionWasThrown = true;
            }

            // validate
            Assert.True(exceptionWasThrown);
            streamWriter.Verify(s => s.Write("condition was true"), Times.Never);
            streamWriter.Verify(s => s.Write("condition was false"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithLessTahnConditionWithValueOf_CallInsideScopeWhereOneValueIsStringAnotherIsNumber_RuntimeExceptionIsThrown()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'literal', 'field3': '5.0'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field2"
                        }
                    },
                    ConditionType = ConditionType.LessThan,
                    RightHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field3"
                        }
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());
            var exceptionWasThrown = false;
            try
            {

                ifInstruction.Execute(streamWriter.Object, null, 0, false);
            }
            catch (RuntimeException)
            {
                exceptionWasThrown = true;
            }

            // validate
            Assert.True(exceptionWasThrown);
            streamWriter.Verify(s => s.Write("condition was true"), Times.Never);
            streamWriter.Verify(s => s.Write("condition was false"), Times.Never);
        }

        [Fact]
        public void DefinedIfInstructionWithLessEqualThanConditionWithValueOf_CallInsideScopeWhereBothValuesAreNumbers_ShouldPerformIfBlock()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,0,8],'field2':'5', 'field3': '5.0'}")}
                }
            };
            var ifInstruction = new IfInstruction(outer_scope, new IfExpression
            {
                Condition = new ConditionWithValue
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field2"
                        }
                    },
                    ConditionType = ConditionType.LessEqualThan,
                    RightHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field3"
                        }
                    }

                }
            });
            ifInstruction.IfBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was true"
            }));
            ifInstruction.ElseBlock.Add(new LiteralInstruction(ifInstruction.Scope, new Literal
            {
                Content = "condition was false"
            }));

            // act
            var streamWriter = new Mock<StreamWriter>(new MemoryStream());

            ifInstruction.Execute(streamWriter.Object, null, 0, false);

            // validate
            streamWriter.Verify(s => s.Write("condition was true"), Times.Once);
            streamWriter.Verify(s => s.Write("condition was false"), Times.Never);
        }
    }
}