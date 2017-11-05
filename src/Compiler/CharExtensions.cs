using System.Linq;

namespace Compiler
{
    internal static class CharExtensions
    {
        public static bool IsEOF(this char source)
        {
            return source == '\0';
        }
        public static bool IsDigit(this char source)
        {
            return char.IsDigit(source);
        }
        public static bool IsLetter(this char source)
        {
            return char.IsLetter(source);
        }
        public static bool IsLetterOrDigit(this char source)
        {
            return char.IsLetterOrDigit(source);
        }
        public static bool IsNewLine(this char source)
        {
            return source == '\n';
        }
        public static bool IsWhiteSpace(this char source)
        {
            return (char.IsWhiteSpace(source) || source.IsEOF()) && !source.IsNewLine();
        }
        public static bool IsPunctuation(this char source)
        {
            return "<>{}()[]!%^&*+-=/.,?;:|".Contains(source);
        }
        public static bool IsIdentifier(this char source)
        {
            return source.IsLetterOrDigit() || source == '_';
        }
    }
}
