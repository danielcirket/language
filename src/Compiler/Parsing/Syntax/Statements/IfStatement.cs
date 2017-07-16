using System;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Parsing.Syntax.Statements
{
    internal class IfStatement : Statement
    {
        public override SyntaxKind Kind => SyntaxKind.IfStatement;
        public Expression Predicate { get; }
        public BlockStatement Body { get; }
        public ElseStatement Else { get; }

        public IfStatement(SourceFilePart filePart, Expression predicate, BlockStatement body, ElseStatement @else = null) 
            : base(filePart)
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
