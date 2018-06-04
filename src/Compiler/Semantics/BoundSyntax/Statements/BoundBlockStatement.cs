using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Semantics.BoundSyntax.Statements
{
    internal class BoundBlockStatement : BoundStatement
    {
        public IEnumerable<BoundSyntaxNode> Contents { get; }

        public BoundBlockStatement(
            BlockStatement statement, 
            IEnumerable<BoundSyntaxNode> contents,
            Scope scope
        ) 
            : base(statement, scope)
        {
            if (contents == null)
                throw new ArgumentNullException(nameof(contents));

            Contents = contents;
        }
    }
}
