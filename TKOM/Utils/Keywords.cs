using System.Collections.Generic;
using static TKOM.Utils.Token;
using TKOM.Structures.AST;

namespace TKOM.Utils
{
    public static class Keywords
    {

        public static Dictionary<char, List<(string tail, TokenType tokenType)>> KeywordAutomaton
                = new Dictionary<char, List<(string, TokenType)>> {
                    {':', new List<(string, TokenType)> {
                        {("def", TokenType.Def)},
                        {("for", TokenType.For)},
                        {("if", TokenType.If)},
                        {("else", TokenType.Else)}
                    }},
                    {'i', new List<(string, TokenType)> {
                        {("n", TokenType.In)}
                    }}
                };
        public static Dictionary<char, (TokenType?, Dictionary<char, TokenType>)> SpecialSignDict
                = new Dictionary<char, (TokenType?, Dictionary<char, TokenType>)> {
                    {'<', (TokenType.PointyBracketOpen,
                            new Dictionary<char, TokenType> {{'=', TokenType.LessEqualThan},
                                                            {'/', TokenType.TagClose}})},
                    {'>', (TokenType.PointyBracketClose,
                            new Dictionary<char, TokenType> {{'=', TokenType.GreaterEqualThan}})},
                    {'=', (TokenType.AssignmentMark,
                            new Dictionary<char, TokenType> {{'=', TokenType.Equal}})},
                    {'/', (null,
                            new Dictionary<char, TokenType> {{'>', TokenType.TagCloseInline}})},
                    {'{', (TokenType.CurlyBracketOpen, null)},
                    {'}', (TokenType.CurlyBracketClose, null)},
                    {'(', (TokenType.ParenthesisOpen, null)},
                    {')', (TokenType.ParenthesisClose, null)},
                    {',', (TokenType.Coma, null)},
                    {'.', (TokenType.Dot, null)},
                    {'[', (TokenType.SquareBracketOpen, null)},
                    {']', (TokenType.SquareBracketClose, null)},
                    {'"', (TokenType.QuotationMark, null)},
                    {'!', (TokenType.ExclamationMark,
                            new Dictionary<char, TokenType> {{'=', TokenType.NotEqual}})}
                };

        public static IDictionary<TokenType, ConditionType> ConditionTypeDict =
        new Dictionary<TokenType, ConditionType>
        {
            {TokenType.NotEqual, ConditionType.NotEqual},
            {TokenType.Equal, ConditionType.Equal},
            {TokenType.GreaterEqualThan, ConditionType.GreaterEqualThan},
            {TokenType.PointyBracketClose, ConditionType.GreaterThan},
            {TokenType.LessEqualThan, ConditionType.LessEqualThan},
            {TokenType.PointyBracketOpen, ConditionType.LessThan}
        };

    }
}