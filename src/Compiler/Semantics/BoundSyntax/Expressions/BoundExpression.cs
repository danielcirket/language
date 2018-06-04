using System;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Expressions
{
    internal abstract class BoundExpression : BoundSyntaxNode
    {
        public Scope Scope { get; }
        public virtual BoundTypeExpression Type { get; }
        public bool HasError { get; set; }

        public BoundExpression(
            Expression expression,
            Scope scope
        ) 
            : base(expression)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));

            Scope = scope;
        }
        public BoundExpression(
            Expression expression,
            BoundTypeExpression type,
            Scope scope
        )
            : this(expression, scope)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Type = type;
        }
    }
}
