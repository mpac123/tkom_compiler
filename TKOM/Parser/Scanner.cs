using System.Collections.Generic;
using System.IO;
using static TKOM.Parser.Token;

namespace TKOM.Parser
{
    public class Scanner
    {
        private readonly Reader _reader;
        public Scanner(StreamReader streamReader)
        {
            _reader = new Reader(streamReader);
        }
        public Token Token { private set; get; }
        public void ReadNextToken()
        {
            SkipWhitespaces();

            if (ReadEof())
            {
                return;
            }

            if (ReadKeyword()
                || ReadSpecialTwoCharSign()
                || ReadSpecialOneCharSign()
                || ReadNumber()
                || ReadIdentifier()
                || ReadText()
            )
            {
                return;
            }

            // TODO: throw an error
        }

        private void SkipWhitespaces()
        {
            while (_reader.Peek() == ' ' 
                || _reader.Peek() == '\n'
                || _reader.Peek() == '\r')
            {
                _reader.Read();
                _reader.ClearBuffer();
            }
        }

        private bool ReadKeyword()
        {
            string buffer = "";
            if (_reader.Peek() == '<'         // def, for, if, else
                || _reader.Peek() == 'n'      // not
                || _reader.Peek() == 'i')     // in
            {
                do
                {
                    buffer += (char) _reader.Read();
                }
                while (IsLetter() || _reader.Peek() == '/' || _reader.Peek() == ':');
                if (_reader.Peek() == '>')
                {
                    buffer += (char) _reader.Read();
                }
                else if (_reader.Peek() != ' ')
                {
                    // there must be a space after keywords that do not end with '>',
                    // we can't accept keywords such as 'in?' or '<:def!'
                    _reader.Rewind(buffer.Length);
                    return false;
                }
                TokenType tokenType;
                if (Keywords.KeywordDict.TryGetValue(buffer, out tokenType))
                {
                    _reader.ClearBuffer();
                    Token = new Token(tokenType);
                    return true;
                }
            }
            _reader.Rewind(buffer.Length);
            return false;
        }

        private bool ReadSpecialTwoCharSign()
        {
            Dictionary<char, TokenType> possibleFollowingSignsDict;
            string buffer = "";
            if (Keywords.FollowedSignDict.TryGetValue((char)_reader.Peek(), out possibleFollowingSignsDict))
            {
                buffer += (char)_reader.Read();
                TokenType twoSignTokenType;
                if (possibleFollowingSignsDict.TryGetValue((char)_reader.Peek(), out twoSignTokenType))
                {
                    _reader.Read();
                    _reader.ClearBuffer();
                    Token = new Token(twoSignTokenType);
                    return true;
                }
            }
            _reader.Rewind(buffer.Length);
            return false;
        }

        private bool ReadSpecialOneCharSign()
        {
            TokenType tokenType;
            if (Keywords.SignDict.TryGetValue((char)_reader.Peek(), out tokenType))
            {
                _reader.Read();
                _reader.ClearBuffer();
                Token = new Token(tokenType);
                return true;
            }
            return false;
        }


        private bool ReadNumber()
        {
            if (IsDigit())
            {
                string buffer = "";
                do
                {
                    buffer += (char) _reader.Read();
                }
                while (IsDigit());
                if (_reader.Peek() == '.' || _reader.Peek() == ' '
                    || _reader.Peek() == ')' || _reader.Peek() == ']')
                {
                    _reader.ClearBuffer();
                    Token = new Token(TokenType.Number, buffer);
                    return true;
                }
                _reader.Rewind(buffer.Length);
            }
            return false;
        }

        private bool ReadIdentifier()
        {
            if (IsLetter() || _reader.Peek() == '_')
            {
                string buffer = "";
                do
                {
                    buffer += (char) _reader.Read();
                }
                while (IsAllowedInIdentifierChar());
                if (_reader.Peek() == '.' || _reader.Peek() == '['
                    || _reader.Peek() == ' ' || _reader.Peek() == ')'
                    || _reader.Peek() == '(' || _reader.Peek() == ','
                    || _reader.Peek() == '}')
                {
                    _reader.ClearBuffer();
                    Token = new Token(TokenType.Identifier, buffer);
                    return true;
                }
                _reader.Rewind(buffer.Length);
            }
            return false;
        }

        private bool ReadText()
        {
            string buffer = "";
            do
            {
                EscapeCharacterIfNeeded();
                buffer += (char) _reader.Read();
            }
            while (IsAllowedInText());
            _reader.ClearBuffer();
            Token = new Token(TokenType.Text, buffer);
            return true;
        }

        private bool ReadEof()
        {
            if (_reader.IsEndOfStream())
            {
                Token = new Token(TokenType.Eof);
                return true;
            }
            return false;
        }

        private void EscapeCharacterIfNeeded()
        {
            if (_reader.Peek() == '\\')
            {
                _reader.Read();
                if (!(_reader.Peek() == '{'
                 || _reader.Peek() == '<'
                 || _reader.Peek() == '}'
                 || _reader.Peek() == '>'
                 || _reader.Peek() == '"'
                 || _reader.Peek() == '\\'))
                {
                    _reader.Rewind(1);
                }
            }
        }

        private bool IsLetter()
        {
            var peeked_char = _reader.Peek();
            if ((peeked_char >= 'A' && peeked_char <= 'Z')
                || (peeked_char >= 'a' && peeked_char <= 'z'))
            {
                return true;
            }
            return false;
        }

        private bool IsDigit()
        {
            var peeked_char = _reader.Peek();
            if ((peeked_char >= '0' && peeked_char <= '9'))
            {
                return true;
            }
            return false;
        }

        private bool IsAllowedInIdentifierChar()
        {
            var peeked_char = _reader.Peek();
            if (IsLetter() || IsDigit() || peeked_char == '_' || peeked_char == '-')
            {
                return true;
            }
            return false;
        }

        private bool IsAllowedInText()
        {
            var peeked_char = _reader.Peek();
            // if it starts with special sign (function definitions, if expressions etc
            // are included since they all start with '<' which is a special sign itself)
            if (peeked_char == -1 || Keywords.SignDict.ContainsKey((char)peeked_char)
             || Keywords.FollowedSignDict.ContainsKey((char)peeked_char))
            {
                return false;
            }
            return true;
        }

    }
}