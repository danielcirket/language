using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Expressions
{
    internal class BoundMethodCallExpression : BoundExpression
    {
        //public override SyntaxKind Kind => SyntaxKind.MethodCallExpression;
        public BoundExpression Reference { get; }
        public IEnumerable<BoundExpression> Arguments { get; }
        public override BoundTypeExpression Type => Reference.Type;
        public IEnumerable<BoundTypeExpression> GenericTypeParameters { get; }

        public BoundMethodCallExpression(
            MethodCallExpression expression,
            BoundExpression reference,
            IEnumerable<BoundExpression> arguments,
            IEnumerable<BoundTypeExpression> genericTypeParameters,
            Scope scope
        ) 
            : base(expression, scope)
        {
            if (reference == null)
                throw new ArgumentNullException(nameof(reference));
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));
            if (genericTypeParameters == null)
                throw new ArgumentNullException(nameof(genericTypeParameters));

            Reference = reference;
            Arguments = arguments;
            GenericTypeParameters = genericTypeParameters;
        }
        public BoundMethodCallExpression(
            MethodCallExpression expression,
            BoundExpression reference,
            IEnumerable<BoundExpression> arguments,
            IEnumerable<BoundTypeExpression> genericTypeParameters,
            BoundTypeExpression returnType,
            Scope scope
        )
            : base(expression, returnType, scope)
        {
            if (reference == null)
                throw new ArgumentNullException(nameof(reference));
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));
            if (genericTypeParameters == null)
                throw new ArgumentNullException(nameof(genericTypeParameters));

            Reference = reference;
            Arguments = arguments;
            GenericTypeParameters = genericTypeParameters;
        }
    }
}
