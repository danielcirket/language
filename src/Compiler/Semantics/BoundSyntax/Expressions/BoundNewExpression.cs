using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Semantics.BoundSyntax.Declarations;

namespace Compiler.Semantics.BoundSyntax.Expressions
{
    internal class BoundNewExpression : BoundExpression
    {
        public BoundTypeExpression Reference { get; }
        public IEnumerable<BoundExpression> Arguments { get; }

        public BoundNewExpression(
            NewExpression expression,
            BoundTypeExpression reference,
            IEnumerable<BoundExpression> arguments,
            Scope scope) 
            : base(expression, reference, scope)
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
