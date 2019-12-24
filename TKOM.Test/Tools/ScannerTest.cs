using Xunit;
using static TKOM.Utils.Token;
using TKOM.Readers;
using TKOM.Utils;
using TKOM.Tools;

namespace TKOM.Test.Tools
{
    public class ScannerTest
    {
        [Fact]
        public void EmptyStream_ReadsNextToken_TokenIsEof()
        {
            // prepare
            var reader = new StringsReader("");
            var scanner = new Scanner(reader);

            // act
            scanner.ReadNextToken();
            var token = scanner.Token;

            // validate
            Assert.Equal(TokenType.Eof, token.Type);
        }

        [Fact]
        public void StreamWithTagOpen_ReadsNextToken_TokenIsPointyBracketOpen()
        {
            // prepare
            var reader = new StringsReader("<:def");
            var scanner = new Scanner(reader);

            // act
            scanner.ReadNextToken();
            var first_token = scanner.Token;

            // validate
            Assert.Equal(TokenType.PointyBracketOpen, first_token.Type);
        }

        [Fact]
        public void StreamWithDefKeyword_ReadsNextToken_TokenIsDef()
        {
            // prepare
            var reader = new StringsReader(":def");
            var scanner = new Scanner(reader);

            // act
            scanner.ReadNextToken();
            var first_token = scanner.Token;

            // validate
            Assert.Equal(TokenType.Def, first_token.Type);
        }

        [Fact]
        public void StreamWithDefTagOpen_ReadsNextTokenTwice_TokensAreIsPointyBracketOpenAndDef()
        {
            // prepare
            var reader = new StringsReader("<:def");
            var scanner = new Scanner(reader);

            // act
            scanner.ReadNextToken();
            var first_token = scanner.Token;
            scanner.ReadNextToken();
            var second_token = scanner.Token;
            scanner.ReadNextToken();
            var third_token = scanner.Token;

            // validate
            Assert.Equal(TokenType.PointyBracketOpen, first_token.Type);
            Assert.Equal(TokenType.Def, second_token.Type);
            Assert.Equal(TokenType.Eof, third_token.Type);
        }

        [Fact]
        public void StreamWithDefTagClose_ReadsNextTokenThreeTimes_TokensAreTagCloseAndDefAndPointyBracketClose()
        {
            // prepare
            var reader = new StringsReader("</:def>");
            var scanner = new Scanner(reader);

            // act
            scanner.ReadNextToken();
            var first_token = scanner.Token;
            scanner.ReadNextToken();
            var second_token = scanner.Token;
            scanner.ReadNextToken();
            var third_token = scanner.Token;
            scanner.ReadNextToken();
            var fourth_token = scanner.Token;

            // validate
            Assert.Equal(TokenType.TagClose, first_token.Type);
            Assert.Equal(TokenType.Def, second_token.Type);
            Assert.Equal(TokenType.PointyBracketClose, third_token.Type);
            Assert.Equal(TokenType.Eof, fourth_token.Type);
        }

        [Fact]
        public void StreamWithFunHeaderWithOneArg_ReadNextTokenTillEof_TokensAreCorrect()
        {

            // prepare
            var reader = new StringsReader(@"<:def function(arg1)>");
            var scanner = new Scanner(reader);

            // act
            int size_array = 8;
            Token[] array_of_tokens = new Token[size_array];
            for (int i = 0; i < size_array; i++)
            {
                scanner.ReadNextToken();
                array_of_tokens[i] = scanner.Token;
            }

            // validate
            Assert.Equal(TokenType.PointyBracketOpen, array_of_tokens[0].Type);
            Assert.Equal(TokenType.Def, array_of_tokens[1].Type);
            Assert.Equal(TokenType.Identifier, array_of_tokens[2].Type);
            Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[3].Type);

            Assert.Equal(TokenType.Identifier, array_of_tokens[4].Type);
            Assert.Equal("arg1", array_of_tokens[4].Value);

            Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[5].Type);
            Assert.Equal(TokenType.PointyBracketClose, array_of_tokens[6].Type);

            Assert.Equal(TokenType.Eof, array_of_tokens[7].Type);
        }

        [Fact]
        public void StreamWithFunHeaderAndBody_ReadTokensAndText_TokensAreCorrect()
        {

            // prepare
            var reader = new StringsReader("<:def function(arg1)>\nwhatever inside\n</:def>");
            var scanner = new Scanner(reader);

            // act
            var array_size = 12;
            Token[] array_of_tokens = new Token[array_size];
            for (int i = 0; i < array_size; i++)
            {
                if (i == 7)
                {
                    scanner.TryReadText();
                }
                else
                {
                    scanner.ReadNextToken();
                }
                array_of_tokens[i] = scanner.Token;
            }

            // validate
            Assert.Equal(TokenType.PointyBracketOpen, array_of_tokens[0].Type);
            Assert.Equal(TokenType.Def, array_of_tokens[1].Type);
            Assert.Equal(TokenType.Identifier, array_of_tokens[2].Type);
            Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[3].Type);
            Assert.Equal(TokenType.Identifier, array_of_tokens[4].Type);
            Assert.Equal("arg1", array_of_tokens[4].Value);
            Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[5].Type);
            Assert.Equal(TokenType.PointyBracketClose, array_of_tokens[6].Type);
            Assert.Equal(TokenType.Text, array_of_tokens[7].Type);
            Assert.Equal("whatever inside\n", array_of_tokens[7].Value);
            Assert.Equal(TokenType.TagClose, array_of_tokens[8].Type);
            Assert.Equal(TokenType.Def, array_of_tokens[9].Type);
            Assert.Equal(TokenType.PointyBracketClose, array_of_tokens[10].Type);
            Assert.Equal(TokenType.Eof, array_of_tokens[11].Type);
        }

        [Fact]
        public void StreamWithFunHeaderWithManyArgs_ReadNextTokenTillEof_TokensAreCorrect()
        {

            // prepare
            var reader = new StringsReader("<:def function(arg1, _arg2, _3)>");
            var scanner = new Scanner(reader);

            // act
            int size_array = 12;
            Token[] array_of_tokens = new Token[size_array];
            for (int i = 0; i < size_array; i++)
            {
                scanner.ReadNextToken();
                array_of_tokens[i] = scanner.Token;
            }

            // validate
            Assert.Equal(TokenType.PointyBracketOpen, array_of_tokens[0].Type);
            Assert.Equal(TokenType.Def, array_of_tokens[1].Type);
            Assert.Equal(TokenType.Identifier, array_of_tokens[2].Type);
            Assert.Equal("function", array_of_tokens[2].Value);
            Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[3].Type);

            Assert.Equal(TokenType.Identifier, array_of_tokens[4].Type);
            Assert.Equal("arg1", array_of_tokens[4].Value);

            Assert.Equal(TokenType.Coma, array_of_tokens[5].Type);
            Assert.Equal(TokenType.Identifier, array_of_tokens[6].Type);
            Assert.Equal("_arg2", array_of_tokens[6].Value);

            Assert.Equal(TokenType.Coma, array_of_tokens[7].Type);
            Assert.Equal(TokenType.Identifier, array_of_tokens[8].Type);
            Assert.Equal("_3", array_of_tokens[8].Value);

            Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[9].Type);
            Assert.Equal(TokenType.PointyBracketClose, array_of_tokens[10].Type);

            Assert.Equal(TokenType.Eof, array_of_tokens[11].Type);
        }

        [Fact]
        public void StreamWithFunHeaderAndBodyWithHtmlTag_ReadTokensAndText_TokensAreCorrect()
        {

            // prepare
            var reader = new StringsReader("<:def function()>\na<h>HTML inside</h>\nb</:def>");
            for (int i = 0; i < 17; i++)
            {
                reader.Read();  // so that position is at \n after fun declaration...
            }
            var scanner = new Scanner(reader);

            // act
            var isText = scanner.TryReadText();
            var firstToken = scanner.Token;
            scanner.ReadNextToken();
            var secondToken = scanner.Token;
            scanner.ReadNextToken();
            var thirdToken = scanner.Token;
            scanner.ReadNextToken();
            var forthToken = scanner.Token;
            scanner.TryReadText();
            var fifthToken = scanner.Token;

            // validate
            Assert.True(isText);
            Assert.Equal(TokenType.Text, firstToken.Type);
            Assert.Equal("a", firstToken.Value);
            Assert.Equal(TokenType.PointyBracketOpen, secondToken.Type);
            Assert.Equal(TokenType.Identifier, thirdToken.Type);
            Assert.Equal("h", thirdToken.Value);
            Assert.Equal(TokenType.PointyBracketClose, forthToken.Type);
            Assert.Equal(TokenType.Text, fifthToken.Type);
            Assert.Equal("HTML inside", fifthToken.Value);

        }

        [Fact]
        public void StreamWithFunBodyWithEscapeChars_ReadTokensAndText_TokensAreCorrect()
        {

            // prepare
            var reader = new StringsReader("<:def function()>\n\\<:def >\n</:def>");
            for (int i = 0; i < 17; i++)
            {
                reader.Read();  // so that position is at \n after fun declaration...
            }
            var scanner = new Scanner(reader);

            // act
            var isText = scanner.TryReadText();
            var firstToken = scanner.Token;
            scanner.ReadNextToken();
            var secondToken = scanner.Token;

            // validate
            Assert.True(isText);
            Assert.Equal(TokenType.Text, firstToken.Type);
            Assert.Equal("<:def >\n", firstToken.Value);
            Assert.Equal(TokenType.TagClose, secondToken.Type);

        }

        [Fact]
        public void StreamWithFunBodyWithEscapeCharsEscaped_ReadTokensAndText_TokensAreCorrect()
        {
            // prepare
            var reader = new StringsReader("<:def function()>\n\\\\<:def \n");
            for (int i = 0; i < 17; i++)
            {
                reader.Read();  // so that position is at \n after fun declaration...
            }
            var scanner = new Scanner(reader);

            // act
            scanner.TryReadText();
            var firstToken = scanner.Token;
            scanner.ReadNextToken();
            var secondToken = scanner.Token;
            scanner.ReadNextToken();
            var thirdToken = scanner.Token;

            // validate
            Assert.Equal(TokenType.Text, firstToken.Type);
            Assert.Equal("\\", firstToken.Value);
            Assert.Equal(TokenType.PointyBracketOpen, secondToken.Type);
            Assert.Equal(TokenType.Def, thirdToken.Type);
        }

        [Fact]
        public void StreamWithIdInCurlyBrackets_ReadTokens_TokensAreCorrect()
        {

            // prepare
            var reader = new StringsReader("{identifier}</h>");
            var scanner = new Scanner(reader);

            // act
            var isTextFirst = scanner.TryReadText();
            scanner.ReadNextToken();
            var firstToken = scanner.Token;
            scanner.ReadNextToken();
            var secondToken = scanner.Token;
            scanner.ReadNextToken();
            var thirdToken = scanner.Token;
            var isText = scanner.TryReadText();
            scanner.ReadNextToken();
            var forthToken = scanner.Token;

            // validate
            Assert.Equal(TokenType.CurlyBracketOpen, firstToken.Type);
            Assert.Equal(TokenType.Identifier, secondToken.Type);
            Assert.Equal("identifier", secondToken.Value);
            Assert.Equal(TokenType.CurlyBracketClose, thirdToken.Type);
            Assert.False(isTextFirst);
            Assert.False(isText);
            Assert.Equal(TokenType.TagClose, forthToken.Type);

        }

        [Fact]
        public void StreamWithEscapedCurlyBrackets_ReadTokens_TokensAreCorrect()
        {

            // prepare
            var reader = new StringsReader("\\{identifier}</h>");
            var scanner = new Scanner(reader);

            // act
            var isTextFirst = scanner.TryReadText();
            var firstToken = scanner.Token;
            scanner.ReadNextToken();
            var secondToken = scanner.Token;

            // validate
            Assert.True(isTextFirst);
            Assert.Equal(TokenType.Text, firstToken.Type);
            Assert.Equal("{identifier}", firstToken.Value);
            Assert.Equal(TokenType.TagClose, secondToken.Type);

        }
        [Fact]
        public void StreamWithIdInCurlyBracketsWithSpaces_ReadTokens_TokensAreCorrect()
        {

            // prepare
            var reader = new StringsReader("{ identifier }</h>");
            var scanner = new Scanner(reader);

            // act
            var isTextFirst = scanner.TryReadText();
            scanner.ReadNextToken();
            var firstToken = scanner.Token;
            scanner.ReadNextToken();
            var secondToken = scanner.Token;
            scanner.ReadNextToken();
            var thirdToken = scanner.Token;
            var isText = scanner.TryReadText();
            scanner.ReadNextToken();
            var forthToken = scanner.Token;

            // validate
            Assert.Equal(TokenType.CurlyBracketOpen, firstToken.Type);
            Assert.Equal(TokenType.Identifier, secondToken.Type);
            Assert.Equal("identifier", secondToken.Value);
            Assert.Equal(TokenType.CurlyBracketClose, thirdToken.Type);
            Assert.False(isTextFirst);
            Assert.False(isText);
            Assert.Equal(TokenType.TagClose, forthToken.Type);

        }

        [Fact]
        public void StreamWithInlineTag_ReadTokens_TokensAreCorrect()
        {

            // prepare
            var reader = new StringsReader("<br/>");
            var scanner = new Scanner(reader);

            // act
            scanner.ReadNextToken();
            var firstToken = scanner.Token;
            scanner.ReadNextToken();
            var secondToken = scanner.Token;
            scanner.ReadNextToken();
            var thirdToken = scanner.Token;
            scanner.ReadNextToken();
            var forthToken = scanner.Token;

            // validate
            Assert.Equal(TokenType.PointyBracketOpen, firstToken.Type);
            Assert.Equal(TokenType.Identifier, secondToken.Type);
            Assert.Equal("br", secondToken.Value);
            Assert.Equal(TokenType.TagCloseInline, thirdToken.Type);
            Assert.Equal(TokenType.Eof, forthToken.Type);

        }

        [Fact]
        public void StreamWithIfExp_ReadNextTokenTillEof_TokensAreCorrect()
        {
            // prepare
            var reader = new StringsReader("<:if (a.is_potentially_hazardous_asteroid)>");
            var scanner = new Scanner(reader);

            // act
            var array_size = 9;
            Token[] array_of_tokens = new Token[array_size];
            for (int i = 0; i < array_size; i++)
            {
                scanner.ReadNextToken();
                array_of_tokens[i] = scanner.Token;
            }

            // validate
            Assert.Equal(TokenType.PointyBracketOpen, array_of_tokens[0].Type);
            Assert.Equal(TokenType.If, array_of_tokens[1].Type);
            Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[2].Type);
            Assert.Equal(TokenType.Identifier, array_of_tokens[3].Type);
            Assert.Equal("a", array_of_tokens[3].Value);
            Assert.Equal(TokenType.Dot, array_of_tokens[4].Type);
            Assert.Equal(TokenType.Identifier, array_of_tokens[5].Type);
            Assert.Equal("is_potentially_hazardous_asteroid", array_of_tokens[5].Value);
            Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[6].Type);
            Assert.Equal(TokenType.PointyBracketClose, array_of_tokens[7].Type);
            Assert.Equal(TokenType.Eof, array_of_tokens[8].Type);
        }

        [Fact]
        public void StreamWithIfExpNegated_ReadNextTokenTillEof_TokensAreCorrect()
        {
            // prepare
            var reader = new StringsReader("<:if (!a.is_potentially_hazardous_asteroid)>");
            var scanner = new Scanner(reader);

            // act
            var array_size = 10;
            Token[] array_of_tokens = new Token[array_size];
            for (int i = 0; i < array_size; i++)
            {
                scanner.ReadNextToken();
                array_of_tokens[i] = scanner.Token;
            }

            // validate
            Assert.Equal(TokenType.PointyBracketOpen, array_of_tokens[0].Type);
            Assert.Equal(TokenType.If, array_of_tokens[1].Type);
            Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[2].Type);
            Assert.Equal(TokenType.ExclamationMark, array_of_tokens[3].Type);
            Assert.Equal(TokenType.Identifier, array_of_tokens[4].Type);
            Assert.Equal("a", array_of_tokens[4].Value);
            Assert.Equal(TokenType.Dot, array_of_tokens[5].Type);
            Assert.Equal(TokenType.Identifier, array_of_tokens[6].Type);
            Assert.Equal("is_potentially_hazardous_asteroid", array_of_tokens[6].Value);
            Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[7].Type);
            Assert.Equal(TokenType.PointyBracketClose, array_of_tokens[8].Type);
            Assert.Equal(TokenType.Eof, array_of_tokens[9].Type);
        }

        [Fact]
        public void StreamWithIfExpWithSpaces_ReadNextTokenTillEof_TokensAreCorrect()
        {
            // prepare
            var reader = new StringsReader("<:if ( a.is_potentially_hazardous_asteroid ) >");
            var scanner = new Scanner(reader);

            // act
            var array_size = 9;
            Token[] array_of_tokens = new Token[array_size];
            for (int i = 0; i < array_size; i++)
            {
                scanner.ReadNextToken();
                array_of_tokens[i] = scanner.Token;
            }

            // validate
            Assert.Equal(TokenType.PointyBracketOpen, array_of_tokens[0].Type);
            Assert.Equal(TokenType.If, array_of_tokens[1].Type);
            Assert.Equal(TokenType.ParenthesisOpen, array_of_tokens[2].Type);
            Assert.Equal(TokenType.Identifier, array_of_tokens[3].Type);
            Assert.Equal("a", array_of_tokens[3].Value);
            Assert.Equal(TokenType.Dot, array_of_tokens[4].Type);
            Assert.Equal(TokenType.Identifier, array_of_tokens[5].Type);
            Assert.Equal("is_potentially_hazardous_asteroid", array_of_tokens[5].Value);
            Assert.Equal(TokenType.ParenthesisClose, array_of_tokens[6].Type);
            Assert.Equal(TokenType.PointyBracketClose, array_of_tokens[7].Type);
            Assert.Equal(TokenType.Eof, array_of_tokens[8].Type);
        }

        [Fact]
        public void StreamWithEqualityComparison_ReadNextTokenTillEof_TokensAreCorrect()
        {
            // prepare
            var reader = new StringsReader("a[5] == 10 ");
            var scanner = new Scanner(reader);

            // act
            scanner.ReadNextToken();
            var firstToken = scanner.Token;
            scanner.ReadNextToken();
            var secondToken = scanner.Token;
            scanner.ReadNextToken();
            var thirdToken = scanner.Token;
            scanner.ReadNextToken();
            var forthToken = scanner.Token;
            scanner.ReadNextToken();
            var fifthToken = scanner.Token;
            scanner.ReadNextToken();
            var sixthToken = scanner.Token;

            // validate
            Assert.Equal(TokenType.Identifier, firstToken.Type);
            Assert.Equal("a", firstToken.Value);
            Assert.Equal(TokenType.SquareBracketOpen, secondToken.Type);
            Assert.Equal(TokenType.Number, thirdToken.Type);
            Assert.Equal("5", thirdToken.Value);
            Assert.Equal(TokenType.SquareBracketClose, forthToken.Type);
            Assert.Equal(TokenType.Equal, fifthToken.Type);
            Assert.Equal(TokenType.Number, sixthToken.Type);
            Assert.Equal("10", sixthToken.Value);
        }

        [Fact]
        public void StreamWithInequalityComparison_ReadNextTokenTillEof_TokensAreCorrect()
        {
            // prepare
            var reader = new StringsReader("a[5]!= 10 ");
            var scanner = new Scanner(reader);

            // act
            scanner.ReadNextToken();
            var firstToken = scanner.Token;
            scanner.ReadNextToken();
            var secondToken = scanner.Token;
            scanner.ReadNextToken();
            var thirdToken = scanner.Token;
            scanner.ReadNextToken();
            var forthToken = scanner.Token;
            scanner.ReadNextToken();
            var fifthToken = scanner.Token;
            scanner.ReadNextToken();
            var sixthToken = scanner.Token;

            // validate
            Assert.Equal(TokenType.Identifier, firstToken.Type);
            Assert.Equal("a", firstToken.Value);
            Assert.Equal(TokenType.SquareBracketOpen, secondToken.Type);
            Assert.Equal(TokenType.Number, thirdToken.Type);
            Assert.Equal("5", thirdToken.Value);
            Assert.Equal(TokenType.SquareBracketClose, forthToken.Type);
            Assert.Equal(TokenType.NotEqual, fifthToken.Type);
            Assert.Equal(TokenType.Number, sixthToken.Type);
            Assert.Equal("10", sixthToken.Value);
        }

        [Fact]
        public void StreamWithGreaterEqualComparison_ReadNextTokenTillEof_TokensAreCorrect()
        {
            // prepare
            var reader = new StringsReader("a[5]>=10 ");
            var scanner = new Scanner(reader);

            // act
            scanner.ReadNextToken();
            var firstToken = scanner.Token;
            scanner.ReadNextToken();
            var secondToken = scanner.Token;
            scanner.ReadNextToken();
            var thirdToken = scanner.Token;
            scanner.ReadNextToken();
            var forthToken = scanner.Token;
            scanner.ReadNextToken();
            var fifthToken = scanner.Token;
            scanner.ReadNextToken();
            var sixthToken = scanner.Token;

            // validate
            Assert.Equal(TokenType.Identifier, firstToken.Type);
            Assert.Equal("a", firstToken.Value);
            Assert.Equal(TokenType.SquareBracketOpen, secondToken.Type);
            Assert.Equal(TokenType.Number, thirdToken.Type);
            Assert.Equal("5", thirdToken.Value);
            Assert.Equal(TokenType.SquareBracketClose, forthToken.Type);
            Assert.Equal(TokenType.GreaterEqualThan, fifthToken.Type);
            Assert.Equal(TokenType.Number, sixthToken.Type);
            Assert.Equal("10", sixthToken.Value);
        }

        [Fact]
        public void StreamWithLessEqualComparison_ReadNextTokenTillEof_TokensAreCorrect()
        {
            // prepare
            var reader = new StringsReader("a[5]<= 10 ");
            var scanner = new Scanner(reader);

            // act
            scanner.ReadNextToken();
            var firstToken = scanner.Token;
            scanner.ReadNextToken();
            var secondToken = scanner.Token;
            scanner.ReadNextToken();
            var thirdToken = scanner.Token;
            scanner.ReadNextToken();
            var forthToken = scanner.Token;
            scanner.ReadNextToken();
            var fifthToken = scanner.Token;
            scanner.ReadNextToken();
            var sixthToken = scanner.Token;

            // validate
            Assert.Equal(TokenType.Identifier, firstToken.Type);
            Assert.Equal("a", firstToken.Value);
            Assert.Equal(TokenType.SquareBracketOpen, secondToken.Type);
            Assert.Equal(TokenType.Number, thirdToken.Type);
            Assert.Equal("5", thirdToken.Value);
            Assert.Equal(TokenType.SquareBracketClose, forthToken.Type);
            Assert.Equal(TokenType.LessEqualThan, fifthToken.Type);
            Assert.Equal(TokenType.Number, sixthToken.Type);
            Assert.Equal("10", sixthToken.Value);
        }

        [Fact]
        public void StreamWithForExpr_ReadNextTokenSixTimes_TokensAreCorrect()
        {
            // prepare
            var reader = new StringsReader("<:for (asteroid in model.asteroids)");
            var scanner = new Scanner(reader);

            // act
            scanner.ReadNextToken();
            var firstToken = scanner.Token;
            scanner.ReadNextToken();
            var secondToken = scanner.Token;
            scanner.ReadNextToken();
            var thirdToken = scanner.Token;
            scanner.ReadNextToken();
            var forthToken = scanner.Token;
            scanner.ReadNextToken();
            var fifthToken = scanner.Token;
            scanner.ReadNextToken();
            var sixthToken = scanner.Token;

            // validate
            Assert.Equal(TokenType.PointyBracketOpen, firstToken.Type);
            Assert.Equal(TokenType.For, secondToken.Type);
            Assert.Equal(TokenType.ParenthesisOpen, thirdToken.Type);
            Assert.Equal(TokenType.Identifier, forthToken.Type);
            Assert.Equal("asteroid", forthToken.Value);
            Assert.Equal(TokenType.In, fifthToken.Type);
            Assert.Equal(TokenType.Identifier, sixthToken.Type);
            Assert.Equal("model", sixthToken.Value);
        }

        [Fact]
        public void StreamWithElseExpr_ReadNextTokenTillEof_TokensAreCorrect()
        {
            // prepare
            var reader = new StringsReader("<:else>");
            var scanner = new Scanner(reader);

            // act
            scanner.ReadNextToken();
            var firstToken = scanner.Token;
            scanner.ReadNextToken();
            var secondToken = scanner.Token;
            scanner.ReadNextToken();
            var thirdToken = scanner.Token;
            scanner.ReadNextToken();
            var forthToken = scanner.Token;

            // validate
            Assert.Equal(TokenType.PointyBracketOpen, firstToken.Type);
            Assert.Equal(TokenType.Else, secondToken.Type);
            Assert.Equal(TokenType.PointyBracketClose, thirdToken.Type);
            Assert.Equal(TokenType.Eof, forthToken.Type);
        }

        [Fact]
        public void StreamWithString_ReadNextTokenTillEof_TokensAreCorrect()
        {
            // prepare
            var reader = new StringsReader("\"string\"");
            var scanner = new Scanner(reader);

            // act
            scanner.ReadNextToken();
            var firstToken = scanner.Token;
            var isString = scanner.TryReadString();
            var secondToken = scanner.Token;
            scanner.ReadNextToken();
            var thirdToken = scanner.Token;

            // validate
            Assert.Equal(TokenType.QuotationMark, firstToken.Type);
            Assert.Equal(TokenType.Text, secondToken.Type);
            Assert.Equal("string", secondToken.Value);
            Assert.Equal(TokenType.QuotationMark, thirdToken.Type);
        }

        [Fact]
        public void StreamWithStringWithEscapedQuoteMark_ReadNextTokenTillEof_TokensAreCorrect()
        {
            // prepare
            var reader = new StringsReader("\"st\\\"ring\"");
            var scanner = new Scanner(reader);

            // act
            scanner.ReadNextToken();
            var firstToken = scanner.Token;
            var isString = scanner.TryReadString();
            var secondToken = scanner.Token;
            scanner.ReadNextToken();
            var thirdToken = scanner.Token;

            // validate
            Assert.Equal(TokenType.QuotationMark, firstToken.Type);
            Assert.Equal(TokenType.Text, secondToken.Type);
            Assert.Equal("st\"ring", secondToken.Value);
            Assert.Equal(TokenType.QuotationMark, thirdToken.Type);
        }
    }
}