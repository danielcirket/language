using System;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Expressions
{
    internal class BoundBinaryExpression : BoundExpression
    {
        public BoundExpression Left { get; }
        public BoundExpression Right { get; }
        public BinaryOperator Operator => SyntaxNode<BinaryExpression>().Operator;

        public BoundBinaryExpression(
            BinaryExpression expression, 
            BoundExpression left, 
            BoundExpression right, 
            Scope scope) 
            : base(expression, scope)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            Left = left;
            Right = right;
        }

        public BoundBinaryExpression(
            BinaryExpression expression,
            BoundExpression left,
            BoundExpression right,
            BoundTypeExpression type,
            Scope scope)
            : base(expression, type, scope)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            Left = left;
            Right = right;
        }
    }
}
