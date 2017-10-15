using System;
using Compiler.Semantics;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class ParameterDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.ParameterDeclaration;
        public TypeDeclaration Type { get; }

        public ParameterDeclaration(SourceFilePart span, string name, TypeDeclaration type) : base(span, name)
        {
            Type = type;
        }
        public ParameterDeclaration(SourceFilePart span, string name, TypeDeclaration type, Scope scope) : base(span, name, scope)
        {
            Type = type;
        }
        public ParameterDeclaration(ParameterDeclaration declaration, Scope scope)
            : this(declaration.FilePart, declaration.Name, declaration.Type, scope)
        {

        }
    }
}
