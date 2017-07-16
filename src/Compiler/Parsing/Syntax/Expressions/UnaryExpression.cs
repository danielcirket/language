namespace Compiler.Parsing.Syntax.Expressions
{
    internal class UnaryExpression : Expression
    {
        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;
        public Expression Argument { get; }
        public UnaryOperator Operator { get; }

        public UnaryExpression(SourceFilePart filePart, Expression argument, UnaryOperator @operator) 
            : base(filePart)
        {
            Argument = argument;
            Operator = @operator;
        }
    }
}
