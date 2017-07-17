using System.Collections.Generic;
using System.Linq;

namespace Compiler.Lexing
{
    internal class TokenizerGrammar
    {
        public static TokenizerGrammar Default => new TokenizerGrammar
        {
            Keywords = new List<TokenMatch>
            {
                new TokenMatch(TokenType.Keyword, "import"),
                new TokenMatch(TokenType.Keyword, "module"),
                new TokenMatch(TokenType.Keyword, "public"),
                new TokenMatch(TokenType.Keyword, "private"),
                new TokenMatch(TokenType.Keyword, "internal"),
                new TokenMatch(TokenType.Keyword, "class"),
                new TokenMatch(TokenType.Keyword, "interface"),
                new TokenMatch(TokenType.Keyword, "new"),
                new TokenMatch(TokenType.Keyword, "if"),
                new TokenMatch(TokenType.Keyword, "else"),
                new TokenMatch(TokenType.Keyword, "switch"),
                new TokenMatch(TokenType.Keyword, "case"),
                new TokenMatch(TokenType.Keyword, "default"),
                new TokenMatch(TokenType.Keyword, "break"),
                new TokenMatch(TokenType.Keyword, "return"),
                new TokenMatch(TokenType.Keyword, "while"),
                new TokenMatch(TokenType.Keyword, "do"),
                new TokenMatch(TokenType.Keyword, "for"),
                new TokenMatch(TokenType.Keyword, "var"),
                new TokenMatch(TokenType.Keyword, "true"),
                new TokenMatch(TokenType.Keyword, "false"),
                new TokenMatch(TokenType.Keyword, "int"),
                new TokenMatch(TokenType.Keyword, "string"),
                new TokenMatch(TokenType.Keyword, "void"),
                new TokenMatch(TokenType.Keyword, "float"),
                new TokenMatch(TokenType.Keyword, "double"),
                new TokenMatch(TokenType.Keyword, "decimal"),
                new TokenMatch(TokenType.Keyword, "char"),
                new TokenMatch(TokenType.Keyword, "try"),
                new TokenMatch(TokenType.Keyword, "catch")
            },
            SpecialCharacters = new List<TokenMatch>
            {
                new TokenMatch(TokenType.LineComment, "//"),
                new TokenMatch(TokenType.BlockComment, "/*"),
                new TokenMatch(TokenType.BlockComment, "*/"),
                new TokenMatch(TokenType.LeftBracket, "{"),
                new TokenMatch(TokenType.RightBracket, "}"),
                new TokenMatch(TokenType.LeftBrace, "["),
                new TokenMatch(TokenType.RightBrace, "]"),
                new TokenMatch(TokenType.LeftParenthesis, "("),
                new TokenMatch(TokenType.RightParenthesis, ")"),
                new TokenMatch(TokenType.GreaterThanOrEqual, ">="),
                new TokenMatch(TokenType.GreaterThan, ">"),
                new TokenMatch(TokenType.LessThan, "<"),
                new TokenMatch(TokenType.LessThanOrEqual, "<="),
                new TokenMatch(TokenType.PlusEqual, "+="),
                new TokenMatch(TokenType.PlusPlus, "++"),
                new TokenMatch(TokenType.Plus, "+"),
                new TokenMatch(TokenType.MinusEqual, "-="),
                new TokenMatch(TokenType.MinusMinus, "--"),
                new TokenMatch(TokenType.Minus, "-"),
                new TokenMatch(TokenType.Assignment, "="),
                new TokenMatch(TokenType.Not, "!"),
                new TokenMatch(TokenType.NotEqual, "!="),
                new TokenMatch(TokenType.Mul, "*"),
                new TokenMatch(TokenType.MulEqual, "*="),
                new TokenMatch(TokenType.Div, "/"),
                new TokenMatch(TokenType.DivEqual, "/="),
                new TokenMatch(TokenType.BooleanAnd, "&&"),
                new TokenMatch(TokenType.BooleanOr, "||"),
                new TokenMatch(TokenType.BitwiseAnd, "&"),
                new TokenMatch(TokenType.BitwiseOr, "|"),
                new TokenMatch(TokenType.BitwiseAndEqual, "&="),
                new TokenMatch(TokenType.BitwiseOrEqual, "|="),
                new TokenMatch(TokenType.ModEqual, "%="),
                new TokenMatch(TokenType.Mod, "%"),
                new TokenMatch(TokenType.BitwiseXorEqual, "^="),
                new TokenMatch(TokenType.BitwiseXor, "^"),
                new TokenMatch(TokenType.DoubleQuestion, "??"),
                new TokenMatch(TokenType.Question, "?"),
                new TokenMatch(TokenType.Equal, "=="),
                new TokenMatch(TokenType.BitShiftLeft, "<<"),
                new TokenMatch(TokenType.BitShiftRight, ">>"),
                new TokenMatch(TokenType.Dot, "."),
                new TokenMatch(TokenType.Comma, ","),
                new TokenMatch(TokenType.Semicolon, ";"),
                new TokenMatch(TokenType.Colon, ":"),
                new TokenMatch(TokenType.FatArrow, "=>"),
            }
        };

        public List<TokenMatch> Keywords { get; set; }
        public List<TokenMatch> SpecialCharacters { get; set; }

        public TokenizerGrammar()
        {
            Keywords = Enumerable.Empty<TokenMatch>().ToList();
            SpecialCharacters = Enumerable.Empty<TokenMatch>().ToList();
        }
    }
}
