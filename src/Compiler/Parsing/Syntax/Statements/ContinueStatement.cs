namespace Compiler.Parsing.Syntax.Statements
{
    internal class ContinueStatement : Statement
    {
        public override SyntaxKind Kind => SyntaxKind.ContinueStatement;

        public ContinueStatement(SourceFilePart filePart) 
            : base(filePart)
        {
        }
    }
}
