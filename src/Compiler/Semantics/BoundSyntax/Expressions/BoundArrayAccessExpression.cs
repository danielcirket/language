using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Expressions
{
    internal class BoundArrayAccessExpression : BoundExpression
    {
        public BoundExpression Reference { get; }
        public IEnumerable<BoundExpression> Arguments { get; }

        public BoundArrayAccessExpression(
            ArrayAccessExpression expression,
            BoundExpression reference, 
            IEnumerable<BoundExpression> arguments,
            Scope scope
        ) 
            : base(expression, scope)
        {
            if (reference == null)
                throw new ArgumentNullException(nameof(reference));
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            Reference = reference;
            Arguments = arguments;
        }
    }
}
