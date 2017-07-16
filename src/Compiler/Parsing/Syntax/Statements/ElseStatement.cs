using System;

namespace Compiler.Parsing.Syntax.Statements
{
    internal class ElseStatement : Statement
    {
        public override SyntaxKind Kind => SyntaxKind.ElseStatement;
        public BlockStatement Body { get; }

        public ElseStatement(SourceFilePart filePart, BlockStatement body) 
            : base(filePart)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            Body = body;
        }
    }
}
