using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Statements;
using Compiler.Semantics;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class ConstructorDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.ConstructorDeclaration;
        public BlockStatement Body { get; }
        public IEnumerable<ParameterDeclaration> Parameters { get; }

        public ConstructorDeclaration(SourceFilePart span, IEnumerable<ParameterDeclaration> parameters, BlockStatement body) : base(span, "constructor")
        {
            Body = body;
            Parameters = parameters;
        }
        public ConstructorDeclaration(SourceFilePart span, IEnumerable<ParameterDeclaration> parameters, BlockStatement body, Scope scope)
            : base(span, "constructor", scope)
        {
            Body = body;
            Parameters = parameters;
        }
        public ConstructorDeclaration(ConstructorDeclaration declaration, Scope scope)
            : this(declaration.FilePart, declaration.Parameters, declaration.Body, scope)
        {

        }
        public ConstructorDeclaration(ConstructorDeclaration declaration, IEnumerable<ParameterDeclaration> parameters, BlockStatement body, Scope scope)
            : this(declaration.FilePart, declaration.Parameters, body, scope)
        {

        }
    }
}
