using System;
using Compiler.Parsing.Syntax.Statements;
using Compiler.Semantics.BoundSyntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Statements
{
    internal class BoundWhileStatement : BoundStatement
    {
        //public override SyntaxKind Kind => SyntaxKind.WhileStatement;
        public BoundBlockStatement Body { get; }
        public BoundExpression Predicate { get; }
        //public WhileStatementType Type { get; }

        public BoundWhileStatement(
            WhileStatement statement,
            BoundExpression predicate, 
            BoundBlockStatement body, 
            Scope scope)
            : base(statement, scope)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            Predicate = predicate;
            Body = body;
        }
    }
}
