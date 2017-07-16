using System;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Parsing.Syntax.Statements
{
    internal class WhileStatement : Statement
    {
        public override SyntaxKind Kind => SyntaxKind.WhileStatement;
        public BlockStatement Body { get; }
        public Expression Predicate { get; }
        public WhileStatementType Type { get; }

        public WhileStatement(SourceFilePart filePart, Expression predicate, BlockStatement body, WhileStatementType type = WhileStatementType.Default)
            : base(filePart)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (body == null)
                throw new ArgumentNullException(nameof(body));

            Predicate = predicate;
            Body = body;
            Type = type;
        }
    }
}
