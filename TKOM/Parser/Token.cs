namespace TKOM.Parser
{
    public class Token
    {
        public enum TokenType {
            Text,
            Number,
            Identifier,        // '\' - used as an escape character
            CurlyBracketOpen,
            CurlyBracketClose,
            ParenthesisOpen,
            ParenthesisClose,
            Coma,
            Dot,
            QuotationMark,
            SquareBracketOpen,
            SquareBracketClose,
            GreaterThan,
            LessThan,
            GreaterEqualThan,
            LessEqualThan,
            Equal,
            FunctionOpen,       // <:def
            FunctionClose,      // </:def>
            ForExprOpen,        // <:for
            ForExprClose,       // </:for>
            IfExprOpen,         // <:if
            IfExprClose,        // </:if>
            ElseExprOpen,       // <:else>
            ElseExprClose,       // </:else>
            In,
            Not,
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