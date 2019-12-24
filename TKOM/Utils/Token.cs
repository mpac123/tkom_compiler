namespace TKOM.Utils
{
    public class Token
    {
        public enum TokenType {
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

        public Token(TokenType type)
        {
            Type = type;
        }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
        public TokenType Type {private set; get;}
        public string Value {private set; get;}
        public int Line {private set; get;}
        public int Column {private set; get;}
    }
}