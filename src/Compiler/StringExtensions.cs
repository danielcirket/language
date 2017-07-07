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
    }
}
