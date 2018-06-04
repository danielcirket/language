using System;
using Compiler.Parsing.Syntax.Statements;
using Compiler.Semantics.BoundSyntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Statements
{
    internal class BoundIfStatement : BoundStatement
    {
        //public override SyntaxKind Kind => SyntaxKind.IfStatement;
        public BoundExpression Predicate { get; }
        public BoundBlockStatement Body { get; }
        public BoundElseStatement Else { get; }

        public BoundIfStatement(
            IfStatement statement, 
            BoundExpression predicate, 
            BoundBlockStatement body,
            BoundElseStatement @else,
            Scope scope) 
            : base(statement, scope)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (body == null)
                throw new ArgumentNullException(nameof(body));

            Predicate = predicate;
            Body = body;
            Else = @else;
        }
    }
}
