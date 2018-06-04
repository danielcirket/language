using System;
using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Semantics.BoundSyntax.Statements
{
    internal class BoundElseStatement : BoundStatement
    {
        //public override SyntaxKind Kind => SyntaxKind.ElseStatement;
        public BoundBlockStatement Body { get; }

        public BoundElseStatement(
            ElseStatement statement, 
            BoundBlockStatement body, 
            Scope scope) 
            : base(statement, scope)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            Body = body;
        }
    }
}
