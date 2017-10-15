using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Parsing.Syntax.Statements
{
    internal class CaseStatement : Statement
    {
        public override SyntaxKind Kind => SyntaxKind.CaseStatement;
        public BlockStatement Body { get; }
        public IEnumerable<Expression> Cases { get; }

        public CaseStatement(SourceFilePart filePart, IEnumerable<Expression> cases, BlockStatement body) 
            : base(filePart)
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
