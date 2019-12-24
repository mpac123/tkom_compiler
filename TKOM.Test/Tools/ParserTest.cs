using TKOM.Readers;
using TKOM.Tools;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System.Linq;
using TKOM.AST;

namespace TKOM.Test.Tools
{
    public class ParserTest
    {
        [Fact]
        public void EmptyStream_Parse_ReturnsEmptyASTTree()
        {
            // prepare
            var reader = new StringsReader("");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.Empty(tree.Functions);
        }

        [Fact]
        public void StreamWithFunctionWithNoArgumentsWithoutBody_Parse_ReturnsEmptyASTTree()
        {
            // prepare
            var reader = new StringsReader("<:def function()></:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Equal("function", tree.Functions.First().Identifier);
            Assert.Empty(tree.Functions.First().Arguments);
            Assert.Empty(tree.Functions.First().Instructions);
        }

        [Fact]
        public void StreamWithFunctionWithOneArgumentWithoutBody_Parse_ReturnsEmptyASTTree()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)></:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Equal("function", tree.Functions.First().Identifier);
            Assert.NotEmpty(tree.Functions.First().Arguments);
            Assert.Equal(1, tree.Functions.First().Arguments.Count());
            Assert.Equal("arg1", tree.Functions.First().Arguments.First());
            Assert.Empty(tree.Functions.First().Instructions);
        }

        [Fact]
        public void StreamWithFunctionWithArgumentsWithoutBody_Parse_ReturnsEmptyASTTree()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1, arg2)></:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Equal("function", tree.Functions.First().Identifier);
            Assert.NotEmpty(tree.Functions.First().Arguments);
            Assert.Equal(2, tree.Functions.First().Arguments.Count());
            Assert.Equal("arg1", tree.Functions.First().Arguments.First());
            Assert.Equal("arg2", tree.Functions.First().Arguments.Skip(1).First());
            Assert.Empty(tree.Functions.First().Instructions);
        }

        [Fact]
        public void StreamWithFunctionWithOneArgumentWithNewLineInBody_Parse_ReturnsEmptyASTTree()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Equal("function", tree.Functions.First().Identifier);
            Assert.NotEmpty(tree.Functions.First().Arguments);
            Assert.Equal(1, tree.Functions.First().Arguments.Count());
            Assert.Equal("arg1", tree.Functions.First().Arguments.First());
            Assert.Empty(tree.Functions.First().Instructions);
        }

        [Fact]
        public void StreamWithFunctionWithOneArgumentWithLiteralInBody_Parse_ReturnsEmptyASTTree()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\nliteral inside\n</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Equal("function", tree.Functions.First().Identifier);
            Assert.NotEmpty(tree.Functions.First().Arguments);
            Assert.Equal(1, tree.Functions.First().Arguments.Count());
            Assert.Equal("arg1", tree.Functions.First().Arguments.First());
            Assert.NotEmpty(tree.Functions.First().Instructions);
            Assert.Equal(typeof(Literal), tree.Functions.First().Instructions.First().GetType());
            Assert.Equal("literal inside\n", ((Literal) tree.Functions.First().Instructions.First()).Content);
        }
    }
}