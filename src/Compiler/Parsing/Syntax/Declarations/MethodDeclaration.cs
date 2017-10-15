using System;
using System.Collections.Generic;
using System.Text;
using Compiler.Parsing.Syntax.Statements;
using Compiler.Semantics;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class MethodDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.MethodDeclaration;
        public BlockStatement Body { get; }
        public IEnumerable<ParameterDeclaration> Parameters { get; }
        public TypeDeclaration ReturnType { get; }

        public MethodDeclaration(SourceFilePart span, string name, TypeDeclaration returnType, IEnumerable<ParameterDeclaration> parameters, BlockStatement body) : base(span, name)
        {
            ReturnType = returnType;
            Parameters = parameters;
            Body = body;
        }
        public MethodDeclaration(SourceFilePart span, string name, TypeDeclaration returnType, IEnumerable<ParameterDeclaration> parameters, BlockStatement body, Scope scope) : base(span, name, scope)
        {
            ReturnType = returnType;
            Parameters = parameters;
            Body = body;
        }
        public MethodDeclaration(MethodDeclaration declaration, IEnumerable<ParameterDeclaration> parameters, Scope scope)
            : this(declaration.FilePart, declaration.Name, declaration.ReturnType, parameters, declaration.Body, scope)
        {

        }
        public MethodDeclaration(MethodDeclaration declaration, IEnumerable<ParameterDeclaration> parameters, BlockStatement body, Scope scope)
            : this(declaration.FilePart, declaration.Name, declaration.ReturnType, parameters, body, scope)
        {

        }
    }
}
