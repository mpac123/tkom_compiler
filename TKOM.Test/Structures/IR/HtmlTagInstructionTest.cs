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
            Assert.Equal("<div style=\"{color: \"blue\"}\">\n</div>", streamReader.ReadToEnd());

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
    }
}