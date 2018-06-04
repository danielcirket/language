using System;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Expressions
{
    internal class BoundUnaryExpression : BoundExpression
    {
        //public override SyntaxKind Kind => SyntaxKind.UnaryExpression;
        public BoundExpression Argument { get; }
        public UnaryOperator Operator => SyntaxNode<UnaryExpression>().Operator;

        public BoundUnaryExpression(
            UnaryExpression expression,
            BoundExpression argument,
            Scope scope)
            : base(expression, argument.Type, scope)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            Argument = argument;
        }
    }
}
