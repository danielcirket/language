using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Semantics.BoundSyntax.Statements
{ 
    internal class BoundContinueStatement : BoundStatement
    {
        //public override SyntaxKind Kind => SyntaxKind.ContinueStatement;

        public BoundContinueStatement(
            ContinueStatement statement, 
            Scope scope) 
            : base(statement, scope)
        {
        }
    }
}
