using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Semantics.BoundSyntax.Statements
{
    internal class BoundBreakStatement : BoundStatement
    {
        //public override SyntaxKind Kind => SyntaxKind.BreakStatement;

        public BoundBreakStatement(
            BreakStatement statement,
            Scope scope
        ) 
            : base(statement, scope)
        {
        }
    }
}
