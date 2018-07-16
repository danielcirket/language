using System;
using System.Collections.Generic;
using System.Text;

namespace Sample
{
    internal static class StringExtensions
    {
        public static string TrimStart(this string source, string find)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            while (source.StartsWith(find))
            {
                var index = -1;

                while ((index = source.IndexOf(find)) == 0)
                    source = source.Substring(index + find.Length, source.Length - find.Length);
            }

            return source;
        }
    }
}
