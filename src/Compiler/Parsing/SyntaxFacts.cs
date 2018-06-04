using System;
using System.Collections.Generic;
using System.Text;
using Compiler.Lexing;
using Compiler.Parsing.Syntax;

namespace Compiler.Parsing
{
    internal class SyntaxFacts
    {
        public static bool IsPredefinedType(Token token)
        {
            switch (token.TokenType)
            {
                case TokenType.BoolKeyword:
                case TokenType.ByteKeyword:
                case TokenType.SByteKeyword:
                case TokenType.IntKeyword:
                case TokenType.UIntKeyword:
                case TokenType.ShortKeyword:
                case TokenType.UShortKeyword:
                case TokenType.LongKeyword:
                case TokenType.ULongKeyword:
                case TokenType.FloatKeyword:
                case TokenType.DoubleKeyword:
                case TokenType.DecimalKeyword:
                case TokenType.StringKeyword:
                case TokenType.VoidKeyword:
                    return true;
            }

            return false;
        }
        public static SyntaxKind PredefinedTypeExpressionKind(Token token)
        {
            switch (token.TokenType)
            {
                case TokenType.BoolKeyword:
                    return SyntaxKind.BoolKeyword;
                case TokenType.ByteKeyword:
                    return SyntaxKind.ByteKeyword;
                case TokenType.SByteKeyword:
                    return SyntaxKind.SByteKeyword;
                case TokenType.IntKeyword:
                    return SyntaxKind.IntKeyword;
                case TokenType.UIntKeyword:
                    return SyntaxKind.UIntKeyword;
                case TokenType.ShortKeyword:
                    return SyntaxKind.ShortKeyword;
                case TokenType.UShortKeyword:
                    return SyntaxKind.UShortKeyword;
                case TokenType.LongKeyword:
                    return SyntaxKind.LongKeyword;
                case TokenType.ULongKeyword:
                    return SyntaxKind.ULongKeyword;
                case TokenType.FloatKeyword:
                    return SyntaxKind.FloatKeyword;
                case TokenType.DoubleKeyword:
                    return SyntaxKind.DoubleKeyword;
                case TokenType.DecimalKeyword:
                    return SyntaxKind.DecimalKeyword;
                case TokenType.StringKeyword:
                    return SyntaxKind.StringKeyword;
                case TokenType.VoidKeyword:
                    return SyntaxKind.VoidKeyword;
            }

            throw new Exception($"'{token.TokenType}' is not a predefined type");
        }
    }
}