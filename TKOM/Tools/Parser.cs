using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using TKOM.AST;
using TKOM.Exceptions;
using TKOM.Utils;
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
            function.Instructions = ParseInstructionsTillTagClose();
            ParseDefClosingTag();
            return function;
        }

        private List<IInstruction> ParseInstructionsTillTagClose()
        {
            var instructions = new List<IInstruction>();
            if (_scanner.TryReadText())
            {
                instructions.Add(new Literal { Content = _scanner.Token.Value });
            }
            _scanner.ReadNextToken();
            while (_scanner.Token.Type != TokenType.TagClose)
            {
                instructions.Add(ParseStatement());
            }
            return instructions;
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
                ExpectTokenType(expectedTokenType);
                if (expectedTokenType == TokenType.Identifier)
                {
                    functionName.Append(_scanner.Token.Value);
                }
                _scanner.ReadNextToken();
            }
            while (_scanner.Token.Type != TokenType.ParenthesisClose)
            {
                ExpectTokenType(TokenType.Identifier);
                argumentNames.Add(_scanner.Token.Value);
                _scanner.ReadNextToken();
                if (_scanner.Token.Type == TokenType.Coma)
                {
                    _scanner.ReadNextToken();
                }
            }

            _scanner.ReadNextToken();
            ExpectTokenType(TokenType.PointyBracketClose);

            return (functionName.ToString(), argumentNames);
        }

        private IInstruction ParseStatement()
        {

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
            else
            {
                if (_scanner.TryReadText())
                {
                    var literal =  new Literal
                    {
                        Content = _scanner.Token.Value
                    };
                    _scanner.ReadNextToken();
                    return literal;
                }
            }
            throw new ParsingException($"Parsing failed.");
        }

        private IfExpression ParseIfExpression()
        {
            var result = new IfExpression();
            _scanner.ReadNextToken();
            ExpectTokenType(TokenType.ParenthesisOpen);
            _scanner.ReadNextToken();
            if (_scanner.Token.Type == TokenType.ExclamationMark)
            {
                result.Negated = true;
                _scanner.ReadNextToken();
            }
            result.Condition = ParseCondition(result.Negated);
            _scanner.ReadNextToken();
            ExpectTokenType(TokenType.PointyBracketClose);
            result.Instructions = ParseInstructionsTillTagClose();
            // TODO: parse else expressions
            var expectedTokens = new TokenType[] { TokenType.TagClose, TokenType.If, TokenType.PointyBracketClose };
            foreach (var expected in expectedTokens)
            {
                ExpectTokenType(expected);
                _scanner.ReadNextToken();
            }
            return result;
        }

        private ICondition ParseCondition(bool negated)
        {
            ConditionType conditionType;
            ICondition result; ;
            var lhs = ParseValueOf();
            if (!negated && _scanner.Token.Type != TokenType.ParenthesisClose)
            {
                if (Keywords.ConditionTypeDict.TryGetValue(_scanner.Token.Type, out conditionType))
                {
                    _scanner.ReadNextToken();
                    if (conditionType == ConditionType.Equal || conditionType == ConditionType.NotEqual)
                    {
                        result = ParseConditionWithAnyValue(lhs, conditionType);
                    }
                    else
                    {
                        result = ParseConditionWithNumericValue(lhs, conditionType);
                    }
                    ExpectTokenType(TokenType.ParenthesisClose);
                    return result;
                }

            }
            ExpectTokenType(TokenType.ParenthesisClose);
            return new SimpleCondition
            {
                LeftHandSideVariable = lhs
            };

        }

        private ValueOf ParseValueOf()
        {
            ExpectTokenType(TokenType.Identifier);
            var result = new ValueOf
            {
                VariableName = _scanner.Token.Value
            };
            _scanner.ReadNextToken();
            if (_scanner.Token.Type == TokenType.SquareBracketOpen)
            {
                _scanner.ReadNextToken();
                ExpectTokenType(TokenType.Number);
                result.Index = Int32.Parse(_scanner.Token.Value);
                _scanner.ReadNextToken();
                ExpectTokenType(TokenType.SquareBracketClose);
                _scanner.ReadNextToken();
            }
            if (_scanner.Token.Type == TokenType.Dot)
            {
                _scanner.ReadNextToken();
                ExpectTokenType(TokenType.Identifier);
                result.NestedValue = ParseValueOf();
            }
            return result;
        }

        private ConditionWithNumericValue ParseConditionWithNumericValue(ValueOf lhs, ConditionType conditionType)
        {
            var result = new ConditionWithNumericValue
            {
                LeftHandSideVariable = lhs,
                ConditionType = conditionType
            };
            ExpectTokenType(TokenType.Number);
            var value = _scanner.Token.Value;
            _scanner.ReadNextToken();
            if (_scanner.Token.Type == TokenType.Dot)
            {
                var floatBuilder = new StringBuilder($"{value}.");
                _scanner.ReadNextToken();
                ExpectTokenType(TokenType.Number);
                floatBuilder.Append(_scanner.Token.Value);
                result.RealValue = float.Parse(floatBuilder.ToString());
                _scanner.ReadNextToken();
            }
            else
            {
                result.Integer = true;
                result.IntValue = int.Parse(value);
            }
            return result;
        }

        private ICondition ParseConditionWithAnyValue(ValueOf lhs, ConditionType conditionType)
        {
            if (_scanner.Token.Type == TokenType.Number)
            {
                return ParseConditionWithNumericValue(lhs, conditionType);
            }
            if (_scanner.Token.Type == TokenType.QuotationMark)
            {
                return ParseConditionWithString(lhs, conditionType);
            }
            return ParseConditionWithVariable(lhs, conditionType);
        }

        private ConditionWithString ParseConditionWithString(ValueOf lhs, ConditionType conditionType)
        {
            var result = new ConditionWithString
            {
                LeftHandSideVariable = lhs,
                ConditionType = conditionType
            };
            if (!_scanner.TryReadString())
            {
                result.RightHandSideValue = "";
            }
            else
            {
                result.RightHandSideValue = _scanner.Token.Value;
            }
            _scanner.ReadNextToken();
            ExpectTokenType(TokenType.QuotationMark);
            _scanner.ReadNextToken();
            return result;
        }

        private ConditionWithVariable ParseConditionWithVariable(ValueOf lhs, ConditionType conditionType)
        {
            var result = new ConditionWithVariable
            {
                LeftHandSideVariable = lhs,
                ConditionType = conditionType,
                RightHandSideVariable = ParseValueOf()
            };
            return result;
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
                ExpectTokenType(expectedTokenType);
                _scanner.ReadNextToken();
            }
        }

        private void ExpectTokenType(TokenType expected)
        {
            if (_scanner.Token.Type != expected)
            {
                throw new ParsingException(expected, _scanner.Token.Type);
            }
        }
    }
}