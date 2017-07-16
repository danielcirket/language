using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Parsing.Syntax.Statements
{
    internal class SwitchStatement : Statement
    {
        public override SyntaxKind Kind => SyntaxKind.SwitchStatement;
        public Expression Condition { get; }
        public IEnumerable<CaseStatement> Cases { get; }

        public SwitchStatement(SourceFilePart filePart, Expression condition, IEnumerable<CaseStatement> cases) 
            : base(filePart)
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
