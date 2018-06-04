using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Semantics.BoundSyntax.Statements;

namespace Compiler.Semantics.BoundSyntax.Declarations
{
    internal class BoundConstructorDeclaration : BoundDeclaration
    {
        public SyntaxModifier Modifier => SyntaxNode<ConstructorDeclaration>().Modifier;
        public BoundBlockStatement Body { get; }
        public IEnumerable<BoundParameterDeclaration> Parameters { get; }

        public BoundConstructorDeclaration(
            ConstructorDeclaration constructor,
            IEnumerable<BoundParameterDeclaration> parameters,
            Scope scope
        )
            : base(constructor, scope)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            Parameters = parameters;
        }

        public BoundConstructorDeclaration(
            ConstructorDeclaration constructor,
            IEnumerable<BoundParameterDeclaration> parameters,
            BoundBlockStatement body,
            Scope scope
        )
            : base(constructor, scope)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            Parameters = parameters;
            Body = body;
        }
    }
}
