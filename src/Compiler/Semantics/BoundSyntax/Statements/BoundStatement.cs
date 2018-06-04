using System;
using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Semantics.BoundSyntax.Statements
{
    internal abstract class BoundStatement : BoundSyntaxNode
    {
        //public override SyntaxCategory Category => SyntaxCategory.Statement;
        public Scope Scope { get; }

        public BoundStatement(
            Statement statement,
            Scope scope
        ) 
            : base(statement)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));

            Scope = scope;
        }
    }
}
