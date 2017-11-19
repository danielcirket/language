namespace Compiler.Parsing.Syntax
{
    internal enum SyntaxModifier
    {
        None = 0 << 1,
        Public = 1 << 1,
        Internal = 2 << 1,
        Private = 3 << 1,
    }
}
