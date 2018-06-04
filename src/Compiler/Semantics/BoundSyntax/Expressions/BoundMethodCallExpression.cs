using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Expressions
{
    internal class BoundMethodCallExpression : BoundExpression
    {
        //public override SyntaxKind Kind => SyntaxKind.MethodCallExpression;
        public BoundExpression Reference { get; }
        public IEnumerable<BoundExpression> Arguments { get; }
        public override BoundTypeExpression Type => Reference.Type;

        public BoundMethodCallExpression(
            MethodCallExpression expression,
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
        public BoundMethodCallExpression(
            MethodCallExpression expression,
            BoundExpression reference,
            IEnumerable<BoundExpression> arguments,
            BoundTypeExpression returnType,
            Scope scope
        )
            : base(expression, returnType, scope)
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
