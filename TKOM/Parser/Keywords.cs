using System.Collections.Generic;
using static TKOM.Parser.Token;

namespace TKOM.Parser
{
    public static class Keywords
    {
        public static Dictionary<string, TokenType> KeywordDict
                = new Dictionary<string, TokenType> {
                    {"<:def", TokenType.FunctionOpen},
                    {"</:def>", TokenType.FunctionClose},
                    {"<:for", TokenType.ForExprOpen},
                    {"</:for>", TokenType.ForExprClose},
                    {"<:if", TokenType.IfExprOpen},
                    {"</:if>", TokenType.IfExprClose},
                    {"<:else>", TokenType.ElseExprOpen},
                    {"</:else>", TokenType.ElseExprClose},
                    {"in", TokenType.In},
                    {"not", TokenType.Not}
        };

        public static Dictionary<char, TokenType> SignDict
                = new Dictionary<char, TokenType> {
                    {'{', TokenType.CurlyBracketOpen},
                    {'}', TokenType.CurlyBracketClose},
                    {'(', TokenType.ParenthesisOpen},
                    {')', TokenType.ParenthesisClose},
                    {',', TokenType.Coma},
                    {'.', TokenType.Dot},
                    {'[', TokenType.SquareBracketOpen},
                    {']', TokenType.SquareBracketClose},
                    {'>', TokenType.GreaterThan},
                    {'<', TokenType.LessThan}
                };

        public static Dictionary<char, Dictionary<char, TokenType>> FollowedSignDict
                = new Dictionary<char, Dictionary<char, TokenType>> {
                    {'<', new Dictionary<char, TokenType> {{'=', TokenType.LessEqualThan}}},
                    {'>', new Dictionary<char, TokenType> {{'=', TokenType.GreaterEqualThan}}},
                    {'=', new Dictionary<char, TokenType> {{'=', TokenType.Equal}}},
                };

    }
}