using Xunit;
using TKOM.Test.Tools.StreamGenerator;
using TKOM.Parser;
using System.IO;
using static TKOM.Parser.Token;
using System.Collections.Generic;

namespace TKOM.Test.Parser
{
    public class ScannerTest
    {
        [Fact]
        public void ReadsEof()
        {
            using (var stream = StreamGenerator.GenerateStreamFromString(""))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                scanner.ReadNextToken();
                var token = scanner.Token;

                // validate
                Assert.Equal(TokenType.Eof, token.Type);
            }
        }

        [Fact]
        public void ReadsFunctionOpenToken()
        {
            using (var stream = StreamGenerator.GenerateStreamFromString("<:def "))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                scanner.ReadNextToken();
                var first_token = scanner.Token;
                scanner.ReadNextToken();
                var second_token = scanner.Token;

                // validate
                Assert.Equal(TokenType.FunctionOpen, first_token.Type);
                Assert.Equal(TokenType.Eof, second_token.Type);
            }
        }

        [Fact]
        public void ReadsFunctionCloseToken()
        {
            using (var stream = StreamGenerator.GenerateStreamFromString("</:def>"))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                scanner.ReadNextToken();
                var first_token = scanner.Token;
                scanner.ReadNextToken();
                var second_token = scanner.Token;

                // validate
                Assert.Equal(TokenType.FunctionClose, first_token.Type);
                Assert.Equal(TokenType.Eof, second_token.Type);
            }
        }

        [Fact]
        public void ReadFunctionWithOneArgument()
        {
            var s = @"<:def function(arg1)>";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                int size_array = 7;
                Token[] array_of_tokens = new Token[size_array];
                for (int i = 0; i < size_array; i++)
                {
                    scanner.ReadNextToken();
                    array_of_tokens[i] = scanner.Token;
                }

                // validate
                Assert.Equal(TokenType.FunctionOpen, array_of_tokens[0].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[1].Type);
                Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[2].Type);

                Assert.Equal(TokenType.Identifier, array_of_tokens[3].Type);
                Assert.Equal("arg1", array_of_tokens[3].Value);

                Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[4].Type);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[5].Type);

                Assert.Equal(TokenType.Eof, array_of_tokens[6].Type);
            }
        }

        [Fact]
        public void ReadsAllTokensInFunction()
        {
            var s = @"<:def function(arg1)>
whatever inside
</:def>";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                var array_size = 10;
                Token[] array_of_tokens = new Token[array_size];
                for (int i = 0; i < array_size; i++)
                {
                    scanner.ReadNextToken();
                    array_of_tokens[i] = scanner.Token;
                }

                // validate
                Assert.Equal(TokenType.FunctionOpen, array_of_tokens[0].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[1].Type);
                Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[2].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[3].Type);
                Assert.Equal("arg1", array_of_tokens[3].Value);
                Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[4].Type);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[5].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[6].Type);
                Assert.Equal("whatever", array_of_tokens[6].Value);
                Assert.Equal(TokenType.Text, array_of_tokens[7].Type);
                Assert.Equal("inside\n", array_of_tokens[7].Value);
                Assert.Equal(TokenType.FunctionClose, array_of_tokens[8].Type);
                Assert.Equal(TokenType.Eof, array_of_tokens[9].Type);
            }
        }

        [Fact]
        public void ReadFunctionWithMultipleArguments()
        {
            var s = @"<:def function(arg1, _arg2, _3)>";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                int size_array = 11;
                Token[] array_of_tokens = new Token[size_array];
                for (int i = 0; i < size_array; i++)
                {
                    scanner.ReadNextToken();
                    array_of_tokens[i] = scanner.Token;
                }

                // validate
                Assert.Equal(TokenType.FunctionOpen, array_of_tokens[0].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[1].Type);
                Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[2].Type);

                Assert.Equal(TokenType.Identifier, array_of_tokens[3].Type);
                Assert.Equal("arg1", array_of_tokens[3].Value);

                Assert.Equal(TokenType.Coma, array_of_tokens[4].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[5].Type);
                Assert.Equal("_arg2", array_of_tokens[5].Value);

                Assert.Equal(TokenType.Coma, array_of_tokens[6].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[7].Type);
                Assert.Equal("_3", array_of_tokens[7].Value);

                Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[8].Type);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[9].Type);

                Assert.Equal(TokenType.Eof, array_of_tokens[10].Type);
            }
        }

        [Fact]
        public void ReadsAllTokensInFunctionWithHtmlTagsInside()
        {
            var s = @"<:def function()>
<h>HTML inside</h>
</:def>";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                var array_size = 15;
                Token[] array_of_tokens = new Token[array_size];
                for (int i = 0; i < array_size; i++)
                {
                    scanner.ReadNextToken();
                    array_of_tokens[i] = scanner.Token;
                }

                // validate
                Assert.Equal(TokenType.FunctionOpen, array_of_tokens[0].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[1].Type);
                Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[2].Type);
                Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[3].Type);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[4].Type);
                Assert.Equal(TokenType.LessThan, array_of_tokens[5].Type);
                Assert.Equal(TokenType.Text, array_of_tokens[6].Type);
                Assert.Equal("h", array_of_tokens[6].Value);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[7].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[8].Type);
                Assert.Equal("HTML", array_of_tokens[8].Value);
                Assert.Equal(TokenType.Text, array_of_tokens[9].Type);
                Assert.Equal("inside", array_of_tokens[9].Value);
                Assert.Equal(TokenType.LessThan, array_of_tokens[10].Type);
                Assert.Equal(TokenType.Text, array_of_tokens[11].Type);
                Assert.Equal("/h", array_of_tokens[11].Value);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[12].Type);
                Assert.Equal(TokenType.FunctionClose, array_of_tokens[13].Type);
                Assert.Equal(TokenType.Eof, array_of_tokens[14].Type);
            }
        }

        [Fact]
        public void EscapeCharacters()
        {
            var s = "<:def function()>\n\\<:def >\n</:def>";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                var array_size = 10;
                Token[] array_of_tokens = new Token[array_size];
                for (int i = 0; i < array_size; i++)
                {
                    scanner.ReadNextToken();
                    array_of_tokens[i] = scanner.Token;
                }

                // validate
                Assert.Equal(TokenType.FunctionOpen, array_of_tokens[0].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[1].Type);
                Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[2].Type);
                Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[3].Type);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[4].Type);
                Assert.Equal(TokenType.Text, array_of_tokens[5].Type);
                Assert.Equal("<:def ", array_of_tokens[5].Value);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[6].Type);
                Assert.Equal(TokenType.FunctionClose, array_of_tokens[7].Type);
                Assert.Equal(TokenType.Eof, array_of_tokens[8].Type);
            }
        }

        [Fact]
        public void EscapeEscapeCharacters()
        {
            var s = "<:def function()>\n\\\\<:def \n</:def>";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                var array_size = 10;
                Token[] array_of_tokens = new Token[array_size];
                for (int i = 0; i < array_size; i++)
                {
                    scanner.ReadNextToken();
                    array_of_tokens[i] = scanner.Token;
                }

                // validate
                Assert.Equal(TokenType.FunctionOpen, array_of_tokens[0].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[1].Type);
                Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[2].Type);
                Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[3].Type);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[4].Type);
                Assert.Equal(TokenType.Text, array_of_tokens[5].Type);
                Assert.Equal("\\", array_of_tokens[5].Value);
                Assert.Equal(TokenType.FunctionOpen, array_of_tokens[6].Type);
                Assert.Equal(TokenType.FunctionClose, array_of_tokens[7].Type);
                Assert.Equal(TokenType.Eof, array_of_tokens[8].Type);
            }
        }

        [Fact]
        public void ReadsIdentifierInCurlyBrackets()
        {
            var s = "<h>{identifier}</h>";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                var array_size = 10;
                Token[] array_of_tokens = new Token[array_size];
                for (int i = 0; i < array_size; i++)
                {
                    scanner.ReadNextToken();
                    array_of_tokens[i] = scanner.Token;
                }

                // validate
                Assert.Equal(TokenType.LessThan, array_of_tokens[0].Type);
                Assert.Equal(TokenType.Text, array_of_tokens[1].Type);
                Assert.Equal("h", array_of_tokens[1].Value);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[2].Type);
                Assert.Equal(TokenType.CurlyBracketOpen, array_of_tokens[3].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[4].Type);
                Assert.Equal("identifier", array_of_tokens[4].Value);
                Assert.Equal(TokenType.CurlyBracketClose, array_of_tokens[5].Type);
                Assert.Equal(TokenType.LessThan, array_of_tokens[6].Type);
                Assert.Equal(TokenType.Text, array_of_tokens[7].Type);
                Assert.Equal("/h", array_of_tokens[7].Value);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[8].Type);
                Assert.Equal(TokenType.Eof, array_of_tokens[9].Type);
            }
        }

        [Fact]
        public void EscapesCurlyBrackets()
        {
            var s = "<h>\\{identifier\\}</h>";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                var array_size = 8;
                Token[] array_of_tokens = new Token[array_size];
                for (int i = 0; i < array_size; i++)
                {
                    scanner.ReadNextToken();
                    array_of_tokens[i] = scanner.Token;
                }

                // validate
                Assert.Equal(TokenType.LessThan, array_of_tokens[0].Type);
                Assert.Equal(TokenType.Text, array_of_tokens[1].Type);
                Assert.Equal("h", array_of_tokens[1].Value);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[2].Type);
                Assert.Equal(TokenType.Text, array_of_tokens[3].Type);
                Assert.Equal("{identifier}", array_of_tokens[3].Value);
                Assert.Equal(TokenType.LessThan, array_of_tokens[4].Type);
                Assert.Equal(TokenType.Text, array_of_tokens[5].Type);
                Assert.Equal("/h", array_of_tokens[5].Value);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[6].Type);
                Assert.Equal(TokenType.Eof, array_of_tokens[7].Type);
            }
        }

        [Fact]
        public void ReadsIdentifierInCurlyBracketsWithSpaces()
        {
            var s = "<h> { identifier } </h>";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                var array_size = 10;
                Token[] array_of_tokens = new Token[array_size];
                for (int i = 0; i < array_size; i++)
                {
                    scanner.ReadNextToken();
                    array_of_tokens[i] = scanner.Token;
                }

                // validate
                Assert.Equal(TokenType.LessThan, array_of_tokens[0].Type);
                Assert.Equal(TokenType.Text, array_of_tokens[1].Type);
                Assert.Equal("h", array_of_tokens[1].Value);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[2].Type);
                Assert.Equal(TokenType.CurlyBracketOpen, array_of_tokens[3].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[4].Type);
                Assert.Equal("identifier", array_of_tokens[4].Value);
                Assert.Equal(TokenType.CurlyBracketClose, array_of_tokens[5].Type);
                Assert.Equal(TokenType.LessThan, array_of_tokens[6].Type);
                Assert.Equal(TokenType.Text, array_of_tokens[7].Type);
                Assert.Equal("/h", array_of_tokens[7].Value);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[8].Type);
                Assert.Equal(TokenType.Eof, array_of_tokens[9].Type);
            }
        }

        [Fact]
        public void ReadsIfExpression()
        {
            var s = "<:if (a.is_potentially_hazardous_asteroid)>";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                var array_size = 8;
                Token[] array_of_tokens = new Token[array_size];
                for (int i = 0; i < array_size; i++)
                {
                    scanner.ReadNextToken();
                    array_of_tokens[i] = scanner.Token;
                }

                // validate
                Assert.Equal(TokenType.IfExprOpen, array_of_tokens[0].Type);
                Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[1].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[2].Type);
                Assert.Equal("a", array_of_tokens[2].Value);
                Assert.Equal(TokenType.Dot, array_of_tokens[3].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[4].Type);
                Assert.Equal("is_potentially_hazardous_asteroid", array_of_tokens[4].Value);
                Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[5].Type);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[6].Type);
                Assert.Equal(TokenType.Eof, array_of_tokens[7].Type);
            }
        }

        [Fact]
        public void ReadsIfExpressionWithEqualityComparison()
        {
            var s = "<:if (a[5] == 10)>";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                var array_size = 11;
                Token[] array_of_tokens = new Token[array_size];
                for (int i = 0; i < array_size; i++)
                {
                    scanner.ReadNextToken();
                    array_of_tokens[i] = scanner.Token;
                }

                // validate
                Assert.Equal(TokenType.IfExprOpen, array_of_tokens[0].Type);
                Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[1].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[2].Type);
                Assert.Equal("a", array_of_tokens[2].Value);
                Assert.Equal(TokenType.SquareBracketOpen, array_of_tokens[3].Type);
                Assert.Equal(TokenType.Number, array_of_tokens[4].Type);
                Assert.Equal("5", array_of_tokens[4].Value);
                Assert.Equal(TokenType.SquareBracketClose, array_of_tokens[5].Type);
                Assert.Equal(TokenType.Equal, array_of_tokens[6].Type);
                Assert.Equal(TokenType.Number, array_of_tokens[7].Type);
                Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[8].Type);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[9].Type);
                Assert.Equal(TokenType.Eof, array_of_tokens[10].Type);
            }
        }

        [Fact]
        public void ReadsIfExpressionWithInequalityComparison()
        {
            var s = "<:if (a[5] >= 10)>";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                var array_size = 11;
                Token[] array_of_tokens = new Token[array_size];
                for (int i = 0; i < array_size; i++)
                {
                    scanner.ReadNextToken();
                    array_of_tokens[i] = scanner.Token;
                }

                // validate
                Assert.Equal(TokenType.IfExprOpen, array_of_tokens[0].Type);
                Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[1].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[2].Type);
                Assert.Equal("a", array_of_tokens[2].Value);
                Assert.Equal(TokenType.SquareBracketOpen, array_of_tokens[3].Type);
                Assert.Equal(TokenType.Number, array_of_tokens[4].Type);
                Assert.Equal("5", array_of_tokens[4].Value);
                Assert.Equal(TokenType.SquareBracketClose, array_of_tokens[5].Type);
                Assert.Equal(TokenType.GreaterEqualThan, array_of_tokens[6].Type);
                Assert.Equal(TokenType.Number, array_of_tokens[7].Type);
                Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[8].Type);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[9].Type);
                Assert.Equal(TokenType.Eof, array_of_tokens[10].Type);
            }
        }


        [Fact]
        public void ReadsForExpressions()
        {
            var s = "<:for (asteroid in model.asteroids)>{row(asteroid, \"red\")}</:for>";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var scanner = new Scanner(stream_reader);

                // act
                var array_size = 21;
                Token[] array_of_tokens = new Token[array_size];
                for (int i = 0; i < array_size; i++)
                {
                    scanner.ReadNextToken();
                    array_of_tokens[i] = scanner.Token;
                }

                // validate
                Assert.Equal(TokenType.ForExprOpen, array_of_tokens[0].Type);
                Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[1].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[2].Type);
                Assert.Equal("asteroid", array_of_tokens[2].Value);
                Assert.Equal(TokenType.In, array_of_tokens[3].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[4].Type);
                Assert.Equal("model", array_of_tokens[4].Value);
                Assert.Equal(TokenType.Dot, array_of_tokens[5].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[6].Type);
                Assert.Equal("asteroids", array_of_tokens[6].Value);
                Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[7].Type);
                Assert.Equal(TokenType.GreaterThan, array_of_tokens[8].Type);
                Assert.Equal(TokenType.CurlyBracketOpen, array_of_tokens[9].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[10].Type);
                Assert.Equal("row", array_of_tokens[10].Value);
                Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[11].Type);
                Assert.Equal(TokenType.Identifier, array_of_tokens[12].Type);
                Assert.Equal("asteroid", array_of_tokens[12].Value);
                Assert.Equal(TokenType.Coma, array_of_tokens[13].Type);
                Assert.Equal(TokenType.QuotationMark, array_of_tokens[14].Type);
                Assert.Equal(TokenType.Text, array_of_tokens[15].Type);
                Assert.Equal("red", array_of_tokens[15].Value);
                Assert.Equal(TokenType.QuotationMark, array_of_tokens[16].Type);

                Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[17].Type);
                Assert.Equal(TokenType.CurlyBracketClose, array_of_tokens[18].Type);
                Assert.Equal(TokenType.ForExprClose, array_of_tokens[19].Type);
                Assert.Equal(TokenType.Eof, array_of_tokens[20].Type);
            }
        }
    }
}