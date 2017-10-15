using System;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Semantics;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class VariableDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;
        public TypeDeclaration Type { get; }
        public Expression Value { get; }

        public VariableDeclaration(SourceFilePart span, string name, TypeDeclaration type, Expression value) : base(span, name)
        {
            Type = type;
            Value = value;
        }
        public VariableDeclaration(SourceFilePart span, string name, TypeDeclaration type, Expression value, Scope scope) : base(span, name, scope)
        {
            Type = type;
            Value = value;
        }
        public VariableDeclaration(VariableDeclaration declaration, Scope scope)
            : this(declaration.FilePart, declaration.Name, declaration.Type, declaration.Value, scope)
        {

        }
        public VariableDeclaration(VariableDeclaration declaration, Expression value, Scope scope)
            : this(declaration.FilePart, declaration.Name, declaration.Type, value, scope)
        {

        }
    }
}
