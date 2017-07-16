namespace Compiler.Parsing.Syntax.Statements
{
    internal class BreakStatement : Statement
    {
        public override SyntaxKind Kind => SyntaxKind.BreakStatement;

        public BreakStatement(SourceFilePart filePart) 
            : base(filePart)
        {
        }
    }
}
