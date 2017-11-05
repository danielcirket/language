using System;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class PropertyDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.PropertyDeclaration;
        public MethodDeclaration Getter { get; }
        public MethodDeclaration Setter { get; }
        public TypeDeclaration Type { get; }

        public PropertyDeclaration(SourceFilePart filePart, string name, TypeDeclaration type, MethodDeclaration getter, MethodDeclaration setter = null)
            : base(filePart, name)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Type = type;
            Getter = getter;
            Setter = setter;
        }
    }
}
