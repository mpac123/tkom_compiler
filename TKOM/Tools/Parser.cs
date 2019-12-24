using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using TKOM.AST;
using TKOM.Exceptions;
using static TKOM.Utils.Token;

namespace TKOM.Tools
{
    public class Parser
    {
        private Scanner _scanner;
        private readonly ILogger<Parser> _logger;
        public Parser(Scanner scanner, ILogger<Parser> logger)
        {
            _scanner = scanner;
        }
        public AST.Program Parse()
        {
            var syntaxTree = new AST.Program();
            var function = ParseFunction();
            while (function != null)
            {
                syntaxTree.Functions.Add(function);
                function = ParseFunction();
            }
            return syntaxTree;
        }

        private Function ParseFunction()
        {
            _scanner.ReadNextToken();
            if (_scanner.Token.Type == TokenType.Eof)
            {
                return null;
            }
            var function = new Function();
            (function.Identifier, function.Arguments) = ParseDefOpeningTag();
            if (_scanner.TryReadText())
            {
                function.Instructions.Add(new Literal { Content = _scanner.Token.Value });
            }
            _scanner.ReadNextToken();
            while (_scanner.Token.Type != TokenType.TagClose)
            {
                function.Instructions.Add(ParseStatement());
                _scanner.ReadNextToken();
            }
            ParseDefClosingTag();
            return function;
        }

        private (string functionName, List<string> argumentNames) ParseDefOpeningTag()
        {
            var functionName = new StringBuilder();
            var argumentNames = new List<string>();
            List<TokenType> expectedTokenTypes = new List<TokenType> {
                TokenType.PointyBracketOpen,
                TokenType.Def,
                TokenType.Identifier,
                TokenType.ParenthesisOpen
            };
            foreach (var expectedTokenType in expectedTokenTypes)
            {
                if (_scanner.Token.Type != expectedTokenType)
                {
                    throw new ParsingException(expectedTokenType, _scanner.Token.Type);
                }
                if (expectedTokenType == TokenType.Identifier)
                {
                    functionName.Append(_scanner.Token.Value);
                }
                _scanner.ReadNextToken();
            }
            while (_scanner.Token.Type != TokenType.ParenthesisClose)
            {
                if (_scanner.Token.Type != TokenType.Identifier)
                {
                    throw new ParsingException(TokenType.Identifier, _scanner.Token.Type);
                }
                argumentNames.Add(_scanner.Token.Value);
                _scanner.ReadNextToken();
                if (_scanner.Token.Type == TokenType.Coma)
                {
                    _scanner.ReadNextToken();
                }
            }

            _scanner.ReadNextToken();
            if (_scanner.Token.Type != TokenType.PointyBracketClose)
            {
                throw new ParsingException(TokenType.PointyBracketClose, _scanner.Token.Type);
            }

            return (functionName.ToString(), argumentNames);
        }

        private IInstruction ParseStatement()
        {
            if (_scanner.TryReadText())
            {
                return new Literal
                {
                    Content = _scanner.Token.Value
                };
            }
            if (_scanner.Token.Type == TokenType.PointyBracketOpen)
            {
                _scanner.ReadNextToken();
                switch (_scanner.Token.Type)
                {
                    case TokenType.If:
                        return ParseIfExpression();
                    case TokenType.For:
                        return ParseForExpression();
                    case TokenType.Identifier:
                        return ParseHtmlTagOrHtmlTagInline();
                    default:
                        throw new ParsingException($"Expected a keyword or identifier, found {_scanner.Token.Type}.");
                }
            }
            else if (_scanner.Token.Type == TokenType.CurlyBracketOpen)
            {
                _scanner.ReadNextToken();
                return ParseValueOfOrFunctionCall();

            }
            throw new ParsingException($"Parsing failed.");
        }

        private IfExpression ParseIfExpression()
        {
            // TODO
            return new IfExpression();
        }

        private ForExpression ParseForExpression()
        {
            // TODO
            return new ForExpression();
        }

        private IInstruction ParseHtmlTagOrHtmlTagInline()
        {
            // TODO
            return new HtmlInlineTag();
        }

        private IInstruction ParseValueOfOrFunctionCall()
        {
            // TODO
            return new ValueOf();
        }

        private void ParseDefClosingTag()
        {
            List<TokenType> expectedTokenTypes = new List<TokenType> {
                TokenType.TagClose,
                TokenType.Def,
                TokenType.PointyBracketClose
            };
            foreach (var expectedTokenType in expectedTokenTypes)
            {
                if (_scanner.Token.Type != expectedTokenType)
                {
                    throw new ParsingException(expectedTokenType, _scanner.Token.Type);
                }
                _scanner.ReadNextToken();
            }
        }
    }
}