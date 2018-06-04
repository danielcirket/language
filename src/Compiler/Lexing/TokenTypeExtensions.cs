using System.Collections.Generic;

namespace Compiler.Lexing
{
    internal static class TokenTypeExtensions
    {
        private static Dictionary<TokenType, string> _lookup = new Dictionary<TokenType, string>
        {
            { TokenType.LineComment, "# or //" }, // #, //
            { TokenType.BlockComment, "/* or */" }, // /* */
            { TokenType.IntegerLiteral, string.Empty },
            { TokenType.StringLiteral, string.Empty },
            { TokenType.RealLiteral, string.Empty },
            { TokenType.Identifier, "Identifier" },
            { TokenType.LeftBracket, "{" }, // {
            { TokenType.RightBracket, "}" }, // }
            { TokenType.RightBrace, "]" }, // ]
            { TokenType.LeftBrace, "[" }, // [
            { TokenType.LeftParenthesis, "(" }, // (
            { TokenType.RightParenthesis, ")" }, // )
            { TokenType.GreaterThanOrEqual, ">=" }, // >=
            { TokenType.GreaterThan, ">" }, // >
            { TokenType.LessThan, "<" }, // <
            { TokenType.LessThanOrEqual, "<=" }, // <=
            { TokenType.PlusEqual, "+=" }, // +=
            { TokenType.PlusPlus, "++" }, // ++
            { TokenType.Plus, "+" }, // +
            { TokenType.MinusEqual, "-=" }, // -=
            { TokenType.MinusMinus, "--" }, // --
            { TokenType.Minus, "-" }, // -
            { TokenType.Assignment, "=" }, // =
            { TokenType.Not, "!" }, // !
            { TokenType.NotEqual,  "!=" },// !=
            { TokenType.Mul, "*" }, // *
            { TokenType.MulEqual, "*=" }, // *=
            { TokenType.Div, "/" }, // /
            { TokenType.DivEqual, "/=" }, // /=
            { TokenType.BooleanAnd, "&&" }, // &&
            { TokenType.BooleanOr, "||" }, // ||
            { TokenType.BitwiseAnd, "&" }, // &
            { TokenType.BitwiseOr, "|" }, // |
            { TokenType.BitwiseAndEqual, "&=" }, // &=
            { TokenType.BitwiseOrEqual, "|=" }, // |=
            { TokenType.ModEqual, "%=" }, // %=
            { TokenType.Mod, "%" }, // %
            { TokenType.BitwiseXorEqual, "^=" }, // ^=
            { TokenType.BitwiseXor, "^" }, // ^
            { TokenType.DoubleQuestion, "??" }, // ??
            { TokenType.Question, "?" }, // ?
            { TokenType.Equal, "==" }, // ==
            { TokenType.BitShiftLeft, "<<" }, // <<
            { TokenType.BitShiftRight, ">>" }, // >>
            { TokenType.Dot, "." },
            { TokenType.Comma, "," },
            { TokenType.Semicolon, ";" },
            { TokenType.Colon, ":" },
            { TokenType.FatArrow, "=>" }, // =>
            { TokenType.CharLiteral, "'" }, // '
        };

        public static string Value(this TokenType source)
        {
            var result = string.Empty;

            _lookup.TryGetValue(source, out result);
                
            return result;
        }
    }
}
