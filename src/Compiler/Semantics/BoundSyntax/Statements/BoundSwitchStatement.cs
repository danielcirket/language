using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Statements;
using Compiler.Semantics.BoundSyntax.Expressions;

namespace Compiler.Semantics.BoundSyntax.Statements
{
    internal class BoundSwitchStatement : BoundStatement
    {
        //public override SyntaxKind Kind => SyntaxKind.SwitchStatement;
        public BoundExpression Condition { get; }
        public IEnumerable<BoundCaseStatement> Cases { get; }

        public BoundSwitchStatement(
            SwitchStatement statement, 
            BoundExpression condition, 
            IEnumerable<BoundCaseStatement> cases,
            Scope scope) 
            : base(statement, scope)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            if (cases == null)
                throw new ArgumentNullException(nameof(cases));

            Condition = condition;
            Cases = cases;
        }
    }
}
