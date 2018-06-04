using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Semantics.BoundSyntax.Declarations;
using Compiler.Semantics.BoundSyntax.Statements;

namespace Compiler.Semantics.BoundSyntax.Expressions
{
    internal class BoundLambdaExpression : BoundExpression
    {
        public BoundBlockStatement Body { get; }
        public IEnumerable<BoundParameterDeclaration> Parameters { get; }

        public BoundLambdaExpression(
            LambdaExpression expression, 
            IEnumerable<BoundParameterDeclaration> parameters,
            BoundBlockStatement body,
            Scope scope
        )
            : base(expression, scope)
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
