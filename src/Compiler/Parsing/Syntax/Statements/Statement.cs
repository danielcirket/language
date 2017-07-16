namespace Compiler.Parsing.Syntax.Statements
{
    internal abstract class Statement : SyntaxNode
    {
        public override SyntaxCategory Category => SyntaxCategory.Statement;

        public Statement(SourceFilePart filePart) 
            : base(filePart)
        {
        }
    }
}
