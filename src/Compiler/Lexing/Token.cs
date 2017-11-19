using System;
using System.Collections.Generic;

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

        public override bool Equals(object obj)
        {
            var token = obj as Token;
            return token != null &&
                   Category == token.Category &&
                   TokenType == token.TokenType &&
                   EqualityComparer<SourceFileLocation>.Default.Equals(Start, token.Start) &&
                   EqualityComparer<SourceFileLocation>.Default.Equals(End, token.End) &&
                   Value == token.Value;
        }
        public override int GetHashCode()
        {
            var hashCode = 1923680246;
            hashCode = hashCode * -1521134295 + Category.GetHashCode();
            hashCode = hashCode * -1521134295 + TokenType.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<SourceFileLocation>.Default.GetHashCode(Start);
            hashCode = hashCode * -1521134295 + EqualityComparer<SourceFileLocation>.Default.GetHashCode(End);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
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

                case TokenType.BreakKeyword:
                case TokenType.CaseKeyword:
                case TokenType.CatchKeyword:
                case TokenType.CharKeyword:
                case TokenType.ClassKeyword:
                case TokenType.ConstKeyword:
                case TokenType.ConstructorKeyword:
                case TokenType.DecimalKeyword:
                case TokenType.DefaultKeyword:
                case TokenType.DoKeyword:
                case TokenType.DoubleKeyword:
                case TokenType.ElseKeyword:
                case TokenType.EnumKeyword:
                case TokenType.FalseKeyword:
                case TokenType.FloatKeyword:
                case TokenType.ForKeyword:
                case TokenType.IfKeyword:
                case TokenType.ImportKeyword:
                case TokenType.InterfaceKeyword:
                case TokenType.InternalKeyword:
                case TokenType.IntKeyword:
                case TokenType.LetKeyword:
                case TokenType.ModuleKeyword:
                case TokenType.NewKeyword:
                case TokenType.PrivateKeyword:
                case TokenType.PublicKeyword:
                case TokenType.ReturnKeyword:
                case TokenType.StringKeyword:
                case TokenType.SwitchKeyword:
                case TokenType.TrueKeyword:
                case TokenType.TryKeyword:
                case TokenType.VoidKeyword:
                case TokenType.WhileKeyword:
                    return TokenCategory.Identifier;

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
