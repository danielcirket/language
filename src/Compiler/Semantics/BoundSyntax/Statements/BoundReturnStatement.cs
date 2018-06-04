using Compiler.Parsing.Syntax.Statements;
using Compiler.Semantics.BoundSyntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Statements
{
    internal class BoundReturnStatement : BoundStatement
    {
        //public override SyntaxKind Kind => SyntaxKind.ReturnStatement;
        public BoundExpression Value { get; }

        public BoundReturnStatement(
            ReturnStatement statement, 
            BoundExpression value,
            Scope scope) 
            : base(statement, scope)
        {
            Value = value;
        }
    }
}
