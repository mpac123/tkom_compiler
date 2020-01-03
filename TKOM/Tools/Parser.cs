using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using TKOM.Structures.AST;
using TKOM.Exceptions;
using TKOM.Utils;
using static TKOM.Utils.Token;

namespace TKOM.Tools
{
    public class Parser
    {
        private Scanner _scanner;
        public Parser(Scanner scanner)
        {
            _scanner = scanner;
        }
        public Structures.AST.Program Parse()
        {
            var syntaxTree = new Structures.AST.Program();

            _scanner.ReadNextToken();
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
                    case TokenType.Else:
                        return ParseElseExpression();
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
            else if (_scanner.Token.Type == TokenType.Text)
            {
                var literal = new Literal
                {
                    Content = _scanner.Token.Value
                };
                _scanner.ReadNextToken();
                return literal;
            }
            else
            {
                if (_scanner.TryReadText())
                {
                    var literal = new Literal
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

            ExpectSequenceOfTokens(new TokenType[] { TokenType.TagClose, TokenType.If, TokenType.PointyBracketClose });
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
                        result = ParseConditionWithVariableOrNumericValue(lhs, conditionType);
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

        private ConditionWithValue ParseConditionWithVariableOrNumericValue(ValueOf lhs, ConditionType conditionType)
        {
            var result = new ConditionWithValue
            {
                LeftHandSideVariable = lhs,
                ConditionType = conditionType,
            };

            result.RightHandSideVariable = ParseVariableOrNumericValue();
            return result;
        }

        private NumericValue ParseNumericValue()
        {
            var numericValue = new NumericValue();
            ExpectTokenType(TokenType.Number);
            var value = _scanner.Token.Value;
            _scanner.ReadNextToken();
            if (_scanner.Token.Type == TokenType.Dot)
            {
                var doubleBuilder = new StringBuilder($"{value}.");
                _scanner.ReadNextToken();
                ExpectTokenType(TokenType.Number);
                doubleBuilder.Append(_scanner.Token.Value);
                numericValue.RealValue = double.Parse(doubleBuilder.ToString());
                _scanner.ReadNextToken();
            }
            else
            {
                numericValue.Integer = true;
                numericValue.IntValue = int.Parse(value);
            }
            return numericValue;
        }

        private ICondition ParseConditionWithAnyValue(ValueOf lhs, ConditionType conditionType)
        {
            var result = new ConditionWithValue
            {
                LeftHandSideVariable = lhs,
                ConditionType = conditionType,
                RightHandSideVariable = ParseValue()
            };
            return result;
        }


        private Value ParseVariableOrNumericValue()
        {
            if (_scanner.Token.Type == TokenType.Number)
            {
                return ParseNumericValue();
            }
            else if (_scanner.Token.Type == TokenType.Identifier)
            {
                return ParseValueOf();
            }

            throw new ParsingException($"Expected argument (numeric value, variable name), {_scanner.Token.Type.ToString()} was found instead.");

        }

        private Value ParseValue()
        {
            if (_scanner.Token.Type == TokenType.QuotationMark)
            {
                return ParseString();
            }
            else if (_scanner.Token.Type == TokenType.Number)
            {
                return ParseNumericValue();
            }
            else if (_scanner.Token.Type == TokenType.Identifier)
            {
                return ParseValueOf();
            }

            throw new ParsingException($"Expected argument (numeric value, variable name or string), {_scanner.Token.Type.ToString()} was found instead.");

        }

        private StringValue ParseString()
        {
            var stringValue = new StringValue();
            do
            {
                if (!_scanner.TryReadString())
                {
                    _scanner.ReadNextToken();
                    if (_scanner.Token.Type == TokenType.CurlyBracketOpen)
                    {
                        _scanner.ReadNextToken();
                        stringValue.StringComponents.Add(ParseValueOf());
                        ExpectTokenType(TokenType.CurlyBracketClose);
                    }
                }
                else
                {
                    stringValue.StringComponents.Add(new Literal
                    {
                        Content = _scanner.Token.Value
                    });
                }
            } while (_scanner.Token.Type != TokenType.QuotationMark);
            ExpectTokenType(TokenType.QuotationMark);
            _scanner.ReadNextToken();
            return stringValue;
        }

        private ElseExpression ParseElseExpression()
        {
            var result = new ElseExpression();
            _scanner.ReadNextToken();
            ExpectTokenType(TokenType.PointyBracketClose);
            result.Instructions = ParseInstructionsTillTagClose();
            ExpectSequenceOfTokens(new TokenType[] { TokenType.TagClose, TokenType.Else, TokenType.PointyBracketClose });
            return result;
        }

        private ForExpression ParseForExpression()
        {
            var result = new ForExpression();
            _scanner.ReadNextToken();
            ExpectTokenType(TokenType.ParenthesisOpen);
            _scanner.ReadNextToken();
            ExpectTokenType(TokenType.Identifier);
            result.ElementName = _scanner.Token.Value;
            _scanner.ReadNextToken();
            ExpectTokenType(TokenType.In);
            _scanner.ReadNextToken();
            result.Collection = ParseValueOf();
            ExpectTokenType(TokenType.ParenthesisClose);
            _scanner.ReadNextToken();
            ExpectTokenType(TokenType.PointyBracketClose);
            result.Instructions = ParseInstructionsTillTagClose();
            ExpectSequenceOfTokens(new TokenType[] { TokenType.TagClose, TokenType.For, TokenType.PointyBracketClose });
            return result;
        }

        private IInstruction ParseHtmlTagOrHtmlTagInline()
        {
            var tagName = _scanner.Token.Value;
            _scanner.ReadNextToken();
            var attributeList = ParseAttributeList();
            if (_scanner.Token.Type == TokenType.PointyBracketClose)
            {
                return ParseHtmlTag(tagName, attributeList);
            }
            if (_scanner.Token.Type == TokenType.TagCloseInline)
            {
                if (!_scanner.TryReadText())
                {
                    _scanner.ReadNextToken();
                }
                return new HtmlInlineTag
                {
                    TagName = tagName,
                    Attributes = attributeList
                };
            }
            throw new ParsingException($"Expected '>' or '/>', token of type {_scanner.Token.Type} was found instead.");
        }

        private List<(string attributeName, StringValue attributeValue)> ParseAttributeList()
        {
            var attributeList = new List<(string attributeName, StringValue attributeValue)>();

            while (_scanner.Token.Type != TokenType.PointyBracketClose && _scanner.Token.Type != TokenType.TagCloseInline)
            {
                ExpectTokenType(TokenType.Identifier);
                var name = _scanner.Token.Value;
                _scanner.ReadNextToken();
                if (_scanner.Token.Type == TokenType.AssignmentMark)
                {
                    _scanner.ReadNextToken();
                    ExpectTokenType(TokenType.QuotationMark);
                    var string_value = ParseString();
                    attributeList.Add((name, string_value));
                }
                else
                {
                    attributeList.Add((name, null));
                }
            }
            return attributeList;
        }

        private HtmlTag ParseHtmlTag(string tagName, List<(string, StringValue)> attributeList)
        {
            var result = new HtmlTag
            {
                TagName = tagName,
                Attributes = attributeList
            };
            result.Instructions = ParseInstructionsTillTagClose();
            ExpectTokenType(TokenType.TagClose);
            _scanner.ReadNextToken();
            ExpectTokenType(TokenType.Identifier);
            if (_scanner.Token.Value != tagName)
            {
                throw new ParsingException($"Closing tag expected to be \"{tagName}\", found \"{_scanner.Token.Value}\" instead.");
            }
            _scanner.ReadNextToken();
            ExpectTokenType(TokenType.PointyBracketClose);
            if (!_scanner.TryReadText())
            {
                _scanner.ReadNextToken();
            }
            return result;
        }

        private IInstruction ParseValueOfOrFunctionCall()
        {
            var valueOf = ParseValueOf();
            if (valueOf.Index == null || valueOf.NestedValue == null)
            {
                if (_scanner.Token.Type == TokenType.ParenthesisOpen)
                {
                    _scanner.ReadNextToken();
                    var result = new FunctionCall
                    {
                        FunctionName = valueOf.VariableName,
                        ArgumentValues = ParseArgumentList()
                    };
                    ExpectSequenceOfTokens(new TokenType[] { TokenType.ParenthesisClose, TokenType.CurlyBracketClose });
                    return result;
                }
            }
            ExpectTokenType(TokenType.CurlyBracketClose);
            if (!_scanner.TryReadText())
            {
                _scanner.ReadNextToken();
            }
            return valueOf;
        }

        private List<Value> ParseArgumentList()
        {
            var result = new List<Value>();
            while (_scanner.Token.Type != TokenType.ParenthesisClose)
            {
                result.Add(ParseValue());
                if (_scanner.Token.Type == TokenType.Coma)
                {
                    _scanner.ReadNextToken();
                }
            }
            return result;
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

        private void ExpectSequenceOfTokens(TokenType[] expectedTokens)
        {
            foreach (var expected in expectedTokens)
            {
                ExpectTokenType(expected);
                _scanner.ReadNextToken();
            }
        }
    }
}