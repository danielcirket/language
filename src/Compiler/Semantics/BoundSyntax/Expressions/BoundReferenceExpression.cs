using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Expressions
{
    internal class BoundReferenceExpression : BoundExpression
    {
        //public override SyntaxKind Kind => SyntaxKind.ReferenceExpression;
        public IEnumerable<BoundExpression> References { get; }

        public BoundReferenceExpression(
            ReferenceExpression expression, 
            IEnumerable<BoundExpression> references,
            Scope scope
        ) 
            : base(expression, scope)
        {
            if (references == null)
                throw new ArgumentNullException(nameof(references));

            References = references;
        }
        public BoundReferenceExpression(
            ReferenceExpression expression,
            IEnumerable<BoundExpression> references,
            BoundTypeExpression type,
            Scope scope
        )
            : base(expression, type, scope)
        {
            if (references == null)
                throw new ArgumentNullException(nameof(references));

            References = references;
        }
    }
}
