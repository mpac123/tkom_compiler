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
                Token[] array_of_tokens = new Token[8];
                for (int i = 0; i < 8; i++)
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
                Assert.Equal(TokenType.Text, array_of_tokens[6].Type);
                Assert.Equal("\r\nwhatever inside\r\n", array_of_tokens[6].Value);
                Assert.Equal(TokenType.FunctionClose, array_of_tokens[7].Type);
                Assert.Equal(TokenType.Eof, array_of_tokens[8].Type);
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
    }

    
}