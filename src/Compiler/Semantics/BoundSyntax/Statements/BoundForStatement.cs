using Compiler.Parsing.Syntax.Statements;
using Compiler.Semantics.BoundSyntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Statements
{
    internal class BoundForStatement : BoundStatement
    {
        //public override SyntaxKind Kind => SyntaxKind.ForStatement;
        public BoundSyntaxNode Initialiser { get; }
        public BoundExpression Condition { get; }
        public BoundExpression Increment { get; }
        public BoundBlockStatement Body { get; }

        public BoundForStatement(
            ForStatement statement, 
            BoundSyntaxNode initialiser, 
            BoundExpression condition, 
            BoundExpression increment,
            BoundBlockStatement body,
            Scope scope) 
            : base(statement, scope)
        {
            // NOTE(Dan): These can all be empty, e.g. for (;;) {}
            // TODO(Dan): Do we want to allow this?!
            // TODO(Dan): Consider how lowering other loops will impact this too!
            Initialiser = initialiser;
            Condition = condition;
            Increment = increment;
            Body = body;
        }
    }
}
