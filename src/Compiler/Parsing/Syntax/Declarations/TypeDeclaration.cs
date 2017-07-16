namespace Compiler.Parsing.Syntax.Declarations
{
    internal class TypeDeclaration : Declaration
    {
        // TODO(Dan): Fully qualified name
        public override SyntaxKind Kind => SyntaxKind.TypeDeclaration;

        public TypeDeclaration(SourceFilePart filePart, string name) 
            : base(filePart, name)
        {
        }
    }
}
