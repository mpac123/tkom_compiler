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
            Assert.Equal(typeof(Literal), ifExpression.Instructions.First().GetType());
            Assert.Equal("do something\n", ((Literal)ifExpression.Instructions.First()).Content);
            Assert.Equal(typeof(SimpleCondition), ifExpression.Condition.GetType());
            Assert.Equal(typeof(ValueOf), ifExpression.Condition.LeftHandSideVariable.GetType());
            Assert.Equal("cond", ifExpression.Condition.LeftHandSideVariable.VariableName);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
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
            Assert.Equal(typeof(Literal), ifExpression.Instructions.First().GetType());
            Assert.Equal("do something\n", ((Literal)ifExpression.Instructions.First()).Content);
            Assert.Equal(typeof(SimpleCondition), ifExpression.Condition.GetType());
            Assert.Equal(typeof(ValueOf), ifExpression.Condition.LeftHandSideVariable.GetType());
            Assert.Equal("cond", ifExpression.Condition.LeftHandSideVariable.VariableName);
            Assert.Equal(1, ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.NotNull(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal("value", ifExpression.Condition.LeftHandSideVariable.NestedValue.VariableName);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue.Index);
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
            Assert.Equal(typeof(Literal), ifExpression.Instructions.First().GetType());
            Assert.Equal("do something\n", ((Literal)ifExpression.Instructions.First()).Content);
            Assert.Equal(typeof(SimpleCondition), ifExpression.Condition.GetType());
            Assert.Equal(typeof(ValueOf), ifExpression.Condition.LeftHandSideVariable.GetType());
            Assert.Equal("cond", ifExpression.Condition.LeftHandSideVariable.VariableName);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
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
            Assert.Equal(typeof(Literal), ifExpression.Instructions.First().GetType());
            Assert.Equal("do something\n", ((Literal)ifExpression.Instructions.First()).Content);
            Assert.Equal(typeof(ConditionWithValue), ifExpression.Condition.GetType());
            var numericCondition = (ConditionWithValue)ifExpression.Condition;
            Assert.Equal(typeof(NumericValue), numericCondition.RightHandSideVariable.GetType());
            var numericValue = (NumericValue)numericCondition.RightHandSideVariable;
            Assert.True(numericValue.Integer);
            Assert.Equal(5, numericValue.IntValue);
            Assert.Equal(typeof(ValueOf), ifExpression.Condition.LeftHandSideVariable.GetType());
            Assert.Equal("cond", ifExpression.Condition.LeftHandSideVariable.VariableName);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.Equal, numericCondition.ConditionType);
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
            Assert.Equal(typeof(ConditionWithValue), ifExpression.Condition.GetType());
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.NotEqual, ((ConditionWithValue)ifExpression.Condition).ConditionType);
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
            Assert.Equal(typeof(ConditionWithValue), ifExpression.Condition.GetType());
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.LessThan, ((ConditionWithValue)ifExpression.Condition).ConditionType);
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
            Assert.Equal(typeof(ConditionWithValue), ifExpression.Condition.GetType());
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.LessEqualThan, ((ConditionWithValue)ifExpression.Condition).ConditionType);
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
            Assert.Equal(typeof(ConditionWithValue), ifExpression.Condition.GetType());
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.GreaterThan, ((ConditionWithValue)ifExpression.Condition).ConditionType);
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
            Assert.Equal(typeof(ConditionWithValue), ifExpression.Condition.GetType());
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.GreaterEqualThan, ((ConditionWithValue)ifExpression.Condition).ConditionType);
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
            Assert.Equal(typeof(Literal), ifExpression.Instructions.First().GetType());
            Assert.Equal("do something", ((Literal)ifExpression.Instructions.First()).Content);
            Assert.Equal(typeof(ConditionWithValue), ifExpression.Condition.GetType());
            var stringCondition = (ConditionWithValue)ifExpression.Condition;
            Assert.Equal(typeof(Literal), stringCondition.RightHandSideVariable.GetType());
            Assert.Equal("str ing", ((Literal)stringCondition.RightHandSideVariable).Content);
            Assert.Equal(typeof(ValueOf), ifExpression.Condition.LeftHandSideVariable.GetType());
            Assert.Equal("cond", ifExpression.Condition.LeftHandSideVariable.VariableName);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.NotEqual, ((ConditionWithValue)ifExpression.Condition).ConditionType);
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
            Assert.Equal(typeof(Literal), ifExpression.Instructions.First().GetType());
            Assert.Equal("do something", ((Literal)ifExpression.Instructions.First()).Content);
            Assert.Equal(typeof(ConditionWithValue), ifExpression.Condition.GetType());
            var variableCondition = (ConditionWithValue)ifExpression.Condition;
            Assert.Equal(typeof(ValueOf), variableCondition.RightHandSideVariable.GetType());
            var valueOf = (ValueOf)variableCondition.RightHandSideVariable;
            Assert.Equal("nested", valueOf.VariableName);
            Assert.Equal(0, valueOf.Index);
            Assert.Equal("val", valueOf.NestedValue.VariableName);
            Assert.Equal(typeof(ValueOf), ifExpression.Condition.LeftHandSideVariable.GetType());
            Assert.Equal("cond", ifExpression.Condition.LeftHandSideVariable.VariableName);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.Index);
            Assert.Null(ifExpression.Condition.LeftHandSideVariable.NestedValue);
            Assert.Equal(ConditionType.NotEqual, ((ConditionWithValue)ifExpression.Condition).ConditionType);
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

        [Fact]
        public void StreamWithFunctionWithIfElse_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n" +
                                                "<:if (cond == 5)>\n" +
                                                    "do something\n" +
                                                "</:if>\n" +
                                                "<:else>" +
                                                    "do something else" +
                                                "</:else>" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Equal(2, tree.Functions.First().Instructions.Count());
            Assert.Equal(typeof(IfExpression), tree.Functions.First().Instructions.First().GetType());
            Assert.Equal(typeof(ElseExpression), tree.Functions.First().Instructions.Skip(1).First().GetType());
            Assert.Equal("do something else", ((Literal)((ElseExpression)tree.Functions.First().Instructions.Skip(1).First()).Instructions.First()).Content);
        }

        [Fact]
        public void StreamWithFunctionWithIfAndEmptyElse_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n" +
                                                "<:if (cond == 5)>\n" +
                                                    "do something\n" +
                                                "</:if>\n" +
                                                "<:else>" +
                                                "</:else>" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Equal(2, tree.Functions.First().Instructions.Count());
            Assert.Equal(typeof(IfExpression), tree.Functions.First().Instructions.First().GetType());
            Assert.Equal(typeof(ElseExpression), tree.Functions.First().Instructions.Skip(1).First().GetType());
            Assert.Empty(((ElseExpression)tree.Functions.First().Instructions.Skip(1).First()).Instructions);
        }

        [Fact]
        public void StreamWithFunctionWithNestedIfElse_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n" +
                                                "<:if (cond == 5)>\n" +
                                                    "<:if (cond2)>" +
                                                        "do something" +
                                                    "</:if>" +
                                                    "<:else>" +
                                                        "do sth else" +
                                                    "</:else>" +
                                                "</:if>\n" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Single(tree.Functions.First().Instructions);
            var ifExpression = (IfExpression)tree.Functions.First().Instructions.First();
            Assert.Equal(typeof(IfExpression), ifExpression.Instructions.First().GetType());
            Assert.Equal(typeof(ElseExpression), ifExpression.Instructions.Skip(1).First().GetType());
        }

        [Fact]
        public void StreamWithFunctionWithForInBody_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n" +
                                                "<:for (element in arg1.collection)>\n" +
                                                    "<:if (cond2)>" +
                                                        "do something" +
                                                    "</:if>" +
                                                "</:for>\n" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Single(tree.Functions.First().Instructions);
            Assert.Equal(typeof(ForExpression), tree.Functions.First().Instructions.First().GetType());
            var forExpression = (ForExpression)tree.Functions.First().Instructions.First();
            Assert.Equal("element", forExpression.ElementName);
            Assert.Equal("arg1", forExpression.Collection.VariableName);
            Assert.Null(forExpression.Collection.Index);
            Assert.Equal("collection", forExpression.Collection.NestedValue.VariableName);
            Assert.Single(forExpression.Instructions);
            Assert.Equal(typeof(IfExpression), forExpression.Instructions.First().GetType());
        }

        [Fact]
        public void StreamWithFunctionWithEmptyForInBody_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n" +
                                                "<:for (element in arg1[1].collection)>\n" +
                                                "</:for>\n" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Single(tree.Functions.First().Instructions);
            Assert.Equal(typeof(ForExpression), tree.Functions.First().Instructions.First().GetType());
            var forExpression = (ForExpression)tree.Functions.First().Instructions.First();
            Assert.Equal("element", forExpression.ElementName);
            Assert.Equal("arg1", forExpression.Collection.VariableName);
            Assert.Equal(1, forExpression.Collection.Index);
            Assert.Equal("collection", forExpression.Collection.NestedValue.VariableName);
            Assert.Empty(forExpression.Instructions);
        }

        [Fact]
        public void StreamWithFunctionWithHtmlTagWithOneAttribute_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n" +
                                                "<a href=\"...\">" +
                                                    "link" +
                                                "</a>\n" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Single(tree.Functions.First().Instructions);
            Assert.Equal(typeof(HtmlTag), tree.Functions.First().Instructions.First().GetType());
            var forExpression = (HtmlTag)tree.Functions.First().Instructions.First();
            Assert.Equal("a", forExpression.TagName);
            Assert.Single(forExpression.Attributes);
            Assert.Equal("href", forExpression.Attributes.First().attributeName);
            Assert.Equal("...", forExpression.Attributes.First().attributeValue);
            Assert.NotEmpty(forExpression.Instructions);

        }

        [Fact]
        public void StreamWithFunctionWithEmptyHtmlTagWithOneAttribute_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n" +
                                                "<a href=\"...\"></a>\n" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Single(tree.Functions.First().Instructions);
            Assert.Equal(typeof(HtmlTag), tree.Functions.First().Instructions.First().GetType());
            var htmlExpression = (HtmlTag)tree.Functions.First().Instructions.First();
            Assert.Equal("a", htmlExpression.TagName);
            Assert.NotEmpty(htmlExpression.Attributes);
            Assert.Equal("href", htmlExpression.Attributes.First().attributeName);
            Assert.Equal("...", htmlExpression.Attributes.First().attributeValue);
            Assert.Empty(htmlExpression.Instructions);

        }

        [Fact]
        public void StreamWithFunctionWithHtmlTagWithManyAttributes_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n" +
                                                "<a href=\"...\" hidden>" +
                                                    "link" +
                                                "</a>\n" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Single(tree.Functions.First().Instructions);
            Assert.Equal(typeof(HtmlTag), tree.Functions.First().Instructions.First().GetType());
            var htmlExpression = (HtmlTag)tree.Functions.First().Instructions.First();
            Assert.Equal("a", htmlExpression.TagName);
            Assert.Equal(2, htmlExpression.Attributes.Count());
            Assert.Equal("href", htmlExpression.Attributes.First().attributeName);
            Assert.Equal("...", htmlExpression.Attributes.First().attributeValue);
            Assert.Equal("hidden", htmlExpression.Attributes.Skip(1).First().attributeName);
            Assert.Null(htmlExpression.Attributes.Skip(1).First().attributeValue);
            Assert.NotEmpty(htmlExpression.Instructions);

        }

        [Fact]
        public void StreamWithFunctionWithHtmlInlineTag_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n" +
                                                "<br/>" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Single(tree.Functions.First().Instructions);
            Assert.Equal(typeof(HtmlInlineTag), tree.Functions.First().Instructions.First().GetType());
            var htmlExpression = (HtmlInlineTag)tree.Functions.First().Instructions.First();
            Assert.Equal("br", htmlExpression.TagName);
            Assert.Empty(htmlExpression.Attributes);
        }

        [Fact]
        public void StreamWithFunctionWithHtmlInlineTagWithOneAttribute_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n" +
                                                "<br hidden/>" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Single(tree.Functions.First().Instructions);
            Assert.Equal(typeof(HtmlInlineTag), tree.Functions.First().Instructions.First().GetType());
            var htmlExpression = (HtmlInlineTag)tree.Functions.First().Instructions.First();
            Assert.Equal("br", htmlExpression.TagName);
            Assert.Single(htmlExpression.Attributes);
            Assert.Equal("hidden", htmlExpression.Attributes.First().attributeName);
            Assert.Null(htmlExpression.Attributes.First().attributeValue);
        }

        [Fact]
        public void StreamWithFunctionWithFunctionCall_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1)>\n" +
                                                "{function(call)}" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Single(tree.Functions.First().Instructions);
            Assert.Equal(typeof(FunctionCall), tree.Functions.First().Instructions.First().GetType());
            var functionCall = (FunctionCall)tree.Functions.First().Instructions.First();
            Assert.Equal("function", functionCall.FunctionName);
            Assert.Single(functionCall.ArgumentValues);
            Assert.Equal(typeof(ValueOf), functionCall.ArgumentValues.First().GetType());
            Assert.Equal("call", ((ValueOf)functionCall.ArgumentValues.First()).VariableName);
        }

        [Fact]
        public void StreamWithFunctionWithFunctionCallWithManyArguments_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1, arg2)>\n" +
                                                "{function(arg1[1], arg2.val)}" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Single(tree.Functions.First().Instructions);
            Assert.Equal(typeof(FunctionCall), tree.Functions.First().Instructions.First().GetType());
            var functionCall = (FunctionCall)tree.Functions.First().Instructions.First();
            Assert.Equal("function", functionCall.FunctionName);
            Assert.Equal(2, functionCall.ArgumentValues.Count());
            Assert.Equal(typeof(ValueOf), functionCall.ArgumentValues.First().GetType());
            Assert.Equal(typeof(ValueOf), functionCall.ArgumentValues.Skip(1).First().GetType());
            var valueOf1 = (ValueOf)functionCall.ArgumentValues.First();
            var valueOf2 = (ValueOf)functionCall.ArgumentValues.Skip(1).First();
            Assert.Equal("arg1", valueOf1.VariableName);
            Assert.Equal(1, valueOf1.Index);
            Assert.Equal("arg2", valueOf2.VariableName);
            Assert.Equal("val", valueOf2.NestedValue.VariableName);

        }

        [Fact]
        public void StreamWithFunctionWithFunctionCallWithStringAsArgument_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1, arg2)>\n" +
                                                "{function(\"string\")}" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Single(tree.Functions.First().Instructions);
            Assert.Equal(typeof(FunctionCall), tree.Functions.First().Instructions.First().GetType());
            var functionCall = (FunctionCall)tree.Functions.First().Instructions.First();
            Assert.Equal("function", functionCall.FunctionName);
            Assert.Equal(1, functionCall.ArgumentValues.Count());
            Assert.Equal(typeof(Literal), functionCall.ArgumentValues.First().GetType());
            var valueOf1 = (Literal)functionCall.ArgumentValues.First();
            Assert.Equal("string", valueOf1.Content);

        }

        [Fact]
        public void StreamWithFunctionWithFunctionCallWithIntAsArgument_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1, arg2)>\n" +
                                                "{function(5)}" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Single(tree.Functions.First().Instructions);
            Assert.Equal(typeof(FunctionCall), tree.Functions.First().Instructions.First().GetType());
            var functionCall = (FunctionCall)tree.Functions.First().Instructions.First();
            Assert.Equal("function", functionCall.FunctionName);
            Assert.Equal(1, functionCall.ArgumentValues.Count());
            Assert.Equal(typeof(NumericValue), functionCall.ArgumentValues.First().GetType());
            var valueOf1 = (NumericValue)functionCall.ArgumentValues.First();
            Assert.Equal(5, valueOf1.IntValue);
            Assert.True(valueOf1.Integer);

        }

        [Fact]
        public void StreamWithFunctionWithFunctionCallWithFloatAsArgument_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1, arg2)>\n" +
                                                "{function(5.256)}" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Single(tree.Functions.First().Instructions);
            Assert.Equal(typeof(FunctionCall), tree.Functions.First().Instructions.First().GetType());
            var functionCall = (FunctionCall)tree.Functions.First().Instructions.First();
            Assert.Equal("function", functionCall.FunctionName);
            Assert.Equal(1, functionCall.ArgumentValues.Count());
            Assert.Equal(typeof(NumericValue), functionCall.ArgumentValues.First().GetType());
            var valueOf1 = (NumericValue)functionCall.ArgumentValues.First();
            Assert.Equal(5.256, valueOf1.RealValue);
            Assert.False(valueOf1.Integer);

        }

        [Fact]
        public void StreamWithFunctionWithFunctionCallWithVariedArguments_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1, arg2)>\n" +
                                                "{function(5.256, \"string\", variable[100])}" +
                                            "</:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Single(tree.Functions.First().Instructions);
            Assert.Equal(typeof(FunctionCall), tree.Functions.First().Instructions.First().GetType());
            var functionCall = (FunctionCall)tree.Functions.First().Instructions.First();
            Assert.Equal("function", functionCall.FunctionName);
            Assert.Equal(3, functionCall.ArgumentValues.Count());
            Assert.Equal(typeof(NumericValue), functionCall.ArgumentValues.First().GetType());
            var valueOf1 = (NumericValue)functionCall.ArgumentValues.First();
            Assert.Equal(5.256, valueOf1.RealValue);
            Assert.False(valueOf1.Integer);
            Assert.Equal(typeof(Literal), functionCall.ArgumentValues.Skip(1).First().GetType());
            var valueOf2 = (Literal)functionCall.ArgumentValues.Skip(1).First();
            Assert.Equal("string", valueOf2.Content);
            Assert.Equal(typeof(ValueOf), functionCall.ArgumentValues.Skip(2).First().GetType());
            var valueOf3 = (ValueOf)functionCall.ArgumentValues.Skip(2).First();
            Assert.Equal("variable", valueOf3.VariableName);
            Assert.Null(valueOf3.NestedValue);
            Assert.Equal(100, valueOf3.Index);

        }

        [Fact]
        public void StreamWithTwoFunctions_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function(arg1, arg2)></:def>\n\n" +
                                            "   <:def function_two()><br/></:def>");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Equal(2, tree.Functions.Count());
            Assert.Equal("function", tree.Functions.First().Identifier);
            Assert.NotEmpty(tree.Functions.First().Arguments);
            Assert.Equal(2, tree.Functions.First().Arguments.Count());
            Assert.Equal("arg1", tree.Functions.First().Arguments.First());
            Assert.Equal("arg2", tree.Functions.First().Arguments.Skip(1).First());
            Assert.Empty(tree.Functions.First().Instructions);

            var fun2 = tree.Functions.Skip(1).First();
            Assert.Equal("function_two", fun2.Identifier);
            Assert.Empty(fun2.Arguments);
            Assert.Single(fun2.Instructions);
        }

        [Fact]
        public void StreamWithVeryNestedFunction_Parse_ReturnsCorrectAST()
        {
            // prepare
            var reader = new StringsReader("<:def function()>" +
                                                "<:if (cond)>" +
                                                    "<h1>literal{value}</h1>{fun(call)}\n" +
                                                    "<:for (a in value)>" +
                                                        "<br/>literal" +
                                                    "</:for>" +
                                                "</:if>" +
                                            "</:def>\n\n");
            var scanner = new Scanner(reader);
            var logger = new Mock<ILogger<Parser>>();
            var parser = new Parser(scanner, logger.Object);

            // act 
            var tree = parser.Parse();

            // validate
            Assert.NotEmpty(tree.Functions);
            Assert.Single(tree.Functions);
            Assert.Single(tree.Functions.First().Instructions);

            var ifInstruction = (IfExpression)tree.Functions.First().Instructions.First();
            Assert.Equal(3, ifInstruction.Instructions.Count());
            Assert.Equal(typeof(HtmlTag), ifInstruction.Instructions.First().GetType());
            Assert.Equal(typeof(FunctionCall), ifInstruction.Instructions.Skip(1).First().GetType());
            Assert.Equal(typeof(ForExpression), ifInstruction.Instructions.Skip(2).First().GetType());

            var htmlTag = (HtmlTag)ifInstruction.Instructions.First();
            Assert.Equal(2, htmlTag.Instructions.Count());
            Assert.Equal(typeof(Literal), htmlTag.Instructions.First().GetType());
            Assert.Equal(typeof(ValueOf), htmlTag.Instructions.Skip(1).First().GetType());

            var forInstruction = (ForExpression)ifInstruction.Instructions.Skip(2).First();
            Assert.Equal(2, forInstruction.Instructions.Count());
            Assert.Equal(typeof(HtmlInlineTag), forInstruction.Instructions.First().GetType());
            Assert.Equal(typeof(Literal), forInstruction.Instructions.Skip(1).First().GetType());


        }
    }
}