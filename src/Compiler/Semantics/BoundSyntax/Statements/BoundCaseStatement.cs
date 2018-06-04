using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Statements;
using Compiler.Semantics.BoundSyntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Statements
{
    internal class BoundCaseStatement : BoundStatement
    {
        //public override SyntaxKind Kind => SyntaxKind.CaseStatement;
        public BoundBlockStatement Body { get; }
        public IEnumerable<BoundExpression> Cases { get; }

        public BoundCaseStatement(
            CaseStatement statement, 
            IEnumerable<BoundExpression> cases, 
            BoundBlockStatement body, 
            Scope scope
        ) 
            : base(statement, scope)
        {
            if (cases == null)
                throw new ArgumentNullException(nameof(cases));
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            Cases = cases;
            Body = body;
        }
    }
}
