namespace Compiler.Parsing.Syntax.Statements
{
    internal class EmptyStatement : Statement
    {
        public override SyntaxKind Kind => SyntaxKind.EmptyStatement;

        public EmptyStatement(SourceFilePart filePart) 
            : base(filePart)
        {
        }
    }
}
