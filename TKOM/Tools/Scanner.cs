using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TKOM.Readers;
using TKOM.Utils;
using static TKOM.Utils.Token;

namespace TKOM.Tools
{
    public class Scanner : IScanner
    {
        private readonly IReader _reader;
        public Scanner(IReader reader)
        {
            _reader = reader;
        }
        public Token Token { private set; get; }
        private int line = 1;
        private int column = 1;

        public void ReadNextToken()
        {
            SkipWhitespaces();

            line = _reader.Line;
            column = _reader.Column;

            if (TryReadEof())
            {
                return;
            }

            if (TryReadKeyword()
                || TryReadSpecialSign()
                || TryReadNumber()
                || TryReadIdentifier()
            )
            {
                return;
            }

            Token = new Token(TokenType.Invalid, line, column);
        }

        public bool TryReadText()
        {
            var buffer = new StringBuilder();
            buffer.Append(ReadWhitespacesUnlessNewLine());
            while (IsAllowedInText())
            {
                EscapeCharacterInTextIfNeeded();
                buffer.Append((char)_reader.CurrentSign);
                _reader.Read();
            }
            if (buffer.Length > 0)
            {

                Token = new Token(TokenType.Text, buffer.ToString(), line, column);
                return true;
            }
            return false;
        }

        public bool TryReadString()
        {
            var buffer = new StringBuilder();
            while (IsAllowedInString())
            {
                EscapeCharacterInStringIfNeeded();
                buffer.Append((char)_reader.CurrentSign);
                _reader.Read();
            }
            if (buffer.Length > 0)
            {
                Token = new Token(TokenType.Text, buffer.ToString(), line, column);
                return true;
            }
            return false;
        }

        private void SkipWhitespaces()
        {
            while (_reader.CurrentSign == ' '
                || _reader.CurrentSign == '\n'
                || _reader.CurrentSign == '\r')
            {
                _reader.Read();
            }
        }

        private void SkipNewLines()
        {
            while (_reader.CurrentSign == '\n'
                || _reader.CurrentSign == '\r')
            {
                _reader.Read();
            }
        }

        private string ReadWhitespacesUnlessNewLine()
        {
            var builder = new StringBuilder();
            while (_reader.CurrentSign == ' ')
            {
                builder.Append(' ');
                _reader.Read();
            }
            if (_reader.CurrentSign == '\n' ||
                _reader.CurrentSign == '\r')
            {
                SkipWhitespaces();
                return "";
            }
            return builder.ToString();
        }

        private bool TryReadKeyword()
        {
            List<(string tail, TokenType tokenType)> possibleKeywords;
            if (Keywords.KeywordAutomaton.TryGetValue((char)_reader.CurrentSign, out possibleKeywords))
            {
                int signCounter = 0;
                bool matchingFailed = false;
                _reader.Read();
                var possibleTokens = possibleKeywords
                        .Where(pk => pk.tail[signCounter] == _reader.CurrentSign).ToList();
                signCounter++;
                if (possibleTokens.Count() == 0)
                {
                    matchingFailed = true;
                }
                else
                {
                    var possibleToken = possibleTokens.First();
                    while (!matchingFailed && signCounter < possibleToken.tail.Count())
                    {
                        _reader.Read();
                        if (_reader.CurrentSign != possibleToken.tail[signCounter])
                        {
                            matchingFailed = true;
                        }
                        signCounter++;
                    }
                }
                if (matchingFailed)
                {
                    _reader.Rewind(signCounter);
                    return false;
                }
                else
                {
                    Token = new Token(possibleTokens.First().tokenType, line, column);
                    _reader.Read();
                    return true;
                }
            }
            return false;
        }

        private bool TryReadSpecialSign()
        {
            (TokenType? tokenType, Dictionary<char, TokenType> dictionary) possibleFollowingSignsDict;
            if (Keywords.SpecialSignDict.TryGetValue((char)_reader.CurrentSign, out possibleFollowingSignsDict))
            {
                _reader.Read();
                if (possibleFollowingSignsDict.dictionary == null)
                {
                    Token = new Token(possibleFollowingSignsDict.tokenType.Value, line, column);
                    return true;
                }
                TokenType twoSignTokenType;
                if (possibleFollowingSignsDict.dictionary.TryGetValue((char)_reader.CurrentSign, out twoSignTokenType))
                {
                    Token = new Token(twoSignTokenType, line, column);
                    _reader.Read();
                    return true;
                }
                else
                {
                    if (possibleFollowingSignsDict.tokenType != null)
                    {
                        Token = new Token(possibleFollowingSignsDict.tokenType.Value, line, column);
                        return true;
                    }
                    else
                    {
                        _reader.Rewind(1);
                        return false;
                    }
                }
            }
            return false;
        }


        private bool TryReadNumber()
        {
            var buffer = new StringBuilder();
            while (IsDigit())
            {
                buffer.Append((char)_reader.CurrentSign);
                _reader.Read();
            }
            if (buffer.Length > 0)
            {
                if (_reader.CurrentSign == '.' || _reader.CurrentSign == ' '
                    || _reader.CurrentSign == ')' || _reader.CurrentSign == ']'
                    || _reader.CurrentSign == ',' || _reader.CurrentSign == -1)
                {
                    Token = new Token(TokenType.Number, buffer.ToString(), line, column);
                    return true;
                }
                else
                {
                    _reader.Rewind(buffer.Length);
                    return false;
                }
            }
            return false;
        }

        private bool TryReadIdentifier()
        {
            var buffer = new StringBuilder();
            if (IsLetter() || _reader.CurrentSign == '_')
            {
                do
                {
                    buffer.Append((char)_reader.CurrentSign);
                    _reader.Read();
                } while (IsAllowedInIdentifierChar());
            }
            if (buffer.Length > 0)
            {
                if (_reader.CurrentSign == '.' || _reader.CurrentSign == '['
                    || _reader.CurrentSign == ' ' || _reader.CurrentSign == ')'
                    || _reader.CurrentSign == '(' || _reader.CurrentSign == ','
                    || _reader.CurrentSign == '}' || _reader.CurrentSign == '\n'
                    || _reader.CurrentSign == '\r' || _reader.CurrentSign == '>'
                    || _reader.CurrentSign == '/' || _reader.CurrentSign == '='
                    || _reader.CurrentSign == -1)
                {
                    Token = new Token(TokenType.Identifier, buffer.ToString(), line, column);
                    return true;
                }
                else
                {
                    _reader.Rewind(buffer.Length);
                    return false;
                }
            }
            return false;
        }



        private bool TryReadEof()
        {
            if (_reader.CurrentSign == -1)
            {
                Token = new Token(TokenType.Eof, line, column);
                return true;
            }
            return false;
        }

        private void EscapeCharacterInTextIfNeeded()
        {
            if (_reader.CurrentSign == '\\')
            {
                _reader.Read();
                if (!(_reader.CurrentSign == '{'
                 || _reader.CurrentSign == '<'
                 || _reader.CurrentSign == '}'
                 || _reader.CurrentSign == '\\'))
                {
                    _reader.Rewind(1);
                }
            }
        }

        private void EscapeCharacterInStringIfNeeded()
        {
            if (_reader.CurrentSign == '\\')
            {
                _reader.Read();
                if (!(_reader.CurrentSign == '"'
                 || _reader.CurrentSign == '\\'
                 || _reader.CurrentSign == '{'))
                {
                    _reader.Rewind(1);
                }
            }
        }

        private bool IsLetter()
        {
            if ((_reader.CurrentSign >= 'A' && _reader.CurrentSign <= 'Z')
                || (_reader.CurrentSign >= 'a' && _reader.CurrentSign <= 'z'))
            {
                return true;
            }
            return false;
        }

        private bool IsDigit()
        {
            if ((_reader.CurrentSign >= '0' && _reader.CurrentSign <= '9'))
            {
                return true;
            }
            return false;
        }

        private bool IsAllowedInIdentifierChar()
        {
            if (IsLetter() || IsDigit() || _reader.CurrentSign == '_' || _reader.CurrentSign == '-')
            {
                return true;
            }
            return false;
        }

        private bool IsAllowedInText()
        {
            // if it starts with special sign (function definitions, if expressions etc
            // are included since they all start with '<' which is a special sign itself)
            if (_reader.CurrentSign == -1 || _reader.CurrentSign == '<'
                || _reader.CurrentSign == '{')
            {
                return false;
            }
            return true;
        }

        private bool IsAllowedInString()
        {
            // the only thing that is not allowed inside string is " or {}
            if (_reader.CurrentSign == -1 || _reader.CurrentSign == '"'
                || _reader.CurrentSign == '{')
            {
                return false;
            }
            return true;
        }

    }
}