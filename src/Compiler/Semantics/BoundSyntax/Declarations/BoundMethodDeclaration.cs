using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Semantics.BoundSyntax.Expressions;
using Compiler.Semantics.BoundSyntax.Statements;

namespace Compiler.Semantics.BoundSyntax.Declarations
{
    internal class BoundMethodDeclaration : BoundDeclaration
    {
        public SyntaxModifier Modifier => SyntaxNode<MethodDeclaration>().Modifier;
        public BoundBlockStatement Body { get; }
        public IEnumerable<BoundParameterDeclaration> Parameters { get; }
        public BoundTypeExpression ReturnType => Type;

        public BoundMethodDeclaration(
            MethodDeclaration declaration,
            IEnumerable<BoundParameterDeclaration> parameters,
            Scope scope
        )
            : base(declaration, scope)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            Parameters = parameters;
        }
        public BoundMethodDeclaration(
            MethodDeclaration declaration,
            IEnumerable<BoundParameterDeclaration> parameters,
            BoundTypeExpression returnType,
            Scope scope
        )
            : base(declaration, returnType, scope)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            Parameters = parameters;
        }
        public BoundMethodDeclaration(
            MethodDeclaration declaration,
            IEnumerable<BoundParameterDeclaration> parameters,
            BoundTypeExpression returnType,
            BoundBlockStatement body,
            Scope scope
        )
            : base(declaration, returnType, scope)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            Parameters = parameters;
            Body = body;
        }
    }
}
