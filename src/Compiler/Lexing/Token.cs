using System;

namespace Compiler.Lexing
{
    internal class Token
    {
        public TokenCategory Category => GetTokenCategory();
        public TokenType TokenType { get; }
        public SourceFileLocation Start { get; }
        public SourceFileLocation End { get; }
        public string Value { get; }

        public static bool operator != (TokenType left, Token right)
        {
            return left != right?.TokenType;
        }
        public static bool operator == (TokenType left, Token right)
        {
            return left == right?.TokenType;
        }
        public static bool operator != (Token left, TokenType right)
        {
            return left?.TokenType != right;
        }
        public static bool operator == (Token left, TokenType right)
        {
            return left?.TokenType == right;
        }
        public static bool operator != (string left, Token right)
        {
            return left != right?.Value;
        }
        public static bool operator == (string left, Token right)
        {
            return left == right?.Value;
        }
        public static bool operator != (Token left, string right)
        {
            return left?.Value != right;
        }
        public static bool operator == (Token left, string right)
        {
            return left?.Value == right;
        }

        private TokenCategory GetTokenCategory()
        {
            switch (TokenType)
            {
                case TokenType.LineComment:
                case TokenType.BlockComment:
                    return TokenCategory.Comment;

                case TokenType.IntegerLiteral:
                case TokenType.StringLiteral:
                case TokenType.RealLiteral:
                case TokenType.CharLiteral:
                    return TokenCategory.Constant;

                case TokenType.Error:
                    return TokenCategory.Invalid;

                case TokenType.Whitespace:
                case TokenType.NewLine:
                    return TokenCategory.Whitespace;

                case TokenType.Identifier:
                case TokenType.Keyword:
                    return TokenCategory.Identifier;
                
                case TokenType.LeftBracket:
                case TokenType.RightBracket:
                case TokenType.LeftBrace:
                case TokenType.RightBrace:
                case TokenType.LeftParenthesis:
                case TokenType.RightParenthesis:
                    return TokenCategory.Grouping;
                
                case TokenType.GreaterThanOrEqual:
                case TokenType.GreaterThan:
                case TokenType.LessThan:
                case TokenType.LessThanOrEqual:
                case TokenType.PlusEqual:
                case TokenType.PlusPlus:
                case TokenType.Plus:
                case TokenType.MinusEqual: 
                case TokenType.MinusMinus:
                case TokenType.Minus:
                case TokenType.Assignment:
                case TokenType.Not:
                case TokenType.NotEqual:
                case TokenType.Mul:
                case TokenType.MulEqual:
                case TokenType.Div:
                case TokenType.DivEqual:
                case TokenType.BooleanAnd:
                case TokenType.BooleanOr:
                case TokenType.BitwiseAnd:
                case TokenType.BitwiseOr:
                case TokenType.BitwiseAndEqual:
                case TokenType.BitwiseOrEqual:
                case TokenType.ModEqual:
                case TokenType.Mod:
                case TokenType.BitwiseXorEqual:
                case TokenType.BitwiseXor:
                case TokenType.DoubleQuestion:
                case TokenType.Question:
                case TokenType.Equal:
                case TokenType.BitShiftLeft:
                case TokenType.BitShiftRight:
                    return TokenCategory.Operator;
                
                case TokenType.Dot:
                case TokenType.Comma:
                case TokenType.Semicolon:
                case TokenType.Colon:
                case TokenType.FatArrow:
                    return TokenCategory.Punctuation;

                default:
                    return TokenCategory.Unknown;
            }
        }
        
        public Token(TokenType type, string value, SourceFileLocation start, SourceFileLocation end)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (start == null)
                throw new ArgumentNullException(nameof(start));

            if (end == null)
                throw new ArgumentNullException(nameof(end));

            TokenType = type;
            Value = value;
            Start = start;
            End = end;
        }
    }
}
