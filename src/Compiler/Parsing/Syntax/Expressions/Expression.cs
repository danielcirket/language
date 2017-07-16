namespace Compiler.Parsing.Syntax.Expressions
{
    internal abstract class Expression : SyntaxNode
    {
        public override SyntaxCategory Category => SyntaxCategory.Declaration;

        public Expression(SourceFilePart filePart) 
            : base(filePart)
        {
        }
    }
}
