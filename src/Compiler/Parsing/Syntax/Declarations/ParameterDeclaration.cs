using System;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class ParameterDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.ParameterDeclaration;
        public TypeDeclaration Type { get; }

        public ParameterDeclaration(SourceFilePart filePart, string name, TypeDeclaration type)
            : base(filePart, name)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Type = type;
        }
    }
}
