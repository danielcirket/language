using System;

namespace Compiler.Parsing.Syntax.Expressions
{
    internal class BinaryExpression : Expression
    {
        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

        public Expression Left { get; }
        public Expression Right { get; }
        public BinaryOperator Operator { get; }

        public BinaryExpression(SourceFilePart filePart, Expression left, Expression right, BinaryOperator @operator) 
            : base(filePart)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            Left = left;
            Right = right;
            Operator = @operator;
        }
    }
}
