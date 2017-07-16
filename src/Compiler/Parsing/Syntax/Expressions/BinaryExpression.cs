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
            Left = left;
            Right = right;
            Operator = @operator;
        }
    }
}
