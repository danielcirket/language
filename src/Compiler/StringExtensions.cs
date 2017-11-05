using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler
{
    internal static class StringExtensions
    {
        public static bool IsKeyword(this string source, IEnumerable<string> keywords)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return keywords.Contains(source);
        }

        public static string FirstToLower(this string source)
        {
            if (string.IsNullOrEmpty(source) || source.Length < 1 || char.IsLower(source, 0))
                return source;

            return char.ToLower(source[0]) + source.Substring(1);
        }
    }
}
