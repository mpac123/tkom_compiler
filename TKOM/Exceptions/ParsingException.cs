using System;
using static TKOM.Utils.Token;

namespace TKOM.Exceptions
{
    public class ParsingException : Exception
    {
        public ParsingException(string message) 
            : base($"Error while parsing: {message}")
        {

        }

        public ParsingException(string expectedTokenName, string foundTokenName)
            : base($"Error while parsing: expected token of type {expectedTokenName}, {foundTokenName} was found instead.")
        {

        }

        public ParsingException(TokenType expectedToken, TokenType foundToken)
            : base($"Error while parsing: expected token of type {expectedToken.ToString()}, {foundToken.ToString()} was found instead.")
        {

        }
    }
}