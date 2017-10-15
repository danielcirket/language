using System;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Semantics;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class FieldDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.FieldDeclaration;
        public Expression DefaultValue { get; }
        public TypeDeclaration Type { get; }

        public FieldDeclaration(SourceFilePart span, string name, TypeDeclaration type, Expression value) : base(span, name)
        {
            Type = type;
            DefaultValue = value;
        }
        public FieldDeclaration(SourceFilePart span, string name, TypeDeclaration type, Expression value, Scope scope) : base(span, name, scope)
        {
            Type = type;
            DefaultValue = value;
        }
        public FieldDeclaration(FieldDeclaration declaration, Scope scope)
            : this(declaration.FilePart, declaration.Name, declaration.Type, declaration.DefaultValue, scope)
        {

        }
        public FieldDeclaration(FieldDeclaration declaration, Expression defaultValue, Scope scope)
            : this(declaration.FilePart, declaration.Name, declaration.Type, defaultValue, scope)
        {

        }
    }
}
