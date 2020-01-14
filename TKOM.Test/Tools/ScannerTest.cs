using Xunit;
using static TKOM.Utils.Token;
using TKOM.Readers;
using TKOM.Utils;
using TKOM.Tools;
using static TKOM.Test.Utils.ScannerUtils;

namespace TKOM.Test.Tools
{
    public class ScannerTest
    {
        [Fact]
        public void EmptyStream_ReadsNextToken_TokenIsEof()
        {
            var scanner = PrepareScanner("");
            scanner.ActAndValidate(TokenType.Eof);
        }

        [Fact]
        public void TriangularBracketOpen_ReadsNextToken_TokenIsPointyBracketOpen()
        {
            var scanner = PrepareScanner("<");
            scanner.ActAndValidate(TokenType.PointyBracketOpen);
        }

        [Fact]
        public void TriangularBracketClose_ReadsNextToken_TokenIsPointyBracketClose()
        {
            var scanner = PrepareScanner(">");
            scanner.ActAndValidate(TokenType.PointyBracketClose);
        }

        [Fact]
        public void TagClose_ReadsNextToken_TokenIsTagClose()
        {
            var scanner = PrepareScanner("</");
            scanner.ActAndValidate(TokenType.TagClose);
        }

        [Fact]
        public void DefKeyword_ReadsNextToken_TokenIsDef()
        {
            var scanner = PrepareScanner(":def");
            scanner.ActAndValidate(TokenType.Def);
        }

        [Fact]
        public void DefWithoutColon_ReadsNextToken_TokenIsIdentifier()
        {
            var scanner = PrepareScanner("def");
            scanner.ActAndValidate((TokenType.Identifier, "def"));
        }

        [Fact]
        public void DefWithoutColonStartingWithUnderline_ReadsNextToken_TokenIsIdentifier()
        {
            var scanner = PrepareScanner("_def");
            scanner.ActAndValidate((TokenType.Identifier, "_def"));
        }

        [Fact]
        public void StreamWithDefTagOpen_ReadsNextTokenTwice_TokensAreIsPointyBracketOpenAndDef()
        {
            var scanner = PrepareScanner("<:def");
            scanner.ActAndValidate(TokenType.PointyBracketOpen,
                                    TokenType.Def,
                                    TokenType.Eof);
        }

        [Fact]
        public void StreamWithDefTagClose_ReadsNextTokenThreeTimes_TokensAreTagCloseAndDefAndPointyBracketClose()
        {
            var scanner = PrepareScanner("</:def>");
            scanner.ActAndValidate(TokenType.TagClose,
                                    TokenType.Def,
                                    TokenType.PointyBracketClose,
                                    TokenType.Eof);
        }

        [Fact]
        public void ParenthesisOpen_ReadsNextToken_TokenIsParenthesisOpen()
        {
            var scanner = PrepareScanner("(");
            scanner.ActAndValidate(TokenType.ParenthesisOpen);
        }

        [Fact]
        public void ParenthesisClose_ReadsNextToken_TokenIsParenthesisClose()
        {
            var scanner = PrepareScanner(")");
            scanner.ActAndValidate(TokenType.ParenthesisClose);
        }

        [Fact]
        public void StreamWithFunHeaderWithOneArg_ReadNextTokenTillEof_TokensAreCorrect()
        {
            var scanner = PrepareScanner(@"<:def function(arg1)>");
            scanner.ActAndValidate((TokenType.PointyBracketOpen, null),
                                    (TokenType.Def, null),
                                    (TokenType.Identifier, "function"),
                                    (TokenType.ParenthesisOpen, null),
                                    (TokenType.Identifier, "arg1"),
                                    (TokenType.ParenthesisClose, null),
                                    (TokenType.PointyBracketClose, null),
                                    (TokenType.Eof, null));
        }

        [Fact]
        public void StreamWithFunHeaderAndBody_ReadTokensAndText_TokensAreCorrect()
        {
            var scanner = PrepareScanner("<:def function(arg1)>\nwhatever inside\n</:def>");
            scanner.ActAndValidate((TokenType.PointyBracketOpen, null),
                                    (TokenType.Def, null),
                                    (TokenType.Identifier, "function"),
                                    (TokenType.ParenthesisOpen, null),
                                    (TokenType.Identifier, "arg1"),
                                    (TokenType.ParenthesisClose, null),
                                    (TokenType.PointyBracketClose, null),
                                    (TokenType.Text, "whatever inside\n"),
                                    (TokenType.TagClose, null),
                                    (TokenType.Def, null),
                                    (TokenType.PointyBracketClose, null),
                                    (TokenType.Eof, null));
        }

        [Fact]
        public void Coma_ReadsNextToken_TokenIsComa()
        {
            var scanner = PrepareScanner(",");
            scanner.ActAndValidate(TokenType.Coma);
        }

        [Fact]
        public void StreamWithFunHeaderWithManyArgs_ReadNextTokenTillEof_TokensAreCorrect()
        {
            var scanner = PrepareScanner("<:def function(arg1, _arg2, _3)>");
            scanner.ActAndValidate((TokenType.PointyBracketOpen, null),
                                    (TokenType.Def, null),
                                    (TokenType.Identifier, "function"),
                                    (TokenType.ParenthesisOpen, null),
                                    (TokenType.Identifier, "arg1"),
                                    (TokenType.Coma, null),
                                    (TokenType.Identifier, "_arg2"),
                                    (TokenType.Coma, null),
                                    (TokenType.Identifier, "_3"),
                                    (TokenType.ParenthesisClose, null),
                                    (TokenType.PointyBracketClose, null),
                                    (TokenType.Eof, null));
        }

        [Fact]
        public void StreamWithFunHeaderAndBodyWithHtmlTag_ReadTokensAndText_TokensAreCorrect()
        {
            var scanner = PrepareScanner("<h>HTML inside</h>");
            scanner.ActAndValidate((TokenType.PointyBracketOpen, null),
                                    (TokenType.Identifier, "h"),
                                    (TokenType.PointyBracketClose, null),
                                    (TokenType.Text, "HTML inside"),
                                    (TokenType.TagClose, null),
                                    (TokenType.Identifier, "h"),
                                    (TokenType.PointyBracketClose, null));
        }

        [Fact]
        public void StreamWithFunBodyWithEscapeChars_ReadTokensAndText_TokensAreCorrect()
        {
            var scanner = PrepareScanner("\\<:def >");
            scanner.ActAndValidate((TokenType.Text, "<:def >"));
        }

        [Fact]
        public void StreamWithFunBodyWithEscapeCharsEscaped_ReadTokensAndText_TokensAreCorrect()
        {
            var scanner = PrepareScanner("\\\\<:def >");
            scanner.ActAndValidate((TokenType.Text, "\\"),
                                    (TokenType.PointyBracketOpen, null),
                                    (TokenType.Def, null),
                                    (TokenType.PointyBracketClose, null));
        }

        [Fact]
        public void CurlyBracketOpen_ReadsNextToken_TokenIsCurloyBracketOpen()
        {
            var scanner = PrepareScanner("{");
            scanner.ActAndValidate(TokenType.CurlyBracketOpen);
        }

        [Fact]
        public void CurlyBracketClose_ReadsNextToken_TokenIsCurlyBracketClose()
        {
            var scanner = PrepareScanner("}");
            scanner.ActAndValidate(TokenType.CurlyBracketClose);
        }

        [Fact]
        public void StreamWithIdInCurlyBrackets_ReadTokens_TokensAreCorrect()
        {
            var scanner = PrepareScanner("{identifier}");
            scanner.ActAndValidate((TokenType.CurlyBracketOpen, null),
                                    (TokenType.Identifier, "identifier"),
                                    (TokenType.CurlyBracketClose, null),
                                    (TokenType.Eof, null));
        }

        [Fact]
        public void StreamWithEscapedCurlyBrackets_ReadTokens_TokensAreCorrect()
        {
            var scanner = PrepareScanner("\\{identifier}");
            scanner.ActAndValidate((TokenType.Text, "{identifier}"));
        }

        [Fact]
        public void StreamWithIdInCurlyBracketsWithSpaces_ReadTokens_TokensAreCorrect()
        {

            var scanner = PrepareScanner("{ identifier }");
            scanner.ActAndValidate((TokenType.CurlyBracketOpen, null),
                                    (TokenType.Identifier, "identifier"),
                                    (TokenType.CurlyBracketClose, null),
                                    (TokenType.Eof, null));

        }

        [Fact]
        public void InlineTagClose_ReadsNextToken_TokenIsInlineTagClose()
        {
            var scanner = PrepareScanner("/>");
            scanner.ActAndValidate(TokenType.TagCloseInline);
        }

        [Fact]
        public void StreamWithInlineTag_ReadTokens_TokensAreCorrect()
        {
            var scanner = PrepareScanner("<br/>");
            scanner.ActAndValidate((TokenType.PointyBracketOpen, null),
                                    (TokenType.Identifier, "br"),
                                    (TokenType.TagCloseInline, null),
                                    (TokenType.Eof, null));
        }

        [Fact]
        public void IfKeyword_ReadsNextToken_TokenIsIF()
        {
            var scanner = PrepareScanner(":if");
            scanner.ActAndValidate(TokenType.If);
        }

        [Fact]
        public void Dot_ReadsNextToken_TokenIsDot()
        {
            var scanner = PrepareScanner(".");
            scanner.ActAndValidate(TokenType.Dot);
        }

        [Fact]
        public void ExclamationMark_ReadsNextToken_TokenIsExclamationMark()
        {
            var scanner = PrepareScanner("!");
            scanner.ActAndValidate(TokenType.ExclamationMark);
        }

        [Fact]
        public void StreamWithIfExp_ReadNextTokenTillEof_TokensAreCorrect()
        {
            var scanner = PrepareScanner("<:if (a.is_potentially_hazardous_asteroid)>");
            scanner.ActAndValidate((TokenType.PointyBracketOpen, null),
                                    (TokenType.If, null),
                                    (TokenType.ParenthesisOpen, null),
                                    (TokenType.Identifier, "a"),
                                    (TokenType.Dot, null),
                                    (TokenType.Identifier, "is_potentially_hazardous_asteroid"),
                                    (TokenType.ParenthesisClose, null),
                                    (TokenType.PointyBracketClose, null),
                                    (TokenType.Eof, null));

        }

        [Fact]
        public void StreamWithIfExpNegated_ReadNextTokenTillEof_TokensAreCorrect()
        {
            var scanner = PrepareScanner("<:if (!a.is_potentially_hazardous_asteroid)>");
            scanner.ActAndValidate((TokenType.PointyBracketOpen, null),
                                    (TokenType.If, null),
                                    (TokenType.ParenthesisOpen, null),
                                    (TokenType.ExclamationMark, null),
                                    (TokenType.Identifier, "a"),
                                    (TokenType.Dot, null),
                                    (TokenType.Identifier, "is_potentially_hazardous_asteroid"),
                                    (TokenType.ParenthesisClose, null),
                                    (TokenType.PointyBracketClose, null),
                                    (TokenType.Eof, null));
        }

        [Fact]
        public void StreamWithIfExpWithSpaces_ReadNextTokenTillEof_TokensAreCorrect()
        {
            var scanner = PrepareScanner("<:if ( a.is_potentially_hazardous_asteroid ) >");
            scanner.ActAndValidate((TokenType.PointyBracketOpen, null),
                                    (TokenType.If, null),
                                    (TokenType.ParenthesisOpen, null),
                                    (TokenType.Identifier, "a"),
                                    (TokenType.Dot, null),
                                    (TokenType.Identifier, "is_potentially_hazardous_asteroid"),
                                    (TokenType.ParenthesisClose, null),
                                    (TokenType.PointyBracketClose, null),
                                    (TokenType.Eof, null));
        }

        [Fact]
        public void AssignmentMark_ReadsNextToken_TokenIsAssignment()
        {
            var scanner = PrepareScanner("=");
            scanner.ActAndValidate(TokenType.AssignmentMark);
        }

        [Fact]
        public void EqualitySign_ReadsNextToken_TokenIsEqual()
        {
            var scanner = PrepareScanner("==");
            scanner.ActAndValidate(TokenType.Equal);
        }

        [Fact]
        public void InequalitySign_ReadsNextToken_TokenIsNotEqual()
        {
            var scanner = PrepareScanner("!=");
            scanner.ActAndValidate(TokenType.NotEqual);
        }

        [Fact]
        public void GreaterEqualSign_ReadsNextToken_TokenIsGreaterEqual()
        {
            var scanner = PrepareScanner(">=");
            scanner.ActAndValidate(TokenType.GreaterEqualThan);
        }

        [Fact]
        public void LessEqualSign_ReadsNextToken_TokenIsLessEqual()
        {
            var scanner = PrepareScanner("<=");
            scanner.ActAndValidate(TokenType.LessEqualThan);
        }

        [Fact]
        public void SquareBracketOpen_ReadsNextToken_TokenIsSquareBracketOpen()
        {
            var scanner = PrepareScanner("[");
            scanner.ActAndValidate(TokenType.SquareBracketOpen);
        }

        [Fact]
        public void SquareBracketClose_ReadsNextToken_TokenIsSquareBracketClose()
        {
            var scanner = PrepareScanner("]");
            scanner.ActAndValidate(TokenType.SquareBracketClose);
        }

        [Fact]
        public void StreamWithEqualityComparison_ReadNextTokenTillEof_TokensAreCorrect()
        {
            var scanner = PrepareScanner("a[5] == 10");
            scanner.ActAndValidate((TokenType.Identifier, "a"),
                                    (TokenType.SquareBracketOpen, null),
                                    (TokenType.Number, "5"),
                                    (TokenType.SquareBracketClose, null),
                                    (TokenType.Equal, null),
                                    (TokenType.Number, "10"),
                                    (TokenType.Eof, null));
        }

        [Fact]
        public void StreamWithInequalityComparison_ReadNextTokenTillEof_TokensAreCorrect()
        {
            var scanner = PrepareScanner("a[5]!= 10");
            scanner.ActAndValidate((TokenType.Identifier, "a"),
                                    (TokenType.SquareBracketOpen, null),
                                    (TokenType.Number, "5"),
                                    (TokenType.SquareBracketClose, null),
                                    (TokenType.NotEqual, null),
                                    (TokenType.Number, "10"),
                                    (TokenType.Eof, null));

        }

        [Fact]
        public void StreamWithGreaterEqualComparison_ReadNextTokenTillEof_TokensAreCorrect()
        {
            var scanner = PrepareScanner("a[5] >=10");
            scanner.ActAndValidate((TokenType.Identifier, "a"),
                                    (TokenType.SquareBracketOpen, null),
                                    (TokenType.Number, "5"),
                                    (TokenType.SquareBracketClose, null),
                                    (TokenType.GreaterEqualThan, null),
                                    (TokenType.Number, "10"),
                                    (TokenType.Eof, null));
        }

        [Fact]
        public void StreamWithLessEqualComparison_ReadNextTokenTillEof_TokensAreCorrect()
        {
            var scanner = PrepareScanner("a[5] <= 10");
            scanner.ActAndValidate((TokenType.Identifier, "a"),
                                    (TokenType.SquareBracketOpen, null),
                                    (TokenType.Number, "5"),
                                    (TokenType.SquareBracketClose, null),
                                    (TokenType.LessEqualThan, null),
                                    (TokenType.Number, "10"),
                                    (TokenType.Eof, null));
        }

        [Fact]
        public void For_ReadsNextToken_TokenIsFor()
        {
            var scanner = PrepareScanner(":for");
            scanner.ActAndValidate(TokenType.For);
        }

        [Fact]
        public void In_ReadsNextToken_TokenIsIn()
        {
            var scanner = PrepareScanner("in");
            scanner.ActAndValidate(TokenType.In);
        }

        [Fact]
        public void StreamWithForExpr_ReadNextTokenSixTimes_TokensAreCorrect()
        {
            var scanner = PrepareScanner("<:for (asteroid in model.asteroids)");
            scanner.ActAndValidate((TokenType.PointyBracketOpen, null),
                                    (TokenType.For, null),
                                    (TokenType.ParenthesisOpen, null),
                                    (TokenType.Identifier, "asteroid"),
                                    (TokenType.In, null),
                                    (TokenType.Identifier, "model"),
                                    (TokenType.Dot, null),
                                    (TokenType.Identifier, "asteroids"),
                                    (TokenType.ParenthesisClose, null),
                                    (TokenType.Eof, null));
        }

        [Fact]
        public void Else_ReadsNextToken_TokenIsElse()
        {
            var scanner = PrepareScanner(":else");
            scanner.ActAndValidate(TokenType.Else);
        }

        [Fact]
        public void StreamWithElseExpr_ReadNextTokenTillEof_TokensAreCorrect()
        {
            var scanner = PrepareScanner("<:else>");
            scanner.ActAndValidate((TokenType.PointyBracketOpen, null),
                                    (TokenType.Else, null),
                                    (TokenType.PointyBracketClose, null),
                                    (TokenType.Eof, null));
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