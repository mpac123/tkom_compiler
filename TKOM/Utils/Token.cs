namespace TKOM.Utils
{
    public class Token
    {
        public enum TokenType
        {
            Text,
            Number,
            Identifier,
            CurlyBracketOpen,
            CurlyBracketClose,
            ParenthesisOpen,
            ParenthesisClose,
            Coma,
            Dot,
            ExclamationMark,
            AssignmentMark,
            QuotationMark,
            SquareBracketOpen,
            SquareBracketClose,
            PointyBracketClose,
            PointyBracketOpen,
            GreaterEqualThan,
            LessEqualThan,
            Equal,
            NotEqual,
            TagClose,           // </
            TagCloseInline,     // />
            Def,                // :def
            For,                // :for
            If,                 // :if
            Else,               // :else
            In,
            Eof,
            Invalid

        }

        public Token(TokenType type, int line, int column)
        {
            Type = type;
            Column = column - 1;
            Line = line;
        }

        public Token(TokenType type, string value, int line, int column)
        {
            Type = type;
            Value = value;
            Column = column - 1;
            Line = line;
        }
        public TokenType Type { private set; get; }
        public string Value { private set; get; }
        public int Line { private set; get; }
        public int Column { private set; get; }

    }
}