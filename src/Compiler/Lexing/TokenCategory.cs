namespace Compiler.Lexing
{
    internal enum TokenCategory
    {
        Unknown,
        Whitespace,
        Comment,
        Constant,
        Grouping,
        Punctuation,
        Operator,
        Identifier,
        Keyword,
        Invalid
    }
}
