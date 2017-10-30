namespace Compiler.Parsing.Syntax.Declarations
{
    internal class TypeDeclaration : Declaration
    {
        public static TypeDeclaration Empty => new TypeDeclaration(new SourceFilePart(null, null, null, null), "?");
        public override SyntaxKind Kind => SyntaxKind.TypeDeclaration;

        public TypeDeclaration(SourceFilePart filePart, string name) 
            : base(filePart, name)
        {
        }
    }
}
