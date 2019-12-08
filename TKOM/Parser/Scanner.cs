using System.Collections.Generic;
using System.IO;
using static TKOM.Parser.Token;

namespace TKOM.Parser
{
    public class Scanner
    {
        private readonly StreamReader _streamReader;
        public Scanner(StreamReader streamReader)
        {
            _streamReader = streamReader;
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
            while (_streamReader.Peek() == ' ' 
                || _streamReader.Peek() == '\n'
                || _streamReader.Peek() == '\r')
            {
                _streamReader.Read();
            }
        }

        private bool ReadKeyword()
        {
            string buffer = "";
            if (_streamReader.Peek() == '<'         // def, for, if, else
                || _streamReader.Peek() == 'n'      // not
                || _streamReader.Peek() == 'i')     // in
            {
                do
                {
                    buffer += (char) _streamReader.Read();
                }
                while (IsLetter() || _streamReader.Peek() == '/' || _streamReader.Peek() == ':');
                if (_streamReader.Peek() == '>')
                {
                    buffer += (char) _streamReader.Read();
                }
                else if (_streamReader.Peek() != ' ')
                {
                    // there must be a space after keywords that do not end with '>',
                    // we can't accept keywords such as 'in?' or '<:def!'
                    Rewind(buffer.Length);
                    return false;
                }
                TokenType tokenType;
                if (Keywords.KeywordDict.TryGetValue(buffer, out tokenType))
                {
                    Token = new Token(tokenType);
                    return true;
                }
            }
            Rewind(buffer.Length);
            return false;
        }

        private bool ReadSpecialTwoCharSign()
        {
            Dictionary<char, TokenType> possibleFollowingSignsDict;
            string buffer = "";
            if (Keywords.FollowedSignDict.TryGetValue((char)_streamReader.Peek(), out possibleFollowingSignsDict))
            {
                buffer += (char)_streamReader.Read();
                TokenType twoSignTokenType;
                if (possibleFollowingSignsDict.TryGetValue((char)_streamReader.Peek(), out twoSignTokenType))
                {
                    _streamReader.Read();
                    Token = new Token(twoSignTokenType);
                    return true;
                }
            }
            Rewind(buffer.Length);
            return false;
        }

        private bool ReadSpecialOneCharSign()
        {
            TokenType tokenType;
            if (Keywords.SignDict.TryGetValue((char)_streamReader.Peek(), out tokenType))
            {
                _streamReader.Read();
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
                    buffer += (char) _streamReader.Read();
                }
                while (IsDigit());
                if (_streamReader.Peek() == '.' || _streamReader.Peek() == ' '
                    || _streamReader.Peek() == ')')
                {
                    Token = new Token(TokenType.Number, buffer);
                    return true;
                }
                Rewind(buffer.Length);
            }
            return false;
        }

        private bool ReadIdentifier()
        {
            if (IsLetter() || _streamReader.Peek() == '_')
            {
                string buffer = "";
                do
                {
                    buffer += (char) _streamReader.Read();
                }
                while (IsAllowedInIdentifierChar());
                if (_streamReader.Peek() == '.' || _streamReader.Peek() == '['
                    || _streamReader.Peek() == ' ' || _streamReader.Peek() == ')'
                    || _streamReader.Peek() == '(' || _streamReader.Peek() == ',')
                {
                    Token = new Token(TokenType.Identifier, buffer);
                    return true;
                }
                Rewind(buffer.Length);
            }
            return false;
        }

        private bool ReadText()
        {
            string buffer = "";
            do
            {
                EscapeCharacterIfNeeded();
                buffer += (char) _streamReader.Read();
            }
            while (IsAllowedInText());
            Token = new Token(TokenType.Text, buffer);
            return true;
        }

        private bool ReadEof()
        {
            if (_streamReader.Peek() == -1)
            {
                Token = new Token(TokenType.Eof);
                return true;
            }
            return false;
        }

        private void EscapeCharacterIfNeeded()
        {
            if (_streamReader.Peek() == '\\')
            {
                _streamReader.Read();
                if (!(_streamReader.Peek() == '{'
                 || _streamReader.Peek() == '<'
                 || _streamReader.Peek() == '}'
                 || _streamReader.Peek() == '>'
                 || _streamReader.Peek() == '"'
                 || _streamReader.Peek() == '\\'))
                {
                    Rewind(1);
                }
            }
        }

        private bool IsLetter()
        {
            var peeked_char = _streamReader.Peek();
            if ((peeked_char >= 'A' && peeked_char <= 'Z')
                || (peeked_char >= 'a' && peeked_char <= 'z'))
            {
                return true;
            }
            return false;
        }

        private bool IsDigit()
        {
            var peeked_char = _streamReader.Peek();
            if ((peeked_char >= '0' && peeked_char <= '9'))
            {
                return true;
            }
            return false;
        }

        private bool IsAllowedInIdentifierChar()
        {
            var peeked_char = _streamReader.Peek();
            if (IsLetter() || IsDigit() || peeked_char == '_' || peeked_char == '-')
            {
                return true;
            }
            return false;
        }

        private bool IsAllowedInText()
        {
            var peeked_char = _streamReader.Peek();
            // if it starts with special sign (function definitions, if expressions etc
            // are included since they all start with '<' which is a special sign itself)
            if (peeked_char == -1 || Keywords.SignDict.ContainsKey((char)peeked_char)
             || Keywords.FollowedSignDict.ContainsKey((char)peeked_char))
            {
                return false;
            }
            return true;
        }


        private void Rewind(int bufferLength)
        {
            _streamReader.BaseStream.Seek(-bufferLength, SeekOrigin.Current);
        }
    }
}