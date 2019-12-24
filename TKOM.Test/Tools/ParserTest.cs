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
        public void StreamWithFunctionWithOneArgumentWithLiteralInBody_Parse_ReturnsCorrectAST()
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
            Assert.Equal("literal inside\n", ((Literal)tree.Functions.First().Instructions.First()).Content);
        }

        [Fact]
        public void StreamWithFunctionWithSimpleIfInBody_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n<:if (cond)>\ndo something\n</:if>\n</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.NotEmpty(tree.Functions.First().Instructions);
            Assert.Equal(typeof(IfExpression), tree.Functions.First().Instructions.First().GetType());
            var ifExpression = (IfExpression)tree.Functions.First().Instructions.First();
            Assert.Equal(1, ifExpression.Instructions.Count());
            Assert.Empty(ifExpression.InstructionsElse);
            Assert.Equal(typeof(Literal), ifExpression.Instructions.First().GetType());
            Assert.Equal("do something\n", ((Literal)ifExpression.Instructions.First()).Content);
            Assert.Equal(typeof(SimpleCondition), ifExpression.Condition.GetType());
            Assert.Equal(typeof(ValueOf), ifExpression.Condition.LeftHandSideVariable.GetType());
            Assert.Equal("cond", ifExpression.Condition.LeftHandSideVariable.VariableName);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Null(ifExpression.Condition.ConditionType);
        }

        [Fact]
        public void StreamWithFunctionWithSimpleIfWithNestedValueInBody_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n<:if (cond[1].value)>\ndo something\n</:if>\n</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.NotEmpty(tree.Functions.First().Instructions);
            Assert.Equal(typeof(IfExpression), tree.Functions.First().Instructions.First().GetType());
            var ifExpression = (IfExpression)tree.Functions.First().Instructions.First();
            Assert.Equal(1, ifExpression.Instructions.Count());
            Assert.Empty(ifExpression.InstructionsElse);
            Assert.Equal(typeof(Literal), ifExpression.Instructions.First().GetType());
            Assert.Equal("do something\n", ((Literal)ifExpression.Instructions.First()).Content);
            Assert.Equal(typeof(SimpleCondition), ifExpression.Condition.GetType());
            Assert.Equal(typeof(ValueOf), ifExpression.Condition.LeftHandSideVariable.GetType());
            Assert.Equal("cond", ifExpression.Condition.LeftHandSideVariable.VariableName);
            Assert.Equal(1, ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.NotNull(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal("value", ifExpression.Condition.LeftHandSideVariable.NestedValue.VariableName);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue.Index);
            Assert.Null(ifExpression.Condition.ConditionType);
        }

        [Fact]
        public void StreamWithFunctionWithSimpleIfWithNestedValueWithIndexInBody_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n<:if (cond[1].value[3])>\ndo something\n</:if>\n</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.NotEmpty(tree.Functions.First().Instructions);
            Assert.Equal(typeof(IfExpression), tree.Functions.First().Instructions.First().GetType());
            var ifExpression = (IfExpression)tree.Functions.First().Instructions.First();
            Assert.Equal(1, ifExpression.Instructions.Count());
            Assert.Empty(ifExpression.InstructionsElse);
            Assert.False(ifExpression.Negated);
            Assert.Equal(typeof(Literal), ifExpression.Instructions.First().GetType());
            Assert.Equal("do something\n", ((Literal)ifExpression.Instructions.First()).Content);
            Assert.Equal(typeof(SimpleCondition), ifExpression.Condition.GetType());
            Assert.Equal(typeof(ValueOf), ifExpression.Condition.LeftHandSideVariable.GetType());
            Assert.Equal("cond", ifExpression.Condition.LeftHandSideVariable.VariableName);
            Assert.Equal(1, ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.NotNull(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal("value", ifExpression.Condition.LeftHandSideVariable.NestedValue.VariableName);
            Assert.Equal(3, ifExpression.Condition.LeftHandSideVariable.NestedValue.Index);
            Assert.Null(ifExpression.Condition.ConditionType);
        }

        [Fact]
        public void StreamWithFunctionWithSimpleNegatedIfInBody_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n<:if (!cond)>\ndo something\n</:if>\n</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.NotEmpty(tree.Functions.First().Instructions);
            Assert.Equal(typeof(IfExpression), tree.Functions.First().Instructions.First().GetType());
            var ifExpression = (IfExpression)tree.Functions.First().Instructions.First();
            Assert.Equal(1, ifExpression.Instructions.Count());
            Assert.True(ifExpression.Negated);
            Assert.Empty(ifExpression.InstructionsElse);
            Assert.Equal(typeof(Literal), ifExpression.Instructions.First().GetType());
            Assert.Equal("do something\n", ((Literal)ifExpression.Instructions.First()).Content);
            Assert.Equal(typeof(SimpleCondition), ifExpression.Condition.GetType());
            Assert.Equal(typeof(ValueOf), ifExpression.Condition.LeftHandSideVariable.GetType());
            Assert.Equal("cond", ifExpression.Condition.LeftHandSideVariable.VariableName);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Null(ifExpression.Condition.ConditionType);
        }

        [Fact]
        public void StreamWithFunctionWithSimpleNegatedIfWithNestedValueWithIndexInBody_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n<:if (!cond[1].value[3])>\ndo something\n</:if>\n</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.NotEmpty(tree.Functions.First().Instructions);
            Assert.Equal(typeof(IfExpression), tree.Functions.First().Instructions.First().GetType());
            var ifExpression = (IfExpression)tree.Functions.First().Instructions.First();
            Assert.Equal(1, ifExpression.Instructions.Count());
            Assert.Empty(ifExpression.InstructionsElse);
            Assert.True(ifExpression.Negated);
            Assert.Equal(typeof(Literal), ifExpression.Instructions.First().GetType());
            Assert.Equal("do something\n", ((Literal)ifExpression.Instructions.First()).Content);
            Assert.Equal(typeof(SimpleCondition), ifExpression.Condition.GetType());
            Assert.Equal(typeof(ValueOf), ifExpression.Condition.LeftHandSideVariable.GetType());
            Assert.Equal("cond", ifExpression.Condition.LeftHandSideVariable.VariableName);
            Assert.Equal(1, ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.NotNull(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal("value", ifExpression.Condition.LeftHandSideVariable.NestedValue.VariableName);
            Assert.Equal(3, ifExpression.Condition.LeftHandSideVariable.NestedValue.Index);
            Assert.Null(ifExpression.Condition.ConditionType);
        }

        [Fact]
        public void StreamWithFunctionWithIfWithIntInBody_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n<:if (cond == 5)>\ndo something\n</:if>\n</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.NotEmpty(tree.Functions.First().Instructions);
            Assert.Equal(typeof(IfExpression), tree.Functions.First().Instructions.First().GetType());
            var ifExpression = (IfExpression)tree.Functions.First().Instructions.First();
            Assert.Equal(1, ifExpression.Instructions.Count());
            Assert.Empty(ifExpression.InstructionsElse);
            Assert.Equal(typeof(Literal), ifExpression.Instructions.First().GetType());
            Assert.Equal("do something\n", ((Literal)ifExpression.Instructions.First()).Content);
            Assert.Equal(typeof(ConditionWithNumericValue), ifExpression.Condition.GetType());
            var numericCondition = (ConditionWithNumericValue)ifExpression.Condition;
            Assert.True(numericCondition.Integer);
            Assert.Equal(5, numericCondition.IntValue);
            Assert.Equal(typeof(ValueOf), ifExpression.Condition.LeftHandSideVariable.GetType());
            Assert.Equal("cond", ifExpression.Condition.LeftHandSideVariable.VariableName);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.Equal, ifExpression.Condition.ConditionType);
        }

        [Fact]
        public void StreamWithFunctionWithIfNotEqual_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n<:if (cond != 5)>\ndo something\n</:if>\n</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            var ifExpression = (IfExpression)tree.Functions.First().Instructions.First();
            Assert.Equal(typeof(ConditionWithNumericValue), ifExpression.Condition.GetType());
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.NotEqual, ifExpression.Condition.ConditionType);
        }

        [Fact]
        public void StreamWithFunctionWithIfLessThan_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n<:if (cond < 5)>\ndo something\n</:if>\n</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            var ifExpression = (IfExpression)tree.Functions.First().Instructions.First();
            Assert.Equal(typeof(ConditionWithNumericValue), ifExpression.Condition.GetType());
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.LessThan, ifExpression.Condition.ConditionType);
        }

        [Fact]
        public void StreamWithFunctionWithIfLessEqualThan_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n<:if (cond <= 5)>\ndo something\n</:if>\n</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            var ifExpression = (IfExpression)tree.Functions.First().Instructions.First();
            Assert.Equal(typeof(ConditionWithNumericValue), ifExpression.Condition.GetType());
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.LessEqualThan, ifExpression.Condition.ConditionType);
        }

        [Fact]
        public void StreamWithFunctionWithIfGreaterThan_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n<:if (cond > 5)>\ndo something\n</:if>\n</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            var ifExpression = (IfExpression)tree.Functions.First().Instructions.First();
            Assert.Equal(typeof(ConditionWithNumericValue), ifExpression.Condition.GetType());
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.GreaterThan, ifExpression.Condition.ConditionType);
        }

        [Fact]
        public void StreamWithFunctionWithIfGreaterEqualThan_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n<:if (cond >= 5)>\ndo something\n</:if>\n</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            var ifExpression = (IfExpression)tree.Functions.First().Instructions.First();
            Assert.Equal(typeof(ConditionWithNumericValue), ifExpression.Condition.GetType());
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.GreaterEqualThan, ifExpression.Condition.ConditionType);
        }

        [Fact]
        public void StreamWithFunctionWithIfWithString_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader(@"<:def function(arg1)><:if (cond != ""str ing"")>do something</:if></:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.NotEmpty(tree.Functions.First().Instructions);
            Assert.Equal(typeof(IfExpression), tree.Functions.First().Instructions.First().GetType());
            var ifExpression = (IfExpression)tree.Functions.First().Instructions.First();
            Assert.Equal(1, ifExpression.Instructions.Count());
            Assert.Empty(ifExpression.InstructionsElse);
            Assert.Equal(typeof(Literal), ifExpression.Instructions.First().GetType());
            Assert.Equal("do something", ((Literal)ifExpression.Instructions.First()).Content);
            Assert.Equal(typeof(ConditionWithString), ifExpression.Condition.GetType());
            var stringCondition = (ConditionWithString)ifExpression.Condition;
            Assert.Equal("str ing", stringCondition.RightHandSideValue);
            Assert.Equal(typeof(ValueOf), ifExpression.Condition.LeftHandSideVariable.GetType());
            Assert.Equal("cond", ifExpression.Condition.LeftHandSideVariable.VariableName);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.NotEqual, ifExpression.Condition.ConditionType);
        }

        [Fact]
        public void StreamWithFunctionWithIfWithVariable_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader(@"<:def function(arg1)><:if (cond != nested[0].val)>do something</:if></:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.NotEmpty(tree.Functions.First().Instructions);
            Assert.Equal(typeof(IfExpression), tree.Functions.First().Instructions.First().GetType());
            var ifExpression = (IfExpression)tree.Functions.First().Instructions.First();
            Assert.Equal(1, ifExpression.Instructions.Count());
            Assert.Empty(ifExpression.InstructionsElse);
            Assert.Equal(typeof(Literal), ifExpression.Instructions.First().GetType());
            Assert.Equal("do something", ((Literal)ifExpression.Instructions.First()).Content);
            Assert.Equal(typeof(ConditionWithVariable), ifExpression.Condition.GetType());
            var variableCondition = (ConditionWithVariable)ifExpression.Condition;
            Assert.Equal("nested", variableCondition.RightHandSideVariable.VariableName);
            Assert.Equal(0, variableCondition.RightHandSideVariable.Index);
            Assert.Equal("val", variableCondition.RightHandSideVariable.NestedValue.VariableName);
            Assert.Equal(typeof(ValueOf), ifExpression.Condition.LeftHandSideVariable.GetType());
            Assert.Equal("cond", ifExpression.Condition.LeftHandSideVariable.VariableName);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.NotEqual, ifExpression.Condition.ConditionType);
        }

        [Fact]
        public void StreamWithFunctionWithTwoIfsAndLiteralInBody_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n" +
                                                "<:if (cond == 5)>\n" + 
                                                    "do something\n" + 
                                                "</:if>\n" +
                                                "literal" +
                                                "<:if (cond2)></:if>" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Equal(3, tree.Functions.First().Instructions.Count());
            Assert.Equal(typeof(IfExpression), tree.Functions.First().Instructions.First().GetType());
            Assert.Equal(typeof(Literal), tree.Functions.First().Instructions.Skip(1).First().GetType());
            Assert.Equal(typeof(IfExpression), tree.Functions.First().Instructions.Skip(2).First().GetType());
        }


    }
}