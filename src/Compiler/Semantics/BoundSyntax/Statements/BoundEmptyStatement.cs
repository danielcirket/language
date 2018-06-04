using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Semantics.BoundSyntax.Statements
{
    internal class BoundEmptyStatement : BoundStatement
    {
        //public override SyntaxKind Kind => SyntaxKind.EmptyStatement;

        public BoundEmptyStatement(
            EmptyStatement statement, 
            Scope scope) 
            : base(statement, scope)
        {
        }
    }
}
