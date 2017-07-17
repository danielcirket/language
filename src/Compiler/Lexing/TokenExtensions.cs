using System;

namespace Compiler.Lexing
{
    internal static class TokenExtensions
    {
        public static bool IsTrivia(this Token source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Category == TokenCategory.Whitespace || source.Category == TokenCategory.Comment;
        }
    }
}
