using System;

namespace Compiler.Lexing
{
    internal class TokenMatch
    {
        public TokenType TokenType { get; }
        public string Value { get; }

        public TokenMatch(TokenType tokenType, string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            TokenType = tokenType;
            Value = value;
        }
    }
}
