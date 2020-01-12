using System.Collections.Generic;
using System.Linq;
using TKOM.Exceptions;
using TKOM.Structures.AST;
using TKOM.Structures.IR;
using TKOM.Tools;
using Xunit;

namespace TKOM.Test.Tools
{
    public class SemanticsCheckerTest
    {
        [Fact]
        public void ASTTreeDeclaredWithMainAndValueCallInside_SemanticsCheckCalled_IRCorrect()
        {
            // prepare
            var program = new Program {
                Functions = new List<Function> {
                    new Function {
                        Identifier = "main",
                        Arguments = new List<string> { "model" },
                        Instructions = new List<IInstruction> {
                            new ValueOf {
                                VariableName = "model",
                                NestedValue = new ValueOf {
                                    VariableName = "nested_val"
                                }
                            }
                        }
                    }
                }
            };

            // act
            var sem_checker = new SemanticsChecker(program);
            var ir = sem_checker.CheckAST();

            // validate
            Assert.Single(ir);
            var mainBlock = ir.First().Value;
            Assert.Single(mainBlock.NestedBlocks);
            Assert.Single(mainBlock.ScopePrototype.Variables);
            Assert.Equal("model", mainBlock.ScopePrototype.Variables.First());
            Assert.Single(mainBlock.NestedBlocks.First().ScopePrototype.Variables);
            Assert.Equal("model", mainBlock.NestedBlocks.First().ScopePrototype.Variables.First());
        }

        [Fact]
        public void ASTTreeDeclaredWithoutMain_SemanticsCheckCalled_SemanticsExceptionIsCalled()
        {
            // prepare
            var program = new Program {
                Functions = new List<Function> {
                    new Function {
                        Identifier = "not_main",
                        Arguments = new List<string> { "model" },
                        Instructions = new List<IInstruction> {
                            new ValueOf {
                                VariableName = "model",
                                NestedValue = new ValueOf {
                                    VariableName = "nested_val"
                                }
                            }
                        }
                    }
                }
            };

            // act
            var exception_thrown = false;
            try {
            var sem_checker = new SemanticsChecker(program);
            var ir = sem_checker.CheckAST();
            }
            catch (SemanticsException)
            {
                exception_thrown = true;
            }

            // validate
            Assert.True(exception_thrown);
        }

        [Fact]
        public void ASTTreeDeclaredMainWithoutParameters_SemanticsCheckCalled_SemanticsExceptionIsCalled()
        {
            // prepare
            var program = new Program {
                Functions = new List<Function> {
                    new Function {
                        Identifier = "not_main",
                        Arguments = new List<string>(),
                        Instructions = new List<IInstruction> {
                            new ValueOf {
                                VariableName = "model",
                                NestedValue = new ValueOf {
                                    VariableName = "nested_val"
                                }
                            }
                        }
                    }
                }
            };

            // act
            var exception_thrown = false;
            try {
            var sem_checker = new SemanticsChecker(program);
            var ir = sem_checker.CheckAST();
            }
            catch (SemanticsException)
            {
                exception_thrown = true;
            }

            // validate
            Assert.True(exception_thrown);
        }

        [Fact]
        public void ASTTreeDeclaredMainWithManyParameters_SemanticsCheckCalled_SemanticsExceptionIsCalled()
        {
            // prepare
            var program = new Program {
                Functions = new List<Function> {
                    new Function {
                        Identifier = "not_main",
                        Arguments = new List<string> { "model", "unnecessary_arg" },
                        Instructions = new List<IInstruction> {
                            new ValueOf {
                                VariableName = "model",
                                NestedValue = new ValueOf {
                                    VariableName = "nested_val"
                                }
                            }
                        }
                    }
                }
            };

            // act
            var exception_thrown = false;
            try {
            var sem_checker = new SemanticsChecker(program);
            var ir = sem_checker.CheckAST();
            }
            catch (SemanticsException)
            {
                exception_thrown = true;
            }

            // validate
            Assert.True(exception_thrown);
        }

        [Fact]
        public void ASTTreeDeclaredNonexistingValue_SemanticsCheckCalled_SemanticsExceptionIsCalled()
        {
            // prepare
            var program = new Program {
                Functions = new List<Function> {
                    new Function {
                        Identifier = "not_main",
                        Arguments = new List<string> { "model" },
                        Instructions = new List<IInstruction> {
                            new ValueOf {
                                VariableName = "nonexisting_val",
                                NestedValue = new ValueOf {
                                    VariableName = "nested_val"
                                }
                            }
                        }
                    }
                }
            };

            // act
            var exception_thrown = false;
            try {
            var sem_checker = new SemanticsChecker(program);
            var ir = sem_checker.CheckAST();
            }
            catch (SemanticsException)
            {
                exception_thrown = true;
            }

            // validate
            Assert.True(exception_thrown);
        }

        [Fact]
        public void ASTTreeDeclaredWithTwoFunctions_SemanticsCheckCalled_IRCorrect()
        {
            // prepare
            var program = new Program {
                Functions = new List<Function> {
                    new Function {
                        Identifier = "main",
                        Arguments = new List<string> { "model" },
                        Instructions = new List<IInstruction> {
                            new ValueOf {
                                VariableName = "model",
                                NestedValue = new ValueOf {
                                    VariableName = "nested_val"
                                }
                            },
                            new FunctionCall {
                                FunctionName = "function",
                                ArgumentValues = new List<Value> {
                                    new NumericValue {
                                        Integer = true,
                                        IntValue = 5
                                    }
                                }
                            }
                        }
                    },
                    new Function {
                        Identifier = "function",
                        Arguments = new List<string> { "argument", "argument2" },
                        Instructions = new List<IInstruction> {
                            new ValueOf {
                                VariableName = "argument",
                                NestedValue = new ValueOf {
                                    VariableName = "nested_val"
                                }
                            }
                        }
                    }
                }
            };

            // act
            var sem_checker = new SemanticsChecker(program);
            var ir = sem_checker.CheckAST();

            // validate
            Assert.Equal(2, ir.Count());
            var mainBlock = ir.First().Value;
            var functionBlock = ir.Skip(1).First().Value;
            Assert.Equal(2, mainBlock.NestedBlocks.Count());
            Assert.Single(mainBlock.ScopePrototype.Variables);
            Assert.Equal("model", mainBlock.ScopePrototype.Variables.First());
            Assert.Single(mainBlock.NestedBlocks.First().ScopePrototype.Variables);
            Assert.Equal("model", mainBlock.NestedBlocks.First().ScopePrototype.Variables.First());

            Assert.Single(functionBlock.NestedBlocks);
            Assert.Equal(2, functionBlock.ScopePrototype.Variables.Count());
            Assert.Equal("argument", functionBlock.ScopePrototype.Variables.First());
        }

        [Fact]
        public void ASTTreeDeclaredForFunction_SemanticsCheckCalled_SemanticsExceptionIsCalled()
        {
            // prepare
            var program = new Program {
                Functions = new List<Function> {
                    new Function {
                        Identifier = "main",
                        Arguments = new List<string> { "model" },
                        Instructions = new List<IInstruction> {
                            new ForExpression {
                                Collection = new ValueOf {
                                    VariableName = "model",
                                    NestedValue = new ValueOf {
                                        VariableName = "nested"
                                    }
                                },
                                ElementName = "element",
                                Instructions = new List<IInstruction> {
                                    new ValueOf {
                                        VariableName = "element"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // act
            
            var sem_checker = new SemanticsChecker(program);
            var ir = sem_checker.CheckAST();
            

            // validate
            Assert.Single(ir.First().Value.NestedBlocks);

            var for_instr = (ForInstruction) ir.First().Value.NestedBlocks.First();
            Assert.Single(for_instr.Block.ScopePrototype.Variables);
            Assert.NotNull(for_instr.Block.ScopePrototype.UpperScopePrototype);
        }
    }
}