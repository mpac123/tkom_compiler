using TKOM.Readers;
using TKOM.Tools;
using Xunit;
using static TKOM.Utils.Token;

namespace TKOM.Test.Utils
{
    public static class ScannerUtils
    {
        public static Scanner PrepareScanner(string s)
        {
            var reader = new StringsReader(s);
            var scanner = new Scanner(reader);
            return scanner;
        }

        public static void ActAndValidate(this Scanner scanner, params (TokenType Type, string Value)[] expectedTokens)
        {
            foreach (var expectedToken in expectedTokens)
            {
                if (expectedToken.Type == TokenType.Text)
                {
                    scanner.TryReadText();
                }
                else
                {
                    scanner.ReadNextToken();
                }
                Assert.Equal(expectedToken.Type, scanner.Token.Type);
                Assert.Equal(expectedToken.Value, scanner.Token.Value);
            }
        }

        public static void ActAndValidate(this Scanner scanner, params TokenType[] expectedTokenTypes)
        {
            foreach (var tokenType in expectedTokenTypes)
            {
                if (tokenType == TokenType.Text)
                {
                    scanner.TryReadText();
                }
                else
                {
                    scanner.ReadNextToken();
                }
                Assert.Equal(tokenType, scanner.Token.Type);
            }
        }
    }
}