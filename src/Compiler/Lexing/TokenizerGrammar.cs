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
                new TokenMatch(TokenType.ImportKeyword, "import"),
                new TokenMatch(TokenType.ModuleKeyword, "module"),
                new TokenMatch(TokenType.PublicKeyword, "public"),
                new TokenMatch(TokenType.PrivateKeyword, "private"),
                new TokenMatch(TokenType.InterfaceKeyword, "internal"),
                new TokenMatch(TokenType.ClassKeyword, "class"),
                new TokenMatch(TokenType.InterfaceKeyword, "interface"),
                new TokenMatch(TokenType.NewKeyword, "new"),
                new TokenMatch(TokenType.IfKeyword, "if"),
                new TokenMatch(TokenType.ElseKeyword, "else"),
                new TokenMatch(TokenType.SwitchKeyword, "switch"),
                new TokenMatch(TokenType.CaseKeyword, "case"),
                new TokenMatch(TokenType.DefaultKeyword, "default"),
                new TokenMatch(TokenType.BreakKeyword, "break"),
                new TokenMatch(TokenType.ReturnKeyword, "return"),
                new TokenMatch(TokenType.WhileKeyword, "while"),
                new TokenMatch(TokenType.DoKeyword, "do"),
                new TokenMatch(TokenType.ForKeyword, "for"),
                new TokenMatch(TokenType.ConstKeyword, "const"),
                new TokenMatch(TokenType.LetKeyword, "let"),
                new TokenMatch(TokenType.TrueKeyword, "true"),
                new TokenMatch(TokenType.FalseKeyword, "false"),
                new TokenMatch(TokenType.TryKeyword, "try"),
                new TokenMatch(TokenType.CatchKeyword, "catch"),
                new TokenMatch(TokenType.ConstructorKeyword, "constructor"),
                new TokenMatch(TokenType.EnumKeyword, "enum"),
                new TokenMatch(TokenType.BoolKeyword, "bool"),
                new TokenMatch(TokenType.ByteKeyword, "byte"),
                new TokenMatch(TokenType.SByteKeyword, "sbyte"),
                new TokenMatch(TokenType.IntKeyword, "int"),
                new TokenMatch(TokenType.UIntKeyword, "uint"),
                new TokenMatch(TokenType.ShortKeyword, "short"),
                new TokenMatch(TokenType.UShortKeyword, "ushort"),
                new TokenMatch(TokenType.LongKeyword, "long"),
                new TokenMatch(TokenType.ULongKeyword, "ulong"),
                new TokenMatch(TokenType.FloatKeyword, "float"),
                new TokenMatch(TokenType.DoubleKeyword, "double"),
                new TokenMatch(TokenType.DecimalKeyword, "decimal"),
                new TokenMatch(TokenType.StringKeyword, "string"),
                //new TokenMatch(TokenType.ObjectKeyword,
                new TokenMatch(TokenType.VoidKeyword, "void"),
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
