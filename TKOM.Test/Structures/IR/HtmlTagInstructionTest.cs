using System.Collections.Generic;
using System.IO;
using Moq;
using TKOM.Structures.AST;
using TKOM.Structures.IR;
using Xunit;

namespace TKOM.Test.Structures.IR
{
    public class HtmlTagInstructionTest
    {
        [Fact]
        public void DefinedHtmlTag_Call_TagPrintedCorrectly()
        {
            // prepare
            var htmlInstruction = new HtmlTagInstruction(null, new HtmlTag
            {
                TagName = "div",
                Attributes = new List<(string, StringValue)> {
                    ("style", new StringValue {
                        StringComponents = new List<IStringComponent> {
                            new Literal {
                                Content = "{color: \"blue\"}"
                                }
                            }
                        })
                }
            });

            // act
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            htmlInstruction.Execute(streamWriter, null, 0, false);
            streamWriter.Flush();

            // validate
            memoryStream.Position = 0;
            var streamReader = new StreamReader(memoryStream);
            Assert.Equal("<div style=\"{color: \"blue\"}\"></div>", streamReader.ReadToEnd());

        }

        [Fact]
        public void DefinedHtmlTagWithNestedValue_Call_TagAndValuePrintedCorrectly()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,5,8],'field2':'val2'}")}
                }
            };
            var htmlInstruction = new HtmlTagInstruction(outer_scope, new HtmlTag
            {
                TagName = "div",
                Attributes = new List<(string, StringValue)> {
                    ("style", new StringValue {
                        StringComponents = new List<IStringComponent> {
                            new Literal {
                                Content = "{color: \"blue\"}"
                                }
                            }
                        })
                }
            });
            htmlInstruction.Block.Add(new ValueOfInstruction(outer_scope, new ValueOf
            {
                VariableName = "model",
                NestedValue = new ValueOf
                {
                    VariableName = "field2"
                }
            }));

            // act
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            htmlInstruction.Execute(streamWriter, null, 0, false);
            streamWriter.Flush();

            // validate
            memoryStream.Position = 0;
            var streamReader = new StreamReader(memoryStream);
            Assert.Equal("<div style=\"{color: \"blue\"}\">val2</div>", streamReader.ReadToEnd());

        }

        [Fact]
        public void HtmlTagWithLiteralInside_Execute_TagAndLiteralInOneLine()
        {
            // prepare

            var htmlInstruction = new HtmlTagInstruction(null, new HtmlTag
            {
                TagName = "div",
            });
            htmlInstruction.Block.Add(new LiteralInstruction(htmlInstruction.Scope, new Literal
            {
                Content = "literal"
            }));

            // act
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            htmlInstruction.Execute(streamWriter, null, 0, false);
            streamWriter.Flush();

            // validate
            memoryStream.Position = 0;
            var streamReader = new StreamReader(memoryStream);
            Assert.Equal("<div>literal</div>", streamReader.ReadToEnd());

        }

        [Fact]
        public void HtmlTagWithLiteralAndNestedHtmlTagInside_Execute_NestedTagAndLiteralInSeperateLines()
        {
            // prepare

            var htmlInstruction = new HtmlTagInstruction(null, new HtmlTag
            {
                TagName = "div",
            });
            htmlInstruction.Block.Add(new LiteralInstruction(htmlInstruction.Scope, new Literal
            {
                Content = "literal"
            }));
            htmlInstruction.Block.Add(new HtmlInlineTagInstruction(htmlInstruction.Scope, new HtmlInlineTag
            {
                TagName = "br"
            }));

            // act
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            htmlInstruction.Execute(streamWriter, null, 0, false);
            streamWriter.Flush();

            // validate
            memoryStream.Position = 0;
            var streamReader = new StreamReader(memoryStream);
            Assert.Equal("<div>\n  literal\n  <br/>\n</div>", streamReader.ReadToEnd());

        }

        [Fact]
        public void HtmlTagWithLiteralAndNestedIfInside_Execute_NestedTagAndLiteralInSeperateLines()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,5,8],'field2':'val2'}")}
                }
            };

            var htmlInstruction = new HtmlTagInstruction(outer_scope, new HtmlTag
            {
                TagName = "div",
            });
            htmlInstruction.Block.Add(new LiteralInstruction(htmlInstruction.Scope, new Literal
            {
                Content = "literal"
            }));
            var ifInstruction = new IfInstruction(htmlInstruction.Scope, new IfExpression
            {
                Condition = new SimpleCondition
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field1",
                            Index = 0
                        }
                    }
                }
            });
            ifInstruction.IfBlock = new List<Executable> {
                new LiteralInstruction(ifInstruction.Scope, new Literal {
                    Content = " nested literal"
                })
            };
            htmlInstruction.Block.Add(ifInstruction);

            // act
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            htmlInstruction.Execute(streamWriter, null, 0, false);
            streamWriter.Flush();

            // validate
            memoryStream.Position = 0;
            var streamReader = new StreamReader(memoryStream);
            Assert.Equal("<div>\n  literal nested literal\n</div>", streamReader.ReadToEnd());

        }

        [Fact]
        public void HtmlTagWithLiteralAndNestedNegatedIfInside_Execute_LiteralInSeperateLine()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,5,8],'field2':'val2'}")}
                }
            };

            var htmlInstruction = new HtmlTagInstruction(outer_scope, new HtmlTag
            {
                TagName = "div",
            });
            htmlInstruction.Block.Add(new LiteralInstruction(htmlInstruction.Scope, new Literal
            {
                Content = "literal"
            }));
            var ifInstruction = new IfInstruction(htmlInstruction.Scope, new IfExpression
            {
                Condition = new SimpleCondition
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field1",
                            Index = 0
                        }
                    }
                },
                Negated = true
            });
            ifInstruction.IfBlock = new List<Executable> {
                new LiteralInstruction(ifInstruction.Scope, new Literal {
                    Content = " nested literal"
                })
            };
            htmlInstruction.Block.Add(ifInstruction);

            // act
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            htmlInstruction.Execute(streamWriter, null, 0, false);
            streamWriter.Flush();

            // validate
            memoryStream.Position = 0;
            var streamReader = new StreamReader(memoryStream);
            Assert.Equal("<div>\n  literal\n</div>", streamReader.ReadToEnd());

        }

        [Fact]
        public void HtmlTagWithLiteralAndNestedIfWithHtmlTagInside_Execute_NestedTagAndLiteralInSeperateLines()
        {
            // prepare
            var outer_scope = new Scope
            {
                Variables = new HashSet<string> { "model" },
                VariableValues = new Dictionary<string, AssignedValue> {
                    {"model", new AssignedValue("{'field1':[2,5,8],'field2':'val2'}")}
                }
            };

            var htmlInstruction = new HtmlTagInstruction(outer_scope, new HtmlTag
            {
                TagName = "div",
            });
            htmlInstruction.Block.Add(new LiteralInstruction(htmlInstruction.Scope, new Literal
            {
                Content = "literal"
            }));
            var ifInstruction = new IfInstruction(htmlInstruction.Scope, new IfExpression
            {
                Condition = new SimpleCondition
                {
                    LeftHandSideVariable = new ValueOf
                    {
                        VariableName = "model",
                        NestedValue = new ValueOf
                        {
                            VariableName = "field1",
                            Index = 0
                        }
                    }
                }
            });
            ifInstruction.IfBlock = new List<Executable> {
                new HtmlInlineTagInstruction(ifInstruction.Scope, new HtmlInlineTag {
                    TagName = "br"
                })
            };
            htmlInstruction.Block.Add(ifInstruction);

            // act
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            htmlInstruction.Execute(streamWriter, null, 0, false);
            streamWriter.Flush();

            // validate
            memoryStream.Position = 0;
            var streamReader = new StreamReader(memoryStream);
            Assert.Equal("<div>\n  literal\n  <br/>\n</div>", streamReader.ReadToEnd());

        }
    }
}