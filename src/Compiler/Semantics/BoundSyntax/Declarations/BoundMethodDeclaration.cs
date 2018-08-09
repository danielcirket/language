using System;
using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<BoundTypeExpression> GenericTypeParameters { get; }
        public int Arity => Parameters.Count();

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
            IEnumerable<BoundTypeExpression> genericTypeParameters,
            BoundTypeExpression returnType,
            Scope scope
        )
            : base(declaration, returnType, scope)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (genericTypeParameters == null)
                throw new ArgumentNullException(nameof(genericTypeParameters));

            Parameters = parameters;
            GenericTypeParameters = genericTypeParameters;
        }
        public BoundMethodDeclaration(
            MethodDeclaration declaration,
            IEnumerable<BoundParameterDeclaration> parameters,
            IEnumerable<BoundTypeExpression> genericTypeParameters,
            BoundTypeExpression returnType,
            BoundBlockStatement body,
            Scope scope
        )
            : base(declaration, returnType, scope)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (genericTypeParameters == null)
                throw new ArgumentNullException(nameof(genericTypeParameters));

            Parameters = parameters;
            GenericTypeParameters = genericTypeParameters;
            Body = body;
        }
    }
}
