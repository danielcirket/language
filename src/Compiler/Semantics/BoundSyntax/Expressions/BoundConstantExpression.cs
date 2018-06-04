using System;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Expressions
{
    internal class BoundConstantExpression : BoundExpression
    {
        //public override SyntaxKind Kind => SyntaxKind.ConstantExpression;
        public string Value => SyntaxNode<ConstantExpression>().Value;
        public ConstantType ConstantType => SyntaxNode<ConstantExpression>().ConstantType;

        public BoundConstantExpression(
            ConstantExpression expression,
            Scope scope
        ) 
            : base(expression, scope)
        {
        }
        public BoundConstantExpression(
            ConstantExpression expression,
            BoundTypeExpression type,
            Scope scope
        )
            : base(expression, type, scope)
        {
        }
    }
}
