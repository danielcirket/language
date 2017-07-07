using System;

namespace Compiler.Tokenize
{
    internal class Token
    {
        public TokenType TokenType { get; }
        public SourceFileLocation Start { get; }
        public SourceFileLocation End { get; }
        public string Value { get; }
        
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
